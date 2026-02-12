using System;
using Firebot.GameModel.Base;
using Firebot.GameModel.Configuration;
using Firebot.GameModel.Primitives;

namespace Firebot.GameModel.Features.Engineer.Tools;

public class EngineerSubmenu : GameElement
{
    public EngineerSubmenu() : base(Paths.Engineer.Garage.EngineerSubmenu.Root) { }

    public GameButton CloseButton => new(Paths.Engineer.Garage.EngineerSubmenu.CloseButton, this);

    public GameButton ClaimToolsButton => new(Paths.Engineer.Garage.EngineerSubmenu.ClaimToolsButton, this);

    public DateTime FindNextRunTime =>
        new GameText(Paths.Engineer.Garage.EngineerSubmenu.ClaimToolsCooldownTimeLeft, this).Time;
}