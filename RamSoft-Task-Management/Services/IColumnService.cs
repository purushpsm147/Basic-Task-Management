using RamSoft_Task_Management.Models;

namespace RamSoft_Task_Management.Services;

public interface IColumnService
{
    Task<IEnumerable<Column>> GetAllColumnsAsync();
    Task<Column?> GetColumnByIdAsync(int id);
    Task<Column?> GetDefaultColumnAsync();
    Task<JiraProcessResults> CreateColumnAsync(Column column);
    Task<JiraProcessResults> UpdateColumnAsync(Column column);
    Task<JiraProcessResults> DeleteColumnAsync(Column column);
}
