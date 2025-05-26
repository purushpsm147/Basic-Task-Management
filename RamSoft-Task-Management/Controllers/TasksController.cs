using Microsoft.AspNetCore.Mvc;
using RamSoft_Task_Management.Enums;
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

    /// <summary>
    /// Get All Tasks
    /// </summary>
    /// <returns>List of Jira Tasks</returns>
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

    /// <summary>
    /// Dynamic Sorting of Tasks
    /// </summary>
    /// <param name="sortOrder"> Asc or Desc</param>
    /// <param name="prop"> Object property to sort upon </param>
    /// <returns>Sorted List of Jira Tasks</returns>
    [HttpGet("sort", Name = "SortTasks")]
    [ProducesResponseType(typeof(IEnumerable<JiraTask>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<JiraTask>>> SortTasks(
        [FromQuery] SortDirection direction = SortDirection.Asc,
        [FromQuery] string prop = "Name")
    {
        try
        {
            var tasks = await _taskService.GetTasks();
            if (tasks == null || !tasks.Any())
            {
                return NotFound("No tasks available to sort.");
            }
            var sortedTasks = _taskService.SortTask(tasks.ToList(), direction, prop);
            return Ok(sortedTasks);
        }
        catch (ArgumentException ex)
        {
            return BadRequest($"Invalid property name: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while sorting tasks: {ex.Message}");
        }
    }

    /// <summary>
    /// Get Task by Id
    /// </summary>
    /// <param name="id"> Jira Task Id</param>
    /// <returns>Request Jira Task</returns>
    [HttpGet("{id}", Name = "GetTaskById")]
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

    /// <summary>
    /// Get Tasks by Status
    /// </summary>
    /// <param name="status">Jira Task Status</param>
    /// <returns> List of Jira Tasks with the specified status</returns>
    [HttpGet("search", Name = "GetTaskByStatus")]
    [ProducesResponseType(typeof(IEnumerable<JiraTask>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<JiraTask>>> GetTaskByStatus([FromQuery] JiraTaskStatus status)
    {
        var tasks = await _taskService.GetTasks();
        return Ok(tasks.Where(t => t.Status == status));
    }

    /// <summary>
    /// Create a new Task
    /// </summary>
    /// <param name="task">Jira Task Object</param>
    /// <returns>Jira Task Object</returns>
    [HttpPost(Name = "CreateTask")]
    [ProducesResponseType(typeof(JiraTask), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JiraTask>> CreateTask(JiraTask task)
    {
        var results = await _taskService.CreateTask(task);
        if(results.IsFailure)
        {
            return StatusCode(500);
        }
        return CreatedAtRoute("GetTaskById", new { id = task.Id }, task);
    }

    /// <summary>
    /// Update an existing Task
    /// </summary>
    /// <param name="task">Jira Task Object</param>
    /// <returns>Jira Task Object</returns>
    [HttpPut("{id}", Name = "UpdateTask")]
    [ProducesResponseType(typeof(JiraTask), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JiraTask>> UpdateTask(JiraTask task)
    {
        var results = await _taskService.UpdateTask(task);
        if(results.IsFailure)
        {
            return StatusCode(500);
        }
        return NoContent();
    }

    /// <summary>
    /// Delete a Task by Id
    /// </summary>
    /// <param name="id">Id of the Jira Task to delete</param>
    /// <returns>No Content</returns>
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
        var results = await _taskService.DeleteTask(task);
        if(results.IsFailure)
        {
            return StatusCode(500);
        }
        return NoContent();
    }       
}

//ToDo:
/*
 * Here’s a suggested priority backlog—from smallest tweaks to larger new endpoints—so you can grab the easy wins first:

Add unit tests covering all SortTask scenarios (favorite vs. non-favorite, missing prop, asc/desc, invalid prop).

Add a simple “upload image” endpoint (POST /api/task/{id}/image) that accepts a file, saves it to wwwroot and sets JiraTask.ImageUrl.
Add a small DTO or wrapper so UpdateTask can just change Status—i.e. “move” between existing columns.
Introduce a Column entity + endpoints (GET/POST/PUT/DELETE /api/columns) so users can define their own columns.
Implement the UI-friendly “move task” endpoint (POST /api/task/{id}/move?columnId=...).
That ordering nets the full favorite-first/prop-based sort and tests in minutes, then builds out image upload, then adds true column management.


Get a real database, get a real ORM, and get a real UI, get a real cloud
add authorization and authentication

*/
