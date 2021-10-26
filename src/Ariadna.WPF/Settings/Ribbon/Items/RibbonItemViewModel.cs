using Ariadna.Core;

namespace Ariadna.Settings;

internal class RibbonItemViewModel : BaseViewModel
{
    public string Header { get; set; }

    public string Description { get; set; }

    public string DisableReason { get; set; }

    public IFeature Feature { get; set; }

    public string FeatureName => Feature?.ToString();

    public RibbonItemViewModel(UiRibbonItem ribbonItem, IFeature feature)
    {
        Header = ribbonItem.Header;
        Description = ribbonItem.Description;
        DisableReason = ribbonItem.DisableReason;

        Feature = feature;
    }

    public static RibbonItemViewModel CreateItem(UiRibbonItem ribbonItem, IFeature feature, IReadOnlyList<IFeature> features)
    {
        if (ribbonItem.SplitButtonItem != null)
            return new SplitButtonViewModel(ribbonItem, feature, features);

        if (feature is ICommandFeature commandFeature)
            return new ButtonViewModel(ribbonItem, feature);

        if (feature is IComboboxFeature comboboxFeature)
            return new ComboBoxViewModel(ribbonItem, feature);


        return null;
    }
}