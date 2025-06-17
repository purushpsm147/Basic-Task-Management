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
    }    /// <summary>
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
    
    /// <summary>
    /// Move a task to a different column with optional position
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <param name="columnId">Target column ID</param>
    /// <param name="position">Optional position within the column (0-based)</param>
    /// <returns>No Content</returns>
    [HttpPost("{id}/move", Name = "MoveTask")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> MoveTask(int id, [FromQuery] int columnId, [FromQuery] int? position = null)
    {
        try
        {
            // Check if the task exists
            var task = await _taskService.GetTask(id);
            if (task == null)
            {
                return NotFound($"Task with ID {id} not found");
            }
            
            var result = await _taskService.MoveTaskToColumnAsync(id, columnId, position);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }
            
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while moving the task: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Reorder tasks within a column
    /// </summary>
    /// <param name="columnId">Column ID</param>
    /// <param name="taskIds">Ordered array of task IDs</param>
    /// <returns>No Content</returns>
    [HttpPost("reorder", Name = "ReorderTasks")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ReorderTasks([FromQuery] int columnId, [FromBody] IEnumerable<int> taskIds)
    {
        try
        {
            if (taskIds == null || !taskIds.Any())
            {
                return BadRequest("Task IDs are required");
            }
            
            var result = await _taskService.ReorderTasksInColumnAsync(columnId, taskIds);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }
            
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while reordering tasks: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Get tasks by column ID
    /// </summary>
    /// <param name="columnId">Column ID</param>
    /// <returns>List of tasks in the column, ordered by position</returns>
    [HttpGet("byColumn/{columnId}", Name = "GetTasksByColumn")]
    [ProducesResponseType(typeof(IEnumerable<JiraTask>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<JiraTask>>> GetTasksByColumn(int columnId)
    {
        try
        {
            var tasks = await _taskService.GetTasksByColumnAsync(columnId);
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while retrieving tasks: {ex.Message}");
        }
    }

}