using AvalonDock.Layout;

namespace Ariadna;

public class LayoutInitializer : ILayoutUpdateStrategy
{
    public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow,
        ILayoutContainer destinationContainer)
    {
        if (destinationContainer is LayoutAnchorablePane destPane &&
            destPane.FindParent<LayoutFloatingWindow>() != null)
            return false;

        var toolsPane = layout.Descendents().OfType<LayoutAnchorablePane>()
            .FirstOrDefault(d => d.Name == "ToolsPane");

        if (toolsPane != null)
        {
            toolsPane.Children.Add(anchorableToShow);
            return true;
        }

        return false;
    }

    public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown)
    {
    }

    public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow,
        ILayoutContainer destinationContainer)
    {
        return false;
    }

    public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown)
    {
    }
}