using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Ariadna.Core;

namespace Ariadna.Settings.Icons
{
    internal class IconsSettingsViewModel : BaseViewModel
    {
        #region Private Fields

        public readonly AriadnaApp _akimApp;
        private readonly IconsSettings _iconsSettings;

        private readonly List<CommandItemViewModel> _allItems = new List<CommandItemViewModel>();

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

        public IconsSettingsViewModel(AriadnaApp akimApp, IconsSettings iconsSettings)
        {
            _akimApp = akimApp;
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

            _akimApp.UiManager.SetNewIcons(icons);
        }

        public void Cancel()
        {
            Update();
        }

        #endregion

        #region Private Methods

        private void IconSettingsViewModel_PropertyChanged(object sender,
            PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedFilterTitle))
            {
                Items.Clear();

                if (SelectedFilterTitle == FilterTitles[0])
                {
                    Items.AddRange(_allItems);
                }
                else if (SelectedFilterTitle == FilterTitles[1])
                {
                    Items.AddRange(_allItems.Where(item => item.OverrideIcon != null));
                }
                else if (SelectedFilterTitle == FilterTitles[2])
                {
                    Items.AddRange(_allItems.Where(item => item.OverrideIcon == null));
                }

                SelectedItem = Items.FirstOrDefault();
            }
        }

        private void Update()
        {
            var jsonScheme = _akimApp.UiManager.JsonInterface;

            var allCommandFeatures = _akimApp.Features.OfType<ICommandFeature>().ToList();

            _allItems.Clear();

            foreach (var commandFeature in allCommandFeatures)
            {
                var vm = new CommandItemViewModel(commandFeature, jsonScheme?.Icons, this);
                _allItems.Add(vm);
            }

            SelectedFilterTitle = null;
            SelectedFilterTitle = FilterTitles[0];

            SelectedItem = _allItems.FirstOrDefault();

            _iconsSettings.HasChanges = false;
        }

        #endregion
    }
}