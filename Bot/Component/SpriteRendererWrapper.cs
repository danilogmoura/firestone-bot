using UnityEngine;

namespace FireBot.Bot.Component
{
    internal class SpriteRendererWrapper : ComponentWrapper<SpriteRenderer>
    {
        public SpriteRendererWrapper(params string[] path) : base(string.Join("/", path))
        {
        }

        public bool Enabled()
        {
            return ComponentCached != null && ComponentCached.enabled && ComponentCached.gameObject.activeSelf &&
                   ComponentCached.sprite != null;
        }
    }
}