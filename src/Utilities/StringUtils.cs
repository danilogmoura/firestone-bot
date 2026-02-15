using System.Linq;
using System.Text.RegularExpressions;

namespace Firebot.Utilities;

public abstract class StringUtils
{
    public static string JoinPath(string parent, params string[] children)
    {
        var result = parent?.TrimEnd('/') ?? "";

        return children.Where(child => !string.IsNullOrEmpty(child)).Aggregate(result,
            (current, child) => $"{current}/{child.TrimStart('/').TrimEnd('/')}");
    }

    public static string Ellipsize(string value, int head = 20, int tail = 20)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= head + tail + 3) return value;
        return value[..head] + "..." + value[^tail..];
    }

    /// <summary>
    ///     Formats a PascalCase string into a human-readable format by inserting spaces
    ///     and removing a specific suffix.
    ///     Example: "CloseEventPromotionalAutomation" -> "Close Event Promotional"
    /// </summary>
    /// <param name="input">The string to be formatted.</param>
    /// <param name="suffixToRemove">The suffix to strip from the end of the string.</param>
    /// <returns>A formatted, human-readable string.</returns>
    public static string Humanize(string input, string suffixToRemove = "Task")
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        // 1. Remove the suffix if the string ends with it
        var cleaned = input.EndsWith(suffixToRemove)
            ? input.Substring(0, input.Length - suffixToRemove.Length)
            : input;

        // 2. Insert spaces before each capital letter (except the first one)
        // Regex pattern: Lookbehind for "not start of line", Lookahead for "uppercase letter"
        return Regex.Replace(cleaned, @"(?<!^)(?=[A-Z])", " ").Trim();
    }
}