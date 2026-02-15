using System;
using System.Text.RegularExpressions;
using Firebot.Core;

namespace Firebot.Utilities;

/// <summary>
///     Utility to parse time expressions like "6d 12:30:15", "12:30", "9:28" or "6d".
///     Returns a TimeSpan or TimeSpan.Zero for invalid/empty input.
/// </summary>
public static class TimeParser
{
    private static readonly Regex TimeRegex = new(
        @"(?:(?<days>\d+)d\s*)?(?:(?<h>\d+):(?<m>\d+):(?<s>\d+)|(?<m_only>\d+):(?<s_only>\d+))",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static TimeSpan Parse(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return TimeSpan.Zero;

        var match = TimeRegex.Match(raw.Trim());

        if (!match.Success)
            return TimeSpan.Zero;

        int days = 0, hours = 0, minutes = 0, seconds = 0;

        if (match.Groups["days"].Success)
            days = int.Parse(match.Groups["days"].Value);

        if (match.Groups["h"].Success)
        {
            hours = int.Parse(match.Groups["h"].Value);
            minutes = int.Parse(match.Groups["m"].Value);
            seconds = int.Parse(match.Groups["s"].Value);
        }
        else
        {
            minutes = int.Parse(match.Groups["m_only"].Value);
            seconds = int.Parse(match.Groups["s_only"].Value);
        }

        return new TimeSpan(days, hours, minutes, seconds);
    }

    public static DateTime ParseExpectedTime(string raw, double bufferSeconds = 0)
    {
        var duration = Parse(raw);

        Logger.Debug($"Raw: '{raw}' -> Parsed: {duration} -> Date: {DateTime.Now.Add(duration):HH:mm:ss}");

        return duration == TimeSpan.Zero
            ? DateTime.MinValue
            : DateTime.Now.Add(duration).AddSeconds(bufferSeconds);
    }
}