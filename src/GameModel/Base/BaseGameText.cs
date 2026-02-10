using Il2CppTMPro;

namespace Firebot.GameModel.Base;

public class BaseGameText : GameElement
{
    private TMP_Text _cachedComponent;

    public BaseGameText(string path, string contextName, GameElement parent = null) :
        base(path, contextName, parent) { }

    private TMP_Text Component
    {
        get
        {
            if (_cachedComponent != null && _cachedComponent.gameObject == null)
                _cachedComponent = null;

            if (_cachedComponent != null || TryGetComponent(out _cachedComponent)) return _cachedComponent;

            if (IsVisible())
                Debug($"Component TMP_Text not found at path: {Path}");

            return null;
        }
    }

    public string Text
    {
        get
        {
            if (!IsVisible()) return string.Empty;

            var comp = Component;
            if (comp == null) return string.Empty;

            var text = comp.text;

            Debug($"Text at '{ContextName}': {text}");
            return text;
        }
    }
}