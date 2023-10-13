namespace AutoCrudAdmin.Demo.SqlServer.Tests.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using Models.Models;

public static class TaskTestData
{
    public static Task GetTask(int id, int projectId)
        => new Faker<Task>()
            .CustomInstantiator(f => new Task
            {
                Id = id,
                Name = $"Task {id}",
                OpenDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(1),
                LabelType = f.PickRandom<TaskLabelType>(),
                ExecutionType = f.PickRandom<TaskExecutionType>(),
                ProjectId = projectId,
            })
            .Generate();

    public static List<Task> GetTasks(int total, int totalProjects)
        => Enumerable
            .Range(1, total)
            .Select(i => GetTask(i, new Faker().Random.Number(1, totalProjects)))
            .ToList();
}