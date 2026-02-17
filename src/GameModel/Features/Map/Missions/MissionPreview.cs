using System;
using System.Collections;
using Firebot.GameModel.Base;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.Map.Missions;

public static class MissionPreview
{
    public static IEnumerator Close =>
        new GameButton(Paths.MenusLoc.CanvasLoc.MapLoc.MissionsLoc.PreviewLoc.CloseBtn).Click();

    public static IEnumerator StartMission =>
        new GameButton(Paths.MenusLoc.CanvasLoc.MapLoc.MissionsLoc.PreviewLoc.StartBtn).Click();

    public static bool IsNotEnoughSquads =>
        new GameElement(Paths.MenusLoc.CanvasLoc.MapLoc.MissionsLoc.PreviewLoc.NotEnoughSquadsTxt).IsVisible();

    public static DateTime NextRunTime =>
        new GameText(Paths.MenusLoc.CanvasLoc.MapLoc.MissionsLoc.PreviewLoc.NextRunTimeTxt).Time;
}