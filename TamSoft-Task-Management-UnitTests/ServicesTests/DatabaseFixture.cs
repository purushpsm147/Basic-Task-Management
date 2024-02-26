using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RamSoft_Task_Management.Enums;
using RamSoft_Task_Management.Infrastructure;
using RamSoft_Task_Management.Models;
using RamSoft_Task_Management.Services;

namespace TamSoft_Task_Management_UnitTests.ServicesTests;

public class DatabaseFixture: IDisposable
{
    public readonly JiraAppDbContext Context;
    public DatabaseFixture()
    {
        var options = new DbContextOptionsBuilder<JiraAppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        Context = new JiraAppDbContext(options);
        Context.Database.EnsureCreated();

        // Seed some data for tests
        Context.JiraTask.AddRange(new List<JiraTask>()
        {
            new() { Name = "Task 1", Description = "This is task 1", Deadline = DateTime.UtcNow.AddDays(7), Id = 1, IsFavorite = true, Status = JiraTaskStatus.Unassigned },
            new() { Name = "Task 2", Description = "This is task 2", Deadline = DateTime.UtcNow.AddDays(5), Id = 2, IsFavorite = false, Status = JiraTaskStatus.Approved },
            new() { Name = "Task 3", Description = "This is task 3", Deadline = DateTime.UtcNow.AddDays(1), Id = 3, IsFavorite = false, Status = JiraTaskStatus.Done }

        });
        Context.SaveChanges();
    }

    public ITaskRepository TaskRepository => new TaskRepository(Context, new Mock<ILogger<TaskRepository>>().Object);
    public void Dispose()
    {
        Context.Dispose();
        GC.SuppressFinalize(this);
    }
}
