using Firebot.GameModel.Base;
using UnityEngine;

namespace Firebot.GameModel.Primitives;

public class GameSprite : GameElement
{
    public GameSprite(string path = null, GameElement parent = null, Transform transform = null)
        : base(path, parent, transform) { }

    public string SpriteName
    {
        get
        {
            if (!IsVisible()) return string.Empty;

            if (TryGetComponent(out SpriteRenderer sprite))
            {
                var name = sprite.sprite != null ? sprite.sprite.name : "null_sprite";
                return name;
            }

            return string.Empty;
        }
    }

    public Color Color
    {
        get
        {
            if (TryGetComponent(out SpriteRenderer sr))
                return sr.color;

            return Color.white;
        }
    }

    public bool IsRendererEnabled
    {
        get
        {
            if (TryGetComponent(out SpriteRenderer sr))
                return sr.enabled;

            return false;
        }
    }

    public int SortingOrder
    {
        get
        {
            if (TryGetComponent(out SpriteRenderer sr))
                return sr.sortingOrder;

            return 0;
        }
    }
}