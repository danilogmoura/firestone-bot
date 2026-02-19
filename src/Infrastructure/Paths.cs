namespace Firebot.Infrastructure;

public static class Paths
{
    public static class Watchdog
    {
        private const string CanvasRoot = "menusRoot/menuCanvasParent/SafeArea/menuCanvas";
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

            public const string EngineerBtn = Root + "/Engineer";
            public const string WarfrontCampaignBtn = Root + "/WarfrontCampaign";
            public const string FreePickaxesBtn = Root + "/FreePickaxes";
            public const string ExpeditionsBtn = Root + "/Expeditions";
            public const string QuestsBtn = Root + "/Quests";
            public const string GuardianTrainingBtn = Root + "/GuardianTraining";
            public const string FirestoneResearchBtn = Root + "/FirestoneResearch";
            public const string ExperimentsBtn = Root + "/Experiments";
            public const string OracleRitualsBtn = Root + "/OracleRituals";
            public const string MapMissionsBtn = Root + "/MapMissions";
        }
    }

    public static class MenusLoc
    {
        private const string Root = "menusRoot";

        public static class CanvasLoc
        {
            private const string Root = MenusLoc.Root + "/menuCanvasParent/SafeArea/menuCanvas";

            private static class PopupsLoc
            {
                public const string Root = CanvasLoc.Root + "/popups";
                public const string CloseBtn = "/bg/closeButton";
            }

            public static class MapLoc
            {
                private const string Root = CanvasLoc.Root + "/menus/WorldMap";
                private const string Sub = Root + "/submenus/mapMissionsSubmenu";
                public const string CloseBtn = Root + "/closeButton";

                public const string NextRunTimeTxt =
                    Sub + "/bottomLeftUI/missionRefreshCanvas/missionRefreshBg/missionRefreshText";

                public static class MissionsLoc
                {
                    public static class PreviewLoc
                    {
                        private const string Root = PopupsLoc.Root + "/PreviewMission";
                        public const string CloseBtn = Root + PopupsLoc.CloseBtn;
                        public const string StartBtn = Root + "/bg/managementBg/container/startMissionButton";

                        public const string NotEnoughSquadsTxt =
                            Root + "/bg/managementBg/previewMissionNotEnoughSquads";

                        public const string NextRunTimeTxt =
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
                    public const string NextRunTimeTxt = LootRoot + "/nextLootTimeLeft";
                    public const string ClaimBtn = LootRoot + "/claimButton";
                }
            }

            public static class TownLoc
            {
                public static class EngineerLoc
                {
                    private const string Root = CanvasLoc.Root + "/menus/Engineer";
                    public const string CloseBtn = Root + "/closeButton";

                    public const string ClaimBtn =
                        Root + "/submenus/bg/engineerSubmenu/toolsProductionSection/claimToolsButton";

                    public const string NextRunTimeTxt = ClaimBtn + "/cooldownOn/cooldownTimeLeft";
                }

                public static class MagicQuarters
                {
                    private const string Root = CanvasLoc.Root + "/menus/MagicQuarters";
                    public const string CloseBtn = Root + "/closeButton";
                    public const string Guardoians = Root + "/guardianList";
                    private const string UnlockedGuadian = Root + "/submenus/bg/infoSubmenu/activities/unlocked";
                    public const string EnlightenmentBtn = UnlockedGuadian + "/enlightenment/enlightenmentButton";
                    public const string TrainBtn = UnlockedGuadian + "/train/trainButton";
                    public const string NextRunTimeTxt = TrainBtn + "/cooldownOn/cooldownTimeLeft";

                    public const string
                        GuardianStarsIcon =
                            "/starsParent"; // Relative to each guardian button, e.g. guardianList/guardian0/starsParent

                    public static class LockedGuardianLoc
                    {
                        private const string Root = PopupsLoc.Root + "/LockedGuardian";
                        public const string CloseBtn = Root + "/bg/closeButton";
                    }
                }

                public static class LibraryLoc
                {
                    private const string Root = CanvasLoc.Root + "/menus/Library";
                    public const string CloseBtn = Root + "/closeButton";

                    public static class ResearchPanelLoc
                    {
                        public const string Root = LibraryLoc.Root + "/submenus/firestoneResearch/researchPanel";

                        public const string SelectResearchTable = Root + "/selectResearchTable";

                        public const string ClaimBtn = "/container/claimButton";

                        public const string NextRunTimeTxt =
                            "/container/researchInfo/progressBarBg/timeLeftText";
                    }

                    public static class NodeLoc
                    {
                        public const string Root = LibraryLoc.Root +
                                                   "/submenus/firestoneResearch/researchScrollView/viewport/content/submenus";

                        public const string Glow = "/glow";

                        public const string ProgressBar = "/progressBarBg";

                        public const string CompletedTxt = "/genericText";
                    }

                    public static class PreviewLoc
                    {
                        private const string Root = PopupsLoc.Root + "/FirestoneResearchPreview";

                        public const string UnlockedTxt = Root + "/bg/innerBg/unlocked";

                        public const string MaxedTxt = Root + "/bg/innerBg/maxed";

                        public const string ActivateBtn =
                            Root + "/bg/innerBg/unlocked/buttonHolder/researchActivateButton";
                    }
                }
            }

            public static class GuildLoc
            {
                public static class ExpeditionLoc
                {
                    private const string Root = PopupsLoc.Root + "/Expeditions";
                    public const string CloseBtn = Root + PopupsLoc.CloseBtn;

                    public const string NextRunTimeTxt = Root + "/bg/timeLeftBg/timeLeftText";

                    private const string ExpeditionsParents = Root + "/bg/expeditionsParent";

                    public const string ActiveExpedition =
                        ExpeditionsParents + "/activeExpeditionParent/activeExpedition";

                    public const string ClaimBtn = ActiveExpedition + "/claimButton";

                    public const string CurrentRunTimeTxt =
                        ActiveExpedition + "/expeditionProgressBg/timeLeftText";

                    public const string StartBtn =
                        ExpeditionsParents +
                        "/pendingExpeditionsParent/expeditionsScroll/Viewport/grid/expeditionPending0/startButton";
                }
            }
        }
    }
}