using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Ariadna;

internal static class MenuItemFactory
{
    public static void AddChildrenItems(ItemsControl parent,
        List<UiMenuItem> children, List<UiIcon> icons, IReadOnlyList<IFeature> features,
        List<UiKeyBinding> hotKeys, IInterfaceHelper interfaceHelper)
    {
        var itemsSource = parent.ItemsSource as ObservableCollection<Control>;

        foreach (var uiMenuItem in children)
        {
            if (uiMenuItem.Id != null)
            {
                var feature = features.FirstOrDefault(f => f.Id == uiMenuItem.Id);

                if (feature == null)
                    continue;

                if (feature is ICommandFeature commandFeature)
                {
                    var enabledIcon = interfaceHelper.GetIcon(icons, commandFeature);

                    var commandMenuItem = CreateItem(commandFeature, enabledIcon, uiMenuItem.Header, hotKeys,
                        interfaceHelper);
                    itemsSource.Add(commandMenuItem);
                }

                if (feature is IComboboxFeature comboboxFeature)
                {
                    parent.AddBehavior(new ComboBoxMenuItemBehavior(comboboxFeature, itemsSource.Count));
                }
            }
            else if (uiMenuItem.Header == "$Separator")
            {
                var separator = new Separator();

                itemsSource.Add(separator);
            }
            else if (uiMenuItem.Children != null)
            {
                var menuItem = new MenuItem
                {
                    Header = uiMenuItem.Header,
                    ItemsSource = new ObservableCollection<Control>()
                };

                menuItem.AddBehavior(new HiddenMenuItemBehavior());

                AddChildrenItems(menuItem, uiMenuItem.Children, icons,
                    features, hotKeys, interfaceHelper);

                itemsSource.Add(menuItem);
            }
        }
    }

    public static MenuItem CreateItem(ICommandFeature commandFeature, FrameworkElement enabledIcon, string header,
        List<UiKeyBinding> hotKeys, IInterfaceHelper interfaceHelper)
    {
        var kb = interfaceHelper.CreateKeyBinding(hotKeys, commandFeature);

        var menuItem = new MenuItem
        {
            Header = header,
            Command = commandFeature,
            Icon = enabledIcon,
            InputGestureText = InterfaceHelper.GetGesture(kb)
        };

        menuItem.AddBehavior(new InterfaceFeatureBehavior(commandFeature));

        if (commandFeature is IToggleCommandFeature toggleCommandFeature)
        {
            menuItem.IsCheckable = true;
            menuItem.AddBehavior(new ToggleFeatureBehavior(toggleCommandFeature));
        }

        return menuItem;
    }
}