using Firebot.Core;
using Firebot.GameModel.Primitives;

namespace Firebot.GameModel.Features.Engineer.Tools;

public static class GaragePopup
{
    public static GameButton CloseButton => new(Paths.MenusLoc.CanvasLoc.EngineerLoc.GarageLoc.Close);

    public static GameButton EngineerButton => new(Paths.MenusLoc.CanvasLoc.EngineerLoc.GarageLoc.EngineerBtn);
}