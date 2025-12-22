namespace AutoCrudAdmin.IntegrationTests;

using AutoCrudAdmin.Controllers;
using AutoCrudAdmin.Demo.Models.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class CrudOperationsTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture fixture;

    public CrudOperationsTests(DatabaseFixture fixture)
    {
        this.fixture = fixture;
        this.fixture.ResetDatabase();
    }

    [Fact]
    public async Task Index_ReturnsViewResult_WithEmployees()
    {
        var controller = this.CreateEmployeeController();

        var result = await controller.Index();

        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;
        viewResult!.ViewData.Should().ContainKey("grid");
    }

    [Fact]
    public async Task Create_Get_ReturnsViewResult_WithFormControls()
    {
        var controller = this.CreateEmployeeController();

        var result = await controller.Create();

        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;
        viewResult!.Model.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_Post_ValidEmployee_AddsToDatabase()
    {
        var controller = this.CreateEmployeeController();
        var employeeDict = new Dictionary<string, string>
        {
            { "Username", "new.employee" },
            { "Email", "new@example.com" },
            { "Phone", "555-1234" },
        };

        var result = await controller.Create(employeeDict);

        result.Should().BeOfType<RedirectToActionResult>();
        var redirectResult = result as RedirectToActionResult;
        redirectResult!.ActionName.Should().Be("Index");

        var newEmployee = await this.fixture.DbContext.Employees
            .FirstOrDefaultAsync(e => e.Username == "new.employee");
        newEmployee.Should().NotBeNull();
        newEmployee!.Email.Should().Be("new@example.com");
    }

    [Fact]
    public async Task Create_Post_InvalidEmployee_ReturnsViewWithErrors()
    {
        var controller = this.CreateEmployeeController();
        var employeeDict = new Dictionary<string, string>
        {
            { "Email", "invalid@example.com" },
        };

        var result = await controller.Create(employeeDict);

        result.Should().BeOfType<ViewResult>();
        controller.TempData["danger_message"].Should().NotBeNull();
    }

    [Fact]
    public async Task Edit_Get_ExistingEmployee_ReturnsViewWithEmployee()
    {
        var controller = this.CreateEmployeeController();
        var complexId = new Dictionary<string, string> { { "Id", "1" } };

        var result = await controller.Edit(complexId);

        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;
        viewResult!.Model.Should().NotBeNull();
    }

    [Fact]
    public async Task Edit_Get_NonExistentEmployee_ReturnsNotFound()
    {
        var controller = this.CreateEmployeeController();
        var complexId = new Dictionary<string, string> { { "Id", "999" } };

        var result = await controller.Edit(complexId);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Edit_Post_ValidData_UpdatesEmployee()
    {
        var controller = this.CreateEmployeeController();
        var complexId = new Dictionary<string, string> { { "Id", "1" } };
        var employeeDict = new Dictionary<string, string>
        {
            { "Id", "1" },
            { "Username", "updated.username" },
            { "Email", "updated@example.com" },
            { "Phone", "999-8888" },
        };

        var result = await controller.Edit(complexId, employeeDict);

        result.Should().BeOfType<RedirectToActionResult>();

        var updatedEmployee = await this.fixture.DbContext.Employees.FindAsync(1);
        updatedEmployee.Should().NotBeNull();
        updatedEmployee!.Username.Should().Be("updated.username");
        updatedEmployee.Email.Should().Be("updated@example.com");
    }

    [Fact]
    public async Task Delete_Get_ExistingEmployee_ReturnsViewWithEmployee()
    {
        var controller = this.CreateEmployeeController();
        var complexId = new Dictionary<string, string> { { "Id", "1" } };

        var result = await controller.Delete(complexId);

        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;
        viewResult!.Model.Should().NotBeNull();
    }

    [Fact]
    public async Task Delete_Post_ExistingEmployee_RemovesFromDatabase()
    {
        var employee = new Employee
        {
            Id = 100,
            Username = "to.delete",
            Email = "delete@example.com",
            Phone = "000-0000",
        };
        this.fixture.DbContext.Employees.Add(employee);
        await this.fixture.DbContext.SaveChangesAsync();

        var controller = this.CreateEmployeeController();
        var complexId = new Dictionary<string, string> { { "Id", "100" } };

        var result = await controller.DeleteConfirmed(complexId);

        result.Should().BeOfType<RedirectToActionResult>();

        var deletedEmployee = await this.fixture.DbContext.Employees.FindAsync(100);
        deletedEmployee.Should().BeNull();
    }

    [Fact]
    public async Task GetAll_ReturnsAllEmployees()
    {
        var controller = this.CreateEmployeeController();

        var result = controller.GetAll();

        result.Should().NotBeNull();
        result.Count().Should().BeGreaterThan(0);
    }

    private EmployeeController CreateEmployeeController()
    {
        var controller = new EmployeeController(this.fixture.DbContext)
        {
            TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>()),
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            },
        };

        controller.HttpContext.Items["layout_name"] = "_Layout";
        controller.HttpContext.Items["application_name"] = "Test App";

        return controller;
    }

    private class EmployeeController : AutoCrudAdminController<Employee, TestDbContext>
    {
        public EmployeeController(TestDbContext context)
            : base(context)
        {
        }
    }
}
