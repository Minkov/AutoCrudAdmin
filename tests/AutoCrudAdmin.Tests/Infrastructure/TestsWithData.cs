namespace AutoCrudAdmin.Tests.Infrastructure;

using System;
using Demo.SqlServer;
using Microsoft.EntityFrameworkCore;

public abstract class TestsWithData
{
    protected TestsWithData()
    {
        var options = new DbContextOptionsBuilder<TaskSystemDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.Data = new TaskSystemDbContext(options);
    }

    protected TaskSystemDbContext Data { get; }
}