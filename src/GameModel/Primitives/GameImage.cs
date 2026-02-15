using Firebot.GameModel.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Firebot.GameModel.Primitives;

public class GameImage : GameElement
{
    public GameImage(string path = null, GameElement parent = null, Transform transform = null)
        : base(path, parent, transform) { }

    public Color Color
    {
        get
        {
            if (!IsVisible())
            {
                Debug($"Read Color ignored: Element invisible/inactive. Path: {Path}");
                return Color.white;
            }

            if (TryGetComponent(out Image img))
            {
                Debug($"Color read: {img.color}. Path: {Path}");
                return img.color;
            }

            return Color.white;
        }
    }

    public float FillAmount
    {
        get
        {
            if (!IsVisible())
            {
                Debug($"Read FillAmount ignored: Element invisible/inactive. Path: {Path}");
                return 0f;
            }

            if (TryGetComponent(out Image img))
            {
                Debug($"FillAmount read: {img.fillAmount * 100}%. Path: {Path}");
                return img.fillAmount;
            }

            return 0f;
        }
    }

    public void SetColor(Color newColor)
    {
        if (!IsVisible())
        {
            Debug($"SetColor ignored: Element invisible. Path: {Path}");
            return;
        }

        if (TryGetComponent(out Image img))
        {
            img.color = newColor;
            Debug($"Color changed to {newColor}. Path: {Path}");
        }
    }
}