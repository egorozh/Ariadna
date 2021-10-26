using Fluent;
using Serilog;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ariadna;

public class InterfaceHelper : IInterfaceHelper
{
    #region Private Fields

    private readonly AriadnaApp _ariadnaApp;
    private readonly ILogger _logger;
    private readonly ISvgHelper _svgHelper;
    private readonly IStorage _storage;

    #endregion

    #region Constructor

    public InterfaceHelper(AriadnaApp ariadnaApp, ILogger logger,
        ISvgHelper svgHelper, IStorage storage)
    {
        _ariadnaApp = ariadnaApp;
        _logger = logger;
        _svgHelper = svgHelper;
        _storage = storage;
    }

    #endregion

    #region Public Methods

    public object CreateToolTip(IInterfaceFeature feature, List<UiHelpVideo>? videos,
        List<UiKeyBinding>? hotKeys, string? header, string? description, string? disableReason)
    {
        var kb = feature is ICommandFeature commandFeature ? CreateKeyBinding(hotKeys, commandFeature) : null;

        var screenTip = new ScreenTip
        {
            Title = $"{header} {GetGesture(kb, true)}",
            Text = description,
            DisableReason = disableReason,
            HelpTopic = new Grid()
            //Width = 190,
        };

        return screenTip;
    }

    public static RibbonControlSizeDefinition ConvertDefinitions(RibbonItemSize size) => size switch
    {
        RibbonItemSize.Large => new RibbonControlSizeDefinition("Large"),
        RibbonItemSize.Middle => new RibbonControlSizeDefinition("Middle"),
        RibbonItemSize.Small => new RibbonControlSizeDefinition("Small"),
        _ => new RibbonControlSizeDefinition("Middle")
    };

    public static string GetGesture(KeyBinding? keyBinding, bool isAddBrackets = false)
    {
        if (keyBinding == null)
            return string.Empty;

        var key = keyBinding.Key;
        var mod = keyBinding.Modifiers;

        var keyGesture = new KeyGesture(key, mod);

        var strokeKeyGesture = keyGesture.GetDisplayStringForCulture(CultureInfo.InvariantCulture);

        return isAddBrackets ? $"({strokeKeyGesture})" : strokeKeyGesture;
    }


    public KeyBinding? CreateKeyBinding(List<UiKeyBinding>? hotKeys, ICommandFeature commandFeature)
    {
        var id = commandFeature.Id;

        var item = hotKeys?.FirstOrDefault(keyBinding => keyBinding.Id == id);

        if (item != null)
        {
            var converter = new KeyGestureConverter();

            try
            {
                var kg = (KeyGesture) converter.ConvertFromInvariantString(item.Keys);

                return new KeyBinding(commandFeature, kg);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
        }
        else
        {
            var kb = commandFeature.GetDefaultKeyBinding();

            if (kb != null)
                return kb;
        }

        return null;
    }

    #region Icons

    public FrameworkElement GetIcon(List<UiIcon> icons, ICommandFeature commandFeature)
    {
        var icon = commandFeature.GetDefaultIcon();

        var userIconPath = icons?.FirstOrDefault(fullIcon => fullIcon.Id == commandFeature.Id)?.Path;

        return GetIcon(userIconPath, icon);
    }

    public (FrameworkElement, FrameworkElement) GetIcon(List<UiIcon> icons,
        ITwoStateCommandFeature commandFeature)
    {
        var icon = commandFeature.GetDefaultIcon();
        var icon2 = commandFeature.CreateAlternativeIcon();

        var uiIcon = icons?.FirstOrDefault(fullIcon => fullIcon.Id == commandFeature.Id);

        if (uiIcon == null)
            return (icon, icon2);

        var path1 = uiIcon.Path;
        var path2 = uiIcon.AltPath;

        return (GetIcon(path1, icon), GetIcon(path2, icon));
    }

    public FrameworkElement GetIcon(string? relativeIconPath, FrameworkElement defaultIcon)
    {
        if (!string.IsNullOrWhiteSpace(relativeIconPath))
        {
            var iconDirectory = _storage.IconsDirectory;

            var filePath = Path.Combine(iconDirectory, relativeIconPath);

            if (File.Exists(filePath))
                return GetIcon(filePath);
        }

        return defaultIcon;
    }

    public FrameworkElement GetIcon(string iconFilePath)
    {
        if (iconFilePath.ToUpper().EndsWith(".SVG"))
            return _svgHelper.CreateImageFromSvg(iconFilePath);

        return new Image
        {
            Source = new BitmapImage(new Uri(iconFilePath)),
            Stretch = Stretch.Uniform
        };
    }

    #endregion

    #endregion
}