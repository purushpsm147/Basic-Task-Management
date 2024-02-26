using RamSoft_Task_Management.Models;

namespace RamSoft_Task_Management.Services;

public interface ITaskService
{
    Task<IEnumerable<JiraTask>> GetTasks();
    Task<JiraTask?> GetTask(int id);
    Task CreateTask(JiraTask task);
    Task UpdateTask(JiraTask task);
    Task DeleteTask(JiraTask task);
    IList<JiraTask> SortTask(IList<JiraTask> tasks);
}
