using Ariadna.Core;
using System.Windows;

namespace Ariadna;

internal class FeatureViewModel : BaseViewModel
{
    public FrameworkElement? Icon { get; set; }

    public string Header { get; set; }

    public IInterfaceFeature CommandFeature { get; }

    public FeatureViewModel(IInterfaceFeature feature)
    {
        CommandFeature = feature;
            
        if (feature is ICommandFeature commandFeature)
            Icon = commandFeature.GetDefaultIcon();

        Header = feature.ToString();
    }
}