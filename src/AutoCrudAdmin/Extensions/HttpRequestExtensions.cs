namespace AutoCrudAdmin.Extensions;

using AutoCrudAdmin.Enumerations;
using AutoCrudAdmin.Helpers;
using Microsoft.AspNetCore.Http;

public static class HttpRequestExtensions
{
    public static bool TryGetQueryValueForColumnFilter(
        this HttpRequest request,
        string columnName,
        out string value,
        GridNumberFilterType numberFilterType = GridNumberFilterType.Equals)
    {
        if (request.Query.TryGetValue(columnName, out var result))
        {
            value = result;
            return true;
        }

        if (request.Query.TryGetValue(
            UrlsHelper.GetQueryParamForColumnFilter(columnName, numberFilterType), out result))
        {
            value = result;
            return true;
        }

        value = string.Empty;
        return false;
    }
}