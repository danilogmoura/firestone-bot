using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace FireBot.Bot.Component
{
    internal class ButtonWrapper : ComponentWrapper<Button>
    {
        public ButtonWrapper(params string[] path) : base(string.Join("/", path))
        {
        }

        public IEnumerator Click(float delay = 0.5f)
        {
            ComponentCached?.onClick.Invoke();
            yield return new WaitForSeconds(delay);
        }

        public bool IsInteractable()
        {
            return IsActive() && ComponentCached.interactable;
        }

        public bool IsNotInteractable()
        {
            return !IsInteractable();
        }

        public bool IsClickable()
        {
            return ComponentCached.gameObject.activeInHierarchy
                   && ComponentCached.enabled
                   && ComponentCached.interactable;
        }
    }
}