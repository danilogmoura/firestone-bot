using UnityEngine;
using Logger = Firebot.Core.Diagnostics.Logger;

namespace Firebot.GameModel.Base;

public class GameElement
{
    protected Transform _cachedTransform;

    public GameElement(string path, string contextName = null, GameElement parent = null)
    {
        Path = path?.Trim('/');
        Parent = parent;
        ContextName = contextName ?? path;
    }

    public string Path { get; protected set; }
    public string ContextName { get; protected set; }
    protected GameElement Parent { get; }

    public Transform Root
    {
        get
        {
            if (_cachedTransform != null) return _cachedTransform;

            if (Parent != null)
            {
                var parentTrans = Parent.Root;
                if (parentTrans != null) _cachedTransform = parentTrans.Find(Path);
                Debug($"Resolved '{Path}' under parent '{Parent.Path}' to '{_cachedTransform?.name}'");
            }
            else
            {
                var obj = GameObject.Find(Path);
                if (obj != null) _cachedTransform = obj.transform;
                Debug($"Resolved '{Path}' under '{obj?.name}'");
            }

            return _cachedTransform;
        }
    }

    public bool IsVisible() => Root != null && Root.gameObject.activeInHierarchy;

    public T GetComponent<T>() where T : Component => Root?.GetComponent<T>();

    public void Refresh() => _cachedTransform = null;

    protected void Log(string msg) => Logger.Info(ContextName, msg);
    protected void LogWarning(string msg) => Logger.Warning(ContextName, msg);
    protected void LogError(string msg) => Logger.Error(ContextName, msg);
    protected void Debug(string msg) => Logger.Debug(ContextName, msg);
}