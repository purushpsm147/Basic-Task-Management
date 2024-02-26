
using Microsoft.EntityFrameworkCore;
using RamSoft_Task_Management.Infrastructure;
using RamSoft_Task_Management.Models;

namespace RamSoft_Task_Management.Services
{
    public class TaskRepository : ITaskRepository
    {
        public readonly JiraAppDbContext jiraAppDbContext;
        private readonly ILogger<TaskRepository> logger;
        public TaskRepository( JiraAppDbContext _jiraAppDbContext, ILogger<TaskRepository> _logger)
        {
            jiraAppDbContext = _jiraAppDbContext;
            logger = _logger;

        }
        public async Task<JiraProcessResults> CreateTask(JiraTask task)
        {
            try
            {
                await jiraAppDbContext.JiraTask.AddAsync(task);
                await jiraAppDbContext.SaveChangesAsync();

                return JiraProcessResults.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in CreateTask");
                return JiraProcessResults.Failure(JiraProcessError.InternalServerError);
            }
            
        }

        public async Task<JiraProcessResults> DeleteTask(JiraTask task)
        {
            try
            {
                jiraAppDbContext.JiraTask.Remove(task);
                await jiraAppDbContext.SaveChangesAsync();
                return JiraProcessResults.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in DeleteTask");
                return JiraProcessResults.Failure(JiraProcessError.InternalServerError);
            }
        }

        public async Task<JiraTask?> GetTask(int id)
        {
            return await jiraAppDbContext.JiraTask.FindAsync(id);
        }

        public async Task<IEnumerable<JiraTask>> GetTasks()
        {
            return await jiraAppDbContext.JiraTask.ToListAsync();
        }

        public async Task<JiraProcessResults> UpdateTask(JiraTask task)
        {
            try
            {
                jiraAppDbContext.Update(task);
                await jiraAppDbContext.SaveChangesAsync();
                return JiraProcessResults.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in UpdateTask");
                return JiraProcessResults.Failure(JiraProcessError.InternalServerError);
            }
        }
    }
}
