using Ariadna.Core;
using Prism.Commands;
using System.Collections.ObjectModel;

namespace Ariadna;

internal class FeatureBrowserViewModel : BaseViewModel
{
    #region Private Methods

    private readonly FeaturesBrowserDialog _featuresBrowserDialog;
    private IEnumerable<IInterfaceFeature> _allCommandFeatures;
    private Dictionary<string, IEnumerable<IInterfaceFeature>> _filters;
    private bool _isSelect;

    #endregion

    #region Public Properties

    public ObservableCollection<string> FilterTitles { get; set; } = new ObservableCollection<string>();

    public string SelectedFilterTitle { get; set; }

    public ObservableCollection<FeatureViewModel> Features { get; set; } =
        new ObservableCollection<FeatureViewModel>();

    public FeatureViewModel SelectedFeature { get; set; }

    #endregion

    #region Commands

    public DelegateCommand CancelCommand { get; }
    public DelegateCommand SelectCommand { get; }

    #endregion

    #region Constructor

    public FeatureBrowserViewModel(FeaturesBrowserDialog featuresBrowserDialog)
    {
        _featuresBrowserDialog = featuresBrowserDialog;

        CancelCommand = new DelegateCommand(Cancel);
        SelectCommand = new DelegateCommand(Select, CanSelect);

        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(SelectedFilterTitle))
                Features = _filters.ContainsKey(SelectedFilterTitle)
                    ? new ObservableCollection<FeatureViewModel>(_filters[SelectedFilterTitle]
                        .Select(f => new FeatureViewModel(f)))
                    : new ObservableCollection<FeatureViewModel>(
                        _allCommandFeatures.Select(f => new FeatureViewModel(f)));

            if (e.PropertyName == nameof(SelectedFeature)) SelectCommand.RaiseCanExecuteChanged();
        };
    }

    #endregion

    #region Public Methods

    public IInterfaceFeature Load(IEnumerable<IInterfaceFeature> allCommandFeatures,
        Dictionary<string, IEnumerable<IInterfaceFeature>> filters)
    {
        _allCommandFeatures = allCommandFeatures;
        _filters = filters;

        FilterTitles.Add("Все");

        foreach (var key in filters.Keys) 
            FilterTitles.Add(key);

        SelectedFilterTitle = FilterTitles.Count > 1 ? FilterTitles[1] : FilterTitles[0];


        _featuresBrowserDialog.ShowDialog();

        return _isSelect ? SelectedFeature.CommandFeature : null;
    }

    #endregion

    #region Private Methods

    private bool CanSelect()
    {
        return SelectedFeature != null;
    }

    private void Cancel()
    {
        _featuresBrowserDialog.Close();
    }

    private void Select()
    {
        _isSelect = true;

        _featuresBrowserDialog.Close();
    }

    #endregion
}