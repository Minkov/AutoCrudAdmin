namespace AutoCrudAdmin.Test.Helpers;

using AutoCrudAdmin.Helpers;
using FluentAssertions;
using Xunit;

public class UrlsHelperTests
{
    [Theory]
    [InlineData("Column1", "Equals", "Column1-equals")]
    [InlineData("ColumnWithCamelCase", "NotEquals", "ColumnWithCamelCase-not-equals")]
    [InlineData("Column_With_Underscores", "Contains", "Column_With_Underscores-contains")]
    public void GetQueryParamForColumnAndFilter_ReturnsExpected(string columnName, string filterName, string expected)
    {
        // Act
        var result = UrlsHelper.GetQueryParamForColumnAndFilter(columnName, filterName);

        // Assert
        expected.Should().Be(result);
    }
}