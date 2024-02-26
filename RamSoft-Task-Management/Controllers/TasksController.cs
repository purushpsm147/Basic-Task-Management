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
    [ProducesResponseType(typeof(IEnumerable<JiraTask>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<JiraTask>>> GetTasks()
    {
        var tasks = await _taskService.GetTasks();
        return Ok(tasks);
    }

    [HttpGet("sort", Name = "SortTasks")]
    [ProducesResponseType(typeof(IEnumerable<JiraTask>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<JiraTask>>> SortTasks()
    {
        var tasks = await _taskService.GetTasks();
        return Ok(_taskService.SortTask(tasks.ToList()));
    }

    [HttpGet("{id}", Name = "GetTask")]
    [ProducesResponseType(typeof(JiraTask), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
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
    [ProducesResponseType(typeof(JiraTask), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JiraTask>> CreateTask(JiraTask task)
    {
        await _taskService.CreateTask(task);
        return CreatedAtRoute("GetTask", new { id = task.Id }, task);
    }

    [HttpPut("{id}", Name = "UpdateTask")]
    [ProducesResponseType(typeof(JiraTask), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JiraTask>> UpdateTask(int id, JiraTask task)
    {
        if (id != task.Id)
        {
            return BadRequest();
        }
        await _taskService.UpdateTask(task);
        return NoContent();
    }

    [HttpDelete("{id}", Name = "DeleteTask")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JiraTask>> DeleteTask(int id)
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
