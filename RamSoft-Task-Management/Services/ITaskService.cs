using RamSoft_Task_Management.Enums;
using RamSoft_Task_Management.Models;

namespace RamSoft_Task_Management.Services;

public interface ITaskService
{
    Task<IEnumerable<JiraTask>> GetTasks();
    Task<JiraTask?> GetTask(int id);
    Task<JiraProcessResults> CreateTask(JiraTask task);
    Task<JiraProcessResults> UpdateTask(JiraTask task);
    Task<JiraProcessResults> DeleteTask(JiraTask task);
    List<JiraTask> SortTask(List<JiraTask> tasks, SortDirection order, string prop);
    
    // New methods for column-based task management
    Task<IEnumerable<JiraTask>> GetTasksByColumnAsync(int columnId);
    Task<JiraProcessResults> MoveTaskToColumnAsync(int taskId, int columnId, int? position = null);
    Task<JiraProcessResults> ReorderTasksInColumnAsync(int columnId, IEnumerable<int> taskIds);
}
