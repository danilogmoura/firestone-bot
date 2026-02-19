using System;
using System.Collections;
using Firebot.GameModel.Base;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.Town.Library.FirestoneResearch;

public class ResearchPanel : GameElement
{
    public ResearchPanel() : base(Paths.MenusLoc.CanvasLoc.TownLoc.LibraryLoc.ResearchPanelLoc.Root) { }

    public static bool HasEmptySlot =>
        new GameElement(Paths.MenusLoc.CanvasLoc.TownLoc.LibraryLoc.ResearchPanelLoc.SelectResearchTable)
            .IsVisible();

    public IEnumerator Claim()
    {
        foreach (var child in GetChildren())
            if (child.IsVisible() && child.Name.StartsWith("researchSlot"))
                yield return new GameButton(
                        Paths.MenusLoc.CanvasLoc.TownLoc.LibraryLoc.ResearchPanelLoc.ClaimBtn, child)
                    .Click();
    }

    public DateTime NextRunTime()
    {
        var minTime = DateTime.MaxValue;
        foreach (var child in GetChildren())
            if (child.IsVisible() && child.Name.StartsWith("researchSlot"))
            {
                var time = new GameText(
                    Paths.MenusLoc.CanvasLoc.TownLoc.LibraryLoc.ResearchPanelLoc.NextRunTimeTxt, child).Time;
                if (time < minTime) minTime = time;
            }

        return minTime == DateTime.MaxValue ? DateTime.MinValue : minTime;
    }
}