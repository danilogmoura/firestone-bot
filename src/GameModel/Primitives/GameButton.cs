using System.Collections;
using Firebot.Core;
using Firebot.GameModel.Base;
using UnityEngine;
using UnityEngine.UI;
using Logger = Firebot.Core.Logger;

namespace Firebot.GameModel.Primitives;

public class GameButton : GameElement
{
    public GameButton(string path, GameElement parent = null) :
        base(path, parent) { }

    public GameButton(Transform root, string path = null) : base(root, path) { }

    public override bool IsVisible()
    {
        if (!base.IsVisible()) return false;
        return !TryGetComponent(out CanvasGroup group) || group.alpha != 0;
    }

    public bool IsClickable() => IsVisible() && TryGetComponent(out Button button) &&
                                 button.enabled && button.interactable;

    public IEnumerator Click()
    {
        if (!IsVisible())
        {
            Logger.Debug($"Click attempt failed: button not visible (path: '{Path ?? "N/A"}').");
            yield break;
        }

        if (TryGetComponent(out Button button))
        {
            if (button.interactable && button.enabled)
            {
                button.onClick.Invoke();
                yield return new WaitForSeconds(BotSettings.InteractionDelay);
            }

            else
                Logger.Debug($"Button found but not interactable or disabled (path: '{Path ?? "N/A"}').");
        }
        else
            Logger.Debug($"Object found at path '{Path ?? "N/A"}' but it does not have a Button component.");
    }
}