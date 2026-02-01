namespace Firebot.Core;

public static class GamePaths
{
    private const string BaseCanvas = "menusRoot/menuCanvasParent/SafeArea/menuCanvas";

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
        public const string EventManager = Root + "/EventManager/bg";
        public const string DecoratedShop = Root + "/DecoratedHeroesShop/bg";
        public const string MiniEvents = Root + "/MiniEvents/bg";
    }

    public static class Popups
    {
        private const string Root = BaseCanvas + "/popups";
        public const string Expeditions = Root + "/Expeditions/bg";
        public const string GuildBank = Root + "/GuildBank/bg";

        public const string PreviewMission = Root + "/PreviewMission/bg";
        public const string MissionRewards = Root + "/MissionRewards/bg";
    }

    public static class Notifications
    {
        private const string Root = "battleRoot/battleMain/battleCanvas/SafeArea";
        public const string NotificationGrid = Root + "/leftSideUI/notifications/Viewport/grid";
    }
}