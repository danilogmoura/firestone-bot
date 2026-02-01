using System;
using System.Runtime.CompilerServices;
using Firebot.Core;
using UnityEngine;
using Logger = Firebot.Core.Logger;
using UnityGameObject = UnityEngine.GameObject;

namespace Firebot.GameModel.Base;

internal abstract class MappedObjectBase
{
    protected readonly Logger Log;
    protected readonly string Path;
    private Transform _cachedTransform;

    protected MappedObjectBase(string path)
    {
        if (string.IsNullOrEmpty(path))
            throw new Exception(CorrelationId);

        Path = path;
        Log = new Logger(GetType().Name);
    }

    protected static string CorrelationId => BotContext.CorrelationId;

    public string Name => RunSafe(() => CachedTransform.name);

    protected Transform CachedTransform
    {
        get
        {
            if (_cachedTransform == null) FindAndCacheTransform();
            return _cachedTransform;
        }
    }

    private void FindAndCacheTransform() =>
        RunSafe(() =>
        {
            var slashIndex = Path.IndexOf('/');

            var rootName = slashIndex == -1 ? Path : Path[..slashIndex];
            var rootObj = UnityGameObject.Find(rootName);

            if (rootObj == null)
                throw new Exception("rootName, contextInfo: Path, correlationId: CorrelationId");

            _cachedTransform = slashIndex == -1
                ? rootObj.transform
                : rootObj.transform.Find(Path[(slashIndex + 1)..]);

            if (_cachedTransform == null)
                throw new Exception(@"rootName,
                    message: $""Root '{rootName}' found, but child at path '{Path}' is missing."", contextInfo: Path");
        }, CorrelationId);

    public void RunSafe(Action action, string contextInfo = null, [CallerMemberName] string actionName = null) =>
        SafeExecutor.Run(action, Log, CorrelationId, contextInfo ?? Path, actionName);

    public T RunSafe<T>(Func<T> func, string contextInfo = null, T defaultValue = default,
        [CallerMemberName] string actionName = null) =>
        SafeExecutor.Run(func, Log, CorrelationId, contextInfo ?? Path, defaultValue, actionName);

    public virtual bool Exists() => RunSafe(() => CachedTransform != null, defaultValue: false);

    public virtual bool IsActive() =>
        RunSafe(() => Exists() && CachedTransform.gameObject.activeInHierarchy, defaultValue: false);

    public bool HasChildren() => RunSafe(() => CachedTransform.childCount > 0, defaultValue: false);

    public int ChildCount() => RunSafe(() => CachedTransform.childCount, defaultValue: 0);

    public Transform GetChild(int level) => RunSafe(() => CachedTransform.GetChild(level));
}