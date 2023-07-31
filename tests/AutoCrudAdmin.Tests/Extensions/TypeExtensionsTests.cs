namespace AutoCrudAdmin.Tests.Extensions;

using System.Collections.Generic;
using AutoCrudAdmin.Extensions;
using Demo.Models.Models;
using FluentAssertions;
using Infrastructure;
using Xunit;

public class TypeExtensionsTests : TestsWithData
{
    [Fact]
    public void GetPrimaryKeyPropertyInfos_ReturnsExpected()
    {
        // Arrange
        var type = typeof(Project);

        // Act
        var result = type.GetPrimaryKeyPropertyInfos();

        // Assert
        result.Should().Contain(p => p.Name == nameof(Project.Id));
    }

    [Theory]
    [InlineData(1)]
    public void GetPrimaryKeyValue_ReturnsExpected(int id)
    {
        // Arrange
        var entity = new Project { Id = id };

        // Act
        var result = typeof(Project).GetPrimaryKeyValue(entity);

        // Assert
        result.Should().AllSatisfy(item =>
        {
            item.Key.Should().Be(Constants.Entity.SinglePrimaryKeyName);
            item.Value.Should().Be(id);
        });
    }

    [Fact]
    public void IsSubclassOfRawGeneric_ShouldDetectsSubclass()
    {
        // Arrange
        var subtype = typeof(TestSubEntity);
        var baseType = typeof(TestBaseEntity<>);

        // Act
        var result = subtype.IsSubclassOfRawGeneric(baseType);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsSubclassOfRawGeneric_ShouldReturnFalseForSameType()
    {
        // Arrange
        var type = typeof(TestBaseEntity<>);

        // Act
        var result = type.IsSubclassOfRawGeneric(type);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsSubclassOfAnyType_DetectsSubclasses()
    {
        // Arrange
        var subclass = typeof(TestSubEntity);
        var baseClass = typeof(TestBaseEntity);
        var genericBaseClass = typeof(TestBaseEntity<>);

        // Act
        var result1 = subclass.IsSubclassOfAnyType(baseClass);
        var result2 = subclass.IsSubclassOfAnyType(genericBaseClass);

        // Assert
        result1.Should().BeTrue(); // Subclass of direct base class
        result2.Should().BeTrue(); // Subclass of generic base class
    }

    [Fact]
    public void IsSubclassOfAnyType_ReturnsFalseWhenNoRelation()
    {
        // Arrange
        var type1 = typeof(TestBaseEntity);
        var type2 = typeof(Project);

        // Act
        var result = type1.IsSubclassOfAnyType(type2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsNavigationProperty_DetectsNavigationTypes()
    {
        // Arrange
        var navigationType = typeof(ICollection<Project>);
        var nonNavigationType = typeof(int);

        // Act
        var result1 = navigationType.IsNavigationProperty();
        var result2 = nonNavigationType.IsNavigationProperty();

        // Assert
        result1.Should().BeTrue();
        result2.Should().BeFalse();
    }

    [Fact]
    public void IsNavigationProperty_HandlesString()
    {
        // Arrange

        // Act
        var result = typeof(string).IsNavigationProperty();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void UnProxy_ReturnsSameTypeIfNotProxy()
    {
        // Arrange
        var type = typeof(Project);

        // Act
        var result = type.UnProxy();

        // Assert
        result.Should().BeSameAs(type);
    }

    [Fact]
    public void IsEnumerableExceptString_DetectsEnumerableTypes()
    {
        // Arrange
        var enumerableType = typeof(List<int>);

        // Act
        var result = enumerableType.IsEnumerableExceptString();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsEnumerableExceptString_ExcludesString()
    {
        // Act
        var result = typeof(string).IsEnumerableExceptString();

        // Assert
        result.Should().BeFalse();
    }

    private class TestBaseEntity
    {
    }

    private class TestBaseEntity<T> : TestBaseEntity
    {
    }

    private class TestSubEntity : TestBaseEntity<int>
    {
    }
}