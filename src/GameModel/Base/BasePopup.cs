using Firebot.Core;
using Firebot.GameModel.Configuration;
using Firebot.GameModel.Primitives;

namespace Firebot.GameModel.Base;

public class BasePopup : GameElement
{
    public BasePopup(string path, GameElement parent = null) : base(path, parent) { }

    private GameButton CloseButton => new(GamePaths.Popups.CloseButton, this);

    public void Close()
    {
        if (!IsVisible()) return;

        if (CloseButton.IsVisible())
            CloseButton.Click();
        else
            Logger.Debug("Close button not found or not visible.");
    }
}