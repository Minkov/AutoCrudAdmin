namespace AutoCrudAdmin.Extensions;

using AutoCrudAdmin.Helpers;
using Microsoft.AspNetCore.Http;

/// <summary>
/// The HttpRequestExtensions class provides extension methods for the HttpRequest class.
/// These extensions add functionality related to the AutoCrudAdmin system.
/// </summary>
internal static class HttpRequestExtensions
{
    /// <summary>
    /// The TryGetQueryValueForColumnFilter method tries to get a query value for a column filter from the HttpRequest.
    /// </summary>
    /// <param name="request">The HttpRequest instance that this method extends.</param>
    /// <param name="columnName">The name of the column for which the filter is being applied.</param>
    /// <param name="value">When this method returns, contains the value associated with the specified column, if the column is found; otherwise, contains the default value.</param>
    /// <param name="filter">The filter to be applied to the column.</param>
    /// <returns>true if the HttpRequest contains an element with the specified column; otherwise, false.</returns>
    public static bool TryGetQueryValueForColumnFilter(
        this HttpRequest request,
        string columnName,
        out string value,
        string filter)
    {
        if (request.Query.TryGetValue(columnName, out var result))
        {
            value = result;
            return true;
        }

        if (request.Query.TryGetValue(
            UrlsHelper.GetQueryParamForColumnAndFilter(columnName, filter), out result))
        {
            value = result;
            return true;
        }

        value = string.Empty;
        return false;
    }
}