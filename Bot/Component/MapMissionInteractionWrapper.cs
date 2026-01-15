using Il2Cpp;
using UnityEngine.EventSystems;

namespace FireBot.Bot.Component
{
    internal class MapMissionInteractionWrapper : ComponentWrapper<MapMissionInteraction>
    {
        public MapMissionInteractionWrapper(params string[] path) : base(string.Join("/", path))
        {
        }

        public void OnClick()
        {
            var fakeEvent = new PointerEventData(EventSystem.current)
            {
                button = PointerEventData.InputButton.Left
            };

            ComponentCached.OnPointerClick(fakeEvent);
        }
    }
}