using System;
using System.Collections;
using System.Linq;
using Firebot.GameModel.Base;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;
using Firebot.Utilities;

namespace Firebot.GameModel.Features.Town.Oracle;

public class Rituals : GameElement
{
    public Rituals() : base(Paths.MenusLoc.CanvasLoc.TownLoc.OracleLoc.RitualLoc.Rituals) { }

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

    public DateTime CurrentRunTime()
    {
        DateTime? earliest = null;

        foreach (var child in GetChildren().Where(child => child.IsVisible()))
        {
            var dateTime = new GameText(Paths.MenusLoc.CanvasLoc.TownLoc.OracleLoc.RitualLoc.CurrentRunTimeTxt, child);
            if (!dateTime.IsVisible()) continue;
            earliest = dateTime.Time;
            break;
        }

        return earliest ?? NextRunTime();
    }

    private static DateTime NextRunTime()
    {
        var text = new GameText(Paths.MenusLoc.CanvasLoc.TownLoc.OracleLoc.RitualLoc.NextRunTimeTxt).GetParsedText();
        return TimeParser.ParseFromText(text);
    }
}