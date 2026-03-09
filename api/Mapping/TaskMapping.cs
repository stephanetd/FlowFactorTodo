using System;
using api.Entities;
using api.DTOs;

namespace api.Mapping;

public static class TaskMapping
{
    public static TaskItem ToEntity(this CreateTaskDTO createTaskDto, int userId)
    {
        return new TaskItem
        {
            Title = createTaskDto.Title,
            Description = createTaskDto.Description,
            Status = createTaskDto.Status,
            UserId = userId
        };
    }

    public static TaskSummaryDTO ToTaskSummaryDTO(this TaskItem taskItem)
    {
        return new TaskSummaryDTO
        (
            Id: taskItem.Id,
            Title: taskItem.Title,
            Description: taskItem.Description,
            Status: taskItem.Status,
            User: taskItem.User!.FirstName + " " + taskItem.User!.LastName
        );
    }

    public static TaskDetailsDTO ToTaskDetailsDTO(this TaskItem taskItem)
    {
        return new TaskDetailsDTO
        (
            Id: taskItem.Id,
            Title: taskItem.Title,
            Description: taskItem.Description,
            Status: taskItem.Status,
            UserId: taskItem.UserId
        );
    }


}
