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
            if (!IsVisible())
            {
                Debug($"Read SpriteName ignored: Element invisible/inactive. Path: {Path}");
                return string.Empty;
            }

            if (TryGetComponent(out SpriteRenderer sprite))
            {
                var name = sprite.sprite != null ? sprite.sprite.name : "null_sprite";
                Debug($"SpriteName read: '{name}'. Path: {Path}");
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
            {
                Debug($"Sprite Color read: {sr.color}. Path: {Path}");
                return sr.color;
            }

            return Color.white;
        }
    }

    public bool IsRendererEnabled
    {
        get
        {
            if (TryGetComponent(out SpriteRenderer sr))
            {
                Debug($"Renderer state: {sr.enabled}. Path: {Path}");
                return sr.enabled;
            }

            return false;
        }
    }

    public int SortingOrder
    {
        get
        {
            if (TryGetComponent(out SpriteRenderer sr))
            {
                Debug($"SortingOrder read: {sr.sortingOrder}. Path: {Path}");
                return sr.sortingOrder;
            }

            return 0;
        }
    }
}