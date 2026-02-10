using Firebot.GameModel.Base;
using Firebot.GameModel.Configuration;
using Firebot.GameModel.Primitives;

namespace Firebot.GameModel.Roots;

public class Menus : BasePage
{
    public Menus() : base(GamePaths.RightSideMenu.Root) { }

    private BasePage MenusContainer => new("menus", this);

    public GameButton TownButton => new("/townButton", MenusContainer);
    public GameButton MapButton => new("/mapButton", MenusContainer);
    public GameButton GuildButton => new("/guildButton", MenusContainer);
    public GameButton StoreButton => new("/storeButton", MenusContainer);
}