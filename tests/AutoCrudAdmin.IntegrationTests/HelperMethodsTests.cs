namespace AutoCrudAdmin.IntegrationTests;

using AutoCrudAdmin.Demo.Models.Models;
using AutoCrudAdmin.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Xunit;

public class HelperMethodsTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture fixture;

    public HelperMethodsTests(DatabaseFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public void ReflectionHelper_FindsDbContexts()
    {
        var dbContexts = ReflectionHelper.DbContexts;

        dbContexts.Should().NotBeNull();
        dbContexts.Should().NotBeEmpty();
        dbContexts.Should().Contain(t => t == typeof(TestDbContext));
    }

    [Fact]
    public void ReflectionHelper_FindsDbSetProperties()
    {
        var dbSetProperties = ReflectionHelper.DbSetProperties;

        dbSetProperties.Should().NotBeNull();
        dbSetProperties.Should().NotBeEmpty();

        var testDbContextProps = dbSetProperties
            .Where(kvp => kvp.Key == typeof(TestDbContext))
            .SelectMany(kvp => kvp.Value)
            .ToList();

        testDbContextProps.Should().Contain(p => p.Name == "Employees");
        testDbContextProps.Should().Contain(p => p.Name == "Projects");
        testDbContextProps.Should().Contain(p => p.Name == "Tasks");
    }

    [Fact]
    public void TypeExtensions_GetPrimaryKeyProperties_ReturnsCorrectKeys()
    {
        var employeeType = typeof(Employee);

        var primaryKeys = employeeType.GetPrimaryKeyProperties();

        primaryKeys.Should().NotBeNull();
        primaryKeys.Should().HaveCount(1);
        primaryKeys.First().Name.Should().Be("Id");
    }

    [Fact]
    public void TypeExtensions_GetPrimaryKeyProperties_ForCompositeKey_ReturnsMultipleKeys()
    {
        var employeeTasksType = typeof(EmployeeTasks);

        var primaryKeys = employeeTasksType.GetPrimaryKeyProperties();

        primaryKeys.Should().NotBeNull();
        primaryKeys.Should().HaveCount(2);

        var keyNames = primaryKeys.Select(p => p.Name).ToList();
        keyNames.Should().Contain("EmployeeId");
        keyNames.Should().Contain("TaskId");
    }

    [Fact]
    public void TypeExtensions_IsSimpleType_IdentifiesPrimitiveTypes()
    {
        typeof(int).IsSimpleType().Should().BeTrue();
        typeof(string).IsSimpleType().Should().BeTrue();
        typeof(bool).IsSimpleType().Should().BeTrue();
        typeof(decimal).IsSimpleType().Should().BeTrue();
        typeof(DateTime).IsSimpleType().Should().BeTrue();
    }

    [Fact]
    public void TypeExtensions_IsSimpleType_IdentifiesComplexTypes()
    {
        typeof(Employee).IsSimpleType().Should().BeFalse();
        typeof(Project).IsSimpleType().Should().BeFalse();
    }

    [Fact]
    public void ExpressionsBuilder_ForByEntityPrimaryKey_BuildsCorrectExpression()
    {
        var complexId = new System.Collections.Generic.Dictionary<string, string>
        {
            { "Id", "1" },
        };

        var expression = ExpressionsBuilder.ForByEntityPrimaryKey<Employee>(complexId);

        expression.Should().NotBeNull();

        var employee = this.fixture.DbContext.Employees.FirstOrDefault(expression);
        employee.Should().NotBeNull();
        employee!.Id.Should().Be(1);
    }

    [Fact]
    public void ExpressionsBuilder_ForByEntityPrimaryKey_WithCompositeKey_BuildsCorrectExpression()
    {
        var employee = new Employee { Username = "test1", Email = "test1@test.com", Phone = "111" };
        var task = new Demo.Models.Models.Task
        {
            Name = "Test Task",
            Description = "Test",
            ExecutionType = TaskExecutionType.Sprint,
            LabelType = TaskLabelType.Bug,
        };
        this.fixture.DbContext.Employees.Add(employee);
        this.fixture.DbContext.Tasks.Add(task);
        this.fixture.DbContext.SaveChanges();

        var employeeTask = new EmployeeTasks
        {
            EmployeeId = employee.Id,
            TaskId = task.Id,
        };
        this.fixture.DbContext.EmployeeTasks.Add(employeeTask);
        this.fixture.DbContext.SaveChanges();

        var complexId = new System.Collections.Generic.Dictionary<string, string>
        {
            { "EmployeeId", employee.Id.ToString() },
            { "TaskId", task.Id.ToString() },
        };

        var expression = ExpressionsBuilder.ForByEntityPrimaryKey<EmployeeTasks>(complexId);

        expression.Should().NotBeNull();

        var foundEmployeeTask = this.fixture.DbContext.EmployeeTasks.FirstOrDefault(expression);
        foundEmployeeTask.Should().NotBeNull();
        foundEmployeeTask!.EmployeeId.Should().Be(employee.Id);
        foundEmployeeTask.TaskId.Should().Be(task.Id);
    }

    [Fact]
    public void FormControlsHelper_GeneratesCorrectNumberOfControls()
    {
        var helper = new FormControlsHelper();
        var employee = new Employee
        {
            Id = 1,
            Username = "test",
            Email = "test@example.com",
            Phone = "123-456-7890",
        };

        var formControls = helper.GenerateFormControls(
            employee,
            AutoCrudAdmin.Enumerations.EntityAction.Create).ToList();

        formControls.Should().NotBeNull();
        formControls.Should().NotBeEmpty();
        formControls.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public void FormControlsHelper_HandlesNullableProperties()
    {
        var helper = new FormControlsHelper();
        var project = new Project
        {
            Id = 1,
            Name = "Test Project",
            Description = null,
        };

        var formControls = helper.GenerateFormControls(
            project,
            AutoCrudAdmin.Enumerations.EntityAction.Create).ToList();

        formControls.Should().NotBeNull();
        var descriptionControl = formControls.FirstOrDefault(fc => fc.PropertyName == "Description");
        descriptionControl.Should().NotBeNull();
    }
}
