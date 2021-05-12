using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Fluent;

namespace Ariadna
{
    public class InterfaceHelper : IInterfaceHelper
    {
        #region Private Fields

        private readonly AriadnaApp _ariadnaApp;

        #endregion

        #region Constructor

        public InterfaceHelper(AriadnaApp ariadnaApp)
        {
            _ariadnaApp = ariadnaApp;
        }

        #endregion

        #region Public Methods

        public object CreateToolTip(IInterfaceFeature feature, List<UiHelpVideo> videos,
            List<UiKeyBinding> hotKeys, string header, string description, string disableReason)
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

        public TipPopupViewModel CreateTipViewModel(IInterfaceFeature feature, List<UiHelpVideo> videos,
            List<UiKeyBinding> hotKeys, string header,
            string description, string disableReason)
        {
            var kb = feature is ICommandFeature commandFeature ? CreateKeyBinding(hotKeys, commandFeature) : null;

            var tipVm = new TipPopupViewModel
            {
                Header = $"{header} {GetGesture(kb, true)}",
                Description = description,
                DisableReason = disableReason,
            };

            var helpVideoPath = videos?.FirstOrDefault(fullIcon => fullIcon.Id == feature.Id)?.Path;

            if (helpVideoPath != null)
            {
                var basePath = _ariadnaApp.GetRootDirectory();

                var iconDirectory = basePath.CreateSubdirectory("Videos");

                var filePath = Path.Combine(iconDirectory.FullName, helpVideoPath);

                if (File.Exists(filePath))
                    tipVm.HelpVideo = new Uri(filePath);
            }

            return tipVm;
        }

        public static RibbonControlSizeDefinition ConvertDefinitions(RibbonItemSize size)
        {
            switch (size)
            {
                case RibbonItemSize.Large:
                    return new RibbonControlSizeDefinition("Large");
                case RibbonItemSize.Middle:
                    return new RibbonControlSizeDefinition("Middle");
                case RibbonItemSize.Small:
                    return new RibbonControlSizeDefinition("Small");
                default:
                    return new RibbonControlSizeDefinition("Middle");
            }
        }

        public static string GetGesture(KeyBinding keyBinding, bool isAddBrackets = false)
        {
            if (keyBinding == null)
                return string.Empty;

            var key = keyBinding.Key;
            var mod = keyBinding.Modifiers;

            var keyGesture = new KeyGesture(key, mod);

            var strokeKeyGesture = keyGesture.GetDisplayStringForCulture(CultureInfo.InvariantCulture);

            return isAddBrackets ? $"({strokeKeyGesture})" : strokeKeyGesture;
        }

        public KeyBinding CreateKeyBinding(List<UiKeyBinding> hotKeys, ICommandFeature commandFeature)
        {
            var id = commandFeature.Id;

            var item = hotKeys?.FirstOrDefault(keyBinding => keyBinding.Id == id);

            if (item != null)
            {
                var converter = new KeyGestureConverter();

                try
                {
                    var kg = (KeyGesture) converter.ConvertFromInvariantString(item.Keys);

                    return new KeyBinding(commandFeature.Command, kg);
                }
                catch (Exception e)
                {
                    _ariadnaApp.Logger.Error(e.Message);
                }
            }
            else if (commandFeature.KeyBinding != null)
                return commandFeature.KeyBinding;

            return null;
        }

        #region Icons

        public FrameworkElement GetIcon(List<UiIcon> icons, ICommandFeature commandFeature)
        {
            var icon = commandFeature.CreateDefaultIcon();

            var userIconPath = icons?.FirstOrDefault(fullIcon => fullIcon.Id == commandFeature.Id)?.Path;

            return GetIcon(userIconPath, icon);
        }

        public FrameworkElement GetIcon(string relativeIconPath, FrameworkElement defaultIcon)
        {
            if (relativeIconPath != null)
            {
                var basePath = _ariadnaApp.GetRootDirectory();

                var iconDirectory = basePath.CreateSubdirectory("Icons");

                var filePath = Path.Combine(iconDirectory.FullName, relativeIconPath);

                if (File.Exists(filePath))
                    return GetIcon(filePath);
            }

            return defaultIcon;
        }

        public FrameworkElement GetIcon(string iconFilePath)
        {
            var filePath = iconFilePath;

            if (iconFilePath.ToUpper().EndsWith(".SVG"))
                return _ariadnaApp.SvgHelper.CreateImageFromSvg(filePath);

            return new Image
            {
                Source = new BitmapImage(new Uri(filePath)),
                Stretch = Stretch.Uniform
            };
        }

        public string GetIconPath()
        {
            //var dialog = new IconsStorageDialog(_ariadnaApp);

            //return dialog.GetIconPath();
            return string.Empty;
        }

        #endregion

        #endregion
    }
}