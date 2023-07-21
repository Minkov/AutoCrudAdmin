namespace AutoCrudAdmin.Demo.PostgreSQL.Controllers;

using System;
using System.Collections.Generic;
using System.Linq;
using AutoCrudAdmin.Controllers;
using AutoCrudAdmin.Demo.Models.Models;
using AutoCrudAdmin.Models;
using AutoCrudAdmin.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class ProjectsController
    : AutoCrudAdminController<Project>
{
    protected override IEnumerable<Func<Project, Project, AdminActionContext, ValidatorResult>> EntityValidators
        => new Func<Project, Project, AdminActionContext, ValidatorResult>[]
        {
            (_, newProject, _) => ValidateProjectNameLength(newProject),
            (_, newProject, _) => ValidateProjectNameCharacters(newProject),
        };

    protected override IEnumerable<string> ShownColumnNames
        => new[]
        {
            nameof(Project.Name),
            nameof(Project.Id),
        };

    protected override IEnumerable<GridAction> CustomActions
        => new[]
        {
            new GridAction
            {
                Action = nameof(this.This),
            },
            new GridAction
            {
                Name = nameof(this.That) + " with Id",
                Action = nameof(this.That),
            },
        };

    protected override IEnumerable<CustomGridColumn<Project>> CustomColumns
        => new[]
        {
            new CustomGridColumn<Project>
            {
                Name = "Tasks Count",
                ValueFunc = p => p.Tasks.Count.ToString(),
            },
            new CustomGridColumn<Project>
            {
                Name = "Project Employees",
                ValueFunc = p => string.Join(
                    ", ",
                    p.Tasks
                        .SelectMany(t => t.EmployeeTasks)
                        .Select(et => et.Employee)
                        .Select(e => e.Username)),
            },
        };

    public IActionResult This()
        => this.Ok("It works!");

    public IActionResult That(string id)
        => this.Ok($"It works with Id: {id}");

    protected override IQueryable<Project> ApplyIncludes(IQueryable<Project> set)
        => set.Include(x => x.Tasks)
            .ThenInclude(t => t.EmployeeTasks)
            .ThenInclude(et => et.Employee);

    private static ValidatorResult ValidateProjectNameLength(Project project)
        => project.Name.Length <= 40
            ? ValidatorResult.Success()
            : ValidatorResult.Error("Name must be at max 40 characters");

    private static ValidatorResult ValidateProjectNameCharacters(Project project)
        => project.Name.Contains('@')
            ? ValidatorResult.Error("Name cannot contain '@'")
            : ValidatorResult.Success();
}