using Firebot.GameModel.TMProComponents;

namespace Firebot.GameModel.UI;

internal class TimeDisplay : TextMeshProWrapper
{
    public TimeDisplay(string path) : base(path) { }

    public int ParseToSeconds() =>
        RunSafe(() =>
        {
            var timeString = Text;
            if (string.IsNullOrEmpty(timeString)) return 0;

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
        }, defaultValue: 0);
}