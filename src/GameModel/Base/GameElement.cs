using UnityEngine;
using Logger = Firebot.Old.Core.Diagnostics.Logger;

namespace Firebot.GameModel.Base;

public abstract class GameElement
{
    protected Transform CachedTransform;

    protected GameElement(string path, string contextName, GameElement parent = null)
    {
        Path = path?.Trim('/');
        Parent = parent;
        ContextName = contextName;
    }

    protected GameElement(Transform root, string contextName, string path = null)
    {
        ContextName = contextName;
        Path = path?.Trim('/');

        if (root == null)
        {
            Debug($"Root provided is null. Cannot resolve path '{Path ?? "N/A"}' for {ContextName}.");
            return;
        }

        CachedTransform = string.IsNullOrEmpty(Path) ? root : root.Find(Path);

        if (CachedTransform == null)
            Debug($"Could not find element at path '{Path}' under provided root '{root.name}'.");
    }

    protected string Path { get; set; }
    protected string ContextName { get; set; }

    protected GameElement Parent { get; }

    public Transform Root
    {
        get
        {
            if (CachedTransform != null && CachedTransform.gameObject == null) CachedTransform = null;

            if (CachedTransform != null) return CachedTransform;

            if (string.IsNullOrEmpty(Path) && Parent == null) return null;

            if (Parent != null)
            {
                var parentTrans = Parent.Root;
                if (parentTrans == null)
                    return null;

                CachedTransform = parentTrans.Find(Path);

                if (CachedTransform == null) Debug($"Failed to resolve '{Path}' under parent '{Parent.ContextName}'");
            }
            else
            {
                var obj = GameObject.Find(Path);
                if (obj != null) CachedTransform = obj.transform;
            }

            return CachedTransform;
        }
    }

    public virtual bool IsVisible() => Root != null && Root.gameObject.activeInHierarchy;

    public void Refresh() => CachedTransform = null;

    protected bool TryGetComponent<T>(out T component) where T : Component
    {
        component = null;
        return Root != null && Root.TryGetComponent(out component);
    }

    protected void Log(string msg) => Logger.Info(ContextName, msg);
    protected void LogWarning(string msg) => Logger.Warning(ContextName, msg);
    protected void LogError(string msg) => Logger.Error(ContextName, msg);
    protected void Debug(string msg) => Logger.Debug(ContextName, msg);
}