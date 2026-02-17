using System;
using System.Collections;
using Firebot.GameModel.Base;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.MapMissions.Missions;

public static class MissionPreview
{
    public static IEnumerator Close =>
        new GameButton(Paths.MenusLoc.CanvasLoc.MapMissionsLoc.MissionsLoc.PreviewLoc.Close).Click();

    public static IEnumerator StartMission =>
        new GameButton(Paths.MenusLoc.CanvasLoc.MapMissionsLoc.MissionsLoc.PreviewLoc.StartBtn).Click();

    public static bool IsNotEnoughSquads =>
        new GameElement(Paths.MenusLoc.CanvasLoc.MapMissionsLoc.MissionsLoc.PreviewLoc.NotEnoughSquads).IsVisible();

    public static DateTime MissionProgress =>
        new GameText(Paths.MenusLoc.CanvasLoc.MapMissionsLoc.MissionsLoc.PreviewLoc.MissionProgress).Time;
}