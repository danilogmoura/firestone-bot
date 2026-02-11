using Firebot.GameModel.Base;
using Firebot.GameModel.Configuration;
using Firebot.GameModel.Primitives;

namespace Firebot.GameModel.Features.MapMissions;

public class SpeedUpButton : GameButton
{
    public SpeedUpButton(GameElement parent) : base(Paths.MapMissions.PreviewMission.SpeedUpButton, parent) { }

    private GameElement CurrencyIcon => new GameImage(Paths.MapMissions.PreviewMission.SpeedUpButtonIcon, this);

    public bool IsPaidAction => CurrencyIcon.IsVisible();
}