namespace AutoCrudAdmin.Helpers;

using AutoCrudAdmin.Extensions;

/// <summary>
/// The UrlsHelper static class provides a method for generating query parameters for URL construction
/// in the context of the AutoCrudAdmin system.
/// </summary>
internal static class UrlsHelper
{
    /// <summary>
    /// The GetQueryParamForColumnAndFilter method constructs a string to be used as a query parameter in a URL.
    /// The constructed string is a combination of a column name and a filter type, separated by a hyphen.
    /// The filter type string is also transformed into a hyphen-separated word format.
    /// </summary>
    /// <param name="columnName">The name of the column to be included in the query parameter.</param>
    /// <param name="filterType">The type of the filter to be included in the query parameter.</param>
    /// <returns>A string representing a query parameter for a URL.</returns>
    public static string GetQueryParamForColumnAndFilter(string columnName, string filterType)
        => $"{columnName}-{filterType.ToHyphenSeparatedWords()}";
}