using System;
using System.Collections;
using Firebot.GameModel.Base;
using Il2Cpp;
using UnityEngine;
using UnityEngine.UI;
using static Firebot.Core.BotSettings;

namespace Firebot.GameModel.Primitives;

public class GameNotificationButton : GameButton
{
    public GameNotificationButton(string path = null, GameElement parent = null, Transform transform = null)
        : base(path, parent, transform) { }

    public override IEnumerator Click()
    {
        if (TryGetComponent(out NotificationInteraction interaction))
            try
            {
                interaction.notificationButton.onClick.Invoke();
                Debug($"[QUICK ACCESS] Triggered via NotificationInteraction. Path: {Path}");
            }
            catch (Exception e)
            {
                Debug($"[QUICK ACCESS] Error calling NotificationInteraction: {e.Message}. Path: {Path}");
            }
        else if (TryGetComponent(out Button button))
            try
            {
                button.onClick.Invoke();
                Debug($"[QUICK ACCESS] Triggered via UI.Button Invoke. Path: {Path}");
            }
            catch (Exception e)
            {
                Debug($"[QUICK ACCESS] Error calling UI.Button: {e.Message}. Path: {Path}");
            }
        else
            Debug($"[QUICK ACCESS] FAILED: No valid interaction component found. Path: {Path}");

        yield return new WaitForSeconds(InteractionDelay);
    }
}