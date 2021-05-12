using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Ariadna.Core;
using Microsoft.Xaml.Behaviors.Core;

namespace Ariadna.Settings.Icons
{
    internal sealed class CommandItemViewModel : BaseViewModel
    {
        #region Private Fields

        private readonly IconsSettingsViewModel _mainVm;

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

        public CommandItemViewModel(ICommandFeature commandFeature, List<UiIcon> icons, IconsSettingsViewModel mainVm)
        {
            _mainVm = mainVm;
            SetDefaultIconCommand = new ActionCommand(SetDefaultIcon);
            SelectOvverideIconCommand = new ActionCommand(SelectOvverideIcon);

            CommandFeature = commandFeature;
            DefaultIcon = commandFeature.CreateDefaultIcon();
            DefaultIconSmall = commandFeature.CreateDefaultIcon();
            Header = commandFeature.ToString();

            OverridedIconPath = icons?.FirstOrDefault(icon => icon.Id == commandFeature.Id)?.Path;

            DrawOverridedIcon();
        }

        #endregion

        #region Command Methods

        private void SelectOvverideIcon()
        {
            var iconsStorage = new IconsStorageDialog(CommandFeature.AriadnaApp);

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
                var basePath = CommandFeature.AriadnaApp.GetRootDirectory();

                var iconDirectory = basePath.CreateSubdirectory("Icons");

                var filePath = Path.Combine(iconDirectory.FullName, _overridedIconPath);

                if (filePath.ToUpper().EndsWith(".SVG"))
                {
                    if (File.Exists(filePath))
                        OverrideIcon = CommandFeature.AriadnaApp.SvgHelper.CreateImageFromSvg(filePath);

                    return;
                }

                if (File.Exists(filePath))
                    OverrideIcon = new Image()
                    {
                        Source = new BitmapImage(new Uri(filePath))
                    };
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
}