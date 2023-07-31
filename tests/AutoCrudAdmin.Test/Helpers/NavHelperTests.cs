namespace AutoCrudAdmin.Test.Helpers;

using System.Linq;
using AutoCrudAdmin.Helpers;
using Demo.SqlServer;
using FluentAssertions;
using Xunit;

public class NavHelperTests
{
    [Fact]
    public void GetNavItems_ReturnsDbSetPropertyNames()
    {
        // Arrange
        var dbSetPropertiesNames = ReflectionHelper
            .DbSetProperties
            .Select(p => p.Name)
            .ToList();

        var expectedDbProperties = new[]
        {
            nameof(TaskSystemDbContext.Employees),
            nameof(TaskSystemDbContext.EmployeeTasks),
            nameof(TaskSystemDbContext.Projects),
            nameof(TaskSystemDbContext.Tasks),
        };

        // Act
        var result = NavHelper.GetNavItems().ToList();

        // Assert
        result.Should().NotBeEmpty();
        result.Should().BeEquivalentTo(dbSetPropertiesNames);
        result.Should().BeEquivalentTo(expectedDbProperties);
    }
}