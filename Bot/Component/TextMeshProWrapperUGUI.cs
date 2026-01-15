using Il2CppTMPro;

namespace FireBot.Bot.Component
{
    internal class TextMeshProWrapperUGUI : ComponentWrapper<TextMeshProUGUI>
    {
        public TextMeshProWrapperUGUI(params string[] path) : base(string.Join("/", path))
        {
        }

        public string GetParsedText()
        {
            return ComponentCached != null ? ComponentCached.GetParsedText() : string.Empty;
        }
    }
}