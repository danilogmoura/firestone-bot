namespace FireBot.Bot.Component
{
    internal abstract class ComponentWrapper<T> : MappedObjectBase where T : UnityEngine.Component
    {
        protected ComponentWrapper(string path) : base(path)
        {
            ComponentCached = CachedTransform?.GetComponent<T>();
        }

        protected T ComponentCached { get; private set; }
    }
}