using System.Collections;
using System.Collections.Generic;
using FireBot.Bot.Component;
using FireBot.Utils;
using static FireBot.Utils.Paths.OracleRituals;
using static FireBot.Utils.StringUtils;

namespace FireBot.Bot.Automation.Oracle
{
    public static class OracleRitualsAutomation
    {
        private static readonly List<Ritual> RitualsCache = new List<Ritual>();

        public static IEnumerator Process()
        {
            if (!Buttons.RitualNotification.IsInteractable())
                yield break;

            yield return Buttons.RitualNotification.Click();

            if (!Panel.OraclePanel.IsActive()) yield break;
            LogManager.SubHeader("Oracle Rituals");

            if (!Panel.OracleRitualGrid.IsActive()) yield break;
            UpdateRitualCache();

            yield return RitualsCache.Find(r => r.IsClaimable)?.ClaimButton.Click();

            yield return RitualsCache.Find(ritual => ritual.IsReady)?.StartButton.Click();

            yield return Buttons.CloseRituals.Click();
        }

        private static void UpdateRitualCache()
        {
            RitualsCache.Clear();

            var ritualRoot = Panel.OracleRitualGrid.Transform;
            if (ritualRoot == null) return;

            for (var i = 0; i < ritualRoot.childCount; i++)
            {
                var ritual = ritualRoot.GetChild(i);
                if (ritual == null || !ritual.gameObject.activeSelf || ritual.Find("locked").gameObject.activeSelf ||
                    ritual.Find("claimedObj").gameObject.activeSelf) continue;

                var nodeName = ritual.name;
                var ready = ritual.Find("startButton").gameObject.activeSelf;
                var claimable = ritual.Find("claimButton").gameObject.activeSelf;

                RitualsCache.Add(new Ritual(nodeName, ready, claimable));
            }
        }

        private class Ritual
        {
            public Ritual(string nodeName, bool ready, bool claimable)
            {
                IsReady = ready;
                IsClaimable = claimable;

                ClaimButton = new ButtonWrapper(JoinPath(RitualGrid, nodeName, "claimButton"));
                StartButton = new ButtonWrapper(JoinPath(RitualGrid, nodeName, "startButton"));
            }

            public bool IsReady { get; }
            public bool IsClaimable { get; }
            public ButtonWrapper ClaimButton { get; }
            public ButtonWrapper StartButton { get; }
        }

        private static class Buttons
        {
            public static ButtonWrapper RitualNotification => new ButtonWrapper(Notification);

            public static ButtonWrapper CloseRituals => new ButtonWrapper(CloseButton);
        }

        private static class Panel
        {
            public static ObjectWrapper OraclePanel => new ObjectWrapper(MenuOracle);

            public static ObjectWrapper OracleRitualGrid => new ObjectWrapper(RitualGrid);
        }
    }
}