using System;
using Firebot.Core;
using Firebot.UI.Config;
using Firebot.UI.Widgets;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logger = Firebot.Core.Logger;

namespace Firebot.UI;

/// <summary>
///     Host of the configuration panel.
///     Builds the hierarchy once per scene, opens and closes via SetActive, and registers no Update
///     of its own: the tick comes from Main.OnUpdate, which already reads input every frame.
/// </summary>
public static class BotPanel
{
    private const string RootName = "firebotPanel";
    private const int SortingOrder = 9999;
    private const string DetailHint = "Hover or click an option to see its description.";

    /// <summary>Title of the description box while nothing is being described.</summary>
    private const string DetailTitle = "Description";

    private static UiWindow _window;
    private static RectTransform _rows;
    private static TMP_Text _detailTitle;
    private static TMP_Text _detailBody;
    private static bool _isOpen;
    private static int _builtForHeight;

    public static bool IsOpen => _isOpen;

    // ---- Lifecycle ----

    public static void Initialize()
    {
        Logger.Info($"[UI] Panel ready. Press {BotSettings.PanelKey} to toggle it.");
    }

    /// <summary>
    ///     Called on every scene change. The panel objects are not DontDestroyOnLoad, so the scene has
    ///     already destroyed them by the time this runs — here we only drop the references.
    ///     The panel is rebuilt lazily on the next open.
    /// </summary>
    public static void OnSceneChanged()
    {
        _window = null;
        _rows = null;
        _detailTitle = null;
        _detailBody = null;
        _isOpen = false;
        _builtForHeight = 0;

        // The keybind capture is static and outlives the destroyed rows, so it has to be dropped here
        // rather than waiting for the next build.
        KeybindRow.Cancel();
    }

    public static void Tick()
    {
        KeybindRow.Tick();

        // The gate, not the live state: the capture was consumed by the line above, so asking the row now
        // would always answer "nothing is being recorded" and the recorded key would also open or close
        // the panel.
        if (!HotkeyGate.IsCapturing && Input.GetKeyDown(BotSettings.PanelKey)) Toggle();

        ConfigRegistry.FlushPendingSaves();

        // The panel scale is derived from screen height. If the user changes resolution, the
        // CanvasScaler rescales the tree on its own — nothing needs rebuilding.
        if (_window != null && _window.IsAlive && _builtForHeight != Screen.height) ApplyScale();

        if (!_isOpen) return;

        UiContentBuilder.Poll();
        UiBadges.Refresh();
    }

    private static void ApplyScale()
    {
        _builtForHeight = Screen.height;
        UiFactory.ApplyResponsiveScale(_window.Root.GetComponent<CanvasScaler>());
    }

    public static void Toggle()
    {
        if (_isOpen) Close();
        else Open();
    }

    public static void Open()
    {
        try
        {
            EnsureBuilt();
        }
        catch (Exception e)
        {
            Logger.Error($"[UI] Failed to build the panel: {e.GetType().Name} - {e.Message}");
            _window = null;
            _rows = null;
            return;
        }

        if (_window == null || !_window.IsAlive) return;

        _window.SetActive(true);
        _isOpen = true;

        ResetDetail();

        // Without an EventSystem the GraphicRaycaster runs but nobody feeds it input: clicks never
        // arrive. We never create an EventSystem — the game's is the one in charge.
        if (EventSystem.current == null)
            Logger.Warning("[UI] No EventSystem found in the scene: panel buttons will not receive clicks.");
    }

    /// <summary>
    ///     Puts the description box back to its hint. While the panel is open the last description stays on
    ///     screen on purpose — a long text has to be readable — so opening and closing the panel is what
    ///     returns the box to the state that tells a new user the box is there at all.
    /// </summary>
    private static void ResetDetail()
    {
        SetDetail(DetailTitle, DetailHint);
    }

    public static void Close()
    {
        // Closing only hides the window: the rows, and with them the capture, outlive it. A recording has
        // to end here, otherwise it stays armed behind an invisible panel, showing nothing, and — now that
        // every hotkey yields to the capture — it would block the panel's own hotkey, leaving no way to
        // open it again.
        KeybindRow.Cancel();

        _window?.SetActive(false);
        _isOpen = false;

        ResetDetail();
    }

    // ---- Construction ----

    private static void EnsureBuilt()
    {
        if (_window != null && _window.IsAlive) return;

        // The chrome (canvas, blocker, window and header) is the same as the status screen's — it
        // lives in UiWindow.
        _window = UiWindow.Create(RootName, "Firebot", UiTheme.WindowWidth, UiTheme.WindowHeight, SortingOrder,
            Close, UiTheme.DetailHeight + UiTheme.DetailGap);
        _builtForHeight = Screen.height;

        UiBadges.Build(_window.Header);
        BuildDetailBox(_window.Window);

        var rows = BuildScrollView(_window.Window);
        BuildContent(rows, _window.Root.GetComponent<GraphicRaycaster>());
    }

    /// <summary>Fixed box at the bottom of the window: this is where each option's description appears.</summary>
    private static void BuildDetailBox(Transform window)
    {
        // Frame: a light rectangle with the dark background inside it, offset by 1px. Avoids
        // depending on a bordered sprite (9-slice) just to draw a single line.
        var frame = UiFactory.CreateImage("detailFrame", window, UiTheme.DetailEdge, false);
        var frameRect = frame.rectTransform;
        frameRect.anchorMin = new Vector2(0f, 0f);
        frameRect.anchorMax = new Vector2(1f, 0f);
        frameRect.offsetMin = new Vector2(UiTheme.Padding, UiTheme.Padding);
        frameRect.offsetMax = new Vector2(-UiTheme.Padding, UiTheme.Padding + UiTheme.DetailHeight);

        var box = UiFactory.CreateImage("detail", frame.transform, UiTheme.Detail, false);
        UiFactory.Stretch(box.rectTransform, UiTheme.DetailBorderWidth);

        _detailTitle = UiFactory.CreateLabel(box.transform, DetailTitle, UiTheme.DetailTitleFontSize,
            UiTheme.Accent, TextAlignmentOptions.TopLeft);
        var titleRect = _detailTitle.rectTransform;
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.offsetMin = new Vector2(UiTheme.DetailInsetX,
            -(UiTheme.DetailInsetTop + UiTheme.DetailTitleHeight));
        titleRect.offsetMax = new Vector2(-UiTheme.DetailInsetX, -UiTheme.DetailInsetTop);

        // wrap + Ellipsis: descriptions range from a single line to whole paragraphs.
        _detailBody = UiFactory.CreateLabel(box.transform, DetailHint, UiTheme.DetailFontSize,
            UiTheme.TextMuted, TextAlignmentOptions.TopLeft, true);
        var bodyRect = _detailBody.rectTransform;
        bodyRect.anchorMin = Vector2.zero;
        bodyRect.anchorMax = Vector2.one;
        bodyRect.offsetMin = new Vector2(UiTheme.DetailInsetX, UiTheme.DetailInsetBottom);
        bodyRect.offsetMax = new Vector2(-UiTheme.DetailInsetX, -UiTheme.DetailBodyTop);
    }

    private static void SetDetail(string title, string body)
    {
        if (_detailTitle == null || _detailBody == null) return;

        _detailTitle.text = title;
        _detailBody.text = body;
    }

    private static RectTransform BuildScrollView(Transform window)
    {
        var content = UiFactory.CreateNode("content", window);
        content.anchorMin = Vector2.zero;
        content.anchorMax = Vector2.one;
        content.offsetMin = new Vector2(UiTheme.Padding,
            UiTheme.Padding + UiTheme.DetailHeight + UiTheme.DetailGap);
        content.offsetMax = new Vector2(-UiTheme.Padding, -(UiTheme.HeaderHeight + UiTheme.ContentTopGap));

        var scroll = content.gameObject.AddComponent<ScrollRect>();
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.scrollSensitivity = 30f;

        // RectMask2D instead of Mask + Image: it clips without needing a sprite or a stencil material.
        var viewport = UiFactory.CreateImage("viewport", content, UiTheme.Transparent, true);
        UiFactory.Stretch(viewport.rectTransform);
        viewport.gameObject.AddComponent<RectMask2D>();

        _rows = UiFactory.CreateNode("rows", viewport.rectTransform);
        _rows.anchorMin = new Vector2(0f, 1f);
        _rows.anchorMax = new Vector2(1f, 1f);
        _rows.pivot = new Vector2(0.5f, 1f);
        _rows.sizeDelta = Vector2.zero;
        _rows.anchoredPosition = Vector2.zero;

        scroll.viewport = viewport.rectTransform;
        scroll.content = _rows;
        return _rows;
    }

    /// <summary>
    ///     The real content comes from the preferences registry: collapsible sections, one row per
    ///     entry. The panel knows no task — UiContentBuilder is what assembles it.
    /// </summary>
    private static void BuildContent(RectTransform rows, GraphicRaycaster raycaster)
    {
        UiContentBuilder.Build(rows, raycaster, SetDetail);
    }
}
