using System;
using Firebot.Core;
using Il2CppTMPro;
using UnityEngine;
using Logger = Firebot.Core.Logger;

namespace Firebot.UI;

/// <summary>
///     Colors, metrics and assets shared by the panel.
///     Nothing is written to disk: the sprite is generated in memory and the font is borrowed from
///     the game itself, always lazily, once per session.
/// </summary>
internal static class UiTheme
{
    // ---- Metrics at scale 1, against a 1920x1080 reference ----
    // The CanvasScaler scales these dynamically by screen height (see UiFactory.ApplyResponsiveScale),
    // so these numbers are relative units, never screen pixels.
    //
    // WindowHeight sits exactly at the height ceiling (90% of 1080 = 972). With match = 1 the height
    // fraction is WindowHeight/1080 at ANY resolution — the ceiling does not move if the minimum
    // supported resolution goes up. Growing past this requires raising UiFactory.MaxPanelHeight or
    // moving the layout to two columns.
    public const float WindowWidth = 833f;
    public const float WindowHeight = 972f;
    public const float HeaderHeight = 76f;
    public const float HeaderInsetX = 21f;

    // Distance from the button to the right edge of the header. Horizontal only: the vertical
    // position is derived from HeaderHeight in UiWindow.BuildHeader, so the button stays centred if
    // CloseButtonSize changes.
    public const float CloseButtonSize = 36f;
    public const float CloseButtonMargin = 12f;

    /// <summary>Font size of the "X". Its own because the X must not inherit content button sizing.</summary>
    public const float CloseButtonFontSize = 17f;

    /// <summary>Area right of the title used by the state indicators, before the close button.</summary>
    public const float HeaderBadgeWidth = 600f;

    /// <summary>
    ///     Metrics of the indicators themselves. They exist separately so their font can be reduced
    ///     without touching any other text in the panel.
    /// </summary>
    public const float BadgeFontSize = 15f;
    public const float BadgeDotSize = 16f;
    public const float BadgeDotInset = 24f;
    public const float BadgeDotGap = 10f;
    public const float SectionHeight = 45f;
    public const float DetailHeight = 149f;
    public const float DetailGap = 17f;
    public const float DetailBorderWidth = 1f;
    public const float DetailInsetX = 21f;
    public const float DetailInsetTop = 14f;
    public const float DetailInsetBottom = 14f;
    public const float DetailTitleHeight = 35f;
    public const float DetailTitleToBody = 7f;

    /// <summary>Where the description body starts, measured from the top of the box.</summary>
    public const float DetailBodyTop = DetailInsetTop + DetailTitleHeight + DetailTitleToBody;

    public const float Padding = 14f;
    public const float RowHeight = 52f;
    public const float RowSpacing = 10f;

    public const float TitleFontSize = 33f;
    public const float SectionFontSize = 24f;
    public const float DetailTitleFontSize = 23f;
    public const float DetailFontSize = 21f;
    public const float LabelFontSize = 26f;
    public const float ButtonFontSize = 24f;
    public const float HintFontSize = 21f;

    // ---- Internal widget metrics ----
    // Centralized on purpose: changing the panel size has to be an adjustment in this file, not a
    // hunt for magic numbers scattered across the widgets. That was exactly what got left behind when
    // the metrics above went up.
    public const float RowInsetLeft = 10f;
    public const float RowInsetRight = 7f;
    public const float ControlInsetLeft = 3f;
    public const float ControlInsetRight = 10f;
    public const float ControlInsetY = 5f;
    public const float ControlMarginRight = 10f;
    public const float ToggleWidth = 76f;
    public const float ToggleHeight = 38f;
    public const float CheckInset = 7f;

    // ---- Chip choice (one option or several) ----
    // The row label gives up most of the width because four option names do not fit in the default
    // control column (0.44). A much larger option set would need another row.
    public const float ChipLabelRatio = 0.26f;
    public const float ChipHeight = 34f;
    public const float ChipPaddingX = 16f;
    public const float ChipGap = 8f;
    public const float ChipFontSize = 19f;

    /// <summary>Width per character. Only used when the real font measurement fails.</summary>
    public const float ChipCharRatio = 0.52f;
    public const float SliderHeight = 28f;
    public const float SliderFillInset = 10f;
    public const float SliderHandleWidth = 21f;
    public const float SliderHandleInset = 3f;
    public const float KeybindWidth = 180f;
    public const float InputHeight = 38f;
    public const float InputInsetRight = 21f;
    public const float InputTextInset = 7f;
    public const float SectionInsetLeft = 14f;
    public const float SectionInsetRight = 42f;
    public const float SectionMarkerInset = 14f;

    // ---- Status screen ----
    // Wider than the panel: the 5 table columns do not fit in WindowWidth without cutting off
    // Status, Time Left and Last Run.
    public const float StatusWindowWidth = 925f;
    public const float StatusRowHeight = 44f;
    public const float StatusColumnHeaderHeight = 42f;

    /// <summary>
    ///     Height ceiling of the status screen (66% of 1080). The window grows with the number of tasks
    ///     and stops here; past this the content scrolls. With the current 10 tasks it lands at 696 and
    ///     does not scroll.
    /// </summary>
    public const float StatusMaxHeight = 720f;

    // ---- Paleta ----
    public static readonly Color Blocker = new(0f, 0f, 0f, 0.68f);
    public static readonly Color Window = new(0.098f, 0.106f, 0.125f, 0.99f);
    public static readonly Color Header = new(0.157f, 0.169f, 0.196f, 1f);
    public static readonly Color SectionHeader = new(0.180f, 0.196f, 0.231f, 1f);
    public static readonly Color Detail = new(0.075f, 0.082f, 0.098f, 1f);

    /// <summary>Border of the description box. It is what separates the box from the rest of the window.</summary>
    public static readonly Color DetailEdge = new(1f, 1f, 1f, 0.18f);
    public static readonly Color Row = new(1f, 1f, 1f, 0.08f);
    public static readonly Color RowHighlight = new(1f, 1f, 1f, 0.17f);
    public static readonly Color RowPressed = new(1f, 1f, 1f, 0.26f);
    public static readonly Color Text = new(0.914f, 0.925f, 0.945f, 1f);
    public static readonly Color TextMuted = new(0.596f, 0.627f, 0.675f, 1f);
    public static readonly Color Accent = new(0.980f, 0.718f, 0.180f, 1f);
    public static readonly Color Danger = new(0.898f, 0.298f, 0.278f, 1f);

    /// <summary>Green of the indicators that are on. The off red is Danger.</summary>
    public static readonly Color StateOn = new(0.231f, 0.820f, 0.435f, 1f);
    public static readonly Color Button = new(0.235f, 0.255f, 0.302f, 1f);
    public static readonly Color Transparent = new(0f, 0f, 0f, 0f);

    private static Sprite _whiteSprite;
    private static Sprite _circleSprite;
    private static TMP_FontAsset _font;

    public static float LineHeight => RowHeight + RowSpacing;

    /// <summary>1x1 white sprite generated in memory, used as the base of every box.</summary>
    public static Sprite WhiteSprite
    {
        get
        {
            if (_whiteSprite != null) return _whiteSprite;

            try
            {
                var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                texture.hideFlags = HideFlags.HideAndDontSave;
                texture.SetPixel(0, 0, Color.white);
                texture.Apply();

                _whiteSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 100f);
                _whiteSprite.hideFlags = HideFlags.HideAndDontSave;
            }
            catch (Exception e)
            {
                Logger.Warning($"[UI] Failed to build the 1x1 sprite: {e.GetType().Name} - {e.Message}");
            }

            return _whiteSprite;
        }
    }

    /// <summary>
    ///     White circle generated in memory, for the state indicators.
    ///     <para>
    ///         Drawn instead of using the ● glyph because the game's font atlas may not contain the
    ///         character — TMP bakes only the character set defined in the asset, and a static asset
    ///         gains no glyphs at runtime. A missing glyph becomes an empty box, not a circle.
    ///     </para>
    /// </summary>
    public static Sprite CircleSprite
    {
        get
        {
            if (_circleSprite != null) return _circleSprite;

            try
            {
                _circleSprite = CreateCircleSprite();
            }
            catch (Exception e)
            {
                Logger.Warning($"[UI] Failed to build the circle sprite: {e.GetType().Name} - {e.Message}");
            }

            return _circleSprite;
        }
    }

    private static Sprite CreateCircleSprite()
    {
        const int size = 32;
        const float radius = size / 2f;

        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.hideFlags = HideFlags.HideAndDontSave;

        for (var y = 0; y < size; y++)
            for (var x = 0; x < size; x++)
            {
                var dx = x + 0.5f - radius;
                var dy = y + 0.5f - radius;

                // Alpha proportional to the distance from the centre: the edge comes out smoothed,
                // with no jaggies.
                var alpha = Mathf.Clamp01(radius - Mathf.Sqrt(dx * dx + dy * dy) + 0.5f);
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }

        texture.Apply();

        var sprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 100f);
        sprite.hideFlags = HideFlags.HideAndDontSave;
        return sprite;
    }

    /// <summary>
    ///     The TMP font. Without it a TextMeshProUGUI renders nothing, so we resolve it from the game
    ///     itself: first TMP's default asset, then any text already in the scene.
    ///     The cache is only considered valid once a font is actually found, so that an early failure
    ///     (assets still loading) does not leave the panel mute for the rest of the session.
    /// </summary>
    public static TMP_FontAsset Font
    {
        get
        {
            if (_font != null) return _font;

            _font = ResolveFont();
            return _font;
        }
    }

    private static TMP_FontAsset ResolveFont()
    {
        try
        {
            var settingsFont = TMP_Settings.defaultFontAsset;
            if (settingsFont != null)
            {
                Logger.Debug($"[UI] Font from TMP_Settings: {SafeName(settingsFont)}");
                return settingsFont;
            }
        }
        catch (Exception e)
        {
            Logger.Debug($"[UI] TMP_Settings.defaultFontAsset unavailable: {e.GetType().Name}");
        }

        try
        {
            // FindObjectsOfTypeAll also sees inactive objects, far more useful than FindObjectOfType.
            var candidates = Resources.FindObjectsOfTypeAll<TMP_Text>();
            for (var i = 0; i < candidates.Length; i++)
            {
                var candidate = candidates[i];
                if (candidate == null) continue;

                var font = candidate.font;
                if (font == null) continue;

                Logger.Debug($"[UI] Font borrowed from scene text: {SafeName(candidate)}");
                return font;
            }
        }
        catch (Exception e)
        {
            Logger.Warning($"[UI] Font scan failed: {e.GetType().Name} - {e.Message}");
        }

        Logger.Warning("[UI] No TMP_FontAsset found. The panel will render without text.");
        return null;
    }

    private static string SafeName(UnityEngine.Object obj)
    {
        try
        {
            return obj.name;
        }
        catch
        {
            return "<stale>";
        }
    }
}
