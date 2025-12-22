namespace AutoCrudAdmin.IntegrationTests;

using AutoCrudAdmin.Demo.Models.Models;
using Microsoft.EntityFrameworkCore;

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; } = default!;

    public DbSet<Project> Projects { get; set; } = default!;

    public DbSet<Task> Tasks { get; set; } = default!;

    public DbSet<EmployeeTasks> EmployeeTasks { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<EmployeeTasks>()
            .HasKey(et => new { et.EmployeeId, et.TaskId });
    }
}
