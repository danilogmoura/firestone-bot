using System;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Shared;

[Obsolete("This class is deprecated. Will be removed in a future update.")]
public class MainHUD
{
    public static GameButton TownButton => new(Paths.BattleLoc.MainHudLoc.TownButton);
    public static GameButton MapButton => new(Paths.BattleLoc.MainHudLoc.MapButton);
    public static GameButton GuildButton => new(Paths.BattleLoc.MainHudLoc.GuildButton);
    public static GameButton StoreButton => new(Paths.BattleLoc.MainHudLoc.StoreButton);
    public static GameButton PathOfGloryButton => new(Paths.BattleLoc.MainHudLoc.PathOfGloryButton);
    public static GameButton EventsButton => new(Paths.BattleLoc.MainHudLoc.EventsButton);
}