namespace AutoCrudAdmin.Demo.SqlServer.Tests.Controllers;

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
}