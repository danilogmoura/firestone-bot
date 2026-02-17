using System;
using System.Collections;
using Firebot.GameModel.Base;
using UnityEngine;
using UnityEngine.UI;
using static Firebot.Core.BotSettings;

namespace Firebot.GameModel.Primitives;

public class GameButton : GameElement
{
    public GameButton(string path = null, GameElement parent = null, Transform transform = null) :
        base(path, parent, transform) { }

    private bool IsClickable(out Button button)
    {
        button = null;
        if (!IsVisible()) return false;

        if (TryGetComponent(out button)) return button.enabled && button.interactable;
        return false;
    }

    public virtual IEnumerator Click()
    {
        if (IsClickable(out var button))
            try
            {
                button.onClick.Invoke();
            }
            catch (Exception e)
            {
                Debug($"[FAILED] Click threw exception: {e.Message}. Path: {Path}");
            }
        else
        {
            if (button != null)
                Debug($"[FAILED] Click ignored: Button disabled/non-interactable. Path: {Path}");
        }

        yield return new WaitForSeconds(InteractionDelay);
    }
}