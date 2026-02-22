using System;
using System.Collections;
using System.Linq;
using Firebot.GameModel.Base;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.Town.Oracle;

public class Rituals : GameElement
{
    public Rituals() : base(Paths.MenusLoc.CanvasLoc.TownLoc.OracleLoc.RitualLoc.Root) { }

    public IEnumerator Claim()
    {
        foreach (var child in GetChildren().Where(child => child.IsVisible()))
            yield return new GameButton(Paths.MenusLoc.CanvasLoc.TownLoc.OracleLoc.RitualLoc.ClaimBtn, child).Click();
    }

    public IEnumerator Start()
    {
        foreach (var child in GetChildren().Where(child => child.IsVisible()))
            yield return new GameButton(Paths.MenusLoc.CanvasLoc.TownLoc.OracleLoc.RitualLoc.StartBtn, child).Click();
    }

    public DateTime NextRunTime()
    {
        DateTime? earliest = null;

        foreach (var child in GetChildren().Where(child => child.IsVisible()))
        {
            var dateTime = new GameText(Paths.MenusLoc.CanvasLoc.TownLoc.OracleLoc.RitualLoc.NextRunTimeTxt, child);
            if (!dateTime.IsVisible()) continue;
            if (!earliest.HasValue || dateTime.Time < earliest.Value) earliest = dateTime.Time;
        }

        return earliest.GetValueOrDefault();
    }
}