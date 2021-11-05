namespace GenericDotNetCoreAdmin.Example.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoCrudAdmin.Controllers;
    using GenericDotNetCoreAdmin.Example.Models;
    using Microsoft.EntityFrameworkCore;

    public class TasksController
        : AutoCrudAdminController<Task>
    {
        // protected override IEnumerable<Tuple<int, string>> PageSizes
        //     => new[]
        //     {
        //         new Tuple<int, string>(5, "5"),
        //         new Tuple<int, string>(15, "15"),
        //         new Tuple<int, string>(25, "25"),
        //         new Tuple<int, string>(0, "All"),
        //     };

        protected override IQueryable<Task> Set
            => base.Set.Include(x => x.Project);

        protected override IEnumerable<string> HiddenColumnNames
            => new[] { nameof(Task.EmployeeTasks), nameof(Task.LabelType) };
    }
}