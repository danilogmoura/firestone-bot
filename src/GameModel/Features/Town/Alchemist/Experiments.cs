using System;
using System.Collections;
using System.Linq;
using Firebot.Core;
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
            var speedupFinishDesc = $"/{Slot}{resource}/{Paths.MenusLoc.CanvasLoc.TownLoc.AlchemistLoc.ExperimentsLoc.SpeedupFinishDesc}";
            var speedupFinish = new GameElement(speedupFinishDesc, this);
            if (!speedupFinish.IsVisible())
            {
                var speedBtnPath = $"/{Slot}{resource}/{Paths.MenusLoc.CanvasLoc.TownLoc.AlchemistLoc.ExperimentsLoc.SpeedupBtn}";
                var button = new GameButton(speedBtnPath, this);
                if (button.IsClickable()) yield return button.Click();
            }

            var claimBtnPath = $"/{Type}{resource}/{Paths.MenusLoc.CanvasLoc.TownLoc.AlchemistLoc.ExperimentsLoc.StartBtn}";
            var gameButton = new GameButton(claimBtnPath, this);
            yield return gameButton.Click();
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

    public DateTime NextRunTime(string[] experimentResources)
    {
        var count = GetChildren().Count(c => c.Name.StartsWith(Slot));
        if (count == 0) return DateTime.Now.AddHours(1); // No experiment slots found, assume next run is in 1 hour

        var minTime = DateTime.MaxValue;
        foreach (var resource in experimentResources)
        {
            var path = $"/{Slot}{resource}/{Paths.MenusLoc.CanvasLoc.TownLoc.AlchemistLoc.ExperimentsLoc.NextRunTimeTxt}";
            var time = new GameText(path, this).Time.AddSeconds(-BotSettings.FreeSpeedupSeconds);
            if (time < minTime) minTime = time;
        }

        return minTime == DateTime.MaxValue ? DateTime.MinValue : minTime;
    }
}