namespace AutoCrudAdmin.Demo.Models.Extensions;

using System;
using System.Collections.Generic;
using AutoCrudAdmin.Demo.Models.Models;
using Microsoft.EntityFrameworkCore;

public static class DbContextOptionsBuilderExtensions
{
    private static IEnumerable<Project> Projects => new[]
    {
        new Project
        {
            Id = 1,
            Name = "Setup migration to PostgreSQL",
            OpenDate = DateTime.Now,
            DueDate = DateTime.Parse("2022-01-01 12:03:25.0000000"),
        },
        new Project
        {
            Id = 2,
            Name = "Update packages",
            OpenDate = DateTime.Now,
            DueDate = DateTime.Parse("2021-10-04 12:03:25.0000000"),
        },
        new Project
        {
            Id = 3,
            Name = "Integrate AutoCrudAdmin",
            OpenDate = DateTime.Now,
            DueDate = DateTime.Parse("2021-11-08 12:03:25.0000000"),
        },
    };

    private static IEnumerable<Task> Tasks =>
        new[]
        {
            new Task
            {
                Id = 1,
                Name = "Check incompatible entities",
                DueDate = DateTime.Parse("2021/11/01"),
                ExecutionType = TaskExecutionType.Finished,
                ProjectId = 1,
            },
            new Task
            {
                Id = 2,
                Name = "Change Db connection",
                DueDate = DateTime.Parse("2021/12/01"),
                ExecutionType = TaskExecutionType.InProgress,
                ProjectId = 1,
            },
            new Task
            {
                Id = 3,
                Name = "Setup PostgreSQL server",
                DueDate = DateTime.Parse("2022/01/01"),
                ExecutionType = TaskExecutionType.ProductBackLog,
                ProjectId = 1,
            },
            new Task
            {
                Id = 4,
                Name = "Update all packages",
                DueDate = DateTime.Parse("2021/10/04"),
                ExecutionType = TaskExecutionType.Finished,
                ProjectId = 2,
            },
            new Task
            {
                Id = 5,
                Name = "Install AutoCrudAdmin",
                DueDate = DateTime.Parse("2021/11/08"),
                ExecutionType = TaskExecutionType.InProgress,
                ProjectId = 3,
            },
            new Task
            {
                Id = 6,
                Name = "Setup AutoCrudAdmin",
                DueDate = DateTime.Parse("2021/11/08"),
                ExecutionType = TaskExecutionType.InProgress,
                ProjectId = 3,
            },
        };

    public static void Seed(this ModelBuilder builder)
    {
        builder.Entity<Project>()
            .HasData(Projects);

        builder.Entity<Task>()
            .HasData(Tasks);
    }
}