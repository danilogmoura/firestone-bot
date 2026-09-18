using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logger = Firebot.Core.Logger;

namespace Firebot.BotActions;

public class Hotkey
{
    /// <summary>
    ///     How long a click waits for its button to become clickable. Waiting is normal — the leader panel is
    ///     hidden while a task holds a menu open — so the wait only has to end eventually; without a deadline,
    ///     a button that never becomes clickable holds the combo forever, in silence.
    /// </summary>
    private const float ButtonWaitSeconds = 15f;

    private GameObject _cachedGameObject;
    private bool _warnedUnclickable;

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

    /// <summary>
    ///     Not clickable yet: the panel is hidden, or the game disabled the button. Destroyed counts as well —
    ///     the cached object can go away with a scene change while this is waiting.
    /// </summary>
    private static bool IsUnavailable(Button btn) => btn == null || !btn.isActiveAndEnabled || !btn.interactable;

    public IEnumerator Click()
    {
        var target = CachedGameObject;

        if (target != null)
        {
            var btn = target.GetComponent<Button>();

            if (btn != null)
            {
                // Real time on purpose, and one frame per check: with the game clock stopped a scaled wait never
                // wakes up to notice the deadline, which is precisely the hang this bounds. A frame wait also
                // needs no type this mod has not already proven in game, and the frames keep coming when only
                // timeScale is zero — the status screen relies on the same fact.
                var deadline = Time.unscaledTime + ButtonWaitSeconds;
                while (IsUnavailable(btn) && Time.unscaledTime < deadline)
                    yield return null;

                // Skipped rather than fired: Button.Press only checks IsActive(), so invoking onClick on a button
                // the game disabled goes through anyway. Losing one combo step is free — the loop replays the
                // sequence on the next cycle — while firing a click the game refused is not.
                if (IsUnavailable(btn))
                {
                    // Once per episode: the sequence replays every cycle, so a static panel would otherwise write
                    // this line for as long as it stays hidden.
                    if (!_warnedUnclickable)
                    {
                        _warnedUnclickable = true;
                        Logger.Warning($"[Hotkey] Skipped: the button was still not clickable after " +
                                       $"{ButtonWaitSeconds:0}s. Path: {Path}");
                    }

                    yield break;
                }

                _warnedUnclickable = false;
            }

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