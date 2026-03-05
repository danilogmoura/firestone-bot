using System;
using System.Globalization;
using Firebot.GameModel.Base;
using Firebot.Utilities;
using Il2CppTMPro;
using UnityEngine;

namespace Firebot.GameModel.Primitives;

public class GameText : GameElement
{
    public GameText(string path = null, GameElement parent = null, Transform transform = null)
        : base(path, parent, transform) { }

    public DateTime Time => TimeParser.ParseExpectedTime(GetParsedText());

    public DateTime TimeMultiplier(double multiplier = 1) =>
        TimeParser.ParseExpectedTime(GetParsedText(), multiplier: multiplier);

    public int GetParsedInt(int fallback = 0)
    {
        var parsedText = GetParsedText();
        return int.TryParse(parsedText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
            ? value
            : fallback;
    }

    public double GetParsedDouble(double fallback = 0)
    {
        var parsedText = GetParsedText();

        if (double.TryParse(parsedText, NumberStyles.Float, CultureInfo.InvariantCulture, out var invariantValue))
            return invariantValue;

        return double.TryParse(parsedText, NumberStyles.Float, CultureInfo.CurrentCulture, out var currentCultureValue)
            ? currentCultureValue
            : fallback;
    }

    public string GetParsedText()
    {
        if (!IsVisible()) return string.Empty;

        if (!TryGetComponent(out TMP_Text tmp)) return string.Empty;
        try
        {
            return tmp.text;
        }
        catch (Exception e)
        {
            Debug($"[FAILED] Exception while reading text: {e.Message}. Path: {Path}");
            return string.Empty;
        }
    }

    public void SetColor(Color newColor)
    {
        if (TryGetComponent(out TMP_Text tmp))
            tmp.color = newColor;
    }

    public void SetOutline(Color color, float thickness = 0.2f)
    {
        if (!TryGetComponent(out TMP_Text tmp) || tmp.fontSharedMaterial == null)
        {
            Debug($"[FAILED] FAILED to set outline: TMP_Text or Material missing. Path: {Path}");
            return;
        }

        tmp.fontSharedMaterial.EnableKeyword("OUTLINE_ON");
        tmp.outlineColor = color;
        tmp.outlineWidth = thickness;
        tmp.UpdateMeshPadding();
        tmp.SetAllDirty();
    }

    public void RemoveOutline()
    {
        if (TryGetComponent(out TMP_Text tmp) && tmp.fontSharedMaterial != null)
        {
            tmp.outlineWidth = 0f;
            tmp.fontSharedMaterial.DisableKeyword("OUTLINE_ON");
            tmp.UpdateMeshPadding();
            tmp.SetAllDirty();
        }
    }
}