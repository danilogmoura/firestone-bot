using UnityEngine;

namespace Firebot.Bot.Component
{
    internal class SpriteRendererWrapper : ComponentWrapper<SpriteRenderer>
    {
        public SpriteRendererWrapper(string path) : base(path)
        {
        }

        public bool Enabled()
        {
            return ComponentCached != null && ComponentCached.enabled && ComponentCached.gameObject.activeSelf &&
                   ComponentCached.sprite != null;
        }
    }
}