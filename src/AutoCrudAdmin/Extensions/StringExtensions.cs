namespace AutoCrudAdmin.Extensions;

using System.Linq;
using System.Text.RegularExpressions;

public static class StringExtensions
{
    private const string PascalCaseWordsRegexPattern = @"([A-Z][a-z]+)";

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
}