using UnityEngine;

namespace Firebot.Utilities;

public static class TransformExtensions
{
    public static string GetPath(this Transform transform)
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
}