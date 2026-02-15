using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Logger = Firebot.Core.Logger;

namespace Firebot.GameModel.Base;

public class GameElement
{
    private readonly string _className;

    public GameElement(string path = null, GameElement parent = null, Transform transform = null)
    {
        _className = GetType().Name;

        if (transform != null)
            Path = GetGameObjectPath(transform);
        else if (parent != null)
        {
            var parentPath = !string.IsNullOrEmpty(parent.Path) ? parent.Path : GetGameObjectPath(parent.Root);
            if (!string.IsNullOrEmpty(parentPath))
                Path = string.IsNullOrEmpty(path) ? parentPath.Trim('/') : $"{parentPath.Trim('/')}/{path.Trim('/')}";
            else
                Path = path?.Trim('/');
        }
        else
            Path = path?.Trim('/');

        if (!string.IsNullOrEmpty(Path) && Path.StartsWith("/")) Path = Path[1..];

        Debug(!string.IsNullOrEmpty(Path)
            ? $"[SUCCESS] GameElement initialized. Path: {Path}"
            : "[FAILED] GameElement initialized with empty path.");
    }

    protected string Path { get; }

    protected Transform Root
    {
        get
        {
            var resolved = ResolvePath(Path);

            if (resolved == null && !string.IsNullOrEmpty(Path))
                Debug($"[FAILED] Critical: Could not resolve Transform. Path: {Path}");

            return resolved;
        }
    }

    public string Name => Root?.name ?? string.Empty;

    private Transform ResolvePath(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        var slashIndex = path.IndexOf('/');

        if (slashIndex == -1)
        {
            var obj = GameObject.Find(path);
            if (obj == null)
                Debug($"[FAILED] Root Object not found in scene: {path}");
            return obj?.transform;
        }

        var rootName = path[..slashIndex];
        var rootObj = GameObject.Find(rootName);

        if (rootObj == null)
        {
            Debug($"[FAILED] Root '{rootName}' missing. Hierarchy search aborted. Path: {path}");
            return null;
        }

        var relativePath = path[(slashIndex + 1)..];
        var result = rootObj.transform.Find(relativePath);

        if (result == null)
            Debug($"[FAILED] Path broken: '{rootName}' exists, but child '{relativePath}' is missing. Full: {path}");

        return result;
    }

    public virtual bool IsVisible()
    {
        var success = Root != null && Root.gameObject.activeInHierarchy;

        if (!success)
            Debug($"[FAILED] Element is hidden or inactive. Path: {Path}");

        return success;
    }

    public bool TryGetComponent<T>(out T component) where T : Component
    {
        component = null;
        var currentRoot = Root;
        if (currentRoot == null) return false;

        var success = currentRoot.TryGetComponent(out component);
        if (!success)
            Debug($"[FAILED] Component <{typeof(T).Name}> missing on: {currentRoot.name}. Path: {Path}");

        return success;
    }

    public IEnumerable<GameElement> GetChildren()
    {
        var currentRoot = Root;
        if (currentRoot == null)
        {
            Debug($"[FAILED] Cannot get children: Root is null. Path: {Path}");
            yield break;
        }

        for (var i = 0; i < currentRoot.childCount; i++)
            yield return new GameElement(transform: currentRoot.GetChild(i));
    }

    private static string GetGameObjectPath(Transform transform)
    {
        if (transform == null) return string.Empty;
        var path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }

        return path;
    }

    protected void Debug(string message, [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        => Logger.Debug($"[{_className}::{member}:{line}] {message}");
}