using UnityEngine;
using UnityGameObject = UnityEngine.GameObject;

namespace Firebot.Bot.Component
{
    internal abstract class MappedObjectBase
    {
        private readonly string _path;
        private Transform _cachedTransform;

        protected MappedObjectBase(string path)
        {
            _path = path;
        }

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
            if (string.IsNullOrEmpty(_path)) return;

            var rootObj = UnityGameObject.Find(_path.Split('/')[0]);
            if (rootObj == null) return;

            _cachedTransform = !_path.Contains("/")
                ? rootObj.transform
                : rootObj.transform.Find(_path.Substring(_path.IndexOf('/') + 1));
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
            return IsActive() && CachedTransform?.childCount > 0;
        }

        public int? ChildCount()
        {
            return CachedTransform?.childCount;
        }

        public Transform GetChild(int level)
        {
            return CachedTransform?.GetChild(level);
        }
    }
}