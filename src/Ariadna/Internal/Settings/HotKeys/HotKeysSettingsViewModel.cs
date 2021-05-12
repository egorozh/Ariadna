using Ariadna.Core;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Ariadna.Settings.HotKeys
{
    internal class HotKeysSettingsViewModel : BaseViewModel
    {
        #region Private Fields

        private readonly AriadnaApp _akimApp;
        private readonly ISettings _settings;

        private readonly List<HotKeyItemViewModel> _allItems = new List<HotKeyItemViewModel>();

        #endregion

        #region Public Properties

        public string[] FilterTitles { get; set; } =
        {
            "Все",
            "С горячими клавишами",
            "Без горячих клавиш"
        };

        public string SelectedFilterTitle { get; set; }

        public ObservableCollection<HotKeyItemViewModel> Items { get; set; } =
            new ObservableCollection<HotKeyItemViewModel>();

        public HotKeyItemViewModel SelectedItem { get; set; }

        #endregion

        #region Constructor

        public HotKeysSettingsViewModel(AriadnaApp akimApp, ISettings settings)
        {
            _akimApp = akimApp;
            _settings = settings;

            PropertyChanged += IconSettingsViewModel_PropertyChanged;

            Update();
        }

        #endregion

        #region Public Methods

        public void Accept()
        {
            var currentFiltre = SelectedFilterTitle;
            SelectedFilterTitle = null;
            SelectedFilterTitle = currentFiltre;

            var hotKeys = new List<UiKeyBinding>();

            foreach (var viewModel in _allItems)
            {
                if (viewModel.Keys != null)
                {
                    hotKeys.Add(new UiKeyBinding
                    {
                        Id = viewModel.CommandFeature.Id,
                        Keys = viewModel.Keys.ToString()
                    });
                }
            }

            _akimApp.UiManager.SetNewHotKeys(hotKeys);
        }

        public void Cancel() => Update();

        public async void KeysChanged(HotKeyItemViewModel hotKeyItemViewModel, HotKey oldValue, HotKey newValue)
        {
            if (ContainsHotKey(hotKeyItemViewModel.Keys, hotKeyItemViewModel, out var vm))
            {
                var showDialogResult = await DialogCoordinator.Instance.ShowMessageAsync(this,
                    $"\"{hotKeyItemViewModel.Keys}\" уже используется в {vm.CommandFeature}",
                    "Переназначить команду?",
                    MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings
                    {
                        NegativeButtonText = "Отмена",
                        ColorScheme = MetroDialogColorScheme.Theme,
                        DialogTitleFontSize = 18,
                        DialogMessageFontSize = 16,
                    });

                if (showDialogResult == MessageDialogResult.Affirmative)
                {
                    foreach (var item in _allItems)
                    {
                        if (item != hotKeyItemViewModel && item.Keys?.ToString() == hotKeyItemViewModel.Keys.ToString())
                        {
                            item.SetKeys(null);
                            return;
                        }
                    }
                }
                else
                {
                    hotKeyItemViewModel.SetKeys(oldValue);
                    return;
                }
            }

            _settings.HasChanges = true;
        }

        public bool ContainsHotKey(HotKey? keys, HotKeyItemViewModel itemViewModel, out HotKeyItemViewModel vm)
        {
            if (keys == null)
            {
                vm = null;
                return false;
            }

            foreach (var item in _allItems)
            {
                if (item != itemViewModel && item.Keys?.ToString() == keys?.ToString())
                {
                    vm = item;
                    return true;
                }
            }

            vm = null;
            return false;
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
                    Items.AddRange(_allItems.Where(item => item.Keys != null));
                }
                else if (SelectedFilterTitle == FilterTitles[2])
                {
                    Items.AddRange(_allItems.Where(item => item.Keys == null));
                }

                SelectedItem = Items.FirstOrDefault();
            }
        }

        private void Update()
        {
            var jsonScheme = _akimApp.UiManager.JsonInterface;

            var allCommandFeatures = _akimApp.Features.Where(f => !(f is IToggleCommandFeature))
                .OfType<ICommandFeature>().ToList();

            _allItems.Clear();

            foreach (var commandFeature in allCommandFeatures)
            {
                var vm = new HotKeyItemViewModel(commandFeature, jsonScheme?.HotKeys, this);
                _allItems.Add(vm);
            }

            SelectedFilterTitle = null;
            SelectedFilterTitle = FilterTitles[0];

            SelectedItem = _allItems.FirstOrDefault();

            _settings.HasChanges = false;
        }

        #endregion
    }
}