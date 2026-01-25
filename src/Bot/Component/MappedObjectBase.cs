using System;
using System.Runtime.CompilerServices;
using Firebot.Utils;
using UnityEngine;
using UnityGameObject = UnityEngine.GameObject;

namespace Firebot.Bot.Component;

internal abstract class MappedObjectBase
{
    private readonly string _path;
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
            if (_cachedTransform == null)
                FindAndCacheTransform();

            return _cachedTransform;
        }
    }

    private void FindAndCacheTransform()
    {
        ExecuteSafe(() =>
        {
            if (string.IsNullOrEmpty(_path)) return;

            var rootObj = UnityGameObject.Find(_path.Split('/')[0]);
            if (rootObj == null)
            {
                LogManager.Debug("MapError", $"{ObjectName}: Root not found for {_path}");
                return;
            }

            _cachedTransform = !_path.Contains('/')
                ? rootObj.transform
                : rootObj.transform.Find(_path.Substring(_path.IndexOf('/') + 1));

            if (_cachedTransform != null)
                LogManager.Debug("MapSuccess", $"{ObjectName}: Cached {_path}");
        });
    }

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

    public void InvalidateCache()
    {
        _cachedTransform = null;
    }

    public bool Exists()
    {
        return CachedTransform != null;
    }

    public bool IsActive()
    {
        return CachedTransform != null && CachedTransform.gameObject.activeInHierarchy;
    }

    public bool HasChilden()
    {
        return IsActive() && ExecuteSafe(() => CachedTransform.childCount > 0);
    }

    public int? ChildCount()
    {
        return ExecuteSafe(() => CachedTransform?.childCount);
    }

    public Transform GetChild(int level)
    {
        return ExecuteSafe(() => CachedTransform?.GetChild(level));
    }
}