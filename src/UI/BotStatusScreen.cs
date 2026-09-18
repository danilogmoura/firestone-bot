using System;
using System.Collections.Generic;
using Firebot.Core;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;
using Logger = Firebot.Core.Logger;

namespace Firebot.UI;

/// <summary>
///     Task status screen: the same table the console prints, but readable without leaving the game.
///     <para>
///         Unlike the configuration panel, nothing here is editable: these are text rows. The table is
///         built once per scene and afterwards only the <c>.text</c> of the cells changes, once per
///         second, and only when the value actually changed.
///     </para>
/// </summary>
public static class BotStatusScreen
{
    private const string RootName = "firebotStatus";
    private const string Title = "Task Status";

    /// <summary>Comes from the config, so the user can remap it like any other key.</summary>
    private static KeyCode ToggleKey => BotSettings.StatusKey;

    private const int SortingOrder = 9990;
    private const float RefreshInterval = 1f;

    private static readonly string[] ColumnTitles = { "Task", "Status", "Time Left", "Next Run", "Last Run" };

    // Proportions, not pixels. Task is sized on the longest name ("Warfront Campaign Loot"), Status on the
    // longest value ("Locked (200)") plus the gap that keeps it from reading as glued to Time Left, Time Left
    // on the widest duration it prints ("12h 11m 19s") and Next Run on "dd/MM HH:mm". Last Run prints the
    // same format as Next Run and takes what is left.
    private static readonly float[] ColumnRatios = { 0.325f, 0.158f, 0.152f, 0.197f, 0.168f };

    /// <summary>Buffer reused between refreshes: once per second with no new list allocated.</summary>
    private static readonly List<TaskStatusRow> Buffer = new();

    private static readonly List<TMP_Text[]> Cells = new();

    private static UiWindow _window;
    private static RectTransform _rows;
    private static bool _isOpen;
    private static int _builtForHeight;
    private static float _lastRefresh;

    public static bool IsOpen => _isOpen;

    /// <summary>Toggle key. Exposed so Main can explain when it is being ignored.</summary>
    public static KeyCode Key => ToggleKey;

    // ---- Lifecycle ----

    public static void Initialize()
    {
        Logger.Info($"[UI] Status screen ready. Press {ToggleKey} to toggle it.");
    }

    /// <summary>The scene destroys the objects; here we only drop the references. Rebuilds on the next open.</summary>
    public static void OnSceneChanged()
    {
        _window = null;
        _rows = null;
        _isOpen = false;
        _builtForHeight = 0;
        Cells.Clear();
    }

    /// <summary>
    ///     The capture state comes from the gate, which Main freezes once per frame precisely so that it can
    ///     still be trusted here. While a keybind is recording, the key belongs to the capture, not to this
    ///     screen — otherwise recording a hotkey onto F2 would also open this window.
    /// </summary>
    public static void Tick()
    {
        if (!HotkeyGate.IsCapturing && Input.GetKeyDown(ToggleKey)) Toggle();

        if (!_isOpen) return;

        // The scale is derived from screen height: the CanvasScaler rescales on its own when the
        // resolution changes.
        if (_window != null && _window.IsAlive && _builtForHeight != Screen.height) ApplyScale();

        if (Time.unscaledTime - _lastRefresh < RefreshInterval) return;

        _lastRefresh = Time.unscaledTime;
        Refresh();
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
            Logger.Error($"[UI] Failed to build the status screen: {e.GetType().Name} - {e.Message}");
            _window = null;
            _rows = null;
            Cells.Clear();
            return;
        }

        if (_window == null || !_window.IsAlive) return;

        _window.SetActive(true);
        _isOpen = true;
        _lastRefresh = 0f;
        Refresh();
    }

    public static void Close()
    {
        _window?.SetActive(false);
        _isOpen = false;
    }

    // ---- Construction ----

    private static void EnsureBuilt()
    {
        if (_window != null && _window.IsAlive) return;

        var snapshot = BotManager.GetStatusRows(DateTime.Now);

        // The window follows the number of tasks, with a ceiling. Above the ceiling the content scrolls.
        // At least one row is reserved so the empty-list message has somewhere to sit: with the real count
        // the window would be shorter than the message it has to show.
        var body = UiTheme.StatusColumnHeaderHeight + UiTheme.RowSpacing
                   + Mathf.Max(snapshot.Count, 1) * (UiTheme.StatusRowHeight + UiTheme.RowSpacing);
        var height = Mathf.Min(UiTheme.HeaderHeight + UiTheme.ContentTopGap + body + 2f * UiTheme.Padding,
            UiTheme.StatusMaxHeight);

        // The title names the key that closes it, so the window does not have to be discovered.
        var title = $"{Title}";

        _window = UiWindow.Create(RootName, title, UiTheme.StatusWindowWidth, height, SortingOrder, Close);
        _builtForHeight = Screen.height;

        UiBadges.Build(_window.Header);
        BuildTable(_window.Content, snapshot.Count);
        Refresh();
    }

    private static void BuildTable(RectTransform content, int rowCount)
    {
        var scroll = content.gameObject.AddComponent<ScrollRect>();
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.scrollSensitivity = 40f;

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

        var columnHeader = UiFactory.CreateImage("columns", _rows, UiTheme.SectionHeader, false);
        UiFactory.PlaceTop(columnHeader.rectTransform, 0f, UiTheme.StatusColumnHeaderHeight);

        var titles = CreateCells(columnHeader.rectTransform);
        for (var i = 0; i < titles.Length; i++)
        {
            titles[i].text = ColumnTitles[i];
            titles[i].color = UiTheme.Accent;
        }

        Cells.Clear();

        var y = UiTheme.StatusColumnHeaderHeight + UiTheme.RowSpacing;
        for (var i = 0; i < rowCount; i++)
        {
            var row = UiFactory.CreateImage($"row{i}", _rows, UiTheme.Row, false);
            UiFactory.PlaceTop(row.rectTransform, y, UiTheme.StatusRowHeight);

            Cells.Add(CreateCells(row.rectTransform));
            y += UiTheme.StatusRowHeight + UiTheme.RowSpacing;
        }

        // With no task loaded — the screen can be opened before the bot is started — the table would be a
        // column header over nothing, which reads as a build failure rather than as an empty list.
        if (rowCount == 0)
        {
            var empty = UiFactory.CreateLabel(_rows, "No tasks loaded.", UiTheme.HintFontSize,
                UiTheme.TextMuted, TextAlignmentOptions.Center);
            UiFactory.PlaceTop(empty.rectTransform, y, UiTheme.StatusRowHeight);

            _rows.sizeDelta = new Vector2(0f, y + UiTheme.StatusRowHeight);
            return;
        }

        _rows.sizeDelta = new Vector2(0f, Mathf.Max(0f, y - UiTheme.RowSpacing));
    }

    /// <summary>Splits the horizontal band into columns, by anchor — no LayoutGroup.</summary>
    private static TMP_Text[] CreateCells(RectTransform row)
    {
        var cells = new TMP_Text[ColumnRatios.Length];
        var left = 0f;

        for (var i = 0; i < ColumnRatios.Length; i++)
        {
            var right = left + ColumnRatios[i];
            var last = i == ColumnRatios.Length - 1;

            var label = UiFactory.CreateLabel(row, string.Empty, UiTheme.HintFontSize, UiTheme.Text,
                TextAlignmentOptions.Left);

            // A narrow column has to cut with ellipsis, not spill into its neighbour.
            label.overflowMode = TextOverflowModes.Ellipsis;

            var rect = label.rectTransform;
            rect.anchorMin = new Vector2(left, 0f);
            rect.anchorMax = new Vector2(right, 1f);
            rect.offsetMin = new Vector2(i == 0 ? UiTheme.RowInsetLeft : 0f, 0f);
            rect.offsetMax = new Vector2(last ? -UiTheme.RowInsetRight : 0f, 0f);

            cells[i] = label;
            left = right;
        }

        return cells;
    }

    // ---- Refresh ----

    private static void Refresh()
    {
        Buffer.Clear();
        BotManager.AppendStatusRows(Buffer, DateTime.Now);

        UiBadges.Refresh();

        for (var i = 0; i < Cells.Count; i++)
        {
            var cells = Cells[i];

            if (i >= Buffer.Count)
            {
                for (var c = 0; c < cells.Length; c++) SetText(cells[c], string.Empty, UiTheme.TextMuted);
                continue;
            }

            var row = Buffer[i];
            SetText(cells[0], row.Name, UiTheme.Text);
            SetText(cells[1], row.Status, StatusColor(row.Status));
            SetText(cells[2], row.TimeLeft, UiTheme.Text);
            SetText(cells[3], FormatCompact(row.NextRun), UiTheme.Text);
            SetText(cells[4], FormatCompact(row.LastRun), UiTheme.TextMuted);
        }
    }

    /// <summary>Only writes when the text changes: assigning the same value would make TMP rebuild the mesh for nothing.</summary>
    private static void SetText(TMP_Text label, string value, Color color)
    {
        if (label.color != color) label.color = color;
        if (label.text != value) label.text = value;
    }

    /// <summary>Today shows only the time; another day shows the date.</summary>
    private static string FormatCompact(DateTime? value)
    {
        if (value == null) return TaskStatusRow.NoValue;

        var moment = value.Value;
        return moment.Date == DateTime.Today ? moment.ToString("HH:mm:ss") : moment.ToString("dd/MM HH:mm");
    }

    /// <summary>What is actionable stands out; what is idle stays dim.</summary>
    private static Color StatusColor(string status)
        => status == TaskStatusRow.Ready || status == TaskStatusRow.Popup ? UiTheme.Accent : UiTheme.TextMuted;
}
