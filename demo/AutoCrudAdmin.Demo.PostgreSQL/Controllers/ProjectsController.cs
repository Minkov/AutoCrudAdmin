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
using NonFactors.Mvc.Grid;

public class ProjectsController
    : AutoCrudAdminController<Project>
{
    protected override IEnumerable<Func<Project, Project, AdminActionContext, ValidatorResult>> EntityValidators
        => new[]
        {
            ValidateProjectNameLength,
            ValidateProjectNameCharacters,
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

    public IActionResult This()
        => this.Ok("It works!");

    public IActionResult That(string id)
        => this.Ok($"It works with Id: {id}");

    protected override IGridColumnsOf<Project> BuildGridColumns(
        IGridColumnsOf<Project> columns,
        int? stringMaxLength)
    {
        base.BuildGridColumns(columns, stringMaxLength);

        columns.Add(c => c.Tasks.Count).Titled("Tasks Count");
        columns.Add(p => string.Join(
                ", ",
                p.Tasks
                    .SelectMany(t => t.EmployeeTasks)
                    .Select(et => et.Employee)
                    .Select(e => e.Username)))
            .Titled("Project Employees");

        return columns;
    }

    protected override IQueryable<Project> ApplyIncludes(IQueryable<Project> set)
        => set.Include(x => x.Tasks)
            .ThenInclude(t => t.EmployeeTasks)
            .ThenInclude(et => et.Employee);

    private static ValidatorResult ValidateProjectNameLength(
        Project existingProject,
        Project newProject,
        AdminActionContext context)
        => newProject.Name.Length <= 40
            ? ValidatorResult.Success()
            : ValidatorResult.Error("Name must be at max 40 characters");

    private static ValidatorResult ValidateProjectNameCharacters(
        Project existingProject,
        Project newProject,
        AdminActionContext context)
        => newProject.Name.Contains('@')
            ? ValidatorResult.Error("Name cannot contain '@'")
            : ValidatorResult.Success();
}