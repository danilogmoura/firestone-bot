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
        private const string Root = "battleRoot/battleMain/battleCanvas/SafeArea";

        public static class NotificationsLoc
        {
            public const string Root = BattleLoc.Root + "/leftSideUI/notifications/Viewport/grid";

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

        public static class PlayerAvatarLoc
        {
            private const string Root = BattleLoc.Root + "/topLeftSideUI/playerAvatar";

            public const string CharacterLevel = Root + "/characterLevelBg/characterLevel";
        }

        public static class BottomSideUIDesktopLoc
        {
            private const string Root = BattleLoc.Root + "/bottomSideUIDesktop";

            public static class LeaderPanelLoc
            {
                private const string Root = BottomSideUIDesktopLoc.Root + "/leaderPanel";

                public const string HotKeyOneBtn = Root + "/abilityBattle (0)";

                public const string HotKeyTwoBtn = Root + "/abilityBattle (1)";

                public const string HotKeyThreeBtn = Root + "/abilityBattle (2)";
            }
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

                        public const string SpeedUpBtn = Root + "/bg/managementBg/container/speedUpButton";

                        public const string SpeedUpFinishDesc = SpeedUpBtn + "/finishDesc";

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

                    public static class MissionRewardsLoc
                    {
                        private const string Root = PopupsLoc.Root + "/MissionRewards";

                        public const string CloseBtn = Root + PopupsLoc.CloseBtn;
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
                        GuardianStarsIcon = "/starsParent";

                    public static class LockedGuardianLoc
                    {
                        private const string Root = PopupsLoc.Root + "/LockedGuardian";

                        public const string CloseBtn = Root + PopupsLoc.CloseBtn;
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

                        public const string NextRunTimeTxt = "/container/researchInfo/progressBarBg/timeLeftText";

                        public const string SpeedUpBtn = "/container/speedUpButton";

                        public const string SpeedUpFinishDesc = SpeedUpBtn + "/finishDesc";
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

                public static class OracleLoc
                {
                    private const string Root = CanvasLoc.Root + "/menus/Oracle";

                    public const string CloseBtn = Root + "/closeButton";

                    public static class RitualLoc
                    {
                        private const string Root = OracleLoc.Root + "/submenus/bg/ritualSubmenu";

                        public const string Rituals = Root + "/ritualsGrid";

                        public const string ClaimBtn = "/claimButton";

                        public const string CurrentRunTimeTxt = "/ritualProgressBg/timeLeftText";

                        public const string StartBtn = "/startButton";

                        public const string NextRunTimeTxt = Root + "/timeBg/timeLeft";
                    }
                }
            }

            public static class GuildLoc
            {
                public static class GuildShopLoc
                {
                    private const string Root = CanvasLoc.Root + "/menus/GuildShop";

                    public const string CloseBtn = Root + "/closeButton";

                    public static class FreePickaxeLoc
                    {
                        private const string Root = GuildShopLoc.Root + "/bg/submenus/supplies/items/freePickaxe";

                        public const string ClaimBtn = Root;

                        public const string QuantityTxt = Root + "/claimBg/itemBg/itemQuantity";

                        public const string NextRunTimeTxt = Root + "/nextFreeObj/progressBarBg/timeLeftText";
                    }
                }

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