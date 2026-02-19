using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;
using Logger = Firebot.Core.Logger;

namespace Firebot.GameModel.Base;

public class GameElement
{
    private readonly string _className;
    private readonly HashSet<string> _loggedFailures = new();

    public GameElement(string path = null, GameElement parent = null, Transform transform = null)
    {
        _className = GetType().Name;

        if (transform != null)
        {
            var transformPath = GetGameObjectPath(transform);
            Path = BuildPath(transformPath, path);
        }
        else if (parent != null)
        {
            var parentPath = !string.IsNullOrEmpty(parent.Path) ? parent.Path : GetGameObjectPath(parent.Root);
            Path = BuildPath(parentPath, path);
        }
        else
            Path = CleanPath(path);

        if (string.IsNullOrEmpty(Path))
            DebugOnce("init-empty-path", "[FAILED] GameElement initialized with empty path.");
    }

    protected string Path { get; }

    // Always resolve from Path; do not cache transforms.
    protected Transform Root
    {
        get
        {
            var resolved = ResolvePath(Path);

            if (resolved == null && !string.IsNullOrEmpty(Path))
                DebugOnce($"root-resolve:{Path}", $"[FAILED] Critical: Could not resolve Transform. Path: {Path}");

            return resolved;
        }
    }

    public string Name => Root?.name ?? string.Empty;

    private static string CleanPath(string path) =>
        string.IsNullOrEmpty(path) ? path : Regex.Replace(path, @"/+", "/").Trim('/');

    private Transform ResolvePath(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        var slashIndex = path.IndexOf('/');

        if (slashIndex == -1)
        {
            var obj = GameObject.Find(path);
            if (obj == null)
                DebugOnce($"root-missing:{path}", $"[FAILED] Root Object not found in scene: {path}");
            return obj?.transform;
        }

        var rootName = path[..slashIndex];
        var rootObj = GameObject.Find(rootName);

        if (rootObj == null)
        {
            DebugOnce($"root-missing:{rootName}",
                $"[FAILED] Root '{rootName}' missing. Hierarchy search aborted. Path: {path}");
            return null;
        }

        var relativePath = path[(slashIndex + 1)..];
        var result = rootObj.transform.Find(relativePath);

        if (result == null)
            DebugOnce($"path-broken:{path}",
                $"[FAILED] Path broken: '{rootName}' exists, but child '{relativePath}' is missing. Full: {path}");

        return result;
    }

    public virtual bool IsVisible()
    {
        var currentRoot = Root;
        var success = currentRoot != null && currentRoot.gameObject.activeInHierarchy;

        if (!success)
            DebugOnce($"hidden:{Path}", $"[FAILED] Element is hidden or inactive. Path: {Path}");

        return success;
    }

    public bool TryGetComponent<T>(out T component) where T : Component
    {
        component = null;
        var currentRoot = Root;
        if (currentRoot == null) return false;

        var success = currentRoot.TryGetComponent(out component);
        if (!success)
            DebugOnce($"component-missing:{typeof(T).Name}:{Path}",
                $"[FAILED] Component <{typeof(T).Name}> missing on: {currentRoot.name}. Path: {Path}");

        return success;
    }

    public IEnumerable<GameElement> GetChildren()
    {
        var currentRoot = Root;
        if (currentRoot == null)
        {
            DebugOnce($"children-root-null:{Path}", $"[FAILED] Cannot get children: Root is null. Path: {Path}");
            yield break;
        }

        for (var i = 0; i < currentRoot.childCount; i++)
            yield return new GameElement(transform: currentRoot.GetChild(i));
    }

    public GameElement GetChild(int i)
    {
        try
        {
            return new GameElement(transform: Root.GetChild(i));
        }
        catch (Exception e)
        {
            Debug($" [EXCEPTION] Failed to get child at index {i}. Path: {Path}. Exception: {e}");
            return null;
        }
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

    private static string BuildPath(string basePath, string path)
    {
        if (string.IsNullOrEmpty(basePath))
            return CleanPath(path);

        if (string.IsNullOrEmpty(path))
            return CleanPath(basePath);

        return CleanPath($"{basePath}/{path}");
    }

    protected void Debug(string message, [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        => Logger.Debug($"[{_className}::{member}:{line}] {message}");

    private void DebugOnce(string key, string message, [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        if (_loggedFailures.Add(key)) Debug(message, member, line);
    }
}