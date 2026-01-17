namespace FireBot.Utils
{
    public abstract class Paths
    {
        private const string BattleUI = "battleRoot/battleMain/battleCanvas/SafeArea/leftSideUI";

        private const string NotificationsGrid = BattleUI + "/notifications/Viewport/grid";

        private const string MenuBase = "menusRoot/menuCanvasParent/SafeArea/menuCanvas";

        private const string Menus = MenuBase + "/menus";

        private const string Popups = MenuBase + "/popups";

        internal static class Engineer
        {
            public const string EngineerGridNotification = NotificationsGrid + "/Engineer";

            public const string ClaimToolsButton =
                Menus + "/Engineer/submenus/bg/engineerSubmenu/toolsProductionSection/claimToolsButton";

            public const string CloseButton = Menus + "/Engineer/closeButton";
        }

        internal static class WarfrontCampaign
        {
            public const string WarfrontCampaignNotification = NotificationsGrid + "/WarfrontCampaign";

            public const string ClaimToolsButton =
                Menus + "/WorldMap/submenus/warfrontCampaignSubmenu/loot/claimButton";

            public const string CloseButton = Menus + "/WorldMap/closeButton";
        }

        internal static class Missions
        {
            public const string MenuWorldMap = Menus + "/WorldMap";

            public const string MissionRegion = "menusRoot/mapRoot/mapElements/missions";

            public const string SquadsQuantity = MenuWorldMap + "/counters/squads/quantity";

            public const string StartMissionButton =
                Popups + "/PreviewMission/bg/managementBg/container/startMissionButton";

            public const string MissionRefreshTime =
                MenuWorldMap +
                "/submenus/mapMissionsSubmenu/bottomLeftUI/missionRefreshCanvas/missionRefreshBg/missionRefreshText";

            public const string MissionCloseButton = MenuWorldMap + "/closeButton";

            public const string MapMissionNotification = NotificationsGrid + "/MapMissions";
        }

        internal static class Expedition
        {
            public const string ExpeditionNotification = NotificationsGrid + "/Expeditions";

            public const string ExpeditionsParents = Popups + "/Expeditions/bg/expeditionsParent";

            public const string CurrentExpeditionPath = ExpeditionsParents + "/activeExpeditionParent/activeExpedition";

            public const string PendingExpeditionPath =
                ExpeditionsParents + "/pendingExpeditionsParent/expeditionsScroll/Viewport/grid/expeditionPending0";

            public const string CloseButton = Popups + "/Expeditions/bg/closeButton";
        }

        internal static class FirestoneResearch
        {
            public const string LibrarySubmenus = Menus + "/Library/submenus";

            public const string FirestoneResearchNotificationPath = NotificationsGrid + "/FirestoneResearch";

            public const string MissionCloseButton = Menus + "/Library/closeButton";

            public const string ResearchPanelDownPath = LibrarySubmenus + "/firestoneResearch/researchPanel";

            public const string SelectResearchTablePath =
                LibrarySubmenus + "/firestoneResearch/researchPanel/selectResearchTable";

            public const string SubmenusTreePath =
                LibrarySubmenus + "/firestoneResearch/researchScrollView/viewport/content/submenus";

            public const string PopupActivateButton =
                Popups + "/FirestoneResearchPreview/bg/innerBg/unlocked/buttonHolder/researchActivateButton";
        }

        internal static class OracleRituals
        {
            public const string OracleRitualNotification = NotificationsGrid + "/OracleRituals";

            public const string MenuOracle = Menus + "/Oracle";

            public const string CloseButton = MenuOracle + "/closeButton";

            public const string RitualGrid = MenuOracle + "/submenus/bg/ritualSubmenu/ritualsGrid";
        }

        internal static class BattleRoot
        {
            public const string OfflineProgressPopup = MenuBase + "/popups/OfflineProgress";

            public const string OfflineProgressPopupClaimButton = OfflineProgressPopup + "/bg/collectButton";
        }

        internal static class GuardianTraining
        {
            public const string GuardianTrainingNotification = NotificationsGrid + "/GuardianTraining";

            public const string MenuMagicQuarters = Menus + "/MagicQuarters";

            public const string GuardianList = MenuMagicQuarters + "/guardianList";

            public const string CloseButton = MenuMagicQuarters + "/closeButton";

            public const string TrainingButton =
                MenuMagicQuarters + "/submenus/bg/infoSubmenu/activities/unlocked/train/trainButton";
        }

        internal static class GuildShop
        {
            public const string FreePickaxesNotificationPath = NotificationsGrid + "/FreePickaxes";

            public const string MenuGuildShopPath = Menus + "/GuildShop";

            public const string CloseButtonPath = MenuGuildShopPath + "/closeButton";

            public const string FreePickaxeItemPath = MenuGuildShopPath + "/bg/submenus/supplies/items/freePickaxe";
        }
    }
}