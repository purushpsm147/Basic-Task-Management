
using RamSoft_Task_Management.Models;

namespace RamSoft_Task_Management.Services;

public class TaskServices : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    public TaskServices( ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }
    public async Task CreateTask(JiraTask task)
    {
        await _taskRepository.CreateTask(task);
    }

    public async Task DeleteTask(JiraTask task)
    {
        await _taskRepository.DeleteTask(task);
    }

    public async Task<JiraTask?> GetTask(int id)
    {
        return await _taskRepository.GetTask(id);
    }

    public async Task<IEnumerable<JiraTask>> GetTasks()
    {
        return await _taskRepository.GetTasks();

    }

    public async Task UpdateTask(JiraTask task)
    {
        await _taskRepository.UpdateTask(task);
    }
}
