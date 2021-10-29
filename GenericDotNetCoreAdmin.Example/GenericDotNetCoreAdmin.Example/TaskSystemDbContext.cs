namespace GenericDotNetCoreAdmin.Example
{
    using GenericDotNetCoreAdmin.Example.Models;
    using Microsoft.EntityFrameworkCore;

    public class TaskSystemDbContext : DbContext
    {
        public TaskSystemDbContext(DbContextOptions<TaskSystemDbContext> options)
            : base(options)
        {
        }

        public DbSet<Task> Tasks { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<EmployeeTasks> EmployeeTasks { get; set; }
    }
}