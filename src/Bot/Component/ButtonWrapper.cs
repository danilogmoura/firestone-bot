using System;
using System.Collections;
using Firebot.Core;
using Firebot.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Firebot.Bot.Component;

internal class ButtonWrapper : ComponentWrapper<Button>
{
    public ButtonWrapper(string path) : base(path) { }

    public bool IsInteractable() => IsActive() && ComponentCached.enabled && ComponentCached.interactable;

    public IEnumerator Click() => Click(BotSettings.InteractionDelay);

    private IEnumerator Click(float delay)
    {
        var success = ExecuteSafe(() =>
        {
            if (!IsInteractable())
                throw new InvalidOperationException("Button is not interactable.");

            ComponentCached.Select();
            ComponentCached.onClick.Invoke();

            LogManager.Debug(ObjectName, $"Clicked button at path: {_path}");
            return true;
        });

        if (success && delay > 0) yield return new WaitForSeconds(delay);
    }
}