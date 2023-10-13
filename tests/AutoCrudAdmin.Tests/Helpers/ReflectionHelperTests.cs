namespace AutoCrudAdmin.Tests.Helpers;

using System.Linq;
using AutoCrudAdmin.Helpers;
using Demo.SqlServer;
using FluentAssertions;
using Xunit;

public class ReflectionHelperTests
{
    [Fact]
    public void DbContexts_ShouldReturnAllDbContextTypes()
    {
        // Arrange
        var dbContexts = ReflectionHelper.DbContexts;

        // Assert
        dbContexts.Should().ContainSingle(t => t == typeof(TaskSystemDbContext));
    }

    [Fact]
    public void DbSetProperties_ShouldReturnAllDbSetProperties()
    {
        // Arrange
        var dbSetProperties = ReflectionHelper.DbSetProperties;

        var expectedDbProperties = new[]
        {
            nameof(TaskSystemDbContext.Tasks),
            nameof(TaskSystemDbContext.Employees),
            nameof(TaskSystemDbContext.Projects),
            nameof(TaskSystemDbContext.EmployeeTasks),
        };

        // Assert
        dbSetProperties.Select(p => p.Name).Should().BeEquivalentTo(expectedDbProperties);
    }
}