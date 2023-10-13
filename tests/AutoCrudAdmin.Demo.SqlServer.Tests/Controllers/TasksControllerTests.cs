namespace AutoCrudAdmin.Demo.SqlServer.Tests.Controllers;

using System.Collections.Generic;
using MyTested.AspNetCore.Mvc;
using SqlServer.Controllers;
using ViewModels.Pages;
using Xunit;

public class TasksControllerTests
{
    [Fact]
    public void IndexShouldReturnDefaultViewWithCorrectModel()
        => MyController<TasksController>
            .Instance()
            .Calling(c => c.Index())
            .ShouldReturn()
            .View(view => view
                .WithModelOfType<AutoCrudAdminIndexViewModel>());

    [Fact]
    public void GetCreate_ReturnsViewResult()
        => MyController<TasksController>
            .Instance()
            .Calling(c => c.Create(
                new Dictionary<string, string>(),
                null))
            .ShouldReturn()
            .View("../AutoCrudAdmin/EntityForm");
}