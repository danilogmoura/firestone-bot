using Firebot.Core;
using Firebot.GameModel.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Firebot.GameModel.Primitives;

public class GameButton : GameElement
{
    public GameButton(string path, string contextName = null, GameElement parent = null) :
        base(path, contextName, parent) { }

    public GameButton(Transform root) : base("", root.name)
    {
        _cachedTransform = root;
    }

    public void Click()
    {
        if (!IsVisible())
        {
            BotLog.Warning($"Tentativa de clique em botão invisível/inexistente: {ContextName}");
            return;
        }

        var btn = GetComponent<Button>();
        if (btn != null && btn.interactable)
            btn.onClick.Invoke();
        else
            BotLog.Warning($"Botão encontrado mas sem componente Button: {ContextName}");
    }
}