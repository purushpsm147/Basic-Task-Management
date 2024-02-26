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
        var tasks = await _repository.GetTasks();

        Assert.Equal(3, tasks.Count());
    }

    [Fact]
    public async Task UpdateTask_Should_Update_Task_Successfully()
    {
        var task = await _repository.GetTask(1);
        task.Name = "Updated Task";

        var result = await _repository.UpdateTask(task);

        Assert.True(result.IsSuccess);

        var updatedTask = await _repository.GetTask(1);
        Assert.Equal("Updated Task", updatedTask.Name);
    }

}

