using Fluent;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Button = Fluent.Button;
using MenuItem = Fluent.MenuItem;
using ToggleButton = Fluent.ToggleButton;

namespace Ariadna;

internal static class ButtonFactory
{
    public static Control CreateButton(IReadOnlyList<IFeature> features, RibbonItemSize size, string header,
        ICommandFeature feature,
        List<UiKeyBinding> hotKeys, IInterfaceHelper interfaceHelper, UiQuickActionItem item, List<UiIcon> icons,
        List<UiHelpVideo> videos, UiSplitButtonItem? splitButtonItem, bool isFluent = true)
    {
        if (splitButtonItem != null)
        {
            return CreateSplitButton(features, size, header, feature, hotKeys, interfaceHelper, item, icons, videos,
                splitButtonItem);
        }

        var icon = interfaceHelper.GetIcon(icons, feature);

        ButtonBase button = feature switch
        {
            IToggleCommandFeature _ => isFluent
                ? new ToggleButton()
                : new System.Windows.Controls.Primitives.ToggleButton(),

            _ => isFluent
                ? new Button()
                : new System.Windows.Controls.Button(),
        };

        button.Command = feature;

        button.Focusable = false;
        button.Margin = new Thickness(5, 2, 5, 2);
        button.ToolTip = interfaceHelper.CreateToolTip(feature, videos, hotKeys, item.Header, item.Description,
            item.DisableReason);

        if (!isFluent)
        {
            button.Content = icon;
        }

        if (button is IRibbonControl ribbonControl)
        {
            ribbonControl.SizeDefinition = InterfaceHelper.ConvertDefinitions(size);
            ribbonControl.Header = header;
            ribbonControl.Icon = icon;
        }

        if (button is ILargeIconProvider largeIconProvider)
            largeIconProvider.LargeIcon = icon;


        if (feature is IToggleCommandFeature toggleCommandFeature)
            button.AddBehavior(new ToggleFeatureBehavior(toggleCommandFeature));

        if (feature is ITwoStateCommandFeature twoStateCommandFeature)
        {
            var (icon1, altIcon) = interfaceHelper.GetIcon(icons, twoStateCommandFeature);

            var ribbonPropAlt = twoStateCommandFeature.GetAlternativeRibbonProperties();

            button.AddBehavior(new TwoStateFeatureBehavior(twoStateCommandFeature, interfaceHelper,
                header, ribbonPropAlt.Header, icon1, altIcon, item));
        }

        return button;
    }

    private static Control CreateSplitButton(IReadOnlyList<IFeature> features, RibbonItemSize size, string header,
        ICommandFeature feature,
        List<UiKeyBinding> hotKeys, IInterfaceHelper interfaceHelper, UiQuickActionItem item, List<UiIcon> icons,
        List<UiHelpVideo> videos, UiSplitButtonItem splitButtonItem)
    {
        var icon = interfaceHelper.GetIcon(icons, feature);
        var kb = interfaceHelper.CreateKeyBinding(hotKeys, feature);


        SplitButton splitButton = new()
        {
            SizeDefinition = InterfaceHelper.ConvertDefinitions(size),
            Header = splitButtonItem.Header,
            Icon = icon,
            LargeIcon = icon,
            Command = feature,
            Focusable = false,
            Margin = new Thickness(5, 2, 5, 2),
            ToolTip = interfaceHelper.CreateToolTip(feature, videos, hotKeys, item.Header, item.Description,
                item.DisableReason),
        };


        if (feature is IToggleCommandFeature toggleCommandFeature)
            splitButton.AddBehavior(new ToggleFeatureBehavior(toggleCommandFeature));

        if (feature is ITwoStateCommandFeature twoStateCommandFeature)
        {
            var ribbonPropAlt = twoStateCommandFeature.GetAlternativeRibbonProperties();


            splitButton.AddBehavior(new TwoStateFeatureBehavior(twoStateCommandFeature, interfaceHelper, header,
                ribbonPropAlt.Header, icon, twoStateCommandFeature.CreateAlternativeIcon(), item));
        }

        splitButton.ItemsSource =
            CreateSplitButtonItems(features, interfaceHelper, splitButtonItem.Items, hotKeys, icons);

        return splitButton;
    }

    private static ObservableCollection<MenuItem> CreateSplitButtonItems(IReadOnlyList<IFeature> features,
        IInterfaceHelper interfaceHelper, List<UiRibbonItem> splitButtonItems, List<UiKeyBinding> hotKeys,
        List<UiIcon> icons)
    {
        ObservableCollection<MenuItem> items = new();

        foreach (var item in splitButtonItems)
        {
            var feature = features.FirstOrDefault(f => f.Id == item.Id);

            if (feature is not ICommandFeature commandFeature)
                continue;

            var icon = interfaceHelper.GetIcon(icons, commandFeature);

            items.Add(CreateItem(commandFeature, icon, item.Header, hotKeys, interfaceHelper));
        }

        return items;
    }

    private static MenuItem CreateItem(ICommandFeature commandFeature,
        FrameworkElement enabledIcon, string header,
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