using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Controls;
using Ariadna.Core;
using Prism.Commands;

namespace Ariadna.Settings;

internal class QuickActionsPanelViewModel : BaseViewModel
{
    #region Private Methods

    private readonly QuickActionsSettingsViewModel _mainVm;
    private readonly IReadOnlyList<IFeature> _features;
    private DelegateCommand _moveUpGroupCommand;
    private DelegateCommand _moveDownGroupCommand;

    #endregion

    #region Public Properties

    public string Header { get; set; }

    public ObservableCollection<QuickActionsGroupViewModel> Groups { get; set; } =
        new ObservableCollection<QuickActionsGroupViewModel>();

    public QuickActionsGroupViewModel SelectedGroup { get; set; }

    public ContextMenu GroupsContextMenu { get; set; }

    #endregion

    #region Events

    public event EventHandler ItemChanged;

    #endregion

    #region Constructor

    public QuickActionsPanelViewModel(List<UiQuickActionsGroup> groups, QuickActionsSettingsViewModel mainVm,
        string header, IReadOnlyList<IFeature> features)
    {
        _mainVm = mainVm;
        _features = features;

        Header = header;

        foreach (var uiQuickActionsGroup in groups)
        {
            var groupVm = new QuickActionsGroupViewModel(uiQuickActionsGroup, mainVm, features);

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

        var newGroup = new QuickActionsGroupViewModel(new UiQuickActionsGroup
        {
            Header = tbDialog.Text
        }, _mainVm, _features);

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

    private static bool IsIndexOutOfBounds(int index, ObservableCollection<QuickActionsGroupViewModel> groups) =>
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
                var newGroup = e.NewItems[0] as QuickActionsGroupViewModel;
                newGroup.ItemChanged += GroupVm_ItemChanged;
                break;
            case NotifyCollectionChangedAction.Remove:
                var oldGroup = e.OldItems[0] as QuickActionsGroupViewModel;
                oldGroup.ItemChanged -= GroupVm_ItemChanged;
                break;
            case NotifyCollectionChangedAction.Reset:

                foreach (QuickActionsGroupViewModel button in e.OldItems)
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