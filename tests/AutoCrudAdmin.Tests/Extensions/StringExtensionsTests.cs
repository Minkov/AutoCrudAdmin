namespace AutoCrudAdmin.Tests.Extensions;

using AutoCrudAdmin.Extensions;
using FluentAssertions;
using Xunit;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("PascalCase", "pascal-case")]
    [InlineData("camelCase", "camel-case")]
    public void ToHyphenSeparatedWords_ReturnsExpected(string input, string expected)
    {
        // Act
        var result = input.ToHyphenSeparatedWords();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("PascalCase", "Pascal Case")]
    public void ToSpaceSeparatedWords_ReturnsExpected(string input, string expected)
    {
        // Act
        var result = input.ToSpaceSeparatedWords();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("TestController", "Test")]
    [InlineData("ProductsController", "Products")]
    [InlineData("AdminController", "Admin")]
    public void ToControllerBaseUri_RemovesControllerSuffix(string input, string expected)
    {
        // Act
        var result = input.ToControllerBaseUri();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("This is a very long string", 5)]
    public void ToEllipsis_ShortensString(string input, int maxLength)
    {
        // Act
        var result = input.ToEllipsis(maxLength);

        // Assert
        result.Should().Be("Thi...");
    }

    [Theory]
    [InlineData("Short", 10)]
    public void ToEllipsis_ReturnsOriginalIfShorter(string input, int maxLength)
    {
        // Act
        var result = input.ToEllipsis(maxLength);

        // Assert
        result.Should().Be("Short");
    }
}