using Microsoft.AspNetCore.Mvc;
using RamSoft_Task_Management.Models;
using RamSoft_Task_Management.Services;

namespace RamSoft_Task_Management.Controllers;

[Route("api/columns")]
[ApiController]
public class ColumnsController : ControllerBase
{
    private readonly IColumnService _columnService;

    public ColumnsController(IColumnService columnService)
    {
        _columnService = columnService;
    }

    /// <summary>
    /// Get all columns
    /// </summary>
    /// <returns>List of columns</returns>
    [HttpGet(Name = "GetColumns")]
    [ProducesResponseType(typeof(IEnumerable<Column>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<Column>>> GetColumns()
    {
        try
        {
            var columns = await _columnService.GetAllColumnsAsync();
            return Ok(columns);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while retrieving columns: {ex.Message}");
        }
    }

    /// <summary>
    /// Get column by id
    /// </summary>
    /// <param name="id">Column id</param>
    /// <returns>Column</returns>
    [HttpGet("{id}", Name = "GetColumnById")]
    [ProducesResponseType(typeof(Column), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Column>> GetColumn(int id)
    {
        try
        {
            var column = await _columnService.GetColumnByIdAsync(id);
            if (column == null)
            {
                return NotFound($"Column with ID {id} not found.");
            }
            return Ok(column);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while retrieving the column: {ex.Message}");
        }
    }

    /// <summary>
    /// Create a new column
    /// </summary>
    /// <param name="column">Column data</param>
    /// <returns>The created column</returns>
    [HttpPost(Name = "CreateColumn")]
    [ProducesResponseType(typeof(Column), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Column>> CreateColumn(Column column)
    {
        try
        {
            if (column == null)
            {
                return BadRequest("Column data is required.");
            }

            var result = await _columnService.CreateColumnAsync(column);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return CreatedAtRoute("GetColumnById", new { id = column.Id }, column);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating the column: {ex.Message}");
        }
    }

    /// <summary>
    /// Update an existing column
    /// </summary>
    /// <param name="id">Column id</param>
    /// <param name="column">Updated column data</param>
    /// <returns>No content</returns>
    [HttpPut("{id}", Name = "UpdateColumn")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateColumn(int id, Column column)
    {
        try
        {
            if (column == null || id != column.Id)
            {
                return BadRequest("Invalid column data or mismatched ID.");
            }

            var existingColumn = await _columnService.GetColumnByIdAsync(id);
            if (existingColumn == null)
            {
                return NotFound($"Column with ID {id} not found.");
            }

            var result = await _columnService.UpdateColumnAsync(column);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating the column: {ex.Message}");
        }
    }

    /// <summary>
    /// Delete a column
    /// </summary>
    /// <param name="id">Column id</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}", Name = "DeleteColumn")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteColumn(int id)
    {
        try
        {
            var column = await _columnService.GetColumnByIdAsync(id);
            if (column == null)
            {
                return NotFound($"Column with ID {id} not found.");
            }

            var result = await _columnService.DeleteColumnAsync(column);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting the column: {ex.Message}");
        }
    }
}
