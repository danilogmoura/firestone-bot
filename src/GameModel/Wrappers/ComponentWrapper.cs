using System;
using Firebot.GameModel.Base;
using UnityEngine;

namespace Firebot.GameModel.Wrappers;

internal abstract class ComponentWrapper<T> : MappedObjectBase where T : Component
{
    private T _componentCached;

    protected ComponentWrapper(string path) : base(path) { }

    public T Component =>
        RunSafe(() =>
        {
            if (_componentCached != null) return _componentCached;

            var transform = CachedTransform;
            if (transform == null || !CachedTransform.TryGetComponent(out _componentCached))
                throw new Exception("typeof(T).Name, correlationId: CorrelationId, contextInfo: Path");

            return _componentCached;
        });

    public override bool Exists() => RunSafe(() => Component != null);

    public override bool IsActive() => RunSafe(() => Component != null && Component.gameObject.activeInHierarchy,
        defaultValue: false);
}