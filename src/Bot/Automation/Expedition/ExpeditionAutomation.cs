using System.Collections;
using FireBot.Bot.Automation.Core;
using FireBot.Bot.Component;
using FireBot.Utils;
using static FireBot.Utils.Paths.Expedition;
using static FireBot.Utils.StringUtils;

namespace FireBot.Bot.Automation.Expedition
{
    public class ExpeditionAutomation : AutomationObserver
    {
        public override bool ToogleCondition()
        {
            return Button.Notification.IsActive();
        }

        public override IEnumerator OnNotificationTriggered()
        {
            if (!Button.Notification.IsActive()) yield break;

            LogManager.SubHeader("Expedition");
            yield return Button.Notification.Click();

            if (Expeditions.CurrentExpedition.IsCompleted())
                yield return Expeditions.CurrentExpedition.CollectRewards();

            if (!Expeditions.CurrentExpedition.IsActive() && Expeditions.PendingExpedition.IsActive())
                yield return Expeditions.PendingExpedition.StartExpedition();

            yield return Button.Close.Click();
        }

        private static class Button
        {
            public static readonly ButtonWrapper Notification = new ButtonWrapper(ExpeditionNotification);
            public static readonly ButtonWrapper Close = new ButtonWrapper(CloseButton);
        }

        private static class Expeditions
        {
            public static readonly CurrentExpeditionSection CurrentExpedition = new CurrentExpeditionSection();
            public static readonly PendingExpeditionSection PendingExpedition = new PendingExpeditionSection();

            public static readonly ObjectWrapper Temp =
                new ObjectWrapper(JoinPath(CurrentExpeditionPath, "claimButton"));
        }

        private class CurrentExpeditionSection : ObjectWrapper
        {
            private readonly ButtonWrapper _claimButton =
                new ButtonWrapper(JoinPath(CurrentExpeditionPath, "claimButton"));

            public CurrentExpeditionSection() : base(CurrentExpeditionPath)
            {
            }

            public bool IsCompleted()
            {
                var timeLabel =
                    new TextMeshProUGUIWrapper(JoinPath(CurrentExpeditionPath, "expeditionProgressBg/timeLeftText"));

                if (!IsActive() && !timeLabel.Exists()) return false;

                var text = timeLabel.GetParsedText();

                //TODO: find better way to check completion
                return text.Contains("Completed");
            }

            public IEnumerator CollectRewards()
            {
                if (_claimButton.IsInteractable()) yield return _claimButton.Click();
            }
        }

        private class PendingExpeditionSection : ObjectWrapper
        {
            private readonly ButtonWrapper _startButton =
                new ButtonWrapper(JoinPath(PendingExpeditionPath, "startButton"));

            public PendingExpeditionSection() : base(PendingExpeditionPath)
            {
            }

            public IEnumerator StartExpedition()
            {
                if (_startButton.IsInteractable())
                    yield return _startButton.Click();
            }
        }
    }
}