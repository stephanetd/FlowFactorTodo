using api.Data;
using api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("FlowFactorTodo");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<ITaskService, TaskService>();
// Register the database context
builder.Services.AddSqlite<AppDbContext>(connectionString);

// Configuration CORS pour permettre les requêtes du frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins(
                    "http://localhost:3000", 
                    "http://frontend:80",     // Pour Docker
                    "http://flowfactor-frontend:80"
                )
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// Utilisation du chemin correct pour la base de données en production
if (builder.Environment.IsProduction())
{
    // Le volume Docker monte le dossier /app/data
    builder.Configuration["ConnectionStrings:DefaultConnection"] = 
        "Data Source=/app/data/flowfactortodo.db";
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

await app.MigrateDbAsync();

app.Run();
