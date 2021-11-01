namespace GenericDotNetCoreAdmin.Example.Controllers
{
    using System.Collections.Generic;
    using GenericDotNetCoreAdmin.Controllers;
    using GenericDotNetCoreAdmin.Example.Models;

    public class TasksController
        : GenericAdminController<Task>
    {
        protected override IEnumerable<string> HiddenColumnNames
            => new[] { nameof(Task.Name) };
    }
}