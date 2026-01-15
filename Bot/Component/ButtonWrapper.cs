using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static FireBot.Utils.BotConstants;

namespace FireBot.Bot.Component
{
    internal class ButtonWrapper : ComponentWrapper<Button>
    {
        public ButtonWrapper(string path) : base(path)
        {
        }

        public bool IsInteractable()
        {
            return IsActive()
                   && (ComponentCached?.enabled ?? false)
                   && (ComponentCached?.interactable ?? false);
        }

        public IEnumerator Click(float delay = InteractionDelay)
        {
            if (!IsInteractable()) yield break;

            ComponentCached.Select();
            ComponentCached.onClick.Invoke();

            if (delay > 0) yield return new WaitForSeconds(delay);
        }
    }
}