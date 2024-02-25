using Microsoft.EntityFrameworkCore;
using RamSoft_Task_Management.Infrastructure;
using RamSoft_Task_Management.Services;

namespace RamSoft_Task_Management
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<JiraAppDbContext>(options => options.UseInMemoryDatabase("JiraAppDb"));
            builder.Services.AddScoped<ITaskRepository,  TaskRepository>();
            builder.Services.AddScoped<ITaskService, TaskServices>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}