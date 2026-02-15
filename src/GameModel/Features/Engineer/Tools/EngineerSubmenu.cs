using System;
using Firebot.Core;
using Firebot.GameModel.Primitives;

namespace Firebot.GameModel.Features.Engineer.Tools;

public static class EngineerSubmenu
{
    public static GameButton CloseButton => new(Paths.MenusLoc.CanvasLoc.EngineerLoc.SubmenuLoc.Close);

    public static GameButton ClaimToolsButton => new(Paths.MenusLoc.CanvasLoc.EngineerLoc.SubmenuLoc.ClaimTools);

    public static DateTime FindNextRunTime =>
        new GameText(Paths.MenusLoc.CanvasLoc.EngineerLoc.SubmenuLoc.Cooldown).Time;
}