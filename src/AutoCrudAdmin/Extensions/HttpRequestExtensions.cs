namespace AutoCrudAdmin.Extensions;

using AutoCrudAdmin.Helpers;
using Microsoft.AspNetCore.Http;

internal static class HttpRequestExtensions
{
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