using Firebot.GameModel.Wrappers;
using UnityEngine;

namespace Firebot.GameModel;

internal class SpriteRendererWrapper : ComponentWrapper<SpriteRenderer>
{
    public SpriteRendererWrapper(string path) : base(path) { }

    public bool Enabled() => RunSafe(() =>
        Component != null && Component.enabled && Component.gameObject.activeSelf && Component.sprite != null);
}