namespace RamSoft_Task_Management.Models;

public class TaskBoard
{
    public List<JiraTask> JiraTasks { get; set; } = new List<JiraTask>();
    public List<string> Columns { get; set; } = new List<string>();
}