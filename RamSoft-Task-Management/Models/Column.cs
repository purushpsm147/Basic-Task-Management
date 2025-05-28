using System.ComponentModel.DataAnnotations;

namespace RamSoft_Task_Management.Models;

public class Column
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [Required]
    public int Order { get; set; }
    
    public string? Color { get; set; }
    
    public bool IsDefault { get; set; }
    
    // Navigation property for tasks in this column
    public virtual ICollection<JiraTask>? Tasks { get; set; }
}
