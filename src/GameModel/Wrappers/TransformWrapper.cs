using System;
using System.Collections.Generic;
using UnityEngine;
using static Firebot.Core.StringUtils;

namespace Firebot.GameModel.Wrappers;

internal class TransformWrapper : ComponentWrapper<Transform>
{
    public TransformWrapper(string path) : base(path) { }

    public List<TransformWrapper> GetChildren() => RunSafe(() =>
    {
        var children = new List<TransformWrapper>();
        for (var i = 0; i < ChildCount(); i++)
        {
            var child = GetChild(i);
            children.Add(new TransformWrapper(JoinPath(Path, child.name)));
        }

        return children;
    });

    public TransformWrapper Find(string path) => RunSafe(() =>
    {
        var transform = Component.Find(path);
        if (transform == null) throw new InvalidOperationException("Child not found: " + path);
        return new TransformWrapper(JoinPath(Path, path));
    });
}