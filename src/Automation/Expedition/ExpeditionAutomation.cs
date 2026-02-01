using System.Collections;
using Firebot.Automation.Core;
using Firebot.GameModel.Wrappers;
using static Firebot.Core.Paths.Expedition;
using static Firebot.Core.StringUtils;

namespace Firebot.Automation.Expedition;

public class ExpeditionAutomation : AutomationObserver
{
    public override string SectionTitle => "Expedition";

    public override int Priority => 80;

    public override bool ShouldExecute() => base.ShouldExecute() && Button.Notification.IsActive();

    public override IEnumerator OnNotificationTriggered()
    {
        if (!Button.Notification.IsActive()) yield break;

        yield return Button.Notification.Click();

        if (Expeditions.CurrentExpedition.IsCompleted())
            yield return Expeditions.CurrentExpedition.CollectRewards();

        if (!Expeditions.CurrentExpedition.IsActive() && Expeditions.PendingExpedition.IsActive())
            yield return Expeditions.PendingExpedition.StartExpedition();

        yield return Button.Close.Click();
    }

    private static class Button
    {
        public static readonly ButtonWrapper Notification = new(ExpeditionNotification);
        public static readonly ButtonWrapper Close = new(CloseButton);
    }

    private static class Expeditions
    {
        public static readonly CurrentExpeditionSection CurrentExpedition = new();
        public static readonly PendingExpeditionSection PendingExpedition = new();
    }

    private class CurrentExpeditionSection : TransformWrapper
    {
        private readonly ButtonWrapper _claimButton = new(JoinPath(CurrentExpeditionPath, "claimButton"));

        public CurrentExpeditionSection() : base(CurrentExpeditionPath) { }

        public bool IsCompleted() => _claimButton.IsInteractable();

        public IEnumerator CollectRewards()
        {
            if (_claimButton.IsInteractable()) yield return _claimButton.Click();
        }
    }

    private class PendingExpeditionSection : TransformWrapper
    {
        private readonly ButtonWrapper _startButton = new(JoinPath(PendingExpeditionPath, "startButton"));

        public PendingExpeditionSection() : base(PendingExpeditionPath) { }

        public IEnumerator StartExpedition()
        {
            if (_startButton.IsInteractable())
                yield return _startButton.Click();
        }
    }
}