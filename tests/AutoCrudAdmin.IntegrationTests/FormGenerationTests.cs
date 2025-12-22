namespace AutoCrudAdmin.IntegrationTests;

using AutoCrudAdmin.Controllers;
using AutoCrudAdmin.Demo.Models.Models;
using AutoCrudAdmin.Enumerations;
using AutoCrudAdmin.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public class FormGenerationTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture fixture;

    public FormGenerationTests(DatabaseFixture fixture)
    {
        this.fixture = fixture;
        this.fixture.ResetDatabase();
    }

    [Fact]
    public void GenerateFormControls_ForEmployee_ReturnsCorrectControls()
    {
        var helper = new FormControlsHelper();
        var employee = new Employee
        {
            Id = 1,
            Username = "test.user",
            Email = "test@example.com",
            Phone = "123-456-7890",
        };

        var formControls = helper.GenerateFormControls(employee, EntityAction.Create);

        formControls.Should().NotBeNull();
        formControls.Should().NotBeEmpty();

        var usernameControl = formControls.FirstOrDefault(fc => fc.PropertyName == "Username");
        usernameControl.Should().NotBeNull();
        usernameControl!.IsRequired.Should().BeTrue();

        var emailControl = formControls.FirstOrDefault(fc => fc.PropertyName == "Email");
        emailControl.Should().NotBeNull();
    }

    [Fact]
    public void GenerateFormControls_WithHiddenFields_HidesCorrectControls()
    {
        var controller = this.CreateEmployeeControllerWithHiddenFields();
        var employee = new Employee { Id = 1, Username = "test", Email = "test@example.com", Phone = "123" };

        var formControls = controller.GenerateFormControls(employee, EntityAction.Create).ToList();

        var idControl = formControls.FirstOrDefault(fc => fc.PropertyName == "Id");
        idControl.Should().NotBeNull();
        idControl!.IsHidden.Should().BeTrue();
    }

    [Fact]
    public void GenerateFormControls_WithShownFields_ShowsOnlySpecifiedControls()
    {
        var controller = this.CreateEmployeeControllerWithShownFields();
        var employee = new Employee { Id = 1, Username = "test", Email = "test@example.com", Phone = "123" };

        var formControls = controller.GenerateFormControls(employee, EntityAction.Create).ToList();

        var usernameControl = formControls.FirstOrDefault(fc => fc.PropertyName == "Username");
        usernameControl.Should().NotBeNull();
        usernameControl!.IsHidden.Should().BeFalse();

        var emailControl = formControls.FirstOrDefault(fc => fc.PropertyName == "Email");
        emailControl.Should().NotBeNull();
        emailControl!.IsHidden.Should().BeFalse();
    }

    [Fact]
    public void GenerateFormControls_ForTask_IncludesEnumProperties()
    {
        var helper = new FormControlsHelper();
        var task = new Demo.Models.Models.Task
        {
            Id = 1,
            Name = "Test Task",
            ExecutionType = TaskExecutionType.Sprint,
            LabelType = TaskLabelType.Bug,
        };

        var formControls = helper.GenerateFormControls(task, EntityAction.Create);

        var executionTypeControl = formControls.FirstOrDefault(fc => fc.PropertyName == "ExecutionType");
        executionTypeControl.Should().NotBeNull();

        var labelTypeControl = formControls.FirstOrDefault(fc => fc.PropertyName == "LabelType");
        labelTypeControl.Should().NotBeNull();
    }

    [Fact]
    public void GenerateFormControls_WithReadOnlyFields_SetsReadOnlyCorrectly()
    {
        var controller = this.CreateEmployeeControllerWithReadOnlyFields();
        var employee = new Employee { Id = 1, Username = "test", Email = "test@example.com", Phone = "123" };

        var formControls = controller.GenerateFormControls(employee, EntityAction.Edit).ToList();

        var usernameControl = formControls.FirstOrDefault(fc => fc.PropertyName == "Username");
        usernameControl.Should().NotBeNull();
        usernameControl!.IsReadonly.Should().BeTrue();
    }

    [Fact]
    public void GenerateFormControls_WithRequiredValidation_MarksFieldsAsRequired()
    {
        var helper = new FormControlsHelper();
        var employee = new Employee();

        var formControls = helper.GenerateFormControls(employee, EntityAction.Create);

        var usernameControl = formControls.FirstOrDefault(fc => fc.PropertyName == "Username");
        usernameControl.Should().NotBeNull();
        usernameControl!.IsRequired.Should().BeTrue();
    }

    [Fact]
    public void GenerateFormControls_WithMaxLengthValidation_SetsMaxLength()
    {
        var helper = new FormControlsHelper();
        var employee = new Employee();

        var formControls = helper.GenerateFormControls(employee, EntityAction.Create);

        var usernameControl = formControls.FirstOrDefault(fc => fc.PropertyName == "Username");
        usernameControl.Should().NotBeNull();
        usernameControl!.MaxLength.Should().Be(30);
    }

    [Fact]
    public void ValidateFormControls_WithInvalidData_ReturnsErrors()
    {
        var helper = new FormControlsHelper();
        var employee = new Employee();
        var formControls = helper.GenerateFormControls(employee, EntityAction.Create).ToList();

        var entityDict = new Dictionary<string, string>
        {
            { "Username", string.Empty },
        };

        var errors = new List<string>();
        foreach (var control in formControls)
        {
            if (control.IsRequired && !entityDict.ContainsKey(control.PropertyName))
            {
                errors.Add($"{control.PropertyName} is required");
            }
        }

        errors.Should().NotBeEmpty();
    }

    private EmployeeControllerWithHiddenFields CreateEmployeeControllerWithHiddenFields()
    {
        var controller = new EmployeeControllerWithHiddenFields(this.fixture.DbContext)
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

    private EmployeeControllerWithShownFields CreateEmployeeControllerWithShownFields()
    {
        var controller = new EmployeeControllerWithShownFields(this.fixture.DbContext)
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

    private EmployeeControllerWithReadOnlyFields CreateEmployeeControllerWithReadOnlyFields()
    {
        var controller = new EmployeeControllerWithReadOnlyFields(this.fixture.DbContext)
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

    private class EmployeeControllerWithHiddenFields : AutoCrudAdminController<Employee, TestDbContext>
    {
        public EmployeeControllerWithHiddenFields(TestDbContext context)
            : base(context)
        {
            this.HiddenFormControlNames = new[] { "Id" };
        }
    }

    private class EmployeeControllerWithShownFields : AutoCrudAdminController<Employee, TestDbContext>
    {
        public EmployeeControllerWithShownFields(TestDbContext context)
            : base(context)
        {
            this.ShownFormControlNames = new[] { "Username", "Email" };
        }
    }

    private class EmployeeControllerWithReadOnlyFields : AutoCrudAdminController<Employee, TestDbContext>
    {
        public EmployeeControllerWithReadOnlyFields(TestDbContext context)
            : base(context)
        {
            this.ReadonlyFormControlNames = new[] { "Username" };
        }
    }
}
