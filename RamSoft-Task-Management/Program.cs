using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RamSoft_Task_Management.Infrastructure;
using RamSoft_Task_Management.Services;
using RamSoft_Task_Management.Validations;
using System.Reflection;

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
            builder.Services.AddSwaggerGen(c =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            // Add CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularDev", builder =>
                {
                    builder.WithOrigins("http://localhost:4200", "http://localhost:4201")
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
            });

            // Configure DbContext
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                builder.Services.AddDbContext<JiraAppDbContext>(options =>
                    options.UseInMemoryDatabase("JiraAppDb"));
            }
            else
            {
                builder.Services.AddDbContext<JiraAppDbContext>(options =>
                    options.UseSqlServer(connectionString));
            }

            builder.Services.AddScoped<ITaskRepository, TaskRepository>();
            builder.Services.AddScoped<ITaskService, TaskServices>();
            builder.Services.AddScoped<IColumnService, ColumnService>();
            builder.Services.AddValidatorsFromAssemblyContaining<JiraTaskValidator>();
            builder.Services.AddLogging(Services => Services.AddConsole());

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {                app.UseSwagger();
                app.UseSwaggerUI();
            }
            
            app.UseHttpsRedirection();

            // Use CORS
            app.UseCors("AllowAngularDev");

            app.MapControllers();

            app.Run();
        }
    }
}