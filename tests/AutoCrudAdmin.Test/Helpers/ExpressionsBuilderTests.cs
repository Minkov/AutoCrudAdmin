namespace AutoCrudAdmin.Test.Helpers;

using System;
using System.Collections.Generic;
using AutoCrudAdmin.Helpers;
using Demo.Models.Models;
using FluentAssertions;
using Xunit;

public class ExpressionsBuilderTests
{
    [Fact]
    public void ForByEntityPrimaryKey_BuildsPredicate()
    {
        // Arrange
        var dict = new Dictionary<string, string>
        {
            [nameof(Task.Id)] = "1",
        };

        // Act
        var result = ExpressionsBuilder.ForByEntityPrimaryKey<Task>(dict);

        // Assert
        result.Should().NotBeNull();
        result.Type.Should().Be(typeof(Func<Task, bool>));
    }

    [Fact]
    public void ForGetProperty_BuildsPredicate()
    {
        // Arrange
        var property = typeof(Task).GetProperty(nameof(Task.Name)) !;

        // Act
        var result = ExpressionsBuilder.ForGetProperty<Task, string>(property);

        // Assert
        result.Should().NotBeNull();
        result.Type.Should().Be(typeof(Func<Task, string>));
    }

    [Fact]
    public void ForCreateInstance_ReturnsFactoryMethod()
    {
        // Act
        var result = ExpressionsBuilder.ForCreateInstance<Task>();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Func<Task>>();
        result().Should().BeOfType<Task>();
    }

    [Theory]
    [InlineData(1)]
    public void ForGetPropertyValue_ReturnsValueGetter(int id)
    {
        // Arrange
        var property = typeof(Task).GetProperty(nameof(Task.Id)) !;

        var entity = new Task { Id = id };

        // Act
        var result = ExpressionsBuilder.ForGetPropertyValue<Task>(property);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Func<Task, object>>();
        result(entity).Should().Be(id);
    }
}