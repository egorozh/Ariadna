using System.Collections.ObjectModel;

namespace Ariadna.Settings;

internal class SplitButtonViewModel : ButtonViewModel
{
    public string SplitHeader { get; set; }

    public ObservableCollection<ButtonViewModel> Buttons { get; } = new();

    public SplitButtonViewModel(UiRibbonItem ribbonItem, IFeature? feature, IReadOnlyList<IFeature> features) : base(
        ribbonItem, feature)
    {
        foreach (var item in ribbonItem.SplitButtonItem.Items)
        {
            var itemFeature = features.FirstOrDefault(f => f.Id == item.Id);

            if (itemFeature is ICommandFeature commandFeature)
                Buttons.Add(new ButtonViewModel(ribbonItem, commandFeature));
        }

        SplitHeader = ribbonItem.SplitButtonItem.Header;
    }
}