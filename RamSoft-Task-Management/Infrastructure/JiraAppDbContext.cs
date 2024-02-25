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

        modelBuilder.Entity<JiraTask>().HasData(
                      new JiraTask
                      {
                          Id = 1,
                          Name = "Task 1",
                          Description = "Description 1",
                          Deadline = DateTime.Now,
                          IsFavorite = true,
                          Status = JiraTaskStatus.InProgress
                      },
                      new JiraTask
                      {
                          Id = 2,
                          Name = "Task 2",
                          Description = "Description 2",
                          Deadline = DateTime.Now,
                          IsFavorite = false,
                          Status = JiraTaskStatus.InProgress
                      });
    }
}
