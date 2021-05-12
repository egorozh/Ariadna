using Ariadna.Core;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Controls;

namespace Ariadna.Settings.Ribbon
{
    internal class TabViewModel : BaseViewModel
    {
        #region Private Methods

        private readonly RibbonSettingsViewModel _mainVm;
        private readonly AriadnaApp _akimApp;
        private DelegateCommand _moveUpGroupCommand;
        private DelegateCommand _moveDownGroupCommand;

        #endregion

        #region Public Properties

        public string Header { get; set; }

        public ObservableCollection<GroupViewModel> Groups { get; set; } = new ObservableCollection<GroupViewModel>();

        public GroupViewModel SelectedGroup { get; set; }

        public ContextMenu GroupsContextMenu { get; set; }

        #endregion

        #region Events

        public event EventHandler ItemChanged;

        #endregion

        #region Constructor

        public TabViewModel(UiTabRibbon tabRibbon, RibbonSettingsViewModel mainVm, AriadnaApp akimApp)
        {
            _mainVm = mainVm;
            _akimApp = akimApp;
            Header = tabRibbon.Header;

            foreach (var uiRibbonGroup in tabRibbon.Boxes)
            {
                var groupVm = new GroupViewModel(uiRibbonGroup, mainVm, akimApp);

                groupVm.ItemChanged += GroupVm_ItemChanged;

                Groups.Add(groupVm);
            }

            SelectedGroup = Groups.FirstOrDefault();

            CreateGroupsContextMenu();

            Groups.CollectionChanged += Groups_CollectionChanged;
            PropertyChanged += TabViewModel_PropertyChanged;
        }

        #endregion

        #region Private Methods

        private void CreateGroupsContextMenu()
        {
            GroupsContextMenu = new ContextMenu();

            _moveUpGroupCommand = new DelegateCommand(MoveUpGroup, CanMoveUpGroup);

            _moveDownGroupCommand = new DelegateCommand(MoveDownGroup, CanMoveDownGroup);

            GroupsContextMenu = new ContextMenu
            {
                ItemsSource = new ObservableCollection<Control>
                {
                    new MenuItem
                    {
                        Header = "Переименовать",
                        Command = new DelegateCommand(RenameGroup)
                    },
                    new Separator(),
                    new MenuItem
                    {
                        Header = "Добавить группу",
                        Command = new DelegateCommand(AddGroup)
                    },
                    new MenuItem
                    {
                        Header = "Удалить группу",
                        Command = new DelegateCommand(DeleteGroup)
                    },
                    new Separator(),
                    new MenuItem
                    {
                        Header = "Переместить вверх",
                        Command = _moveUpGroupCommand
                    },
                    new MenuItem
                    {
                        Header = "Переместить вниз",
                        Command = _moveDownGroupCommand
                    },
                }
            };
        }

        #region ContextMenu Command Methods

        private void AddGroup()
        {
            var tbDialog = new TextBoxDialog
            {
                Title = "Создать группу",
                Text = "Новая группа"
            };

            tbDialog.SelectAll();

            var res = tbDialog.ShowDialog();

            if (res == false)
                return;

            var newGroup = new GroupViewModel(new UiRibbonGroup
            {
                Header = tbDialog.Text
            }, _mainVm, _akimApp);

            var index = Groups.IndexOf(SelectedGroup) + 1;

            Groups.Insert(index, newGroup);

            SelectedGroup = newGroup;
        }

        private void DeleteGroup()
        {
            Groups.Remove(SelectedGroup);
            SelectedGroup = Groups.FirstOrDefault();
        }

        private bool CanMoveUpGroup()
        {
            var oldIndex = Groups.IndexOf(SelectedGroup);
            var newIndex = oldIndex - 1;

            return !IsIndexOutOfBounds(newIndex, Groups);
        }

        private bool CanMoveDownGroup()
        {
            var oldIndex = Groups.IndexOf(SelectedGroup);
            var newIndex = oldIndex + 1;

            return !IsIndexOutOfBounds(newIndex, Groups);
        }

        private static bool IsIndexOutOfBounds(int index, ObservableCollection<GroupViewModel> groups) =>
            index < 0 || index > groups.Count - 1;

        private void MoveDownGroup()
        {
            var oldIndex = Groups.IndexOf(SelectedGroup);
            var newIndex = oldIndex + 1;
            Groups.Move(oldIndex, newIndex);
        }

        private void MoveUpGroup()
        {
            var oldIndex = Groups.IndexOf(SelectedGroup);
            var newIndex = oldIndex - 1;
            Groups.Move(oldIndex, newIndex);
        }

        private void RenameGroup()
        {
            if (SelectedGroup == null)
                return;

            var tbDialog = new TextBoxDialog
            {
                Title = "Переименовать группу",
                Text = SelectedGroup.Header
            };

            tbDialog.SelectAll();

            var res = tbDialog.ShowDialog();

            if (res == false)
                return;

            SelectedGroup.Header = tbDialog.Text;
        }

        #endregion

        private void Groups_CollectionChanged(object sender,
            NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var newGroup = e.NewItems[0] as GroupViewModel;
                    newGroup.ItemChanged += GroupVm_ItemChanged;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    var oldGroup = e.OldItems[0] as GroupViewModel;
                    oldGroup.ItemChanged -= GroupVm_ItemChanged;
                    break;
                case NotifyCollectionChangedAction.Reset:

                    foreach (GroupViewModel button in e.OldItems)
                        button.ItemChanged -= GroupVm_ItemChanged;

                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:

                    _moveUpGroupCommand.RaiseCanExecuteChanged();
                    _moveDownGroupCommand.RaiseCanExecuteChanged();

                    break;
            }

            ItemChanged?.Invoke(sender, e);
        }

        private void TabViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedGroup))
            {
                _moveUpGroupCommand.RaiseCanExecuteChanged();
                _moveDownGroupCommand.RaiseCanExecuteChanged();
            }
        }

        private void GroupVm_ItemChanged(object sender, EventArgs e)
        {
            ItemChanged?.Invoke(sender, e);
        }

        #endregion
    }
}