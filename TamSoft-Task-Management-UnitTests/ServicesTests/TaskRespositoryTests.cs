using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RamSoft_Task_Management.Infrastructure;
using RamSoft_Task_Management.Models;
using RamSoft_Task_Management.Services;

namespace TamSoft_Task_Management_UnitTests.ServicesTests;

public class TaskRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly ITaskRepository _repository;
    private readonly DatabaseFixture _fixture;

    public TaskRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = fixture.TaskRepository;
    }


    [Fact]
    public async Task CreateTask_Should_Create_Task_Successfully()
    {
        var newTask = new JiraTask { Name = "New Task", Description = "This is a new task" };

        var result = await _repository.CreateTask(newTask);

        Assert.True(result.IsSuccess);
        Assert.NotNull(_fixture.Context.JiraTask.Find(newTask.Id));
    }

    [Fact]
    public async Task GetTask_Should_Return_Task_For_Valid_Id()
    {
        var task = await _repository.GetTask(1);

        Assert.NotNull(task);
        Assert.Equal("Task 1", task.Name);
    }

    [Fact]
    public async Task GetTask_Should_Return_Null_For_Invalid_Id()
    {
        var task = await _repository.GetTask(10);

        Assert.Null(task);
    }

    [Fact]
    public async Task GetTasks_Should_Return_All_Tasks()
    {
        var options = new DbContextOptionsBuilder<JiraAppDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Generate unique db name for each test
        .Options;

        var context = new JiraAppDbContext(options);
        context.Database.EnsureCreated();

        context.JiraTask.AddRange(new List<JiraTask>()
    {
        new JiraTask { Name = "Task 1", Description = "This is task 1" },
    });
        context.SaveChanges();

        var repository = new TaskRepository(context, new Mock<ILogger<TaskRepository>>().Object);
        var tasks = await repository.GetTasks();

        Assert.Equal(1, tasks.Count());
    }

    [Fact]
    public async Task UpdateTask_Should_Update_Task_Successfully()
    {
        var options = new DbContextOptionsBuilder<JiraAppDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Generate unique db name for each test
        .Options;

        var context = new JiraAppDbContext(options);
        context.Database.EnsureCreated();

        context.JiraTask.AddRange(new List<JiraTask>()
        {
            new JiraTask { Name = "Task 1", Description = "This is task 1" },
        });
        context.SaveChanges();

        var repository = new TaskRepository(context, new Mock<ILogger<TaskRepository>>().Object);
        var task = await repository.GetTask(1);
        task.Name = "Updated Task";

        var result = await repository.UpdateTask(task);

        Assert.True(result.IsSuccess);

        var updatedTask = await repository.GetTask(1);
        Assert.Equal("Updated Task", updatedTask.Name);
    }

}

