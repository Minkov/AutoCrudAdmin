namespace AutoCrudAdmin.Demo.Sqlite
{
    using AutoCrudAdmin.Demo.Models;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<EmployeeTasks>()
                .HasKey(et => new { et.EmployeeId, et.TaskId });
            modelBuilder.Seed();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.Configure();

        public DbSet<Task> Tasks { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<EmployeeTasks> EmployeeTasks { get; set; }
    }
}