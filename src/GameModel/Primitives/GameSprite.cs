using Firebot.GameModel.Base;
using UnityEngine;

namespace Firebot.GameModel.Primitives;

public class GameSprite : GameElement
{
    public GameSprite(string path, GameElement parent) : base(path, parent) { }

    private SpriteRenderer Renderer
    {
        get
        {
            if (Root != null && Root.TryGetComponent(out SpriteRenderer sr))
                return sr;
            return null;
        }
    }

    public string SpriteName => Renderer?.sprite?.name ?? "";

    public Color Color => Renderer?.color ?? Color.white;

    public bool IsRendererEnabled => Renderer != null && Renderer.enabled;

    public int SortingOrder => Renderer != null ? Renderer.sortingOrder : 0;
}