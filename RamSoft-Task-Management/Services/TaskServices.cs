using RamSoft_Task_Management.Models;

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

    public IList<JiraTask> SortTask(IList<JiraTask> tasks)
    {
        return tasks.OrderBy(t => t.IsFavorite).ThenBy(t => t.Name).ToList();
    }

    public async Task<JiraProcessResults> UpdateTask(JiraTask task)
    {
        return await _taskRepository.UpdateTask(task);
    }
}
