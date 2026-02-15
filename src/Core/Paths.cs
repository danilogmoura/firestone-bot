namespace Firebot.Core;

public static class Paths
{
    public static class BattleLoc
    {
        public const string Root = "battleRoot";

        public static class MainHudLoc
        {
            public const string Root = BattleLoc.Root + "/battleMain/battleCanvas/SafeArea";
            private const string Buttons = Root + "/rightSideUI/menuButtons";

            public const string TownButton = Buttons + "/townButton";
            public const string MapButton = Buttons + "/mapButton";
            public const string GuildButton = Buttons + "/guildButton";
            public const string StoreButton = Buttons + "/storeButton";
            public const string PathOfGloryButton = Root + "/bottomSideUIDesktop/pathOfGloryButton";
            public const string EventsButton = Root + "/bottomSideUIDesktop/eventsButton";
        }
    }

    public static class MenusLoc
    {
        public const string Root = "menusRoot";

        public static class CanvasLoc
        {
            public const string Root = MenusLoc.Root + "/menuCanvasParent/SafeArea/menuCanvas";

            public static class PopupsLoc
            {
                public const string Root = CanvasLoc.Root + "/popups";
                public const string CloseButton = "/bg/closeButton"; // Sufixo comum
            }

            public static class MapMissionsLoc
            {
                public const string Root = CanvasLoc.Root + "/menus/WorldMap";
                private const string Sub = Root + "/submenus/mapMissionsSubmenu";

                public static class HUDLoc
                {
                    private const string Root = MapMissionsLoc.Root;
                    public const string Close = Root + "/closeButton";

                    public const string Refresh =
                        Sub + "/bottomLeftUI/missionRefreshCanvas/missionRefreshBg/missionRefreshText";

                    public const string WarfrontBtn = Root + "/submenuButtons/warfrontCampaignButton";
                }

                public static class MissionsLoc
                {
                    public const string ActiveGrid = Sub + "/activeMissionsCanvas/activeMissions/Viewport/grid";

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

            public static class TownHUDLoc
            {
                private const string Root = CanvasLoc.Root + "/menus/TownIrongard";
                private const string Hub = Root + "/townBg/parent";

                public const string Engineer = Hub + "/engineer";
                public const string MagicQuarters = Hub + "/magicQuarters";
                public const string Library = Hub + "/library";
                public const string Alchemist = Hub + "/alchemist";
                public const string Oracle = Hub + "/oracle";
                public const string Close = Root + "/closeButton";
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