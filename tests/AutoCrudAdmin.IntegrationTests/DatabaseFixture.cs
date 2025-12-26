namespace AutoCrudAdmin.IntegrationTests;

using AutoCrudAdmin.Demo.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

public class DatabaseFixture : IDisposable
{
    public DatabaseFixture()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        this.DbContext = new TestDbContext(options);
        this.SeedDatabase().Wait();
    }

    public TestDbContext DbContext { get; private set; }

    public void Dispose()
    {
        this.DbContext?.Dispose();
    }

    private async Task SeedDatabase()
    {
        var employee1 = new Employee
        {
            Id = 1,
            Username = "john.doe",
            Email = "john@example.com",
            Phone = "123-456-7890",
        };

        var employee2 = new Employee
        {
            Id = 2,
            Username = "jane.smith",
            Email = "jane@example.com",
            Phone = "098-765-4321",
        };

        var project1 = new Project
        {
            Id = 1,
            Name = "Project Alpha",
            Description = "Test project description",
        };

        var task1 = new Demo.Models.Models.Task
        {
            Id = 1,
            Name = "Task 1",
            Description = "Test task",
            ExecutionType = TaskExecutionType.Sprint,
            LabelType = TaskLabelType.Bug,
            ProjectId = 1,
        };

        this.DbContext.Employees.AddRange(employee1, employee2);
        this.DbContext.Projects.Add(project1);
        this.DbContext.Tasks.Add(task1);

        await this.DbContext.SaveChangesAsync();
    }

    public void ResetDatabase()
    {
        this.DbContext.Database.EnsureDeleted();
        this.DbContext.Database.EnsureCreated();
        this.SeedDatabase().Wait();
    }
}
