using Firebot.GameModel.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Firebot.GameModel.Primitives;

public class GameImage : GameElement
{
    public GameImage(string path, GameElement parent = null)
        : base(path, parent) { }

    private Image UnityImage
    {
        get
        {
            if (Root != null && Root.TryGetComponent(out Image img))
                return img;
            return null;
        }
    }

    public Color Color => UnityImage != null ? UnityImage.color : Color.white;

    public float FillAmount => UnityImage != null ? UnityImage.fillAmount : 0f;
}