using System.Collections.Generic;
using Firebot.BotActions;
using Firebot.Core;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Firebot.UI;

/// <summary>
///     State indicators of a window header: one dot and one name per automation.
///     <para>
///         They live here instead of inside one screen because both windows show them, and each window keeps
///         its own objects — a single shared array would leave whichever window was built first frozen.
///     </para>
///     <para>
///         The name carries the state along with the dot. Colour on its own would put the information out of
///         reach of anyone who cannot separate the two shades.
///     </para>
/// </summary>
internal static class UiBadges
{
    private static readonly string[] Names = { "Bot", "AutoSkill", "AutoUpgrade" };

    /// <summary>One entry per built window, in the order the windows were created.</summary>
    private static readonly List<Entry> Built = new();

    private sealed class Entry
    {
        public Image[] Dots;
        public TMP_Text[] Labels;
    }

    /// <summary>
    ///     Hangs the indicators on the right of the header, before the close button. Equal-width columns keep
    ///     the spacing predictable without a LayoutGroup.
    /// </summary>
    public static void Build(RectTransform header)
    {
        var area = UiFactory.CreateNode("badges", header);
        area.anchorMin = new Vector2(1f, 0f);
        area.anchorMax = new Vector2(1f, 1f);
        area.pivot = new Vector2(1f, 0.5f);
        area.sizeDelta = new Vector2(UiTheme.HeaderBadgeWidth, 0f);
        area.anchoredPosition = new Vector2(-(UiTheme.CloseButtonSize + 2f * UiTheme.CloseButtonMargin), 0f);

        var entry = new Entry
        {
            Dots = new Image[Names.Length],
            Labels = new TMP_Text[Names.Length]
        };

        var step = 1f / Names.Length;
        var textLeft = UiTheme.BadgeDotInset + UiTheme.BadgeDotSize + UiTheme.BadgeDotGap;

        for (var i = 0; i < Names.Length; i++)
        {
            var column = UiFactory.CreateNode($"badge{i}", area);
            column.anchorMin = new Vector2(i * step, 0f);
            column.anchorMax = new Vector2((i + 1) * step, 1f);
            column.offsetMin = Vector2.zero;
            column.offsetMax = Vector2.zero;

            var dot = UiFactory.CreateImage("dot", column, UiTheme.Danger, false, UiTheme.CircleSprite);
            var dotRect = dot.rectTransform;
            dotRect.anchorMin = new Vector2(0f, 0.5f);
            dotRect.anchorMax = new Vector2(0f, 0.5f);
            dotRect.pivot = new Vector2(0f, 0.5f);
            dotRect.sizeDelta = new Vector2(UiTheme.BadgeDotSize, UiTheme.BadgeDotSize);
            dotRect.anchoredPosition = new Vector2(UiTheme.BadgeDotInset, 0f);

            var label = UiFactory.CreateLabel(column, Names[i], UiTheme.BadgeFontSize, UiTheme.Text,
                TextAlignmentOptions.Left);
            var labelRect = label.rectTransform;
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(textLeft, 0f);
            labelRect.offsetMax = new Vector2(-UiTheme.BadgeDotGap, 0f);

            entry.Dots[i] = dot;
            entry.Labels[i] = label;
        }

        Built.Add(entry);
    }

    /// <summary>
    ///     Repaints every built set. Sets whose window the scene already destroyed are dropped here: Unity's
    ///     fake-null is what tells "destroyed" apart from "never existed".
    /// </summary>
    public static void Refresh()
    {
        var states = new[] { BotManager.IsRunning, AutoSkill.IsActive, AutoUpgrade.IsActive };

        for (var e = Built.Count - 1; e >= 0; e--)
        {
            var entry = Built[e];

            if (entry.Dots[0] == null)
            {
                Built.RemoveAt(e);
                continue;
            }

            for (var i = 0; i < states.Length; i++) Set(entry, i, states[i]);
        }
    }

    /// <summary>Writes only what changed: assigning the same value makes TMP rebuild the mesh for nothing.</summary>
    private static void Set(Entry entry, int index, bool on)
    {
        var dot = entry.Dots[index];
        var label = entry.Labels[index];
        if (dot == null || label == null) return;

        var color = on ? UiTheme.StateOn : UiTheme.Danger;
        if (dot.color != color) dot.color = color;

        var text = on ? $"{Names[index]} on" : $"{Names[index]} off";
        if (label.text != text) label.text = text;
    }
}
