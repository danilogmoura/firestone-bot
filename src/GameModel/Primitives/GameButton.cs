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

    public IEnumerator Click()
    {
        if (IsClickable(out var button))
            button.onClick.Invoke();
        else
        {
            if (Root == null)
                Debug($"[FAILED] Click ignored: Object NOT FOUND. Path: {Path}");
            else if (!Root.gameObject.activeInHierarchy)
                Debug($"[FAILED] Click ignored: Object INACTIVE. Path: {Path}");
            else
                Debug($"[FAILED] Click ignored: Button DISABLED/NON-INTERACTABLE. Path: {Path}");
        }

        yield return new WaitForSeconds(InteractionDelay);
    }
}