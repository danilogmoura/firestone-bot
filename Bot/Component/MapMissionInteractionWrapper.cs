using Il2Cpp;
using UnityEngine.EventSystems;

namespace FireBot.Bot.Component
{
    internal class MapMissionInteractionWrapper : ComponentWrapper<MapMissionInteraction>
    {
        public MapMissionInteractionWrapper(string path) : base(path)
        {
        }

        public void OnClick()
        {
            if (ComponentCached == null) return;

            var fakeEvent = new PointerEventData(EventSystem.current)
            {
                button = PointerEventData.InputButton.Left
            };
            ComponentCached.OnPointerClick(fakeEvent);
        }
    }
}