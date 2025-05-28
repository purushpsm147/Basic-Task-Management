using Microsoft.EntityFrameworkCore;
using RamSoft_Task_Management.Infrastructure;
using RamSoft_Task_Management.Models;

namespace RamSoft_Task_Management.Services;

public class ColumnService : IColumnService
{
    private readonly JiraAppDbContext _context;
    
    public ColumnService(JiraAppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Column>> GetAllColumnsAsync()
    {
        return await _context.Columns.OrderBy(c => c.Order).ToListAsync();
    }

    public async Task<Column?> GetColumnByIdAsync(int id)
    {
        return await _context.Columns.FindAsync(id);
    }
    
    public async Task<Column?> GetDefaultColumnAsync()
    {
        return await _context.Columns.FirstOrDefaultAsync(c => c.IsDefault) ?? 
               await _context.Columns.OrderBy(c => c.Order).FirstOrDefaultAsync();
    }    public async Task<JiraProcessResults> CreateColumnAsync(Column column)
    {
        try
        {
            // If this is set as default, unset any other defaults
            if (column.IsDefault)
            {
                var existingDefaults = await _context.Columns.Where(c => c.IsDefault).ToListAsync();
                foreach (var existingDefault in existingDefaults)
                {
                    existingDefault.IsDefault = false;
                }
            }
            
            // If no order specified, put it at the end
            if (column.Order <= 0)
            {
                var maxOrder = await _context.Columns.MaxAsync(c => (int?)c.Order) ?? 0;
                column.Order = maxOrder + 1;
            }
            
            await _context.Columns.AddAsync(column);
            await _context.SaveChangesAsync();
            return JiraProcessResults.Success();
        }
        catch (Exception ex)
        {
            return JiraProcessResults.Failure(new JiraProcessError("InternalServerError", ex.Message));
        }
    }    public async Task<JiraProcessResults> UpdateColumnAsync(Column column)
    {
        try
        {
            // If this is set as default, unset any other defaults
            if (column.IsDefault)
            {
                var existingDefaults = await _context.Columns.Where(c => c.IsDefault && c.Id != column.Id).ToListAsync();
                foreach (var existingDefault in existingDefaults)
                {
                    existingDefault.IsDefault = false;
                }
            }
            
            _context.Columns.Update(column);
            await _context.SaveChangesAsync();
            return JiraProcessResults.Success();
        }
        catch (Exception ex)
        {
            return JiraProcessResults.Failure(new JiraProcessError("InternalServerError", ex.Message));
        }
    }    public async Task<JiraProcessResults> DeleteColumnAsync(Column column)
    {
        try
        {
            // Check if this is the only column
            var count = await _context.Columns.CountAsync();
            if (count <= 1)
            {
                return JiraProcessResults.Failure(new JiraProcessError("LastColumn", "Cannot delete the only remaining column."));
            }

            // Check if it's a default column
            if (column.IsDefault)
            {
                // Set another column as default
                var nextColumn = await _context.Columns.FirstOrDefaultAsync(c => c.Id != column.Id);
                if (nextColumn != null)
                {
                    nextColumn.IsDefault = true;
                }
            }
            
            // Move all tasks from this column to the default column
            var defaultColumn = await GetDefaultColumnAsync();
            if (defaultColumn != null)
            {
                var tasksToMove = await _context.JiraTask.Where(t => t.ColumnId == column.Id).ToListAsync();
                foreach (var task in tasksToMove)
                {
                    task.ColumnId = defaultColumn.Id;
                    // Put moved tasks at the end of the target column
                    var maxPosition = await _context.JiraTask
                        .Where(t => t.ColumnId == defaultColumn.Id)
                        .MaxAsync(t => (int?)t.Position) ?? 0;
                    task.Position = maxPosition + 1;
                }
            }

            _context.Columns.Remove(column);
            await _context.SaveChangesAsync();
            return JiraProcessResults.Success();
        }
        catch (Exception ex)
        {
            return JiraProcessResults.Failure(new JiraProcessError("InternalServerError", ex.Message));
        }
    }
}
