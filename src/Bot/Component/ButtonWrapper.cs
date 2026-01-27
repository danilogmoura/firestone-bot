using System;
using System.Collections;
using Firebot.Core;
using UnityEngine;
using UnityEngine.UI;
using Logger = Firebot.Utils.Logger;

namespace Firebot.Bot.Component;

internal class ButtonWrapper : ComponentWrapper<Button>
{
    private static readonly Logger Log = new(nameof(ButtonWrapper));

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

            Log.Debug($"Clicked button at path: {Path}");
            return true;
        });

        if (success && delay > 0) yield return new WaitForSeconds(delay);
    }
}