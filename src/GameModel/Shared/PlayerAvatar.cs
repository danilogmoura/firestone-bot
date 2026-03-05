using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Shared;

public static class PlayerAvatar
{
    public static int CharacterLevel =>
        new GameText(Paths.BattleLoc.PlayerAvatarLoc.CharacterLevel).GetParsedInt();
}