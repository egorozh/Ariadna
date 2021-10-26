using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Controls;
using Ariadna.Core;
using Prism.Commands;

namespace Ariadna.Settings;

internal class QuickActionsGroupViewModel : BaseViewModel
{
    #region Private Methods

    private readonly QuickActionsSettingsViewModel _mainVm;
    private DelegateCommand _moveUpButtonCommand;
    private DelegateCommand _moveDownButtonCommand;

    #endregion

    #region Public Properties

    public ObservableCollection<QuickItemViewModel> Buttons { get; set; } =
        new ObservableCollection<QuickItemViewModel>();

    public QuickItemViewModel SelectedButton { get; set; }

    public ContextMenu ButtonsContextMenu { get; set; }

    public string Header { get; set; }

    #endregion

    #region Events

    public event EventHandler ItemChanged;

    #endregion

    #region Constructor

    public QuickActionsGroupViewModel(
        UiQuickActionsGroup uiQuickActionsGroup,
        QuickActionsSettingsViewModel mainVm,
        IReadOnlyList<IFeature> features)
    {
        _mainVm = mainVm;

        Header = uiQuickActionsGroup.Header;

        foreach (var ribbonItem in uiQuickActionsGroup.Items)
        {
            var feature = features.FirstOrDefault(f => f.Id == ribbonItem.Id);

            if (feature != null)
            {
                var buttonVm = QuickItemViewModel.CreateItem(ribbonItem, feature);

                if (buttonVm == null)
                    continue;

                buttonVm.PropertyChanged += ButtonVm_PropertyChanged;
                Buttons.Add(buttonVm);
            }
        }

        SelectedButton = Buttons.FirstOrDefault();

        CreateButtonsContextMenu();

        Buttons.CollectionChanged += Buttons_CollectionChanged;

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

        var rb = feature.GetDefaultRibbonProperties();
        
        var newButton = QuickItemViewModel.CreateItem(new UiQuickActionItem
        {
            Header = rb.Header ?? "Новая кнопка",
            Description = rb.Description ?? "Новое описание",
            DisableReason = rb.DisableReason ?? "Не описана",

            Id = feature.Id
        }, feature);

        var index = Buttons.IndexOf(SelectedButton) + 1;

        Buttons.Insert(index, newButton);
        SelectedButton = newButton;
    }

    private void DeleteButton()
    {
        var selectedButton = SelectedButton;

        Buttons.Remove(selectedButton);
        SelectedButton = Buttons.FirstOrDefault();
    }

    private bool CanMoveUpButton()
    {
        var oldIndex = Buttons.IndexOf(SelectedButton);
        var newIndex = oldIndex - 1;

        return !IsIndexOutOfBounds(newIndex, Buttons);
    }

    private bool CanMoveDownButton()
    {
        var oldIndex = Buttons.IndexOf(SelectedButton);
        var newIndex = oldIndex + 1;

        return !IsIndexOutOfBounds(newIndex, Buttons);
    }

    private static bool IsIndexOutOfBounds(int index, ObservableCollection<QuickItemViewModel> buttons) =>
        index < 0 || index > buttons.Count - 1;

    private void MoveDownButton()
    {
        var oldIndex = Buttons.IndexOf(SelectedButton);
        var newIndex = oldIndex + 1;
        Buttons.Move(oldIndex, newIndex);
    }

    private void MoveUpButton()
    {
        var oldIndex = Buttons.IndexOf(SelectedButton);
        var newIndex = oldIndex - 1;
        Buttons.Move(oldIndex, newIndex);
    }

    #endregion

    private void Buttons_CollectionChanged(object sender,
        NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                var newButton = e.NewItems[0] as QuickItemViewModel;
                newButton.PropertyChanged += ButtonVm_PropertyChanged;
                break;
            case NotifyCollectionChangedAction.Remove:
                var oldButton = e.OldItems[0] as QuickItemViewModel;
                oldButton.PropertyChanged -= ButtonVm_PropertyChanged;
                break;
            case NotifyCollectionChangedAction.Reset:

                foreach (QuickItemViewModel button in e.OldItems)
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
        if (e.PropertyName == nameof(SelectedButton))
        {
            _moveUpButtonCommand.RaiseCanExecuteChanged();
            _moveDownButtonCommand.RaiseCanExecuteChanged();
        }
    }

    #endregion
}