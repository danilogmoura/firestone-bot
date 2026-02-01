using Firebot.GameModel.Wrappers;
using Il2CppTMPro;

namespace Firebot.GameModel.TMProComponents;

internal class BaseTextWrapper<T> : ComponentWrapper<T> where T : TMP_Text
{
    protected BaseTextWrapper(string path) : base(path) { }

    public string Text => GetText();

    private string GetText() => RunSafe(() => Component?.GetParsedText() ?? string.Empty);

    public bool Contains(string textToCheck) =>
        RunSafe(() => !string.IsNullOrEmpty(Text) && Text.Trim().Contains(textToCheck.Trim()),
            defaultValue: false);
}