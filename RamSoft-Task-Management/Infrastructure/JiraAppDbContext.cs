using Microsoft.EntityFrameworkCore;
using RamSoft_Task_Management.Enums;
using RamSoft_Task_Management.Models;

namespace RamSoft_Task_Management.Infrastructure;

public class JiraAppDbContext : DbContext
{
    public JiraAppDbContext(DbContextOptions<JiraAppDbContext> options) : base(options)
    {
    }
    
    public DbSet<JiraTask> JiraTask { get; set; }
    public DbSet<Column> Columns { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure JiraTask entity
        modelBuilder.Entity<JiraTask>().HasKey(x => x.Id);
        modelBuilder.Entity<JiraTask>().Property(x => x.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<JiraTask>().Property(x => x.Name).HasMaxLength(100);
        modelBuilder.Entity<JiraTask>().Property(x => x.Name).IsRequired();
        modelBuilder.Entity<JiraTask>().Property(x => x.Description).IsRequired();
        modelBuilder.Entity<JiraTask>().Property(x => x.Deadline).IsRequired();
        
        // Configure Column entity
        modelBuilder.Entity<Column>().HasKey(c => c.Id);
        modelBuilder.Entity<Column>().Property(c => c.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Column>().Property(c => c.Name).IsRequired().HasMaxLength(50);
        modelBuilder.Entity<Column>().Property(c => c.Order).IsRequired();
        
        // Configure relationship between Column and JiraTask
        modelBuilder.Entity<Column>()
            .HasMany(c => c.Tasks)
            .WithOne(t => t.Column)
            .HasForeignKey(t => t.ColumnId)
            .OnDelete(DeleteBehavior.SetNull);
        
        // Seed default columns based on existing statuses
        modelBuilder.Entity<Column>().HasData(
            new Column { Id = 1, Name = "Unassigned", Order = 1, IsDefault = true, Color = "#808080" },
            new Column { Id = 2, Name = "Approved", Order = 2, IsDefault = false, Color = "#FFA500" },
            new Column { Id = 3, Name = "In Progress", Order = 3, IsDefault = false, Color = "#1E90FF" },
            new Column { Id = 4, Name = "Done", Order = 4, IsDefault = false, Color = "#32CD32" }
        );
    }
}
