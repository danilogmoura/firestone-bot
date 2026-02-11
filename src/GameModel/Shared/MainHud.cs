using Firebot.GameModel.Base;
using Firebot.GameModel.Configuration;
using Firebot.GameModel.Primitives;

namespace Firebot.GameModel.Shared;

public class MainHud : GameElement
{
    public MainHud() : base(Paths.Hud.Root) { }

    public GameButton TownButton => new(Paths.Hud.TownButton, this);
    public GameButton MapButton => new(Paths.Hud.MapButton, this);
    public GameButton GuildButton => new(Paths.Hud.GuildButton, this);
    public GameButton StoreButton => new(Paths.Hud.StoreButton, this);

    public GameButton PathOfGloryButton => new(Paths.Hud.PathOfGloryButton, this);
    public GameButton EventsButton => new(Paths.Hud.EventsButton, this);
}