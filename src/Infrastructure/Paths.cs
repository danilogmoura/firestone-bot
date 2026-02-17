namespace Firebot.Infrastructure;

public static class Paths
{
    public static class Watchdog
    {
        private const string CanvasRoot = MenusLoc.CanvasLoc.Root;
        public const string EventsRoot = CanvasRoot + "/events";
        public const string PopupsRoot = CanvasRoot + "/popups";
        public const string MenusRoot = CanvasRoot + "/menus";
        public const string CloseSuffix = "/bg/closeButton";
        public const string CollectSuffix = "/bg/collectButton";
        public const string MenuCloseSuffix = "/closeButton";
    }

    public static class BattleLoc
    {
        public const string Root = "battleRoot";

        public static class NotificationsLoc
        {
            private const string Root = BattleLoc.Root +
                                        "/battleMain/battleCanvas/SafeArea/leftSideUI/notifications/Viewport/grid";

            public const string Engineer = Root + "/Engineer";
            public const string WarfrontCampaign = Root + "/WarfrontCampaign";
            public const string FreePickaxes = Root + "/FreePickaxes";
            public const string Expeditions = Root + "/Expeditions";
            public const string Quests = Root + "/Quests";
            public const string GuardianTraining = Root + "/GuardianTraining";
            public const string FirestoneResearch = Root + "/FirestoneResearch";
            public const string Experiments = Root + "/Experiments";
            public const string OracleRituals = Root + "/OracleRituals";
            public const string MapMissions = Root + "/MapMissions";
        }
    }

    public static class MenusLoc
    {
        private const string Root = "menusRoot";

        public static class CanvasLoc
        {
            public const string Root = MenusLoc.Root + "/menuCanvasParent/SafeArea/menuCanvas";

            public static class PopupsLoc
            {
                public const string Root = CanvasLoc.Root + "/popups";
                public const string CloseButton = "/bg/closeButton";
            }

            public static class MapMissionsLoc
            {
                private const string Root = CanvasLoc.Root + "/menus/WorldMap";
                private const string Sub = Root + "/submenus/mapMissionsSubmenu";
                public const string Close = Root + "/closeButton";

                public const string MissionRefresh =
                    Sub + "/bottomLeftUI/missionRefreshCanvas/missionRefreshBg/missionRefreshText";

                public static class MissionsLoc
                {
                    public static class PreviewLoc
                    {
                        private const string Root = PopupsLoc.Root + "/PreviewMission";
                        public const string Close = Root + "/bg/closeButton";
                        public const string StartBtn = Root + "/bg/managementBg/container/startMissionButton";
                        public const string NotEnoughSquads = Root + "/bg/managementBg/previewMissionNotEnoughSquads";

                        public const string MissionProgress =
                            Root +
                            "/bg/rewardBg/previewMissionTime/previewBar/missionProgress/activeMissionProgressText";
                    }

                    public static class PinLoc
                    {
                        public const string Root = MenusLoc.Root + "/mapRoot/mapElements/missions";
                        public const string ActiveIcon = "/missionActiveIcon";
                        public const string TimeReq = "/missionBg/missionTimeBg/missionTimeReq";
                        public const string Tick = "/missionBg/completedTick";
                    }
                }

                public static class WarfrontLoc
                {
                    private const string LootRoot = Root + "/submenus/warfrontCampaignSubmenu/loot";
                    public const string NextLoot = LootRoot + "/nextLootTimeLeft";
                    public const string Claim = LootRoot + "/claimButton";
                }
            }

            public static class EngineerLoc
            {
                public static class GarageLoc
                {
                    private const string Root = PopupsLoc.Root + "/GarageSelection";
                    public const string Close = Root + "/bg/closeButton";
                    public const string EngineerBtn = Root + "/bg/engineer";
                }

                public static class SubmenuLoc
                {
                    private const string Root = CanvasLoc.Root + "/menus/Engineer";
                    public const string Close = Root + "/closeButton";

                    public const string ClaimTools =
                        Root + "/submenus/bg/engineerSubmenu/toolsProductionSection/claimToolsButton";

                    public const string Cooldown = ClaimTools + "/cooldownOn/cooldownTimeLeft";
                }
            }
        }
    }
}