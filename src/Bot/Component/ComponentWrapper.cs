namespace Firebot.Bot.Component;

internal abstract class ComponentWrapper<T> : MappedObjectBase where T : UnityEngine.Component
{
    private T _componentCached;

    protected ComponentWrapper(string path) : base(path) { }

    protected T ComponentCached =>
        ExecuteSafe(() =>
        {
            if (_componentCached != null) return _componentCached;

            var transform = CachedTransform;
            if (transform == null) return null;

            CachedTransform.TryGetComponent(out _componentCached);
            return _componentCached;
        });

    public override bool Exists() => ComponentCached != null;

    public override bool IsActive() => ComponentCached != null && ComponentCached.gameObject.activeInHierarchy;
}