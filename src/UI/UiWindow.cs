using System;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Firebot.UI;

/// <summary>
///     Screen chrome shared by both windows: isolated canvas, input blocker, centred window and a
///     header with title and close button.
///     <para>
///         It exists so the screens do not drift apart. Without it, every new screen would repeat
///         the same set of anchors — and, over time, with slightly different numbers.
///     </para>
/// </summary>
internal sealed class UiWindow
{
    private UiWindow(GameObject root, Transform window, RectTransform header, RectTransform content)
    {
        Root = root;
        Window = window;
        Header = header;
        Content = content;
    }

    /// <summary>Root object, with the Canvas. Doubles as a lifetime handle: the scene destroys it.</summary>
    public GameObject Root { get; }

    /// <summary>Window transform, for hanging content that is not text rows.</summary>
    public Transform Window { get; }

    /// <summary>Top band. Exposed so a screen can hang indicators next to the title.</summary>
    public RectTransform Header { get; }

    /// <summary>Usable area: below the header, with the side paddings already applied.</summary>
    public RectTransform Content { get; }

    /// <summary>
    ///     False once a scene change destroyed the objects. Comparing the GameObject with null works
    ///     because Object.op_Equality can tell "destroyed" apart from "never existed".
    /// </summary>
    public bool IsAlive => Root != null;

    public void SetActive(bool value)
    {
        if (Root != null) Root.SetActive(value);
    }

    public static UiWindow Create(string name, string title, float width, float height, int sortingOrder,
        Action onClose, float contentBottomInset = 0f)
    {
        var root = UiFactory.CreateCanvas(name, sortingOrder);

        // The blocker is what keeps a click from reaching the game while the window is open.
        var blocker = UiFactory.CreateImage("blocker", root.transform, UiTheme.Blocker, true);
        UiFactory.Stretch(blocker.rectTransform);

        var window = UiFactory.CreateImage("window", root.transform, UiTheme.Window, true);
        UiFactory.Center(window.rectTransform, width, height, Vector2.zero);

        var header = BuildHeader(window.transform, title, onClose);

        var content = UiFactory.CreateNode("content", window.transform);
        content.anchorMin = Vector2.zero;
        content.anchorMax = Vector2.one;
        content.offsetMin = new Vector2(UiTheme.Padding, UiTheme.Padding + contentBottomInset);
        content.offsetMax = new Vector2(-UiTheme.Padding, -(UiTheme.HeaderHeight + UiTheme.ContentTopGap));

        return new UiWindow(root, window.transform, header, content);
    }

    private static RectTransform BuildHeader(Transform window, string title, Action onClose)
    {
        var header = UiFactory.CreateImage("header", window, UiTheme.Header, true);
        UiFactory.PlaceTop(header.rectTransform, 0f, UiTheme.HeaderHeight);

        var label = UiFactory.CreateLabel(header.transform, title, UiTheme.TitleFontSize, UiTheme.Text,
            TextAlignmentOptions.Left);
        var labelRect = label.rectTransform;
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(UiTheme.HeaderInsetX, 0f);
        labelRect.offsetMax = new Vector2(
            -(UiTheme.CloseButtonSize + 2f * UiTheme.CloseButtonMargin), 0f);

        // The vertical position is derived, not copied from the margin: with the old size (52) the
        // arithmetic happened to give exactly 12, and the button only looked centred by coincidence.
        // Deriving it keeps the button centred at any size.
        var closeVertical = -(UiTheme.HeaderHeight - UiTheme.CloseButtonSize) / 2f;

        var close = UiFactory.CreateButton("closeButton", header.transform, "X", UiTheme.Danger, onClose,
            UiTheme.CloseButtonFontSize);
        UiFactory.AnchorTopRight(close.GetComponent<RectTransform>(), UiTheme.CloseButtonSize,
            UiTheme.CloseButtonSize, new Vector2(-UiTheme.CloseButtonMargin, closeVertical));

        return header.rectTransform;
    }
}
