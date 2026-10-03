using System;
using System.Collections;
using Firebot.Core;
using Firebot.GameModel.Base;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;
using Firebot.Utilities;

namespace Firebot.GameModel.Features.Town.Alchemist;

public class Experiments : GameElement
{
    private const string Type = "alchExperimentType";
    private const string Slot = "alchExperimentSlot";
    public static readonly string[] ValidResourceIds = { "0", "1", "2" };

    public Experiments() : base(Paths.MenusLoc.CanvasLoc.TownLoc.AlchemistLoc.ExperimentsLoc.Root) { }

    private string ResourceQuantityPath(string resource) =>
        resource switch
        {
            "0" => Paths.MenusLoc.CanvasLoc.TownLoc.AlchemistLoc.CountersLoc.DragonBloodQuantity,
            "1" => Paths.MenusLoc.CanvasLoc.TownLoc.AlchemistLoc.CountersLoc.StrangeDustQuantity,
            "2" => Paths.MenusLoc.CanvasLoc.TownLoc.AlchemistLoc.CountersLoc.ExoticCoinQuantity,
            _ => null
        };

    public double ResourceQuantity(string resource)
        => new GameText(ResourceQuantityPath(resource)).GetParsedDouble();

    public IEnumerator Claim()
    {
        foreach (var resource in ValidResourceIds)
        {
            var speedupFinishDesc =
                $"/{Slot}{resource}/{Paths.MenusLoc.CanvasLoc.TownLoc.AlchemistLoc.ExperimentsLoc.SpeedupFinishDesc}";
            var speedupFinish = new GameElement(speedupFinishDesc, this);
            if (!speedupFinish.IsVisible())
            {
                var speedBtnPath =
                    $"/{Slot}{resource}/{Paths.MenusLoc.CanvasLoc.TownLoc.AlchemistLoc.ExperimentsLoc.SpeedupBtn}";
                var button = new GameButton(speedBtnPath, this);
                if (button.IsClickable()) yield return button.Click();
            }

            var claimBtnPath =
                $"/{Slot}{resource}/{Paths.MenusLoc.CanvasLoc.TownLoc.AlchemistLoc.ExperimentsLoc.ClaimBtn}";
            var gameButton = new GameButton(claimBtnPath, this);
            if (gameButton.IsClickable()) yield return gameButton.Click();
        }
    }

    public IEnumerator Start(string[] experimentResources)
    {
        foreach (var resource in experimentResources)
        {
            var gePath = $"/{Slot}{resource}";
            var experimentSlot = new GameElement(gePath, this);
            if (experimentSlot.IsVisible())
                continue; // Skip if experiment slot is already visible (i.e. experiment is active)

            var path = $"/{Type}{resource}/{Paths.MenusLoc.CanvasLoc.TownLoc.AlchemistLoc.ExperimentsLoc.StartBtn}";
            var gameButton = new GameButton(path, this);
            if (gameButton.IsClickable()) yield return gameButton.Click();
        }
    }

    public DateTime NextRunTime(string[] experimentResources)
    {
        var minTime = DateTime.MaxValue;
        foreach (var resource in experimentResources)
        {
            var path =
                $"/{Slot}{resource}/{Paths.MenusLoc.CanvasLoc.TownLoc.AlchemistLoc.ExperimentsLoc.NextRunTimeTxt}";
            var timerText = new GameText(path, this).GetParsedText();
            var time = TimeParser.ParseExpectedTime(timerText);
            if (time == DateTime.MinValue) continue;

            time = time.AddSeconds(-BotSettings.FreeSpeedupSeconds);
            if (time < minTime) minTime = time;
        }

        return minTime == DateTime.MaxValue ? DateTime.Now.AddHours(1) : minTime;
    }
}