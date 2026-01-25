namespace Firebot.Bot.Component
{
    internal abstract class ComponentWrapper<T> : MappedObjectBase where T : UnityEngine.Component
    {
        private T _componentCached;

        protected ComponentWrapper(string path) : base(path)
        {
        }

        protected T ComponentCached
        {
            get
            {
                if (_componentCached != null) return _componentCached;
                if (CachedTransform != null) _componentCached = CachedTransform.GetComponent<T>();
                return _componentCached;
            }
        }

        public new void InvalidateCache()
        {
            base.InvalidateCache();
            _componentCached = null;
        }

        public bool HasComponent()
        {
            return ComponentCached != null;
        }

        public T Get()
        {
            return ComponentCached;
        }
    }
}