using System;
using System.Text.RegularExpressions;

namespace Firebot.Core;

/// <summary>
///     Utility to parse time expressions like "6d 12:30:15", "12:30", "9:28" or "6d".
///     Returns a TimeSpan or TimeSpan.Zero for invalid/empty input.
/// </summary>
public static class TimeParser
{
    // Regex that matches numbers optionally followed by 'd' (days),
    // or time formats like HH:MM[:SS] and MM:SS.
    private static readonly Regex TimeRegex = new(
        @"(?:(?<days>\d+)d\s*)?(?:(?<hours>\d+):(?<minutes>\d+)(?::(?<seconds>\d+))?|(?<only_min>\d+):(?<only_sec>\d+))",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    ///     Parses a raw string into a TimeSpan.
    ///     Supported formats:
    ///     - "6d 12:30:15" (days + HH:MM:SS)
    ///     - "12:30" or "12:30:15" (HH:MM[:SS])
    ///     - "9:28" (MM:SS)
    ///     Returns TimeSpan.Zero if input is null/empty or doesn't match.
    /// </summary>
    public static TimeSpan Parse(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return TimeSpan.Zero;

        var match = TimeRegex.Match(raw.Trim());
        if (!match.Success) return TimeSpan.Zero;

        int days = 0, hours = 0, minutes = 0, seconds = 0;

        // Capture days (e.g. "6d")
        if (match.Groups["days"].Success) days = int.Parse(match.Groups["days"].Value);

        // If format contains hours (HH:MM or HH:MM:SS)
        if (match.Groups["hours"].Success)
        {
            hours = int.Parse(match.Groups["hours"].Value);
            minutes = int.Parse(match.Groups["minutes"].Value);
            if (match.Groups["seconds"].Success)
                seconds = int.Parse(match.Groups["seconds"].Value);
        }

        // Fallback for short format MM:SS (e.g. "9:28")
        else if (match.Groups["only_min"].Success)
        {
            minutes = int.Parse(match.Groups["only_min"].Value);
            seconds = int.Parse(match.Groups["only_sec"].Value);
        }

        return new TimeSpan(days, hours, minutes, seconds);
    }
}