using System.Collections;
using System.Collections.Generic;
using Firebot.Old._Old.Wrappers;
using Firebot.Old.Automation.Core;
using static Firebot.Old.Core.Paths.OracleRituals;
using static Firebot.Utilities.StringUtils;

namespace Firebot.Old.Automation.Oracle;

public class OracleRitualsAutomation : AutomationObserver
{
    private static readonly List<Ritual> RitualsCache = new();

    public override bool ShouldExecute() => base.ShouldExecute() && Buttons.Notification.IsActive();

    public override IEnumerator OnNotificationTriggered()
    {
        yield return Buttons.Notification.Click();

        if (!Panel.OraclePanel.IsActive()) yield break;

        if (!Panel.OracleRitualGrid.IsActive()) yield break;
        UpdateRitualCache();

        yield return RitualsCache.Find(r => r.IsClaimable)?.ClaimButton.Click();

        yield return RitualsCache.Find(ritual => ritual.IsReady)?.StartButton.Click();

        yield return Buttons.CloseRituals.Click();
    }

    private static void UpdateRitualCache()
    {
        RitualsCache.Clear();

        var ritualRoot = Panel.OracleRitualGrid;
        if (!ritualRoot.Exists()) return;

        var rituals = ritualRoot.GetChildren();
        foreach (var ritual in rituals)
        {
            if (!ritual.IsActive() || ritual.Find("locked").IsActive() ||
                ritual.Find("claimedObj").IsActive()) continue;

            var nodeName = ritual.Name;
            var ready = ritual.Find("startButton").IsActive();
            var claimable = ritual.Find("claimButton").IsActive();

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
        public static readonly ButtonWrapper Notification = new(OracleRitualNotification);
        public static readonly ButtonWrapper CloseRituals = new(CloseButton);
    }

    private static class Panel
    {
        public static readonly TransformWrapper OraclePanel = new(MenuOracle);
        public static readonly TransformWrapper OracleRitualGrid = new(RitualGrid);
    }
}