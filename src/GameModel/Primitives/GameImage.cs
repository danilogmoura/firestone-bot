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
            if (!IsVisible()) return Color.white;
            return TryGetComponent(out Image img) ? img.color : Color.white;
        }
    }

    public float FillAmount
    {
        get
        {
            if (!IsVisible()) return 0f;
            return TryGetComponent(out Image img) ? img.fillAmount : 0f;
        }
    }

    public void SetColor(Color newColor)
    {
        if (!IsVisible()) return;
        if (TryGetComponent(out Image img)) img.color = newColor;
    }
}