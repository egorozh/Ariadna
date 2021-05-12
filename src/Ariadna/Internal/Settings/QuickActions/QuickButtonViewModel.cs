﻿using System.Linq;
using System.Windows;
using Ariadna.Core;

namespace Ariadna.Settings.QuickActions
{
    internal class QuickButtonViewModel : QuickItemViewModel
    {
        #region Public Properties

        public FrameworkElement Icon { get; }

        public FrameworkElement IconLarge { get; }

        #endregion

        #region Constructor

        public QuickButtonViewModel(UiQuickActionItem quickActionItem, IFeature akimFeature) : base(quickActionItem,
            akimFeature)
        {
            if (Feature is ICommandFeature commandFeature)
            {
                Icon = commandFeature.CreateDefaultIcon();
                IconLarge = commandFeature.CreateDefaultIcon();
            }
        }

        #endregion
    }

    internal class QuickItemViewModel : BaseViewModel
    {
        public string Header { get; set; }

        public string Description { get; set; }

        public string DisableReason { get; set; }

        public IFeature Feature { get; set; }

        public string FeatureName => Feature?.ToString();

        public QuickItemViewModel(UiQuickActionItem quickActionItem, IFeature akimFeature)
        {
            Header = quickActionItem.Header;
            Description = quickActionItem.Description;
            DisableReason = quickActionItem.DisableReason;

            Feature = akimFeature;
        }

        public static QuickItemViewModel CreateItem(UiQuickActionItem ribbonItem, AriadnaApp akimApp)
        {
            var feature = akimApp.Features.FirstOrDefault(f => f.Id == ribbonItem.Id);

            if (feature is ICommandFeature commandFeature)
                return new QuickButtonViewModel(ribbonItem, feature);

            if (feature is IComboboxFeature comboboxFeature)
                return new QuickComboBoxViewModel(ribbonItem, feature);

            return null;
        }
    }

    internal class QuickComboBoxViewModel : QuickItemViewModel
    {
        public QuickComboBoxViewModel(UiQuickActionItem quickActionItem, IFeature akimFeature) : base(
            quickActionItem, akimFeature)
        {
        }
    }
}