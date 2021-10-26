using System.Windows.Controls;

namespace Ariadna;

internal static class ItemFactory
{
    public static Control? CreateItem(IReadOnlyList<IFeature> features, 
        UiQuickActionItem ribbonItem,
        IInterfaceFeature feature,
        List<UiKeyBinding> hotKeys,
        List<UiIcon> icons,
        List<UiHelpVideo> helpVideos, 
        IInterfaceHelper interfaceHelper,
        bool isFluent = true,
        RibbonItemSize size = RibbonItemSize.Small)
    {
        var control = feature switch
        {
            ICommandFeature commandFeature => ButtonFactory.CreateButton(features,size, ribbonItem.Header,
                commandFeature, hotKeys, interfaceHelper, ribbonItem, icons, helpVideos,
                ribbonItem.SplitButtonItem, isFluent),
            IComboboxFeature comboboxFeature => ComboBoxFactory.CreateComboBox(comboboxFeature,
                interfaceHelper, ribbonItem, helpVideos, isFluent),
            _ => null
        };

        control?.AddBehavior(new InterfaceFeatureBehavior(feature));

        return control;
    }
}