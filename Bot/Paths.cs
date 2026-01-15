namespace FireBot.Bot
{
    public abstract class Paths
    {
        internal static class Engineer
        {
            public const string GridNotification =
                "battleRoot/battleMain/battleCanvas/SafeArea/leftSideUI/notifications/Viewport/grid/Engineer";

            public const string CloseButton =
                "menusRoot/menuCanvasParent/SafeArea/menuCanvas/menus/Engineer/closeButton";

            public const string ClaimToolsButton =
                "menusRoot/menuCanvasParent/SafeArea/menuCanvas/menus/Engineer/submenus/bg/engineerSubmenu/toolsProductionSection/claimToolsButton";
        }

        internal static class WarfrontCampaign
        {
            public const string GridWarfrontCampaign =
                "battleRoot/battleMain/battleCanvas/SafeArea/leftSideUI/notifications/Viewport/grid/WarfrontCampaign";

            public const string ClaimToolsButton =
                "menusRoot/menuCanvasParent/SafeArea/menuCanvas/menus/WorldMap/submenus/warfrontCampaignSubmenu/loot/claimButton";

            public const string CloseButton =
                "menusRoot/menuCanvasParent/SafeArea/menuCanvas/menus/WorldMap/closeButton";
        }

        internal static class Missions
        {
            public const string MissionRegion = "menusRoot/mapRoot/mapElements/missions";

            public const string SquadsQuantity =
                "menusRoot/menuCanvasParent/SafeArea/menuCanvas/menus/WorldMap/counters/squads/quantity";

            public const string StartMissionButton =
                "menusRoot/menuCanvasParent/SafeArea/menuCanvas/popups/PreviewMission/bg/managementBg/container/startMissionButton";

            public const string MissionRefreshTime =
                "menusRoot/menuCanvasParent/SafeArea/menuCanvas/menus/WorldMap/submenus/mapMissionsSubmenu/bottomLeftUI/missionRefreshCanvas/missionRefreshBg/missionRefreshText";

            public const string MissionCloseButton =
                "menusRoot/menuCanvasParent/SafeArea/menuCanvas/menus/WorldMap/closeButton";

            public const string GridMissionButton =
                "battleRoot/battleMain/battleCanvas/SafeArea/leftSideUI/notifications/Viewport/grid/MapMissions";
        }

        internal static class Expedition
        {
            public const string ExpeditionNotification =
                "battleRoot/battleMain/battleCanvas/SafeArea/leftSideUI/notifications/Viewport/grid/Expeditions";

            public const string ExpeditionCloseButton =
                "menusRoot/menuCanvasParent/SafeArea/menuCanvas/popups/Expeditions/bg/closeButton";

            public const string CurrenteExpedition =
                "menusRoot/menuCanvasParent/SafeArea/menuCanvas/popups/Expeditions/bg/expeditionsParent/activeExpeditionParent/activeExpedition";

            public const string PendingExpedition =
                "menusRoot/menuCanvasParent/SafeArea/menuCanvas/popups/Expeditions/bg/expeditionsParent/pendingExpeditionsParent/expeditionsScroll/Viewport/grid/expeditionPending0";
        }
    }
}