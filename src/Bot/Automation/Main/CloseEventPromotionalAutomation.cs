using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebot.Bot.Automation.Core;
using Firebot.Bot.Component;
using Firebot.Bot.Component.TextMeshPro;
using UnityEngine;
using static Firebot.Core.BotSettings;
using static Firebot.Utils.Paths;
using static Firebot.Utils.StringUtils;

namespace Firebot.Bot.Automation.Main;

public class CloseEventPromotionalAutomation : AutomationObserver
{
    private static readonly HashSet<string> TargetEvents = new()
    {
        "AnniversaryEventPromotional",
        "DecoratedHeroesPromotion"
    };

    private bool _hasExecutedSuccessfully;

    public override int Priority => 25;

    public override bool ShouldExecute() => base.ShouldExecute() && !_hasExecutedSuccessfully;

    public override IEnumerator OnNotificationTriggered()
    {
        var rootEvents = new TransformWrapper(EventsPopupPath);
        if (!rootEvents.IsActive() || !rootEvents.HasChildren()) yield break;

        var activeTargets = rootEvents.GetChildren().Where(child =>
                child.IsActive() && TargetEvents.Contains(child.Name))
            .ToList();

        foreach (var eventFolder in activeTargets)
        {
            var eventName = eventFolder.Name;
            var popupPath = JoinPath(EventsPopupPath, eventName);
            var popup = new PopupWrapper(popupPath);

            log.Debug($"Event: {eventName} ({popup.TitleText.Text})");

            if (!popup.IsActive()) continue;

            log.Info($"Target event found: {eventName}. Closing and disabling automation.");

            yield return popup.CloseButton.Click();
        }

        _hasExecutedSuccessfully = true;
        yield return new WaitForSeconds(InteractionDelay);
    }

    private class PopupWrapper : ComponentWrapper<Transform>
    {
        public PopupWrapper(string path) : base(path) { }
        public ButtonWrapper CloseButton => new(JoinPath(Path, "bg/closeButton"));
        public TextDisplay TitleText => new(JoinPath(Path, "bg/titleBg/menuTitle"));
    }
}