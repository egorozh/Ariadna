using Ariadna.Core;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Controls;

namespace Ariadna.Settings.Ribbon
{
    internal class GroupViewModel : BaseViewModel
    {
        #region Private Methods

        private readonly RibbonSettingsViewModel _mainVm;
        private DelegateCommand _moveUpButtonCommand;
        private DelegateCommand _moveDownButtonCommand;

        #endregion

        #region Public Properties

        public ObservableCollection<RibbonItemViewModel> Items { get; set; } =
            new ObservableCollection<RibbonItemViewModel>();

        public RibbonItemViewModel SelectedItem { get; set; }

        public ContextMenu ButtonsContextMenu { get; set; }

        public string Header { get; set; }

        #endregion

        #region Events

        public event EventHandler ItemChanged;

        #endregion

        #region Constructor

        public GroupViewModel(UiRibbonGroup uiRibbonGroup, RibbonSettingsViewModel mainVm, AriadnaApp akimApp)
        {
            _mainVm = mainVm;
            Header = uiRibbonGroup.Header;

            foreach (var ribbonItem in uiRibbonGroup.Items)
            {
                var buttonVm = RibbonItemViewModel.CreateItem(ribbonItem, akimApp);

                if (buttonVm == null)
                    continue;

                buttonVm.PropertyChanged += ButtonVm_PropertyChanged;
                Items.Add(buttonVm);
            }

            SelectedItem = Items.FirstOrDefault();

            CreateButtonsContextMenu();

            Items.CollectionChanged += Buttons_CollectionChanged;

            PropertyChanged += GroupViewModel_PropertyChanged;
        }

        private void CreateButtonsContextMenu()
        {
            _moveUpButtonCommand = new DelegateCommand(MoveUpButton, CanMoveUpButton);

            _moveDownButtonCommand = new DelegateCommand(MoveDownButton, CanMoveDownButton);

            ButtonsContextMenu = new ContextMenu
            {
                ItemsSource = new ObservableCollection<Control>
                {
                    new MenuItem
                    {
                        Header = "Добавить кнопку",
                        Command = new DelegateCommand(AddButton)
                    },
                    new MenuItem
                    {
                        Header = "Удалить кнопку",
                        Command = new DelegateCommand(DeleteButton)
                    },
                    new Separator(),
                    new MenuItem
                    {
                        Header = "Переместить вверх",
                        Command = _moveUpButtonCommand
                    },
                    new MenuItem
                    {
                        Header = "Переместить вниз",
                        Command = _moveDownButtonCommand
                    },
                }
            };
        }

        #endregion

        #region Private Methods

        #region ContextMenu Command Methods

        private void AddButton()
        {
            var feature = _mainVm.SelectFeature();

            if (feature == null)
                return;

            var newButton = RibbonItemViewModel.CreateItem(new UiRibbonItem
            {
                Header = feature.DefaultRibbonProperties?.Name ?? "Новая кнопка",
                Description = feature.DefaultRibbonProperties?.Description ?? "Новое описание",
                DisableReason = feature.DefaultRibbonProperties?.DisableReason ?? "Не описана",
                Size = feature.DefaultRibbonProperties?.Size ?? RibbonItemSize.Large,

                Id = feature.Id
            }, feature.AriadnaApp);

            var index = Items.IndexOf(SelectedItem) + 1;

            Items.Insert(index, newButton);
            SelectedItem = newButton;
        }

        private void DeleteButton()
        {
            var selectedButton = SelectedItem;

            Items.Remove(selectedButton);
            SelectedItem = Items.FirstOrDefault();
        }

        private bool CanMoveUpButton()
        {
            var oldIndex = Items.IndexOf(SelectedItem);
            var newIndex = oldIndex - 1;

            return !IsIndexOutOfBounds(newIndex, Items);
        }

        private bool CanMoveDownButton()
        {
            var oldIndex = Items.IndexOf(SelectedItem);
            var newIndex = oldIndex + 1;

            return !IsIndexOutOfBounds(newIndex, Items);
        }

        private static bool IsIndexOutOfBounds(int index, ObservableCollection<RibbonItemViewModel> items) =>
            index < 0 || index > items.Count - 1;

        private void MoveDownButton()
        {
            var oldIndex = Items.IndexOf(SelectedItem);
            var newIndex = oldIndex + 1;
            Items.Move(oldIndex, newIndex);
        }

        private void MoveUpButton()
        {
            var oldIndex = Items.IndexOf(SelectedItem);
            var newIndex = oldIndex - 1;
            Items.Move(oldIndex, newIndex);
        }

        #endregion

        private void Buttons_CollectionChanged(object sender,
            NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var newButton = e.NewItems[0] as RibbonItemViewModel;
                    newButton.PropertyChanged += ButtonVm_PropertyChanged;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    var oldButton = e.OldItems[0] as RibbonItemViewModel;
                    oldButton.PropertyChanged -= ButtonVm_PropertyChanged;
                    break;
                case NotifyCollectionChangedAction.Reset:

                    foreach (RibbonItemViewModel button in e.OldItems)
                        button.PropertyChanged -= ButtonVm_PropertyChanged;

                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:

                    _moveUpButtonCommand.RaiseCanExecuteChanged();
                    _moveDownButtonCommand.RaiseCanExecuteChanged();

                    break;
            }

            ItemChanged?.Invoke(sender, e);
        }

        private void ButtonVm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ItemChanged?.Invoke(sender, e);
        }

        private void GroupViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedItem))
            {
                _moveUpButtonCommand.RaiseCanExecuteChanged();
                _moveDownButtonCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion
    }
}