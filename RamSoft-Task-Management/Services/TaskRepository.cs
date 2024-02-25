
using Microsoft.EntityFrameworkCore;
using RamSoft_Task_Management.Infrastructure;
using RamSoft_Task_Management.Models;

namespace RamSoft_Task_Management.Services
{
    public class TaskRepository : ITaskRepository
    {
        public readonly JiraAppDbContext jiraAppDbContext;
        public TaskRepository( JiraAppDbContext _jiraAppDbContext)
        {
            jiraAppDbContext = _jiraAppDbContext;
        }
        public async Task CreateTask(JiraTask task)
        {
            await jiraAppDbContext.JiraTask.AddAsync(task);
            await jiraAppDbContext.SaveChangesAsync();
        }

        public async Task DeleteTask(JiraTask task)
        {
            jiraAppDbContext.JiraTask.Remove(task);
            await jiraAppDbContext.SaveChangesAsync();
        }

        public async Task<JiraTask?> GetTask(int id)
        {
            return await jiraAppDbContext.JiraTask.FindAsync(id);
        }

        public async Task<IEnumerable<JiraTask>> GetTasks()
        {
            return await jiraAppDbContext.JiraTask.ToListAsync();
        }

        public async Task UpdateTask(JiraTask task)
        {
            jiraAppDbContext.Update(task);
            await jiraAppDbContext.SaveChangesAsync();
        }
    }
}
