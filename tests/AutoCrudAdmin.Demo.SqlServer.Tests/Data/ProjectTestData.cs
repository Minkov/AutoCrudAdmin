namespace AutoCrudAdmin.Demo.SqlServer.Tests.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using Models.Models;

public static class ProjectTestData
{
    public static Project GetProject(int id)
        => new Faker<Project>()
            .CustomInstantiator(_ => new Project
            {
                Id = id,
                Name = $"Project {id}",
                OpenDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(1),
            })
            .Generate();

    public static List<Project> GetProjects(int total)
        => Enumerable
            .Range(1, total)
            .Select(GetProject)
            .ToList();
}