using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Ariadna.Core;
using Microsoft.Xaml.Behaviors.Core;

namespace Ariadna.Settings;

internal sealed class CommandItemViewModel : BaseViewModel
{
    #region Private Fields

    private readonly IconsSettingsViewModel _mainVm;
    private readonly IStorage _storage;
    private readonly IInterfaceHelper _interfaceHelper;
    private readonly IImageHelpers _imageHelpers;

    #endregion

    #region Private Methods

    private string _overridedIconPath;

    #endregion

    #region Public Properties

    public FrameworkElement DefaultIconSmall { get; set; }

    public FrameworkElement DefaultIcon { get; set; }

    public FrameworkElement OverrideIcon { get; set; }

    public string Header { get; set; }

    public ICommandFeature CommandFeature { get; }

    public string OverridedIconPath
    {
        get => _overridedIconPath;
        set => SetOverridedIconPath(value);
    }

    #endregion

    #region Commands

    public ActionCommand SetDefaultIconCommand { get; }
    public ActionCommand SelectOvverideIconCommand { get; }

    #endregion

    #region Constructor

    public CommandItemViewModel(ICommandFeature commandFeature,
        List<UiIcon> icons,
        IconsSettingsViewModel mainVm,
        IStorage storage,
        IInterfaceHelper interfaceHelper,
        IImageHelpers imageHelpers)
    {
        _mainVm = mainVm;
        _storage = storage;
        _interfaceHelper = interfaceHelper;
        _imageHelpers = imageHelpers;
        SetDefaultIconCommand = new ActionCommand(SetDefaultIcon);
        SelectOvverideIconCommand = new ActionCommand(SelectOvverideIcon);

        CommandFeature = commandFeature;
        DefaultIcon = commandFeature.GetDefaultIcon();
        DefaultIconSmall = commandFeature.GetDefaultIcon();
        Header = commandFeature.ToString();

        OverridedIconPath = icons?.FirstOrDefault(icon => icon.Id == commandFeature.Id)?.Path;

        DrawOverridedIcon();
    }

    #endregion

    #region Command Methods

    private void SelectOvverideIcon()
    {
        var iconsStorage = new IconsStorageDialog(_interfaceHelper, _storage, _imageHelpers);

        var path = iconsStorage.GetIconPath();

        if (path != null)
        {
            OverridedIconPath = path;
            _mainVm.OverrideIconSelected(this);
        }
    }

    private void SetDefaultIcon()
    {
        OverridedIconPath = null;
        _mainVm.DefaultIconSetted(this);
    }

    #endregion

    #region Private Methods

    private void DrawOverridedIcon()
    {
        if (_overridedIconPath != null)
        {
            var iconDirectory = _storage.IconsDirectory;

            var filePath = Path.Combine(iconDirectory, _overridedIconPath);

            if (File.Exists(filePath))
            {
                var source = _imageHelpers.CreateImageSource(filePath);

                OverrideIcon = new Image()
                {
                    Source = source
                };
            }
        }
        else
        {
            OverrideIcon = null;
        }
    }

    private void SetOverridedIconPath(string value)
    {
        _overridedIconPath = value;

        DrawOverridedIcon();
    }

    #endregion
}