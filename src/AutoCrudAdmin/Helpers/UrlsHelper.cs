namespace AutoCrudAdmin.Helpers;

using AutoCrudAdmin.Extensions;

internal static class UrlsHelper
{
    public static string GetQueryParamForColumnAndFilter(string columnName, string filterType)
        => $"{columnName}-{filterType.ToHyphenSeparatedWords()}";
}