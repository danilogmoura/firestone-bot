using System;
using System.Collections.Generic;
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
///     Builds the panel rows from the preferences registry, in collapsible sections.
///     It does not use VerticalLayoutGroup: every block has a fixed height and is positioned by a
///     cursor. Collapsing neither destroys nor recreates anything — it only repositions and
///     deactivates, so the state of the controls (and any values already typed) survives
///     opening and closing a section.
/// </summary>
internal static class UiContentBuilder
{
    private sealed class Block
    {
        public RectTransform Rect;
        public float Height;
        public int Section;
        public bool AlwaysVisible;
    }

    private readonly struct Detail
    {
        public Detail(string title, string body)
        {
            Title = title;
            Body = body;
        }

        public string Title { get; }
        public string Body { get; }
    }

    private static readonly List<Block> Blocks = new();
    private static readonly List<bool> Expanded = new();
    private static readonly List<TMP_Text> Markers = new();

    // Description under the cursor. The key is the row's InstanceID: the Il2CppInterop wrappers have
    // no stable hash by reference, so comparing Transform inside a dictionary would not work.
    private static readonly Dictionary<int, Detail> Details = new();
    private static readonly Il2CppSystem.Collections.Generic.List<RaycastResult> Hits = new();

    private static RectTransform _rows;
    private static GraphicRaycaster _raycaster;
    private static Action<string, string> _describe;
    private static PointerEventData _pointer;
    private static Vector3 _lastPointer;
    private static int _shownRowId = int.MinValue;
    private static bool _raycasterMissingLogged;
    private static float _lastHoverReport;

    public static void Build(RectTransform rows, GraphicRaycaster raycaster, Action<string, string> describe)
    {
        _rows = rows;
        _raycaster = raycaster;
        _describe = describe;
        _shownRowId = int.MinValue;
        _lastPointer = Vector3.zero;

        Blocks.Clear();
        Expanded.Clear();
        Markers.Clear();
        Details.Clear();

        var groups = ConfigRegistry.Read();
        var rowCount = 0;

        for (var g = 0; g < groups.Count; g++)
        {
            var group = groups[g];
            var section = Expanded.Count;

            // The first section in the file is firebot_settings (created before everything else at
            // boot), so it is the one that opens by default.
            Expanded.Add(g == 0);

            AddSectionHeader(group, section);

            for (var i = 0; i < group.Entries.Count; i++)
            {
                AddRow(group.Entries[i], section);
                rowCount++;
            }
        }

        Relayout();
        Logger.Info($"[UI] Panel content: {groups.Count} section(s), {rowCount} setting(s).");
    }

    private static void AddSectionHeader(ConfigGroup group, int section)
    {
        var header = UiFactory.CreateImage($"section_{group.Category.Identifier}", _rows, UiTheme.SectionHeader, true);

        var button = header.gameObject.AddComponent<Button>();
        button.targetGraphic = header;
        button.onClick.AddListener((Action)(() => ToggleSection(section)));

        var title = UiFactory.CreateLabel(header.transform, group.Title, UiTheme.SectionFontSize, UiTheme.Accent,
            TextAlignmentOptions.Left);
        var titleRect = title.rectTransform;
        titleRect.anchorMin = Vector2.zero;
        titleRect.anchorMax = Vector2.one;
        titleRect.offsetMin = new Vector2(UiTheme.SectionInsetLeft, 0f);
        titleRect.offsetMax = new Vector2(-UiTheme.SectionInsetRight, 0f);

        var marker = UiFactory.CreateLabel(header.transform, "-", UiTheme.SectionFontSize, UiTheme.TextMuted,
            TextAlignmentOptions.Right);
        var markerRect = marker.rectTransform;
        markerRect.anchorMin = Vector2.zero;
        markerRect.anchorMax = Vector2.one;
        markerRect.offsetMin = new Vector2(0f, 0f);
        markerRect.offsetMax = new Vector2(-UiTheme.SectionMarkerInset, 0f);

        Markers.Add(marker);

        Blocks.Add(new Block
        {
            Rect = header.rectTransform,
            Height = UiTheme.SectionHeight,
            Section = section,
            AlwaysVisible = true
        });
    }

    private static void AddRow(ConfigEntry config, int section)
    {
        var background = UiFactory.CreateImage($"row_{config.Entry.Identifier}", _rows, UiTheme.Row, true);
        UiRow.Create(config, background.rectTransform);

        // The whole row is the target. ColorTint replaces the graphic's color, so the tints have to
        // carry the same low alpha as the background — otherwise the row would turn into an opaque
        // white band on hover.
        var button = background.gameObject.AddComponent<Button>();
        button.targetGraphic = background;

        var colors = button.colors;
        colors.normalColor = UiTheme.Row;
        colors.highlightedColor = UiTheme.RowHighlight;
        colors.pressedColor = UiTheme.RowPressed;
        colors.selectedColor = UiTheme.Row;
        colors.disabledColor = UiTheme.Row;
        colors.fadeDuration = 0.06f;
        button.colors = colors;

        var label = config.Label;
        var description = config.Description;
        button.onClick.AddListener((Action)(() => _describe?.Invoke(label, description)));

        Details[background.gameObject.GetInstanceID()] = new Detail(config.Label, config.Description);

        Blocks.Add(new Block
        {
            Rect = background.rectTransform,
            Height = UiTheme.RowHeight,
            Section = section
        });
    }

    private static void ToggleSection(int section)
    {
        Expanded[section] = !Expanded[section];
        Relayout();
    }

    /// <summary>Repositions the visible blocks in sequence and recomputes the content height.</summary>
    private static void Relayout()
    {
        var y = 0f;

        for (var i = 0; i < Blocks.Count; i++)
        {
            var block = Blocks[i];
            var visible = block.AlwaysVisible || Expanded[block.Section];

            var go = block.Rect.gameObject;
            if (go.activeSelf != visible) go.SetActive(visible);

            if (!visible) continue;

            UiFactory.PlaceTop(block.Rect, y, block.Height);
            y += block.Height + UiTheme.RowSpacing;
        }

        for (var s = 0; s < Markers.Count && s < Expanded.Count; s++)
            if (Markers[s] != null)
                Markers[s].text = Expanded[s] ? "-" : "+";

        if (_rows != null) _rows.sizeDelta = new Vector2(0f, Mathf.Max(0f, y - UiTheme.RowSpacing));
    }

    /// <summary>
    ///     Finds which row is under the cursor and publishes its description.
    ///     Uses <em>our</em> canvas raycaster instead of EventSystem.RaycastAll, so it does not sweep
    ///     the whole game UI; and it only runs when the mouse has actually moved.
    ///     The text is not cleared when the cursor leaves: a long description has to stay readable.
    /// </summary>
    public static void Poll()
    {
        if (_rows == null) return;

        if (_raycaster == null)
        {
            if (!_raycasterMissingLogged)
            {
                _raycasterMissingLogged = true;
                Logger.Warning("[UI] No GraphicRaycaster on the panel canvas: hovering will not show descriptions.");
            }

            return;
        }

        var pointer = Input.mousePosition;
        if (pointer.x == _lastPointer.x && pointer.y == _lastPointer.y) return;
        _lastPointer = pointer;

        _pointer ??= new PointerEventData(EventSystem.current);
        _pointer.position = pointer;

        Hits.Clear();
        _raycaster.Raycast(_pointer, Hits);
        ReportHover();

        for (var i = 0; i < Hits.Count; i++)
        {
            var rowTransform = FindRowRoot(Hits[i].gameObject.transform);
            if (rowTransform == null) continue;

            // The key has to be the GameObject: GetInstanceID is per component, and the row's Image
            // has a different id from its Transform.
            var id = rowTransform.gameObject.GetInstanceID();
            if (!Details.TryGetValue(id, out var detail)) return;
            if (id == _shownRowId) return;

            _shownRowId = id;
            _describe?.Invoke(detail.Title, detail.Body);
            return;
        }
    }

    /// <summary>Walks up the hierarchy until it finds the direct child of _rows holding the hit transform.</summary>
    private static Transform FindRowRoot(Transform transform)
    {
        if (_rows == null) return null;

        var rowsId = _rows.GetInstanceID();
        var current = transform;

        while (current != null)
        {
            var parent = current.parent;
            if (parent == null) return null;
            if (parent.GetInstanceID() == rowsId) return current;

            current = parent;
        }

        return null;
    }

    /// <summary>
    ///     Hover report, only with debug_mode on. It exists because a failure here is silent by
    ///     nature: without it there is no way to tell "nothing was hit" from "something that is not
    ///     a row was hit".
    /// </summary>
    private static void ReportHover()
    {
        if (!BotSettings.DebugMode) return;
        if (Time.unscaledTime - _lastHoverReport < 1f) return;
        _lastHoverReport = Time.unscaledTime;

        if (Hits.Count == 0)
        {
            Logger.Debug("[UI] Hover: 0 hits under the pointer.");
            return;
        }

        var first = Hits[0].gameObject;
        var row = FindRowRoot(first.transform);
        var resolved = row != null ? row.gameObject.name : "<not a row>";

        Logger.Debug($"[UI] Hover: {Hits.Count} hit(s), first='{first.name}', row='{resolved}'");
    }
}
