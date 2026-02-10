using Firebot.GameModel.Configuration;
using Firebot.GameModel.Primitives;

namespace Firebot.GameModel.Base;

public class BasePopup : GameElement
{
    public BasePopup(string path, string contextName, GameElement parent = null) : base(path, contextName, parent) { }

    private GameButton CloseButton => new(GamePaths.Popups.CloseButton, "CloseBtn", this);

    public void Close()
    {
        if (!IsVisible()) return;

        if (CloseButton.IsVisible())
            CloseButton.Click();
        else
            Debug("Close button not found or not visible.");
    }
}