namespace GenericDotNetCoreAdmin.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using GenericDotNetCoreAdmin.Models;
    using GenericDotNetCoreAdmin.ViewModels;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using NonFactors.Mvc.Grid;

    public class ProjectsController
        : GenericAdminController<Project>
    {
        protected override IQueryable<Project> Set
            => base.Set.Include(x => x.Tasks)
                .ThenInclude(t => t.EmployeeTasks)
                .ThenInclude(et => et.Employee);

        protected override IEnumerable<string> ColumnNames
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
                    Name = nameof(this.This),
                    Action = nameof(this.This),
                },
                new GridAction
                {
                    Name = nameof(this.That),
                    Action = nameof(this.That),
                },
            };

        protected override IGridColumnsOf<Project> BuildGridColumns(IGridColumnsOf<Project> columns)
        {
            base.BuildGridColumns(columns);
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

        public IActionResult This()
            => this.Ok("It works!");

        public IActionResult That(string id)
            => this.Ok($"It works with Id: {id}");
    }
}