using Firebot.GameModel.Base;
using Firebot.GameModel.Configuration;
using Firebot.GameModel.Primitives;

namespace Firebot.GameModel.Features.Engineer;

public class GaragePopup : GameElement
{
    public GaragePopup() : base(Paths.Engineer.Garage.Root) { }

    public GameButton CloseButton => new(Paths.Engineer.Garage.CloseButton, this);

    public GameButton EngineerButton => new(Paths.Engineer.Garage.EngineerButton, this);
}