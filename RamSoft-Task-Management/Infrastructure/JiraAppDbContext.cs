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
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<JiraTask>().HasKey(x => x.Id);
        modelBuilder.Entity<JiraTask>().Property(x => x.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<JiraTask>().Property(x => x.Name).HasMaxLength(100);
        modelBuilder.Entity<JiraTask>().Property(x => x.Name).IsRequired();
        modelBuilder.Entity<JiraTask>().Property(x => x.Description).IsRequired();
        modelBuilder.Entity<JiraTask>().Property(x => x.Deadline).IsRequired();

    }
}
