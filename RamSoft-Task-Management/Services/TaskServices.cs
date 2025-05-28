using Microsoft.EntityFrameworkCore;
using RamSoft_Task_Management.Enums;
using RamSoft_Task_Management.Infrastructure;
using RamSoft_Task_Management.Models;
using System.Reflection;

namespace RamSoft_Task_Management.Services;

public class TaskServices : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly JiraAppDbContext _context;
    private readonly IColumnService _columnService;
    
    public TaskServices(
        ITaskRepository taskRepository,
        JiraAppDbContext context,
        IColumnService columnService)
    {
        _taskRepository = taskRepository;
        _context = context;
        _columnService = columnService;
    }
    public async Task<JiraProcessResults> CreateTask(JiraTask task)
    {
       return await _taskRepository.CreateTask(task);
    }

    public async Task<JiraProcessResults> DeleteTask(JiraTask task)
    {
        return await _taskRepository.DeleteTask(task);
    }

    public async Task<JiraTask?> GetTask(int id)
    {
        return await _taskRepository.GetTask(id);
    }

    public async Task<IEnumerable<JiraTask>> GetTasks()
    {
        return await _taskRepository.GetTasks();

    }

    //TODO: Flexible, sort by various properties, and order could be asc or desc
    public List<JiraTask> SortTask(List<JiraTask> tasks, SortDirection order, string prop)
    {
        var sortOrder = order.ToFriendlyString().ToLowerInvariant();
        return DynamicSort(tasks, prop, sortOrder);
    }

    public async Task<JiraProcessResults> UpdateTask(JiraTask task)
    {
        return await _taskRepository.UpdateTask(task);
    }    public async Task<IEnumerable<JiraTask>> GetTasksByColumnAsync(int columnId)
    {
        return await _context.JiraTask
            .Where(t => t.ColumnId == columnId)
            .OrderBy(t => t.Position)
            .ToListAsync();
    }

    public async Task<JiraProcessResults> MoveTaskToColumnAsync(int taskId, int columnId, int? position = null)
    {
        try
        {
            // Get the task and column
            var task = await _context.JiraTask.FindAsync(taskId);
            if (task == null)
            {
                return JiraProcessResults.Failure(new JiraProcessError("TaskNotFound", $"Task with ID {taskId} not found."));
            }

            var column = await _context.Columns.FindAsync(columnId);
            if (column == null)
            {
                return JiraProcessResults.Failure(new JiraProcessError("ColumnNotFound", $"Column with ID {columnId} not found."));
            }

            // If task is already in this column and no position is specified, do nothing
            if (task.ColumnId == columnId && !position.HasValue)
            {
                return JiraProcessResults.Success();
            }

            // Update the task's status based on the column
            var oldColumnId = task.ColumnId;
            task.ColumnId = columnId;
            
            // Update status based on column (maintain synchronization with the legacy status field)
            switch (columnId)
            {
                case 1:
                    task.Status = JiraTaskStatus.Unassigned;
                    break;
                case 2:
                    task.Status = JiraTaskStatus.Approved;
                    break;
                case 3:
                    task.Status = JiraTaskStatus.InProgress;
                    break;
                case 4:
                    task.Status = JiraTaskStatus.Done;
                    break;
                default:
                    // For custom columns, default to InProgress
                    task.Status = JiraTaskStatus.InProgress;
                    break;
            }

            // If position is specified, adjust positions
            if (position.HasValue)
            {
                // Get all tasks in the target column except the one being moved
                var tasksInTargetColumn = await _context.JiraTask
                    .Where(t => t.ColumnId == columnId && t.Id != taskId)
                    .OrderBy(t => t.Position)
                    .ToListAsync();
                
                // Calculate the new position, ensuring it's valid
                int newPosition = Math.Max(0, Math.Min(position.Value, tasksInTargetColumn.Count));
                task.Position = newPosition;
                
                // Adjust positions of other tasks
                for (int i = 0; i < tasksInTargetColumn.Count; i++)
                {
                    if (i >= newPosition)
                    {
                        tasksInTargetColumn[i].Position = i + 1;  // Shift down
                    }
                    else
                    {
                        tasksInTargetColumn[i].Position = i;  // Keep position
                    }
                }
            }
            else
            {
                // Put at the end of the column
                var maxPosition = await _context.JiraTask
                    .Where(t => t.ColumnId == columnId)
                    .MaxAsync(t => (int?)t.Position) ?? -1;
                task.Position = maxPosition + 1;
            }

            await _context.SaveChangesAsync();
            return JiraProcessResults.Success();
        }
        catch (Exception ex)
        {
            return JiraProcessResults.Failure(new JiraProcessError("InternalServerError", ex.Message));
        }
    }

    public async Task<JiraProcessResults> ReorderTasksInColumnAsync(int columnId, IEnumerable<int> taskIds)
    {
        try
        {
            // Verify the column exists
            if (!await _context.Columns.AnyAsync(c => c.Id == columnId))
            {
                return JiraProcessResults.Failure(new JiraProcessError("ColumnNotFound", $"Column with ID {columnId} not found."));
            }

            // Get tasks in the column
            var tasksInColumn = await _context.JiraTask
                .Where(t => t.ColumnId == columnId)
                .ToListAsync();
            
            // Extract the task IDs in the column for validation
            var tasksInColumnIds = tasksInColumn.Select(t => t.Id).ToHashSet();
            
            // Convert incoming task IDs to list and validate they belong to this column
            var orderedTaskIds = taskIds.ToList();
            
            // Ensure all specified task IDs exist in this column
            if (orderedTaskIds.Any(id => !tasksInColumnIds.Contains(id)))
            {
                return JiraProcessResults.Failure(new JiraProcessError("InvalidTaskIds", "One or more specified task IDs do not belong to this column."));
            }
            
            // Update positions based on the provided order
            for (int i = 0; i < orderedTaskIds.Count; i++)
            {
                var task = tasksInColumn.First(t => t.Id == orderedTaskIds[i]);
                task.Position = i;
            }
            
            // Tasks not included in the provided list should be placed at the end
            var unspecifiedTasks = tasksInColumn
                .Where(t => !orderedTaskIds.Contains(t.Id))
                .ToList();
            
            int position = orderedTaskIds.Count;
            foreach (var task in unspecifiedTasks)
            {
                task.Position = position++;
            }

            await _context.SaveChangesAsync();
            return JiraProcessResults.Success();
        }
        catch (Exception ex)
        {
            return JiraProcessResults.Failure(new JiraProcessError("InternalServerError", ex.Message));
        }
    }

    /// <summary>
    /// Dynamically sorts a list of objects based on a specified property and direction.
    /// For JiraTask objects, IsFavorite tasks will always be ordered first.
    /// Supports string, numeric, DateTime (e.g. Deadline) and other IComparable properties.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="sortProperty"></param>
    /// <param name="sortDirection"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private static List<T> DynamicSort<T>(List<T> list, string sortProperty, string sortDirection)
    {
        if (list.Count == 0)
            return list;

        // Use case-insensitive lookup so both "deadline" and "Deadline" work.
        var type = typeof(T);
        var propertyInfo = type.GetProperty(sortProperty,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)
            ?? throw new ArgumentException($"Property '{sortProperty}' not found on type '{type.Name}'.");

        int sortReverser = sortDirection.StartsWith("asc", StringComparison.OrdinalIgnoreCase) ? 1 : -1;

        list.Sort((x, y) =>
        {
            // Check if T is JiraTask and apply IsFavorite bubble logic.
            if (x is JiraTask tx && y is JiraTask ty && tx.IsFavorite != ty.IsFavorite)
            {
                // If one is favorite and the other is not, prioritize the favorite.
                return tx.IsFavorite ? -1 : 1;
            }

            // Retrieve the values for the dynamic property.
            var xValue = propertyInfo.GetValue(x) as IComparable;
            var yValue = propertyInfo.GetValue(y) as IComparable;

            // Handle possible nulls.
            if (xValue == null && yValue == null)
                return 0;
            if (xValue == null)
                return -1 * sortReverser;
            if (yValue == null)
                return 1 * sortReverser;

            return sortReverser * xValue.CompareTo(yValue);
        });

        return list;
    }
}
