using Firebot.GameModel.Base;
using Firebot.GameModel.Configuration;
using Firebot.GameModel.Primitives;

namespace Firebot.GameModel.Shared;

public class MainHUD : GameElement
{
    public MainHUD() : base(Paths.MainHud.Root) { }

    public GameButton TownButton => new(Paths.MainHud.TownButton, this);
    public GameButton MapButton => new(Paths.MainHud.MapButton, this);
    public GameButton GuildButton => new(Paths.MainHud.GuildButton, this);
    public GameButton StoreButton => new(Paths.MainHud.StoreButton, this);

    public GameButton PathOfGloryButton => new(Paths.MainHud.PathOfGloryButton, this);
    public GameButton EventsButton => new(Paths.MainHud.EventsButton, this);
}