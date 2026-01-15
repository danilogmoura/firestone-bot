using Il2CppTMPro;

namespace FireBot.Bot.Component
{
    internal class TextMeshProUGUIWrapper : ComponentWrapper<TextMeshProUGUI>
    {
        public TextMeshProUGUIWrapper(string path) : base(path)
        {
        }

        public string Text => ComponentCached?.text ?? string.Empty;

        public string GetParsedText()
        {
            if (!IsActive() || ComponentCached == null) return string.Empty;

            return ComponentCached.GetParsedText();
        }

        public bool Contains(string textToCheck)
        {
            return !string.IsNullOrEmpty(Text) && Text.ToLower().Contains(textToCheck.ToLower());
        }
    }
}