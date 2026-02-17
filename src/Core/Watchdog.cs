using System.Collections;
using System.Collections.Generic;
using Firebot.GameModel.Base;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;
using UnityEngine;
using static Firebot.Core.BotSettings;

namespace Firebot.Core;

public static class Watchdog
{
    private static IEnumerable<string> EnumerateNuisancePaths()
    {
        foreach (var path in EnumerateChildPaths(new GameElement(Paths.Watchdog.EventsRoot),
                     Paths.Watchdog.EventsRoot,
                     Paths.Watchdog.CloseSuffix,
                     "bg/closeButton"))
            yield return path;

        foreach (var path in EnumerateChildPaths(new GameElement(Paths.Watchdog.PopupsRoot),
                     Paths.Watchdog.PopupsRoot,
                     Paths.Watchdog.CloseSuffix,
                     "bg/closeButton"))
            yield return path;

        foreach (var path in EnumerateChildPaths(new GameElement(Paths.Watchdog.PopupsRoot),
                     Paths.Watchdog.PopupsRoot,
                     Paths.Watchdog.CollectSuffix,
                     "bg/collectButton"))
            yield return path;

        foreach (var path in EnumerateChildPaths(new GameElement(Paths.Watchdog.MenusRoot),
                     Paths.Watchdog.MenusRoot,
                     Paths.Watchdog.MenuCloseSuffix,
                     "closeButton"))
            yield return path;
    }

    private static IEnumerable<string> EnumerateChildPaths(GameElement rootElement, string basePath, string suffix,
        string probePath)
    {
        foreach (var child in rootElement.GetChildren())
        {
            var probe = new GameElement(probePath, child);
            if (probe.IsVisible())
                yield return $"{basePath}/{child.Name}{suffix}";
        }
    }

    public static IEnumerator ForceClearAll()
    {
        for (var i = 0; i < 3; i++)
        {
            foreach (var path in EnumerateNuisancePaths())
            {
                var gameButton = new GameButton(path);

                if (!gameButton.IsVisible()) continue;
                Debug.Log($"[Watchdog] Fechando popup: {path}");

                yield return gameButton.Click();
            }

            yield return new WaitForSeconds(InteractionDelay);
        }
    }
}