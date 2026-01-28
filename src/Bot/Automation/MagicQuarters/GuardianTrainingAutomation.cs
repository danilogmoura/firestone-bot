using System.Collections;
using Firebot.Bot.Automation.Core;
using Firebot.Bot.Component;
using Firebot.Core;
using UnityEngine;
using static Firebot.Utils.Paths.GuardianTraining;
using static Firebot.Utils.StringUtils;

namespace Firebot.Bot.Automation.MagicQuarters;

public class GuardianTrainingAutomation : AutomationObserver
{
    public override string SectionTitle => "Guardian Training";

    public override bool ShouldExecute() => base.ShouldExecute() && Button.Notification.IsActive();

    public override IEnumerator OnNotificationTriggered()
    {
        if (!Button.Notification.IsInteractable())
            yield break;

        yield return Button.Notification?.Click();

        if (!Object.Panel.IsActive()) yield break;

        if (Object.CooldownOn == null || Object.CooldownOn.IsActive())
        {
            yield return Button.Close?.Click();
            yield break;
        }


        var guardians = new TransformWrapper(GuardianList).GetChildren();

        foreach (var guardianRoot in guardians)
        {
            var starsParent = guardianRoot.Find("starsParent");
            if (Object.CooldownOn.IsActive()) break;

            if (Button.Training != null &&
                (!starsParent.IsActive() || !Button.Training.IsInteractable())) continue;

            yield return Button.Training?.Click();

            yield return new ButtonWrapper(JoinPath(GuardianList, guardianRoot.Name)).Click();
        }

        yield return Button.Close.Click();

        yield return new WaitForSeconds(BotSettings.InteractionDelay);
    }

    private static class Button
    {
        public static readonly ButtonWrapper Notification = new(GuardianTrainingNotification);
        public static readonly ButtonWrapper Close = new(CloseButton);
        public static readonly ButtonWrapper Training = new(TrainingButton);
    }

    private readonly struct Object
    {
        public static readonly TransformWrapper CooldownOn = new(JoinPath(TrainingButton, "cooldownOn"));
        public static readonly TransformWrapper Panel = new(MenuMagicQuarters);
    }
}