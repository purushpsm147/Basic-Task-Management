using RamSoft_Task_Management.Enums;
using RamSoft_Task_Management.Models;

namespace RamSoft_Task_Management.Services;

public interface ITaskRepository
{
    Task<IEnumerable<JiraTask>> GetTasks();
    Task<JiraTask?> GetTask(int id);
    Task<JiraProcessResults> CreateTask(JiraTask task);
    Task<JiraProcessResults> UpdateTask(JiraTask task);
    Task<JiraProcessResults> DeleteTask(JiraTask task);
}
