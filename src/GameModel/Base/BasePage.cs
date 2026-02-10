using UnityEngine;

namespace Firebot.GameModel.Base;

public class BasePage : GameElement
{
    public BasePage(string path, GameElement parent = null) : base(path, parent) { }

    public override bool IsVisible()
    {
        if (!base.IsVisible()) return false;
        if (Root.localScale.x <= 0.01f) return false;
        if (Root.TryGetComponent(out CanvasGroup group)) return group.alpha > 0;
        return true;
    }
}