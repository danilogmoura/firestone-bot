using Firebot.GameModel.Base;
using Firebot.GameModel.Configuration;
using Firebot.GameModel.Primitives;

namespace Firebot.GameModel;

public class Menus : BasePage
{
    private readonly string _contextName;

    public Menus(string contextName) : base(GamePaths.RightSideMenu.Root, contextName)
    {
        _contextName = contextName;
    }

    private BasePage MenusContainer => new("menus", _contextName, this);

    public GameButton TownButton => new("/townButton", _contextName, MenusContainer);
    public GameButton MapButton => new("/mapButton", _contextName, MenusContainer);
    public GameButton GuildButton => new("/guildButton", _contextName, MenusContainer);
    public GameButton StoreButton => new("/storeButton", _contextName, MenusContainer);
}