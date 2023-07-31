namespace AutoCrudAdmin.Test.Extensions;

using System.Linq;
using AutoCrudAdmin.Extensions;
using Demo.Models.Models;
using FluentAssertions;
using Infrastructure;
using Xunit;

public class DbContextExtensionsTests : TestsWithData
{
    [Fact]
    public void Set_ReturnsQueryableOfGivenType()
    {
        // Arrange
        var entityType = typeof(Project);

        // Act
        var result = this.Data.Set(entityType);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IQueryable<Project>>();
        result.ElementType.Should().Be(entityType);
    }
}