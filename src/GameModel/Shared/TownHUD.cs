using Firebot.Core;
using Firebot.GameModel.Primitives;

namespace Firebot.GameModel.Shared;

public static class TownHUD
{
    public static GameButton CloseButton => new(Paths.MenusLoc.CanvasLoc.TownHUDLoc.Close);

    public static GameButton EngineerButton => new(Paths.MenusLoc.CanvasLoc.TownHUDLoc.Engineer);
}