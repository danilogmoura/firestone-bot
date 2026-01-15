using UnityEngine;
using UnityGameObject = UnityEngine.GameObject;

namespace FireBot.Bot.Component
{
    internal abstract class MappedObjectBase
    {
        private readonly string _cachedPath;

        protected MappedObjectBase(string path)
        {
            _cachedPath = path;
            Transform();
        }

        protected Transform CachedTransform { get; private set; }

        private static UnityGameObject GameObject(string path)
        {
            return UnityGameObject.Find(path);
        }

        private void Transform()
        {
            var rootPath = RootPath(_cachedPath);
            var gameObject = GameObject(rootPath);
            if (gameObject == null)
            {
                return;
            }

            var childPath = ChildPath(_cachedPath);
            if (childPath == null)
            {
                return;
            }

            var transform = gameObject.transform.Find(childPath);
            CachedTransform = !transform ? null : transform;
        }

        public bool Exists()
        {
            return CachedTransform != null;
        }

        public bool IsNull()
        {
            return !Exists();
        }

        public bool IsActive()
        {
            return !IsNull() && CachedTransform.gameObject.active;
        }

        public bool Inactive()
        {
            return !IsActive();
        }

        private static string RootPath(string path)
        {
            return path.Split('/')[0];
        }

        private static string ChildPath(string path)
        {
            var rootPath = RootPath(path);
            return path.Length <= rootPath.Length ? null : path.Substring(rootPath.Length + 1);
        }
    }
}