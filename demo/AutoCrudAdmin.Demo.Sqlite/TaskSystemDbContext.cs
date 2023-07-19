namespace AutoCrudAdmin.Demo.Sqlite;

using AutoCrudAdmin.Demo.Models.Extensions;
using AutoCrudAdmin.Demo.Models.Models;
using AutoCrudAdmin.Demo.Sqlite.Extensions;
using Microsoft.EntityFrameworkCore;

public class TaskSystemDbContext : DbContext
{
    public TaskSystemDbContext()
    {
    }

    public TaskSystemDbContext(DbContextOptions<TaskSystemDbContext> options)
        : base(options)
    {
    }

    public DbSet<Task> Tasks { get; set; } = default!;

    public DbSet<Employee> Employees { get; set; } = default!;

    public DbSet<Project> Projects { get; set; } = default!;

    public DbSet<EmployeeTasks> EmployeeTasks { get; set; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.Configure();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<EmployeeTasks>()
            .HasKey(et => new { et.EmployeeId, et.TaskId });

        modelBuilder.Seed();
    }
}