using Firebot.GameModel.Base;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;
using UnityEngine;

namespace Firebot.GameModel.Features.Town.MagicQuarters;

public class Guardian : GameElement
{
    public Guardian(string path = null, GameElement parent = null, Transform transform = null)
        : base(path, parent, transform) { }

    public bool IsUnlocked =>
        new GameElement(Paths.MenusLoc.CanvasLoc.TownLoc.MagicQuarters.GuardianStarsIcon, this).IsVisible();

    public static GameButton EnlightenmentBtn =>
        new(Paths.MenusLoc.CanvasLoc.TownLoc.MagicQuarters.EnlightenmentBtn);
}