namespace AutoCrudAdmin.Helpers;

using AutoCrudAdmin.Enumerations;
using AutoCrudAdmin.Extensions;

public static class UrlsHelper
{
    public static string GetQueryParamForColumnFilter(
        string columnName,
        GridNumberFilterType numberFilterType = GridNumberFilterType.Equals)
        => $"{columnName}-{numberFilterType.ToString().ToHyphenSeparatedWords()}";
}