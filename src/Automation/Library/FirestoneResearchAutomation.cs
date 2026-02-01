using System.Collections;
using Firebot.Automation.Core;
using Firebot.GameModel.Wrappers;
using static Firebot.Core.BotContext;
using static Firebot.Core.Paths.FirestoneResearch;
using static Firebot.Core.StringUtils;

namespace Firebot.Automation.Library;

internal class FirestoneResearchAutomation : AutomationObserver
{
    public override bool ShouldExecute() => base.ShouldExecute() && Buttons.Notification.IsActive();

    public override IEnumerator OnNotificationTriggered()
    {
        yield return Buttons.Notification.Click();

        if (!Panel.SubmenusWrapper.IsActive() && Panel.SelectResearch.IsActive())
            yield break;

        if (Panel.Slot0.IsActive() && Buttons.ButtonClainSlot0.IsInteractable())
            yield return Buttons.ButtonClainSlot0.Click();

        if (Panel.Slot1.IsActive() && Buttons.ButtonClainSlot1.IsInteractable())
            yield return Buttons.ButtonClainSlot1.Click();

        var submenus = Panel.SubmenusWrapper.GetChildren();

        foreach (var tree in submenus)
        {
            if (!tree.Name.StartsWith("tree") || !tree.IsActive()) continue;
            var slots = tree.GetChildren();

            foreach (var slot in slots)
            {
                if (!Panel.SelectResearch.IsActive()) break;

                var activeSlot = slot.RunSafe(() =>
                {
                    if (!slot.IsActive()) return false;
                    if (!slot.Name.StartsWith("firestoneResearch")) return false;

                    var bar = slot.Find("progressBarBg");
                    var glow = slot.Find("glow");

                    Log.Debug(
                        $"Checking slot {slot.Name}: Bar active = {bar.IsActive()}, Glow active = {glow.IsActive()}",
                        CorrelationId);

                    return bar.IsActive() && !glow.IsActive();
                }, defaultValue: false);

                if (!activeSlot) continue;
                yield return OpenPopup(JoinPath(SubmenusTreePath, tree.Name, slot.Name));

                if (!Panel.SubmenusWrapper.IsActive() || !Buttons.StartResearch.IsInteractable()) continue;

                yield return Buttons.StartResearch.Click();
                Log.Info($"Research started: {slot.Name}", CorrelationId);
            }
        }

        yield return Buttons.Close.Click();
    }

    private static IEnumerator OpenPopup(string paths)
    {
        var button = new ButtonWrapper(paths);
        if (!button.IsInteractable()) yield break;
        yield return button.Click();
    }

    private static class Panel
    {
        public static readonly TransformWrapper SubmenusWrapper = new(SubmenusTreePath);
        public static readonly TransformWrapper Slot0 = new(ResearchPanelDownPath + "/researchSlot0");
        public static readonly TransformWrapper Slot1 = new(ResearchPanelDownPath + "/researchSlot1");
        public static readonly TransformWrapper SelectResearch = new(SelectResearchTablePath);
    }

    private static class Buttons
    {
        public static readonly ButtonWrapper Notification = new(FirestoneResearchNotificationPath);

        public static readonly ButtonWrapper ButtonClainSlot0 =
            new(JoinPath(ResearchPanelDownPath, "researchSlot0/container/claimButton"));

        public static readonly ButtonWrapper ButtonClainSlot1 =
            new(JoinPath(ResearchPanelDownPath, "researchSlot1/container/claimButton"));

        public static readonly ButtonWrapper Close = new(MissionCloseButton);
        public static readonly ButtonWrapper StartResearch = new(PopupActivateButton);
    }
}