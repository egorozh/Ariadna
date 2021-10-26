using System.Collections.ObjectModel;
using System.ComponentModel;
using Ariadna.Core;

namespace Ariadna.Settings;

internal class IconsSettingsViewModel : BaseViewModel
{
    #region Private Fields

    private readonly IUiManager _uiManager;
    private readonly IReadOnlyList<IFeature> _features;
    private readonly IStorage _storage;
    private readonly IInterfaceHelper _interfaceHelper;
    private readonly IImageHelpers _imageHelpers;
    private readonly IconsSettings _iconsSettings;

    private readonly List<CommandItemViewModel> _allItems = new();

    #endregion

    #region Public Properties

    public string[] FilterTitles { get; set; } = new[]
    {
        "Все",
        "С переопределенным значком",
        "С непереопределенным значком"
    };

    public string SelectedFilterTitle { get; set; }

    public ObservableCollection<CommandItemViewModel> Items { get; set; } =
        new ObservableCollection<CommandItemViewModel>();

    public CommandItemViewModel SelectedItem { get; set; }

    #endregion

    #region Constructor

    public IconsSettingsViewModel(IUiManager uiManager,
        IReadOnlyList<IFeature> features,
        IStorage storage,
        IInterfaceHelper interfaceHelper,
        IImageHelpers imageHelpers,
        IconsSettings iconsSettings)
    {
        _uiManager = uiManager;
        _features = features;
        _storage = storage;
        _interfaceHelper = interfaceHelper;
        _imageHelpers = imageHelpers;
        _iconsSettings = iconsSettings;

        PropertyChanged += IconSettingsViewModel_PropertyChanged;

        Update();
    }

    #endregion

    #region Public Methods

    public void DefaultIconSetted(CommandItemViewModel commandItemViewModel)
    {
        if (SelectedFilterTitle == FilterTitles[0])
        {
        }
        else if (SelectedFilterTitle == FilterTitles[1])
        {
            Items.Remove(commandItemViewModel);
            SelectedItem = Items.FirstOrDefault();
        }
        else if (SelectedFilterTitle == FilterTitles[2])
        {
            Items.Add(commandItemViewModel);
            SelectedItem = Items.FirstOrDefault();
        }

        _iconsSettings.HasChanges = true;
    }

    public void OverrideIconSelected(CommandItemViewModel commandItemViewModel)
    {
        _iconsSettings.HasChanges = true;
    }

    public void Accept()
    {
        var icons = new List<UiIcon>();

        foreach (var viewModel in _allItems)
        {
            if (viewModel.OverridedIconPath != null)
            {
                icons.Add(new UiIcon
                {
                    Id = viewModel.CommandFeature.Id,
                    Path = viewModel.OverridedIconPath
                });
            }
        }

        _uiManager.SetNewIcons(icons);
    }

    public void Cancel()
    {
        Update();
    }

    #endregion

    #region Private Methods

    private void IconSettingsViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectedFilterTitle))
        {
            Items.Clear();

            if (SelectedFilterTitle == FilterTitles[0])
            {
                foreach (var item in _allItems)
                    Items.Add(item);
            }
            else if (SelectedFilterTitle == FilterTitles[1])
            {
                foreach (var item in _allItems.Where(item => item.OverrideIcon != null))
                    Items.Add(item);
            }
            else if (SelectedFilterTitle == FilterTitles[2])
            {
                foreach (var item in _allItems.Where(item => item.OverrideIcon == null))
                    Items.Add(item);
            }

            SelectedItem = Items.FirstOrDefault();
        }
    }

    private void Update()
    {
        var jsonScheme = _uiManager.JsonInterface;

        var allCommandFeatures = _features.OfType<ICommandFeature>().ToList();

        _allItems.Clear();

        foreach (var commandFeature in allCommandFeatures)
        {
            var vm = new CommandItemViewModel(commandFeature, jsonScheme?.Icons, this, _storage, _interfaceHelper,
                _imageHelpers);
            _allItems.Add(vm);
        }

        SelectedFilterTitle = null;
        SelectedFilterTitle = FilterTitles[0];

        SelectedItem = _allItems.FirstOrDefault();

        _iconsSettings.HasChanges = false;
    }

    #endregion
}