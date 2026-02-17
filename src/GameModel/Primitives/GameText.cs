using System;
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