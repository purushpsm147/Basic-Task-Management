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
    public List<JiraTask> SortTask(List<JiraTask> tasks, bool order, string prop)
    {
        var sortOrder = order ? "asc" : "desc";
        return (List<JiraTask>)DynamicSort1<JiraTask>(tasks, prop, sortOrder);
        //return tasks.OrderBy(t => t.IsFavorite).ThenBy(t => t.Name).ToList();
    }

    public async Task<JiraProcessResults> UpdateTask(JiraTask task)
    {
        return await _taskRepository.UpdateTask(task);
    }

    private static List<T> DynamicSort1<T>(List<T> genericList, string sortExpression, string sortDirection)
    {
        int sortReverser = sortDirection.ToLower().StartsWith("asc") ? 1 : -1;
        Comparison<T> comparisonDelegate = new Comparison<T>((x, y) =>
        {
            // Just to get the compare method info to compare the values.
            MethodInfo compareToMethod = GetCompareToMethod<T>(x, sortExpression);
            // Getting current object value.
            object xSortExpressionValue = x.GetType().GetProperty(sortExpression).GetValue(x, null);
            // Getting the previous value.
            object ySortExpressionValue = y.GetType().GetProperty(sortExpression).GetValue(y, null);
            // Comparing the current and next object value of collection.
            object result = compareToMethod.Invoke(xSortExpressionValue, new object[] { ySortExpressionValue });
            // Result tells whether the compared object is equal, greater, or lesser.
            return sortReverser * Convert.ToInt16(result);
        });
        // Using the comparison delegate to sort the object by its property.
        genericList.Sort(comparisonDelegate);

        return genericList;
    }

    /// <summary>
    /// Used to get method information using reflection
    /// </summary>
    private static MethodInfo GetCompareToMethod<T>(T genericInstance, string sortExpression)
    {
        Type genericType = genericInstance.GetType();
        object sortExpressionValue = genericType.GetProperty(sortExpression).GetValue(genericInstance, null);
        Type sortExpressionType = sortExpressionValue.GetType();
        MethodInfo compareToMethodOfSortExpressionType = sortExpressionType.GetMethod("CompareTo", new Type[] { sortExpressionType });
        return compareToMethodOfSortExpressionType;
    }
}
