using System.Collections.Generic;
using UnityEngine;
using Logger = Firebot.Core.Logger;

namespace Firebot.GameModel.Base;

public class GameElement
{
    protected Transform CachedTransform;

    protected GameElement(string path, GameElement parent = null)
    {
        Path = path?.Trim('/');
        Parent = parent;
    }

    protected GameElement(Transform root, string path = null)
    {
        Path = path?.Trim('/');

        if (root == null)
        {
            Logger.Debug($"Root provided is null. Cannot resolve path '{Path ?? "N/A"}'.");
            return;
        }

        CachedTransform = string.IsNullOrEmpty(Path) ? root : root.Find(Path);

        if (CachedTransform == null)
            Logger.Debug($"Could not find element at path '{Path}' under provided root '{root.name}'.");
    }

    protected string Path { get; set; }

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

                if (CachedTransform == null)
                    Logger.Debug($"Failed to resolve '{Path}' under parent '{Parent?.Root?.name}'");
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

    public bool TryGetComponent<T>(out T component) where T : Component
    {
        component = null;
        return Root != null && Root.TryGetComponent(out component);
    }

    public List<GameElement> GetChildrens()
    {
        var children = new List<GameElement>();
        for (var i = 0; i < Root.childCount; i++)
        {
            var child = Root.GetChild(i);
            children.Add(new GameElement(child.name, this));
        }

        return children;
    }

    public IEnumerable<GameElement> GetChildren()
    {
        if (Root == null) yield break;

        var count = Root.childCount;
        for (var i = 0; i < count; i++)
        {
            var child = Root.GetChild(i);
            yield return new GameElement(child);
        }
    }
}