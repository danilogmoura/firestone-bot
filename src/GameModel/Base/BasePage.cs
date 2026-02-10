using UnityEngine;

namespace Firebot.GameModel.Base;

public class BasePage : GameElement
{
    public BasePage(string path, string contextName, GameElement parent = null) : base(path, contextName, parent) { }

    public override bool IsVisible()
    {
        if (!base.IsVisible()) return false;
        if (Root.localScale.x <= 0.01f) return false;
        if (Root.TryGetComponent(out CanvasGroup group)) return group.alpha > 0;
        return true;
    }
}