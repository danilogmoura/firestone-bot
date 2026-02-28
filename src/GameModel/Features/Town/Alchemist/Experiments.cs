using System;
using System.Collections;
using System.Linq;
using Firebot.GameModel.Base;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.Town.Alchemist;

public class Experiments : GameElement
{
    private const string Type = "alchExperimentType";
    private const string Slot = "alchExperimentSlot";

    public Experiments() : base(Paths.MenusLoc.CanvasLoc.TownLoc.AlchemistLoc.ExperimentsLoc.Root) { }

    public IEnumerator Claim(string[] experimentResources)
    {
        foreach (var resource in experimentResources)
        {
            var path = $"/{Type}{resource}/{Paths.MenusLoc.CanvasLoc.TownLoc.AlchemistLoc.ExperimentsLoc.StartBtn}";
            var gameButton = new GameButton(path, this);
            if (gameButton.IsClickable()) yield return gameButton.Click();
        }
    }

    public IEnumerator Start(string[] experimentResources)
    {
        foreach (var resource in experimentResources)
        {
            var path = $"/{Slot}{resource}/{Paths.MenusLoc.CanvasLoc.TownLoc.AlchemistLoc.ExperimentsLoc.ClaimBtn}";
            var gameButton = new GameButton(path, this);
            if (gameButton.IsClickable()) yield return gameButton.Click();
        }
    }

    public DateTime NextRunTime()
    {
        var count = GetChildren().Count(c => c.Name.StartsWith(Slot));
        if (count == 0) return DateTime.Now.AddHours(1); // No experiment slots found, assume next run is in 1 hour

        var minTime = DateTime.MaxValue;
        foreach (var child in GetChildren())
            if (child.IsVisible() && child.Name.StartsWith(Slot))
            {
                var time = new GameText(Paths.MenusLoc.CanvasLoc.TownLoc.AlchemistLoc.ExperimentsLoc.NextRunTimeTxt,
                    child).Time;
                if (time < minTime) minTime = time;
            }

        return minTime == DateTime.MaxValue ? DateTime.MinValue : minTime;
    }
}