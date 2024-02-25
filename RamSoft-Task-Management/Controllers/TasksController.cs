using Microsoft.AspNetCore.Mvc;
using RamSoft_Task_Management.Models;
using RamSoft_Task_Management.Services;

namespace RamSoft_Task_Management.Controllers;

[Route("api/task")]
[ApiController]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet(Name = "GetTasks")]
    public async Task<ActionResult<IEnumerable<JiraTask>>> GetTasks()
    {
        var tasks = await _taskService.GetTasks();
        return Ok(tasks);
    }

    [HttpGet("{id}", Name = "GetTask")]
    public async Task<ActionResult<JiraTask>> GetTask(int id)
    {
        var task = await _taskService.GetTask(id);
        if (task == null)
        {
            return NotFound();
        }
        return Ok(task);
    }

    [HttpPost(Name = "CreateTask")]
    public async Task<ActionResult<Task>> CreateTask(JiraTask task)
    {
        await _taskService.CreateTask(task);
        return CreatedAtRoute("GetTask", new { id = task.Id }, task);
    }

    [HttpPut("{id}", Name = "UpdateTask")]
    public async Task<ActionResult<Task>> UpdateTask(int id, JiraTask task)
    {
        if (id != task.Id)
        {
            return BadRequest();
        }
        await _taskService.UpdateTask(task);
        return NoContent();
    }

    [HttpDelete("{id}", Name = "DeleteTask")]
    public async Task<ActionResult<Task>> DeleteTask(int id)
    {
        var task = await _taskService.GetTask(id);
        if (task == null)
        {
            return NotFound();
        }
        await _taskService.DeleteTask(task);
        return NoContent();
    }       
}
