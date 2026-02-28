using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logger = Firebot.Core.Logger;

namespace Firebot.BotAttack;

public class Hotkey
{
    private GameObject _cachedGameObject;

    public Hotkey(string path)
    {
        Path = path;
    }

    private string Path { get; }

    private GameObject CachedGameObject
    {
        get
        {
            if (_cachedGameObject != null) return _cachedGameObject;

            var go = GameObject.Find(Path);
            if (go != null)
                _cachedGameObject = go;

            return _cachedGameObject;
        }
    }

    public IEnumerator Click()
    {
        var target = CachedGameObject;

        if (target != null)
        {
            var btn = target.GetComponent<Button>();

            if (btn != null)
                while (!btn.isActiveAndEnabled || !btn.interactable)
                    yield return new WaitForSeconds(0.5f);

            var pointer = new PointerEventData(EventSystem.current)
            {
                button = PointerEventData.InputButton.Left
            };
            ExecuteEvents.Execute(target, pointer, ExecuteEvents.pointerClickHandler);
        }
        else
            Logger.Debug($"GameObject not found at path: {Path}");

        yield return new WaitForSeconds(1f);
    }
}