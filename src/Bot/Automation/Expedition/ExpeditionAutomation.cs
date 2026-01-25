using System.Collections;
using Firebot.Bot.Automation.Core;
using Firebot.Bot.Component;
using Firebot.Utils;
using MelonLoader;
using static Firebot.Utils.Paths.Expedition;
using static Firebot.Utils.StringUtils;

namespace Firebot.Bot.Automation.Expedition;

public class ExpeditionAutomation : AutomationObserver
{
    private MelonPreferences_Entry<float> _checkInterval;

    private MelonPreferences_Entry<bool> _collectRewards;

    public override string SectionName => "Expedition";

    public override int Priority => 80;

    protected override void OnConfigure(MelonPreferences_Category category)
    {
        _collectRewards = category.CreateEntry("AutoCollect", true, "Auto Collect Rewards", "Auto Collect Rewards");
        _checkInterval = category.CreateEntry("CheckInterval", 60.0f, "Check Interval (s)", "Check Interval (s)");
    }

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
        public static readonly ButtonWrapper Notification = new(ExpeditionNotification);
        public static readonly ButtonWrapper Close = new(CloseButton);
    }

    private static class Expeditions
    {
        public static readonly CurrentExpeditionSection CurrentExpedition = new();
        public static readonly PendingExpeditionSection PendingExpedition = new();
    }

    private class CurrentExpeditionSection : ObjectWrapper
    {
        private readonly ButtonWrapper _claimButton = new(JoinPath(CurrentExpeditionPath, "claimButton"));

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
        private readonly ButtonWrapper _startButton = new(JoinPath(PendingExpeditionPath, "startButton"));

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