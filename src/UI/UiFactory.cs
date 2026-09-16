using System;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Firebot.UI;

/// <summary>
///     Pure UI construction helpers. No state here: each method creates an object and returns the
///     component ready to use.
///     No LayoutGroup anywhere — all positioning is done by manual anchors, which avoids any
///     LayoutRebuilder cost at runtime.
/// </summary>
internal static class UiFactory
{
    /// <summary>Reference resolution at scale 1, against which the UiTheme metrics were designed.</summary>
    private const float ReferenceWidth = 1920f;
    private const float ReferenceHeight = 1080f;

    /// <summary>Minimum physical label size. Below this the text stops being legible.</summary>
    private const float MinLabelPixels = 12f;

    /// <summary>Panel height ceiling, as a fraction of the screen, so it never becomes the whole screen.</summary>
    private const float MaxPanelHeight = 0.90f;

    /// <summary>Panel width ceiling, for the same reason — it is what constrains 4:3 screens.</summary>
    private const float MaxPanelWidth = 0.92f;

    /// <summary>
    ///     Canvas isolated from the game: ScreenSpaceOverlay with a sortingOrder high enough to sit
    ///     above any HUD. We never create an EventSystem — we use the game's.
    /// </summary>
    public static GameObject CreateCanvas(string name, int sortingOrder)
    {
        var go = new GameObject(name);

        var rect = EnsureRect(go);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortingOrder;

        ApplyResponsiveScale(go.AddComponent<CanvasScaler>());
        go.AddComponent<GraphicRaycaster>();
        return go;
    }

    /// <summary>
    ///     Scales the panel by screen height; the width follows on its own, because everything is sized
    ///     in proportional virtual units.
    ///     <para>
    ///         With matchWidthOrHeight = 1 the CanvasScaler ignores the width and uses only
    ///         <c>s = height / referenceResolution.y</c>. Dividing the reference by k multiplies the
    ///         whole panel by k — without touching a single UiTheme metric.
    ///     </para>
    ///     <para>
    ///         k rises only when the physical font would fall below the legibility minimum, and stops
    ///         before the panel reaches the height or width ceiling. On large screens k = 1, meaning
    ///         nothing changes from the designed size.
    ///     </para>
    /// </summary>
    public static void ApplyResponsiveScale(CanvasScaler scaler)
    {
        if (scaler == null) return;

        var screenWidth = Mathf.Max(1, Screen.width);
        var screenHeight = Mathf.Max(1, Screen.height);

        var forLegibility = MinLabelPixels * ReferenceHeight / (UiTheme.LabelFontSize * screenHeight);

        // Height ceiling: the panel occupies WindowHeight * H * k / ReferenceHeight, so the H cancels
        // out and the limit becomes a constant.
        var heightCeiling = MaxPanelHeight * ReferenceHeight / UiTheme.WindowHeight;

        // Width ceiling: here the H does NOT cancel out. Since the scale follows the height, a short
        // and wide screen (4:3) needs a smaller k than the same height at 16:9.
        var widthCeiling = MaxPanelWidth * screenWidth * ReferenceHeight
                           / (UiTheme.WindowWidth * screenHeight);

        // The Max(1, ...) guarantees the designed window never shrinks, not even on degenerate screens.
        var scale = Mathf.Clamp(forLegibility, 1f, Mathf.Max(1f, Mathf.Min(heightCeiling, widthCeiling)));

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(ReferenceWidth, ReferenceHeight / scale);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

        // 1 = height rules. Width only enters as the k ceiling, above.
        scaler.matchWidthOrHeight = 1f;
    }

    /// <summary>Plain container (no Graphic) for grouping children.</summary>
    public static RectTransform CreateNode(string name, Transform parent)
    {
        var go = new GameObject(name);
        var rect = EnsureRect(go);
        rect.SetParent(parent, false);
        return rect;
    }

    public static Image CreateImage(string name, Transform parent, Color color, bool raycastTarget,
        Sprite sprite = null)
    {
        var go = new GameObject(name);
        var rect = EnsureRect(go);
        rect.SetParent(parent, false);

        var image = go.AddComponent<Image>();
        image.sprite = sprite != null ? sprite : UiTheme.WhiteSprite;
        image.type = Image.Type.Simple;
        image.color = color;
        image.raycastTarget = raycastTarget;
        return image;
    }

    public static TMP_Text CreateLabel(Transform parent, string text, float fontSize, Color color,
        TextAlignmentOptions alignment, bool wrap = false)
    {
        var go = new GameObject("label");
        var rect = EnsureRect(go);
        rect.SetParent(parent, false);

        var label = go.AddComponent<TextMeshProUGUI>();
        var font = UiTheme.Font;
        if (font != null) label.font = font;

        label.text = text;
        label.fontSize = fontSize;
        label.color = color;
        label.alignment = alignment;
        label.enableWordWrapping = wrap;
        label.raycastTarget = false;

        // Text of unpredictable size (descriptions) has to be truncated, otherwise it spills out of
        // the box.
        if (wrap) label.overflowMode = TextOverflowModes.Ellipsis;

        return label;
    }

    public static Button CreateButton(string name, Transform parent, string label, Color background, Action onClick,
        float? fontSize = null)
    {
        var go = new GameObject(name);
        var rect = EnsureRect(go);
        rect.SetParent(parent, false);

        var image = go.AddComponent<Image>();
        image.sprite = UiTheme.WhiteSprite;
        image.type = Image.Type.Simple;
        image.color = background;
        image.raycastTarget = true;

        var button = go.AddComponent<Button>();
        button.targetGraphic = image;

        var colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.25f, 1.25f, 1.25f, 1f);
        colors.pressedColor = new Color(0.72f, 0.72f, 0.72f, 1f);
        colors.selectedColor = Color.white;
        colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.08f;
        button.colors = colors;

        // Action -> UnityAction happens through an implicit conversion generated by Il2CppInterop.
        if (onClick != null) button.onClick.AddListener(onClick);

        if (!string.IsNullOrEmpty(label))
        {
            var text = CreateLabel(image.transform, label, fontSize ?? UiTheme.ButtonFontSize, UiTheme.Text,
                TextAlignmentOptions.Center);
            Stretch(text.rectTransform, 4f);
        }

        return button;
    }

    // ---- Geometry (all anchor-based; no LayoutGroup) ----

    public static void Stretch(RectTransform rect, float padding = 0f)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(padding, padding);
        rect.offsetMax = new Vector2(-padding, -padding);
    }

    /// <summary>Horizontal band pinned to the top of the parent, with a fixed height.</summary>
    public static void PlaceTop(RectTransform rect, float top, float height, float padding = 0f)
    {
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.offsetMin = new Vector2(padding, -top - height);
        rect.offsetMax = new Vector2(-padding, -top);
    }

    public static void Center(RectTransform rect, float width, float height, Vector2 offset)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = offset;
    }

    public static void AnchorTopRight(RectTransform rect, float width, float height, Vector2 offset)
    {
        rect.anchorMin = Vector2.one;
        rect.anchorMax = Vector2.one;
        rect.pivot = Vector2.one;
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = offset;
    }

    /// <summary>
    ///     Il2CppInterop strips [RequireComponent] from the generated assemblies, so the automatic
    ///     RectTransform add cannot be relied on: we ensure it explicitly.
    /// </summary>
    private static RectTransform EnsureRect(GameObject go)
    {
        if (go.TryGetComponent(out RectTransform existing)) return existing;
        return go.AddComponent<RectTransform>();
    }
}
