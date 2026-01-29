using System;
using Firebot.Bot.Components.TMProComponents;
using MelonLoader;

namespace Firebot.Bot.Components.UI;

internal class TimeDisplay : TextMeshProWrapper
{
    public TimeDisplay(string path) : base(path) { }

    public int ParseToSeconds()
    {
        var timeString = Text;

        if (string.IsNullOrEmpty(timeString)) return 0;

        try
        {
            var cleanTime = timeString.Trim();
            var parts = cleanTime.Split(':');

            var totalSeconds = 0;

            switch (parts.Length)
            {
                case 1:
                    int.TryParse(parts[0], out totalSeconds);
                    break;
                case 2:
                {
                    if (int.TryParse(parts[0], out var minutes) && int.TryParse(parts[1], out var seconds))
                        totalSeconds = minutes * 60 + seconds;

                    break;
                }
                case 3:
                {
                    if (int.TryParse(parts[0], out var hours) &&
                        int.TryParse(parts[1], out var minutes) &&
                        int.TryParse(parts[2], out var seconds))
                        totalSeconds = hours * 3600 + minutes * 60 + seconds;

                    break;
                }
            }

            return totalSeconds;
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[TimeParser] Erro ao converter '{timeString}': {ex.Message}");
            return 0;
        }
    }
}