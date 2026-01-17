using System.Collections;
using FireBot.Config;
using UnityEngine;
using UnityEngine.UI;

namespace FireBot.Bot.Component
{
    internal class ButtonWrapper : ComponentWrapper<Button>
    {
        public ButtonWrapper(string path) : base(path)
        {
        }

        public bool IsInteractable()
        {
            return IsActive() && HasComponent() && ComponentCached.enabled && ComponentCached.interactable;
        }


        public IEnumerator Click()
        {
            return Click(BotSettings.InteractionDelay.Value);
        }

        public IEnumerator Click(float delay)
        {
            if (!IsInteractable())
                yield break;

            ComponentCached.Select();
            ComponentCached.onClick.Invoke();

            if (delay > 0) yield return new WaitForSeconds(delay);
        }
    }
}