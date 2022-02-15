namespace AutoCrudAdmin.Extensions;

using System.Linq;

public static class StringExtensions
{
    public static string ToHyphenSeparatedWords(this string str)
        => string.Concat(
                str.Select((x, i) => i > 0 && char.IsUpper(x)
                    ? "-" + x
                    : x.ToString()))
            .ToLower();

    public static string ToControllerBaseUri(this string controllerName)
        => controllerName.Replace("Controller", string.Empty);
}