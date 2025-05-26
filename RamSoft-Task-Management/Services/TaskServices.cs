using RamSoft_Task_Management.Enums;
using RamSoft_Task_Management.Models;
using System.Reflection;

namespace RamSoft_Task_Management.Services;

public class TaskServices : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    public TaskServices( ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
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
