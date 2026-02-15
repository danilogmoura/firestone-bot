using System;
using Firebot.Core;
using Firebot.GameModel.Base;
using Firebot.GameModel.Primitives;

namespace Firebot.GameModel.Features.MapMissions.Missions;

public static class MissionPreview
{
    public static GameButton CloseButton => new(Paths.MenusLoc.CanvasLoc.MapMissionsLoc.MissionsLoc.PreviewLoc.Close);

    public static GameButton StartMissionButton =>
        new(Paths.MenusLoc.CanvasLoc.MapMissionsLoc.MissionsLoc.PreviewLoc.StartBtn);

    public static bool IsNotEnoughSquads =>
        new GameElement(Paths.MenusLoc.CanvasLoc.MapMissionsLoc.MissionsLoc.PreviewLoc.NotEnoughSquads).IsVisible();

    public static DateTime MissionProgress =>
        new GameText(Paths.MenusLoc.CanvasLoc.MapMissionsLoc.MissionsLoc.PreviewLoc.MissionProgress).Time;
}