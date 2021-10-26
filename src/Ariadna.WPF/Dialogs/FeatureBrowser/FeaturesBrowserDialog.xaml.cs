using System.Windows;

namespace Ariadna;

internal partial class FeaturesBrowserDialog
{
    private readonly FeatureBrowserViewModel _vm;

    public FeaturesBrowserDialog()
    {
        InitializeComponent();

        Owner = Application.Current.MainWindow;
        ShowInTaskbar = false;

        _vm = new FeatureBrowserViewModel(this);

        DataContext = _vm;
    }

    public IInterfaceFeature ShowDialog(IEnumerable<IInterfaceFeature> allCommandFeatures,
        Dictionary<string, IEnumerable<IInterfaceFeature>> filters)
    {
        return _vm.Load(allCommandFeatures, filters);
    }
}