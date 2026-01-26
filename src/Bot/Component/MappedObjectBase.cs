using System;
using System.Runtime.CompilerServices;
using Firebot.Utils;
using UnityEngine;
using UnityGameObject = UnityEngine.GameObject;

namespace Firebot.Bot.Component;

internal abstract class MappedObjectBase
{
    protected readonly string _path;
    private Transform _cachedTransform;

    protected MappedObjectBase(string path)
    {
        _path = path;
    }

    protected virtual string ObjectName => GetType().Name;

    protected Transform CachedTransform
    {
        get
        {
            if (_cachedTransform == null) FindAndCacheTransform();
            return _cachedTransform;
        }
    }

    private void FindAndCacheTransform() =>
        ExecuteSafe(() =>
        {
            if (string.IsNullOrEmpty(_path)) return;
            var rootObj = UnityGameObject.Find(_path.Split('/')[0]);

            if (rootObj == null)
                throw new InvalidOperationException($"[MapError] {ObjectName}: Root not found for {_path}");

            _cachedTransform = !_path.Contains('/')
                ? rootObj.transform
                : rootObj.transform.Find(_path.Substring(_path.IndexOf('/') + 1));

            if (_cachedTransform != null)
                LogManager.Debug("MapSuccess", $"{ObjectName}: Cached {_path}");
        });

    protected void ExecuteSafe(Action action, [CallerMemberName] string actionName = null)
    {
        try
        {
            action?.Invoke();
        }
        catch (Exception ex)
        {
            LogManager.Error(ObjectName, $"Error in '{actionName}': {ex.Message}");
            LogManager.Debug(ObjectName, ex.StackTrace);
        }
    }

    protected T ExecuteSafe<T>(Func<T> func, T defaultValue = default, [CallerMemberName] string actionName = "")
    {
        try
        {
            return func();
        }
        catch (Exception ex)
        {
            LogManager.Error(ObjectName, $"Error in '{actionName}': {ex.Message}");
            LogManager.Debug(ObjectName, ex.StackTrace);
            return defaultValue;
        }
    }

    public virtual bool Exists() => CachedTransform != null;

    public virtual bool IsActive() => Exists() && CachedTransform.gameObject.activeInHierarchy;

    public bool HasChildren() => IsActive() && ExecuteSafe(() => CachedTransform.childCount > 0);

    public int ChildCount() => Exists() ? CachedTransform.childCount : 0;

    public Transform GetChild(int level) => ExecuteSafe(() => CachedTransform?.GetChild(level));
}