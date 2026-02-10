using Firebot.GameModel.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Firebot.GameModel.Primitives;

public class GameButton : GameElement
{
    public GameButton(string path, string contextName, GameElement parent = null) :
        base(path, contextName, parent) { }

    public GameButton(Transform root, string contextName, string path = null) : base(root, contextName, path) { }

    public override bool IsVisible()
    {
        if (!base.IsVisible()) return false;
        return !TryGetComponent(out CanvasGroup group) || group.alpha != 0;
    }

    public bool IsClickable() => IsVisible() && TryGetComponent(out Button button) &&
                                 button.enabled && button.interactable;

    public void Click()
    {
        if (!IsVisible())
        {
            LogWarning("Tentativa de clique falhou: Botão invisível.");
            return;
        }

        if (TryGetComponent(out Button button))
        {
            if (button.interactable && button.enabled)
                button.onClick.Invoke();
            else
                LogWarning("Botão encontrado mas não interável/habilitado.");
        }
        else
            LogError("Objeto encontrado, mas não possui componente Button.");
    }
}