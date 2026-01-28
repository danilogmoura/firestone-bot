namespace Firebot.Bot.Component;

internal abstract class ComponentWrapper<T> : MappedObjectBase where T : UnityEngine.Component
{
    private T _componentCached;

    protected ComponentWrapper(string path) : base(path) { }

    public T Component =>
        ExecuteSafe(() =>
        {
            if (_componentCached != null) return _componentCached;

            var transform = CachedTransform;
            if (transform == null) return null;

            CachedTransform.TryGetComponent(out _componentCached);
            return _componentCached;
        });

    public override bool Exists() => Component != null;

    public override bool IsActive()
    {
        var active = Component != null && Component.gameObject.activeInHierarchy;
        Log.Debug($"Component {typeof(T).Name} is active: {active}");
        return active;
    }
}