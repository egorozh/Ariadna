using System.Windows;
using System.Windows.Controls;

namespace Ariadna;

internal static class ComboBoxFactory
{
    public static Control CreateComboBox(IComboboxFeature feature, IInterfaceHelper interfaceHelper,
        UiQuickActionItem item, List<UiHelpVideo> videos, bool isFluent = true)
    {
        var toolTip = interfaceHelper.CreateToolTip(feature, videos, null, item.Header, item.Description,
            item.DisableReason);

        if (isFluent)
        {
            var rcb = CreateRibbonComboBox(toolTip, item.Header);
            rcb.ComboBox.AddBehavior(new ComboBoxFeatureBehavior(feature));
            return rcb;
        }

        Control comboBox = CreateQuickBarComboBox(toolTip);

        comboBox.AddBehavior(new ComboBoxFeatureBehavior(feature));

        return comboBox;
    }

    private static ComboBox CreateQuickBarComboBox(object toolTip) => new()
    {
        ToolTip = toolTip,
        Margin = new Thickness(5, 2, 5, 2)
    };

    private static RibbonComboBox CreateRibbonComboBox(object toolTip, string? header) => new()
    {
        ToolTip = toolTip,
        TextBlock = {Text = header},
    };
}