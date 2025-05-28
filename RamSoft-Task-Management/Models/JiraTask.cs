using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RamSoft_Task_Management.Enums;

namespace RamSoft_Task_Management.Models;

public class JiraTask
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string? Name { get; set; }
    
    [Required]
    public string? Description { get; set; }
    
    [Required]
    public DateTime Deadline { get; set; }
    
    public bool IsFavorite { get; set; }
    
    public JiraTaskStatus Status { get; set; }
    
    public string? ImageURL { get; set; }
    
    // New properties for Column relationships
    public int? ColumnId { get; set; }
    
    [ForeignKey("ColumnId")]
    public virtual Column? Column { get; set; }
    
    // Position within the column for UI drag and drop
    public int Position { get; set; }
}
