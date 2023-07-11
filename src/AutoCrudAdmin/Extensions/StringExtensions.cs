namespace AutoCrudAdmin.Extensions;

using System;
using System.Linq;
using System.Text.RegularExpressions;

public static class StringExtensions
{
    private const string PascalCaseWordsRegexPattern = @"([A-Z][a-z]+)";
    private const string Ellipsis = "...";

    public static string ToHyphenSeparatedWords(this string str)
        => string.Concat(
                str.Select((x, i) => i > 0 && char.IsUpper(x)
                    ? "-" + x
                    : x.ToString()))
            .ToLower();

    public static string ToSpaceSeparatedWords(this string str)
        => string.Join(
            " ",
            Regex
                .Matches(str, PascalCaseWordsRegexPattern)
                .Select(m => m.Value));

    public static string ToControllerBaseUri(this string controllerName)
        => controllerName.Replace("Controller", string.Empty);

    public static string? ToEllipsis(this string? longText, int maxLength)
    {
        if (maxLength < Ellipsis.Length)
        {
            throw new ArgumentException("Max length can't be less than the length of ellipsis", nameof(maxLength));
        }

        if (longText?.Length > maxLength)
        {
            return longText[.. (maxLength - Ellipsis.Length + 1)] + Ellipsis;
        }

        return longText;
    }
}