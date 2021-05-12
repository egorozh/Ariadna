using Ariadna.Core;
using System;
using System.Linq;
using System.Windows;

namespace Ariadna.Settings.Ribbon
{
    internal class ButtonViewModel : RibbonItemViewModel
    {
        #region Public Properties

        public RibbonItemSize Size { get; set; }

        public string[] Sizes { get; } =
        {
            "Large",
            "Middle",
            "Small"
        };

        public string SelectedSize { get; set; }

        public FrameworkElement Icon { get; }

        public FrameworkElement IconLarge { get; }

        #endregion

        #region Constructor

        public ButtonViewModel(UiRibbonItem ribbonItem, IFeature feature) : base(ribbonItem, feature)
        {
            Size = ribbonItem.Size;

            if (Feature is ICommandFeature commandFeature)
            {
                Icon = commandFeature.CreateDefaultIcon();
                IconLarge = commandFeature.CreateDefaultIcon();
            }

            SelectedSize = Sizes[(int) Size];

            PropertyChanged += ButtonViewModel_PropertyChanged;
        }

        #endregion

        #region Private Methods

        private void ButtonViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedSize))
            {
                Size = (RibbonItemSize) Array.IndexOf(Sizes, SelectedSize);
            }
        }

        #endregion
    }

    internal class ComboBoxViewModel : RibbonItemViewModel
    {
        public ComboBoxViewModel(UiRibbonItem ribbonItem, IFeature feature) : base(ribbonItem, feature)
        {
        }
    }

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

        public static RibbonItemViewModel CreateItem(UiRibbonItem ribbonItem, AriadnaApp akimApp)
        {
            var feature = akimApp.Features.FirstOrDefault(f => f.Id == ribbonItem.Id);

            if (feature is ICommandFeature commandFeature)
                return new ButtonViewModel(ribbonItem, feature);

            if (feature is IComboboxFeature comboboxFeature)
                return new ComboBoxViewModel(ribbonItem, feature);

            return null;
        }
    }
}