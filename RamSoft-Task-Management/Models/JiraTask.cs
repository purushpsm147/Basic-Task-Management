using RamSoft_Task_Management.Enums;

namespace RamSoft_Task_Management.Models;

public class JiraTask
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime Deadline { get; set; }
    public bool IsFavorite { get; set; }
    public JiraTaskStatus Status { get; set; } 
}
