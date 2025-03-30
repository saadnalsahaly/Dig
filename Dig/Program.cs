using Dig.Data;
using Dig.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Environment = Dig.Models.Environment;

namespace Dig.Backend;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddDbContext<EnvironmentContext>(opt =>
            opt.UseSqlite("Data Source=environmentlog.db"));
        builder.Services.AddDbContext<UserCommandContext>(opt =>
            opt.UseSqlite("Data Source=commandslog.db"));
        builder.Services.AddDbContext<PlantContext>(opt =>
            opt.UseSqlite("Data Source=plants.db"));
        builder.Services.AddDbContext<PlantStatusContext>(opt =>
            opt.UseSqlite("Data Source=plantslog.db"));
        builder.Services.AddDbContext<OperationModeContext>(opt =>
            opt.UseSqlite("Data Source=opmodlog.db"));
        builder.Services.AddDbContext<NotificationContext>(opt =>
            opt.UseSqlite("Data Source=notificationlog.db"));
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
            
        builder.Services.AddSingleton<SseService<Environment>>();
        builder.Services.AddSingleton<SseService<PlantStatus>>();
        builder.Services.AddSingleton<SseService<Notification>>();
            
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}