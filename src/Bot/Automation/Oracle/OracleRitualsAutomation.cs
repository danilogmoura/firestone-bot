using System.Collections;
using System.Collections.Generic;
using FireBot.Bot.Automation.Core;
using FireBot.Bot.Component;
using FireBot.Utils;
using static FireBot.Utils.Paths.OracleRituals;
using static FireBot.Utils.StringUtils;

namespace FireBot.Bot.Automation.Oracle
{
    public class OracleRitualsAutomation : AutomationObserver
    {
        private static readonly List<Ritual> RitualsCache = new List<Ritual>();

        public override bool ToogleCondition()
        {
            return Buttons.Notification.IsActive();
        }

        public override IEnumerator OnNotificationTriggered()
        {
            if (!Buttons.Notification.IsInteractable())
                yield break;

            yield return Buttons.Notification.Click();

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
            public static readonly ButtonWrapper Notification = new ButtonWrapper(OracleRitualNotification);

            public static readonly ButtonWrapper CloseRituals = new ButtonWrapper(CloseButton);
        }

        private static class Panel
        {
            public static readonly ObjectWrapper OraclePanel = new ObjectWrapper(MenuOracle);

            public static readonly ObjectWrapper OracleRitualGrid = new ObjectWrapper(RitualGrid);
        }
    }
}