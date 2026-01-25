using UnityEngine;

namespace Firebot.Bot.Component
{
    internal class ObjectWrapper : MappedObjectBase
    {
        public ObjectWrapper(string path) : base(path)
        {
        }

        public GameObject GameObject => CachedTransform?.gameObject;

        public Transform Transform => CachedTransform;
    }
}