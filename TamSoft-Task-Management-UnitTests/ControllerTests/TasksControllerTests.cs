using Microsoft.AspNetCore.Mvc;
using Moq;
using RamSoft_Task_Management.Controllers;
using RamSoft_Task_Management.Enums;
using RamSoft_Task_Management.Models;
using RamSoft_Task_Management.Services;

namespace TamSoft_Task_Management_UnitTests.ControllerTests;

public class TasksControllerTests
{
    private readonly Mock<ITaskService> _taskServiceMock;

    public TasksControllerTests()
    {
        _taskServiceMock = new Mock<ITaskService>();
    }

    [Fact]
    public async Task GetTasks_ReturnsOkResult()
    {
        // Arrange
        var tasks = new List<JiraTask>
        {
            new() { Id = 1, Name = "Task 1", Description = "Description 1", Status = JiraTaskStatus.InProgress },
            new() { Id = 2, Name = "Task 2", Description = "Description 2", Status = JiraTaskStatus.Done }
        };
        _taskServiceMock.Setup(x => x.GetTasks()).ReturnsAsync(tasks);
        var controller = new TasksController(_taskServiceMock.Object);

        // Act
        var result = await controller.GetTasks();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var model = Assert.IsAssignableFrom<IEnumerable<JiraTask>>(okResult.Value);
        Assert.Equal(2, model.Count());
    }

    [Fact]
    public async Task GetTask_ReturnsNotFoundResult()
    {
        // Arrange
        _taskServiceMock.Setup(x => x.GetTask(It.IsAny<int>())).ReturnsAsync((JiraTask)null);
        var controller = new TasksController(_taskServiceMock.Object);

        // Act
        var result = await controller.GetTask(1);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetTask_ReturnsOkResult()
    {
        // Arrange
        var task = new JiraTask { Id = 1, Name = "Task 1", Description = "Description 1", Status = JiraTaskStatus.InProgress };
        _taskServiceMock.Setup(x => x.GetTask(It.IsAny<int>())).ReturnsAsync(task);
        var controller = new TasksController(_taskServiceMock.Object);

        // Act
        var result = await controller.GetTask(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var model = Assert.IsType<JiraTask>(okResult.Value);
        Assert.Equal(1, model.Id);
    }
    [Fact]
    public async Task GetTaskByStatus_ReturnsOkResult()
    {
        // Arrange
        var tasks = new List<JiraTask>
        {
            new() { Id = 1, Name = "Task 1", Description = "Description 1", Status = JiraTaskStatus.InProgress },
            new() { Id = 2, Name = "Task 2", Description = "Description 2", Status = JiraTaskStatus.Done }
        };
        _taskServiceMock.Setup(x => x.GetTasks()).ReturnsAsync(tasks);
        var controller = new TasksController(_taskServiceMock.Object);

        // Act
        var result = await controller.GetTaskByStatus(JiraTaskStatus.InProgress);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var model = Assert.IsAssignableFrom<IEnumerable<JiraTask>>(okResult.Value);
        Assert.Single(model);
    }

    [Fact]
    public async Task CreateTask_ReturnsCreatedResult()
    {
        // Arrange
        var task = new JiraTask { Id = 1, Name = "Task 1", Description = "Description 1", Status = JiraTaskStatus.InProgress };
        _taskServiceMock.Setup(x => x.CreateTask(It.IsAny<JiraTask>())).ReturnsAsync(JiraProcessResults.Success());
        var controller = new TasksController(_taskServiceMock.Object);

        // Act
        var result = await controller.CreateTask(task);

        // Assert
        var createdResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
        var model = Assert.IsType<JiraTask>(createdResult.Value);
        Assert.Equal(1, model.Id);
    }

    [Fact]
    public async Task CreateTask_ReturnsStatusCode500Result()
    {
        // Arrange
        var task = new JiraTask { Id = 1, Name = "Task 1", Description = "Description 1", Status = JiraTaskStatus.InProgress };
        _taskServiceMock.Setup(x => x.CreateTask(It.IsAny<JiraTask>())).ReturnsAsync(JiraProcessResults.Failure(JiraProcessError.InternalServerError));
        var controller = new TasksController(_taskServiceMock.Object);

        // Act
        var result = await controller.CreateTask(task);

        // Assert
        Assert.IsType<StatusCodeResult>(result.Result);
        Assert.Equal(500, (result.Result as StatusCodeResult)?.StatusCode);
    }

    [Fact]
    public async Task UpdateTask_ReturnsNoContentResult()
    {
        // Arrange
        var task = new JiraTask { Id = 1, Name = "Task 1", Description = "Description 1", Status = JiraTaskStatus.InProgress };
        _taskServiceMock.Setup(x => x.UpdateTask(It.IsAny<JiraTask>())).ReturnsAsync(JiraProcessResults.Success());
        var controller = new TasksController(_taskServiceMock.Object);

        // Act
        var result = await controller.UpdateTask(task);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result.Result);
    }

    [Fact]
    public async Task UpdateTask_ReturnsStatusCode500Result()
    {
        // Arrange
        var task = new JiraTask { Id = 1, Name = "Task 1", Description = "Description 1", Status = JiraTaskStatus.InProgress };
        _taskServiceMock.Setup(x => x.UpdateTask(It.IsAny<JiraTask>())).ReturnsAsync(JiraProcessResults.Failure(JiraProcessError.InternalServerError));
        var controller = new TasksController(_taskServiceMock.Object);

        // Act
        var result = await controller.UpdateTask(task);

        // Assert
        Assert.IsType<StatusCodeResult>(result.Result);
        Assert.Equal(500, (result.Result as StatusCodeResult)?.StatusCode);
    }

    [Fact]
    public async Task DeleteTask_ReturnsNoContentResult()
    {
        // Arrange
        var task = new JiraTask { Id = 1, Name = "Task 1", Description = "Description 1", Status = JiraTaskStatus.InProgress };
        _taskServiceMock.Setup(x => x.GetTask(It.IsAny<int>())).ReturnsAsync(task);
        _taskServiceMock.Setup(x => x.DeleteTask(It.IsAny<JiraTask>())).ReturnsAsync(JiraProcessResults.Success());
        var controller = new TasksController(_taskServiceMock.Object);

        // Act
        var result = await controller.DeleteTask(1);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result.Result);
    }

    [Fact]
    public async Task DeleteTask_ReturnsNotFoundResult()
    {
        // Arrange
        _taskServiceMock.Setup(x => x.GetTask(It.IsAny<int>())).ReturnsAsync((JiraTask)null);
        var controller = new TasksController(_taskServiceMock.Object);

        // Act
        var result = await controller.DeleteTask(1);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteTask_ReturnsStatusCode500Result()
    {
        // Arrange
        var task = new JiraTask { Id = 1, Name = "Task 1", Description = "Description 1", Status = JiraTaskStatus.InProgress };
        _taskServiceMock.Setup(x => x.GetTask(It.IsAny<int>())).ReturnsAsync(task);
        _taskServiceMock.Setup(x => x.DeleteTask(It.IsAny<JiraTask>())).ReturnsAsync(JiraProcessResults.Failure(JiraProcessError.InternalServerError));
        var controller = new TasksController(_taskServiceMock.Object);

        // Act
        var result = await controller.DeleteTask(1);

        // Assert
        Assert.IsType<StatusCodeResult>(result.Result);
        Assert.Equal(500, (result.Result as StatusCodeResult)?.StatusCode);
    }   

}
