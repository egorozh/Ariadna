using System.Windows.Controls;

namespace Ariadna;

internal static class ContextMenuManager
{
    public static void SetItems(ContextMenu contextMenu,
        List<UiMenuItem>? items,
        List<UiIcon> uiFullIcons,
        List<UiKeyBinding> hotKeys, IReadOnlyList<IFeature> features,
        IInterfaceHelper interfaceHelper)
    {
        if (items == null)
            return;

        MenuItemFactory.AddChildrenItems(contextMenu, items, uiFullIcons,
            features, hotKeys, interfaceHelper);
    }

    public static void Tree(IEnumerable<Control> items, Action<Control> action)
    {
        foreach (var item in items)
        {
            action.Invoke(item);

            if (item is ItemsControl {ItemsSource: IEnumerable<Control> children})
                Tree(children, action);
        }
    }
}