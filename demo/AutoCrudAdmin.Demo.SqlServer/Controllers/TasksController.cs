namespace AutoCrudAdmin.Demo.SqlServer.Controllers;

using System;
using System.Collections.Generic;
using System.Linq;
using AutoCrudAdmin.Controllers;
using AutoCrudAdmin.Demo.Models.Models;
using AutoCrudAdmin.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

public class TasksController
    : AutoCrudAdminController<Task>
{
    protected override IEnumerable<Tuple<int, string>> PageSizes
        => new[]
        {
            new Tuple<int, string>(5, "5"),
            new Tuple<int, string>(15, "15"),
            new Tuple<int, string>(25, "25"),
            new Tuple<int, string>(0, "All"),
        };

    protected override IEnumerable<string> HiddenColumnNames
        => new[]
        {
            nameof(Task.EmployeeTasks),
            nameof(Task.LabelType),
        };

    protected override IEnumerable<string> DateTimeFormats
        => new[]
        {
            "d/M/yyyy h:mm:ss tt",
            "d/M/yyyy H:mm:ss tt",
            "dd/MM/yyyy hh:mm:ss tt",
            "M/d/yyyy h:mm:ss tt",
            "M/d/yyyy H:mm:ss tt",
            "MM/dd/yyyy hh:mm:ss tt",
        };

    [InjectScript("/js/tasks.js")]
    public override IActionResult Index()
    {
        return base.Index();
    }

    protected override IQueryable<Task> ApplyIncludes(IQueryable<Task> set)
        => set.Include(x => x.Project);
}