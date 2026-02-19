using System.Collections;
using System.Linq;
using Firebot.GameModel.Base;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.Town.Library.FirestoneResearch;

public class Node : GameElement
{
    public Node() : base(Paths.MenusLoc.CanvasLoc.TownLoc.LibraryLoc.NodeLoc.Root) { }

    private GameElement GetTree() => GetChildren().First(tree => tree.IsVisible());

    private static GameElement GetGrow(GameElement gameElement) =>
        new(Paths.MenusLoc.CanvasLoc.TownLoc.LibraryLoc.NodeLoc.Glow, gameElement);

    private static GameText GetCompletedTxt(GameElement gameElement) =>
        new(Paths.MenusLoc.CanvasLoc.TownLoc.LibraryLoc.NodeLoc.CompletedTxt, gameElement);

    private static GameElement GetProgressBar(GameElement gameElement) =>
        new(Paths.MenusLoc.CanvasLoc.TownLoc.LibraryLoc.NodeLoc.ProgressBar, gameElement);

    private static bool IsActiveNode(GameElement child)
    {
        if (!child.IsVisible()) return false;

        var grow = GetGrow(child);
        if (grow.IsVisible()) return false;

        var completedTxt = GetCompletedTxt(child);
        if (completedTxt.IsVisible()) return false;

        var progressBar = GetProgressBar(child);
        return progressBar.IsVisible();
    }

    public IEnumerator Select(int index)
    {
        var tree = GetTree();
        var child = tree.GetChild(index);

        if (!IsActiveNode(child))
            yield break;

        yield return new GameButton(parent: child).Click();
    }

    public IEnumerator SelectAny()
    {
        var tree = GetTree();
        var child = tree.GetChildren().FirstOrDefault(IsActiveNode);

        if (child == null)
            yield break;

        yield return new GameButton(parent: child).Click();
    }
}