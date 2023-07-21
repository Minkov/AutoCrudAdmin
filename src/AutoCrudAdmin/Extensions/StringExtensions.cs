namespace AutoCrudAdmin.Extensions;

using System;
using System.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// The StringExtensions class provides extension methods for the String class.
/// These extensions add functionality related to the AutoCrudAdmin system.
/// </summary>
public static class StringExtensions
{
    private const string PascalCaseWordsRegexPattern = @"([A-Z][a-z]+)";
    private const string Ellipsis = "...";

    /// <summary>
    /// The ToHyphenSeparatedWords method converts the source string to a hyphen-separated lowercase string.
    /// It inserts a hyphen before each uppercase character (except the first one) and converts all characters to lowercase.
    /// </summary>
    /// <param name="str">The string instance that this method extends.</param>
    /// <returns>A new string that is a hyphen-separated version of the source string.</returns>
    public static string ToHyphenSeparatedWords(this string str)
        => string.Concat(
                str.Select((x, i) => i > 0 && char.IsUpper(x)
                    ? "-" + x
                    : x.ToString()))
            .ToLower();

    /// <summary>
    /// The ToSpaceSeparatedWords method converts the source string to a space-separated string.
    /// It identifies words in the source string by looking for sequences of uppercase letters followed by lowercase letters, and separates these words with spaces.
    /// </summary>
    /// <param name="str">The string instance that this method extends.</param>
    /// <returns>A new string that is a space-separated version of the source string.</returns>
    public static string ToSpaceSeparatedWords(this string str)
        => string.Join(
            " ",
            Regex
                .Matches(str, PascalCaseWordsRegexPattern)
                .Select(m => m.Value));

    /// <summary>
    /// The ToControllerBaseUri method removes the "Controller" suffix from the source string.
    /// This can be used to generate a base URI for a controller based on its name.
    /// </summary>
    /// <param name="controllerName">The string instance that this method extends, which should be the name of a controller.</param>
    /// <returns>A new string that is the base URI of the controller.</returns>
    public static string ToControllerBaseUri(this string controllerName)
        => controllerName.Replace("Controller", string.Empty);

    /// <summary>
    /// The ToEllipsis method shortens the source string to the specified maximum length, replacing the end with an ellipsis ("...") if necessary.
    /// If the source string is already shorter than the maximum length, it is returned unchanged.
    /// </summary>
    /// <param name="longText">The string instance that this method extends.</param>
    /// <param name="maxLength">The maximum length for the resulting string, including the ellipsis. Must be at least the length of the ellipsis.</param>
    /// <returns>A new string that is a shortened version of the source string, with an ellipsis added at the end if it was shortened.</returns>
    /// <exception cref="ArgumentException">Thrown when the maxLength is less than the length of the ellipsis.</exception>
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