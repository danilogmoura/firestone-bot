namespace Firebot.GameModel.Configuration;

public static class GamePaths
{
    private const string BaseCanvas = "menusRoot/menuCanvasParent/SafeArea/menuCanvas";
    private const string SafeArea = "battleRoot/battleMain/battleCanvas/SafeArea";

    public static class Menus
    {
        private const string Root = BaseCanvas + "/menus";

        public const string TreeOfLife = Root + "/TreeOfLife";
        public const string TownGuild = Root + "/TownGuild";
        public const string TownIrongard = Root + "/TownIrongard";
        public const string ExoticMerchant = Root + "/ExoticMerchant";
        public const string MagicQuarters = Root + "/MagicQuarters";
        public const string WorldMap = Root + "/WorldMap";
    }

    public static class Events
    {
        private const string Root = BaseCanvas + "/events";
        public const string EventManager = Root + "/EventManager";
        public const string DecoratedShop = Root + "/DecoratedHeroesShop";
        public const string MiniEvents = Root + "/MiniEvents";
    }

    public static class Popups
    {
        private const string Root = BaseCanvas + "/popups";

        public const string CloseButton = "/bg/closeButton";

        public const string Expeditions = Root + "/Expeditions";
        public const string GuildBank = Root + "/GuildBank";
        public const string PreviewMission = Root + "/PreviewMission";
        public const string MissionRewards = Root + "/MissionRewards";
    }

    public static class Notifications
    {
        private const string Root = SafeArea + "/leftSideUI/notifications/Viewport/grid";
    }

    public static class RightSideMenu
    {
        public const string Root = SafeArea + "/rightSideUI/menuButtons";

        public const string TownButton = Root + "/townButton";
        public const string MapButton = Root + "/mapButton";
        public const string GuildButton = Root + "/guildButton";
        public const string StoreButton = Root + "/storeButton";
    }
}