using System;
using api.Data;
using api.DTOs;
using api.Entities;
using api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public interface ITaskService
{
    Task<TaskSummaryDTO> CreateTaskAsync(CreateTaskDTO createdTask);
    Task<IEnumerable<TaskSummaryDTO>> GetAllTasksAsync();
    Task<TaskDetailsDTO> UpdateTaskByStatusAsync(int id, UpdateTaskDTO updatedTask);
}

public class TaskService(AppDbContext context) : ITaskService
{
    private readonly AppDbContext _dbContext = context;

    public async Task<IEnumerable<TaskSummaryDTO>> GetAllTasksAsync()
    {
        return await _dbContext.Tasks
                        .Include(t => t.User)
                        .AsNoTracking()
                        .Select(task => task.ToTaskSummaryDTO())
                        .ToListAsync();                    
    }

    public async Task<TaskSummaryDTO> CreateTaskAsync(CreateTaskDTO createTaskDto)
    {
        // Ensure the user exists or create a new one
        var user = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.FirstName == createTaskDto.User.FirstName && u.LastName == createTaskDto.User.LastName);
        if (user == null)
        {
            user = new User
            {
                FirstName = createTaskDto.User.FirstName,
                LastName = createTaskDto.User.LastName
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }

        // Adds the new task
        TaskItem createdTask = createTaskDto.ToEntity(user.Id);

        _dbContext.Tasks.Add(createdTask);
        await _dbContext.SaveChangesAsync();

        return createdTask.ToTaskSummaryDTO();
    }

    public async Task<TaskDetailsDTO> UpdateTaskByStatusAsync(int id, UpdateTaskDTO updateTaskDto)
    {
        var existingTask = await _dbContext.Tasks.FindAsync(id) ?? throw new KeyNotFoundException($"Task with id {id} not found.");
        if (updateTaskDto.Status is not null)
        {
            existingTask.Status = updateTaskDto.Status;
        }

        await _dbContext.SaveChangesAsync();

        return existingTask.ToTaskDetailsDTO();
    }

}
