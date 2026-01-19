using System.Collections;
using FireBot.Bot.Automation.Core;
using FireBot.Bot.Component;
using FireBot.Utils;
using static FireBot.Utils.Paths.GuardianTraining;
using static FireBot.Utils.StringUtils;

namespace FireBot.Bot.Automation.MagicQuarters
{
    public class GuardianTrainingAutomation : AutomationObserver
    {
        public override bool ToogleCondition()
        {
            return Button.Notification.IsActive();
        }

        public override IEnumerator OnNotificationTriggered()
        {
            if (!Button.Notification.IsInteractable())
                yield break;

            yield return Button.Notification?.Click();

            if (!Object.Panel.IsActive()) yield break;

            LogManager.SubHeader("Guardian Training");

            if (Object.CooldownOn == null || Object.CooldownOn.IsActive())
            {
                yield return Button.Close?.Click();
                yield break;
            }

            var guardianList = new ObjectWrapper(GuardianList).Transform;
            for (var i = 0; i < guardianList.childCount; i++)
            {
                var guardianRoot = guardianList.GetChild(i);
                var starsParent = guardianRoot.Find("starsParent");

                if (Object.CooldownOn.IsActive()) break;

                if (Button.Training != null &&
                    (!starsParent.gameObject.activeSelf || !Button.Training.IsInteractable())) continue;

                yield return Button.Training?.Click();

                yield return new ButtonWrapper(JoinPath(GuardianList, guardianRoot.name)).Click();
            }

            yield return Button.Close.Click();
        }

        private static class Button
        {
            public static readonly ButtonWrapper Notification = new ButtonWrapper(GuardianTrainingNotification);

            public static readonly ButtonWrapper Close = new ButtonWrapper(CloseButton);

            public static readonly ButtonWrapper Training = new ButtonWrapper(TrainingButton);
        }

        private readonly struct Object
        {
            public static readonly ObjectWrapper CooldownOn = new ObjectWrapper(JoinPath(TrainingButton, "cooldownOn"));

            public static readonly ObjectWrapper Panel = new ObjectWrapper(MenuMagicQuarters);
        }
    }
}