using System;
using System.Collections;
using Firebot.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Firebot.GameModel.Wrappers;

internal class ButtonWrapper : ComponentWrapper<Button>
{
    public ButtonWrapper(string path) : base(path) { }

    public bool IsInteractable() => RunSafe(() => IsActive() && Component.enabled && Component.interactable);

    public IEnumerator Click() => Click(BotSettings.InteractionDelay);

    private IEnumerator Click(float delay)
    {
        RunSafe(() =>
        {
            if (!IsInteractable())
                throw new Exception("\"Button not interactable\", contextInfo: Path, correlationId: CorrelationId");

            Component.Select();
            Component.onClick.Invoke();

            Log.Debug("Click executed", CorrelationId);
        }, CorrelationId);

        if (delay > 0) yield return new WaitForSeconds(delay);
    }
}