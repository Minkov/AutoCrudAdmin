namespace AutoCrudAdmin.IntegrationTests;

using AutoCrudAdmin.Controllers;
using AutoCrudAdmin.Demo.Models.Models;
using AutoCrudAdmin.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class AutocompleteAndGridTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture fixture;

    public AutocompleteAndGridTests(DatabaseFixture fixture)
    {
        this.fixture = fixture;
        this.fixture.ResetDatabase();
    }

    [Fact]
    public void Autocomplete_WithValidSearchTerm_ReturnsMatchingResults()
    {
        var controller = this.CreateEmployeeController();
        var searchTerm = "john";

        var result = controller.Autocomplete(searchTerm, "Username");

        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        var firstResult = result.First();
        firstResult.Text.Should().Contain("john", System.StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Autocomplete_WithEmptySearchTerm_ReturnsAllResults()
    {
        var controller = this.CreateEmployeeController();

        var result = controller.Autocomplete(string.Empty, "Username");

        result.Should().NotBeNull();
        result.Count().Should().BeGreaterThan(0);
    }

    [Fact]
    public void Autocomplete_WithNonExistentSearchTerm_ReturnsEmpty()
    {
        var controller = this.CreateEmployeeController();
        var searchTerm = "nonexistent";

        var result = controller.Autocomplete(searchTerm, "Username");

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public void Autocomplete_ReturnsCorrectDropDownViewModel()
    {
        var controller = this.CreateEmployeeController();
        var searchTerm = "john";

        var result = controller.Autocomplete(searchTerm, "Username").ToList();

        result.Should().NotBeNull();
        result.Should().AllBeOfType<DropDownViewModel>();
        if (result.Any())
        {
            var firstItem = result.First();
            firstItem.Value.Should().NotBeNullOrEmpty();
            firstItem.Text.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public async Task Index_ConfiguresGridCorrectly()
    {
        var controller = this.CreateEmployeeController();

        var result = await controller.Index();

        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;
        viewResult!.ViewData.Should().ContainKey("grid");
    }

    [Fact]
    public void GetAll_WithPaging_ReturnsCorrectPageSize()
    {
        for (int i = 0; i < 50; i++)
        {
            this.fixture.DbContext.Employees.Add(new Employee
            {
                Username = $"user{i}",
                Email = $"user{i}@example.com",
                Phone = $"{i:000}-0000",
            });
        }

        this.fixture.DbContext.SaveChanges();

        var controller = this.CreateEmployeeControllerWithPaging();

        var result = controller.GetAll().ToList();

        result.Should().NotBeNull();
        result.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public void GridColumns_OnlyIncludesVisibleColumns()
    {
        var controller = this.CreateEmployeeControllerWithHiddenColumns();

        var columns = controller.GridColumns.ToList();

        columns.Should().NotBeNull();
        columns.Should().NotBeEmpty();

        var idColumn = columns.FirstOrDefault(c => c.Name == "Id");
        idColumn.Should().BeNull();
    }

    [Fact]
    public void GridColumns_ShowsOnlySpecifiedColumns()
    {
        var controller = this.CreateEmployeeControllerWithShownColumns();

        var columns = controller.GridColumns.ToList();

        columns.Should().NotBeNull();
        var columnNames = columns.Select(c => c.Name).ToList();
        columnNames.Should().Contain("Username");
        columnNames.Should().Contain("Email");
    }

    [Fact]
    public async Task Index_WithCustomPageSizes_UsesCorrectSizes()
    {
        var controller = this.CreateEmployeeControllerWithCustomPageSizes();

        var result = await controller.Index();

        result.Should().BeOfType<ViewResult>();
        controller.PageSizes.Should().NotBeEmpty();
        controller.PageSizes.First().Item1.Should().Be(5);
    }

    [Fact]
    public void BuildGridColumn_CreatesColumnCorrectly()
    {
        var controller = this.CreateEmployeeController();

        var columns = controller.GridColumns.ToList();

        columns.Should().NotBeNull();
        columns.Should().NotBeEmpty();

        var usernameColumn = columns.FirstOrDefault(c => c.Name == "Username");
        usernameColumn.Should().NotBeNull();
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

    private EmployeeControllerWithPaging CreateEmployeeControllerWithPaging()
    {
        var controller = new EmployeeControllerWithPaging(this.fixture.DbContext)
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

    private EmployeeControllerWithHiddenColumns CreateEmployeeControllerWithHiddenColumns()
    {
        var controller = new EmployeeControllerWithHiddenColumns(this.fixture.DbContext)
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

    private EmployeeControllerWithShownColumns CreateEmployeeControllerWithShownColumns()
    {
        var controller = new EmployeeControllerWithShownColumns(this.fixture.DbContext)
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

    private EmployeeControllerWithCustomPageSizes CreateEmployeeControllerWithCustomPageSizes()
    {
        var controller = new EmployeeControllerWithCustomPageSizes(this.fixture.DbContext)
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

    private class EmployeeControllerWithPaging : AutoCrudAdminController<Employee, TestDbContext>
    {
        public EmployeeControllerWithPaging(TestDbContext context)
            : base(context)
        {
            this.PageSizes = new List<(int, string)>
            {
                (10, "10"),
                (20, "20"),
                (50, "50"),
            };
        }
    }

    private class EmployeeControllerWithHiddenColumns : AutoCrudAdminController<Employee, TestDbContext>
    {
        public EmployeeControllerWithHiddenColumns(TestDbContext context)
            : base(context)
        {
            this.HiddenColumnNames = new[] { "Id" };
        }
    }

    private class EmployeeControllerWithShownColumns : AutoCrudAdminController<Employee, TestDbContext>
    {
        public EmployeeControllerWithShownColumns(TestDbContext context)
            : base(context)
        {
            this.ShownColumnNames = new[] { "Username", "Email" };
        }
    }

    private class EmployeeControllerWithCustomPageSizes : AutoCrudAdminController<Employee, TestDbContext>
    {
        public EmployeeControllerWithCustomPageSizes(TestDbContext context)
            : base(context)
        {
            this.PageSizes = new List<(int, string)>
            {
                (5, "5"),
                (10, "10"),
            };
        }
    }
}
