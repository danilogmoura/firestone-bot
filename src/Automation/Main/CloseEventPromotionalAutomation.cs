using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebot.Automation.Core;
using Firebot.GameModel.TMProComponents;
using Firebot.GameModel.Wrappers;
using UnityEngine;
using static Firebot.Core.BotContext;
using static Firebot.Core.BotSettings;
using static Firebot.Core.Paths;
using static Firebot.Core.StringUtils;

namespace Firebot.Automation.Main;

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

            Log.Debug($"Event: {eventName} ({popup.TitleText.Text})", CorrelationId);

            if (!popup.IsActive()) continue;

            Log.Debug($"Target event found: {eventName}. Closing and disabling automation.", CorrelationId);

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