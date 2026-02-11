namespace Firebot.GameModel.Configuration;

public static class Paths
{
    private const string SafeArea = "battleRoot/battleMain/battleCanvas/SafeArea";

    public static class Popups
    {
        public const string CloseButton = "/bg/closeButton";
    }

    public static class Hud
    {
        public const string Root = SafeArea + "/rightSideUI/menuButtons";

        public const string TownButton = "/rightSideUI/menuButtons/townButton";
        public const string MapButton = "/rightSideUI/menuButtons/mapButton";
        public const string GuildButton = "/rightSideUI/menuButtons/guildButton";
        public const string StoreButton = "/rightSideUI/menuButtons/storeButton";

        public const string PathOfGloryButton = "/bottomSideUIDesktop/pathOfGloryButton";
        public const string EventsButton = "/bottomSideUIDesktop/eventsButton";
    }

    public static class MapMissions
    {
        public static class PreviewMission
        {
            public const string Root = SafeArea + "/popups/PreviewMission";

            public const string CloseButton = "/bg/closeButton";
            public const string SpeedUpButton = "/bg/managementBg/container/speedUpButton";
            public const string SpeedUpButtonIcon = "/currencyIcon";
            public const string StartMissionButton = "/bg/managementBg/container/startMissionButton";

            public const string MissionProgress =
                "/bg/rewardBg/previewMissionTime/previewBar/missionProgress/activeMissionProgressText";
        }

        public static class MissionPin
        {
            public const string Root = "menusRoot/mapRoot/mapElements/missions";

            public const string MissionTimeRequirement = "/missionBg/missionTimeBg/missionTimeReq";
            public const string CompletedTick = "/missionBg/completedTick";
        }
    }
}