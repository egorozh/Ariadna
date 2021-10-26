using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using Ariadna.Core;
using Prism.Commands;

namespace Ariadna.Settings;

internal class MenuSettingsViewModel : BaseViewModel
{
    #region Private Fields

    private readonly DelegateCommand _moveUpCommand;
    private readonly DelegateCommand _moveDownCommand;
    private readonly DelegateCommand _deleteCommand;
    private readonly DelegateCommand _addContainerCommand;
    private readonly DelegateCommand _addSeparatorCommand;
    private readonly DelegateCommand _addCommandMenuCommand;
    
    private readonly ISettings _menuSettings;
    private readonly IUiManager _uiManager;
    private readonly IReadOnlyList<IFeature> _features;

    #endregion

    #region Public Properties

    public ObservableCollection<MenuItemViewModel> Items { get; set; } = new();

    public MenuItemViewModel? SelectedItem { get; set; }

    public ContextMenu ContextMenu { get; }
    public MenuItemViewModel? RootMenuItem { get; private set; }

    #endregion

    #region Constructor

    public MenuSettingsViewModel(MenuSettings menuSettings,
        IUiManager uiManager,
        IReadOnlyList<IFeature> features)
    {
        _menuSettings = menuSettings;
        _uiManager = uiManager;
        _features = features;

        _moveUpCommand = new DelegateCommand(MoveUpMenuItem, CanMoveUpMenuItem);
        _moveDownCommand = new DelegateCommand(MoveDownMenuItem, CanMoveDownMenuItem);
        _deleteCommand = new DelegateCommand(DeleteMenuItem, CanDeleteMenuItem);
        _addContainerCommand = new DelegateCommand(AddContainerMenuItem, CanAddContainerMenuItem);

        _addSeparatorCommand = new DelegateCommand(AddSeparatorMenuItem, CanAddSeparatorMenuItem);
        _addCommandMenuCommand = new DelegateCommand(AddCommandMenuItem, CanAddCommandMenuItem);

        ContextMenu = CreateContextMenu();

        PropertyChanged += MenuSettingsViewModel_PropertyChanged;

        Update();
    }

    #endregion

    #region Public Methods

    public void Accept()
    {
        var menus = new List<UiMenuItem>();

        foreach (var item in RootMenuItem.Children)
        {
            var uiItem = CreateItem(item);

            menus.Add(uiItem);
        }

        _uiManager.SetNewMenuItems(menus);
    }

    public void Cancel()
    {
        Update();
    }
        
    public void ItemChanged()
    {
        _menuSettings.HasChanges = true;
    }

    #endregion

    #region Private Methods

    private static UiMenuItem CreateItem(MenuItemViewModel item)
    {
        var header = item switch
        {
            SeparatorItemVm _ => "$Separator",
            _ => item.Header
        };

        var uiItem = new UiMenuItem {Header = header, Id = item.Feature?.Id};

        foreach (var menuItemViewModel in item.Children)
            uiItem.Children.Add(CreateItem(menuItemViewModel));

        return uiItem;
    }

    private void MenuSettingsViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectedItem))
            UpdateCommands();
    }

    private void UpdateCommands()
    {
        _moveUpCommand.RaiseCanExecuteChanged();
        _moveDownCommand.RaiseCanExecuteChanged();

        _deleteCommand.RaiseCanExecuteChanged();
        _addContainerCommand.RaiseCanExecuteChanged();

        _addSeparatorCommand.RaiseCanExecuteChanged();
        _addCommandMenuCommand.RaiseCanExecuteChanged();
    }

    private void Update()
    {
        var jsonScheme = _uiManager.JsonInterface;

        if (jsonScheme?.Menu == null)
            return;

        Items.Clear();

        RootMenuItem = new RootItemVm(new UiMenuItem {Header = "Меню"}, this, null, _features)
        {
            IsExpanded = true
        };

        RootMenuItem.Init();

        Items.Add(RootMenuItem);

        foreach (var uiMenuItem in jsonScheme.Menu)
        {
            var item = new MenuItemViewModel(uiMenuItem, this, RootMenuItem, _features);

            item.Init();

            RootMenuItem.Children.Add(item);
        }

        SelectedItem = RootMenuItem.Children.FirstOrDefault();

        _menuSettings.HasChanges = false;
    }

    private IInterfaceFeature SelectFeature()
    {
        var featureBrowser = new FeaturesBrowserDialog();

        var allCommandFeatures = _features.OfType<IInterfaceFeature>().ToList();

        var noRibbonFeature = allCommandFeatures
            .Where(f => !ContainsInMenu(Items, f))
            .ToList();

        var filters = new Dictionary<string, IEnumerable<IInterfaceFeature>>
        {
            {"Не задействованные в меню", noRibbonFeature}
        };

        var feature = featureBrowser.ShowDialog(allCommandFeatures, filters);
        return feature;
    }

    private static bool ContainsInMenu(ObservableCollection<MenuItemViewModel> items,
        IInterfaceFeature commandFeature)
        => TraverseTree(items, model => model.Feature == commandFeature);

    private static bool TraverseTree(ObservableCollection<MenuItemViewModel> items,
        Predicate<MenuItemViewModel> predicate)
    {
        foreach (var item in items)
        {
            if (predicate.Invoke(item))
                return true;

            if (TraverseTree(item.Children, predicate))
                return true;
        }

        return false;
    }

    private ContextMenu CreateContextMenu() => new()
        {
            ItemsSource = new ObservableCollection<Control>
            {
                new MenuItem
                {
                    Header = "Добавить родительский пункт меню",
                    Command = _addContainerCommand
                },
                new MenuItem
                {
                    Header = "Добавить пункт меню - команду",
                    Command = _addCommandMenuCommand
                },
                new Separator(),
                new MenuItem
                {
                    Header = "Добавить разделитель",
                    Command = _addSeparatorCommand
                },
                new Separator(),
                new MenuItem
                {
                    Header = "Удалить пункт меню",
                    Command = _deleteCommand
                },
                new Separator(),
                new MenuItem
                {
                    Header = "Переместить вверх",
                    Command = _moveUpCommand
                },
                new MenuItem
                {
                    Header = "Переместить вниз",
                    Command = _moveDownCommand
                },
            }
        };

    #region ContextMenu Command Methods

    private bool CanAddCommandMenuItem()
    {
        if (SelectedItem == null)
            return false;

        if (SelectedItem == RootMenuItem)
            return false;

        if (SelectedItem is SeparatorItemVm)
            return false;

        if (SelectedItem.Feature != null)
            return false;

        return true;
    }

    private void AddCommandMenuItem()
    {
        var feature = SelectFeature();

        if (feature == null)
            return;

        var newMenuItem = new MenuItemViewModel(new UiMenuItem
        {
            Header = feature.GetDefaultMenuProperties().Header ?? "Новый пункт меню",
            Id = feature.Id
        }, this, SelectedItem, _features);

        var items = SelectedItem.Children;

        newMenuItem.Init();

        items.Add(newMenuItem);

        newMenuItem.IsSelected = true;

        ItemChanged();

        UpdateCommands();
    }

    private bool CanAddSeparatorMenuItem()
    {
        if (SelectedItem == null)
            return false;

        if (SelectedItem == RootMenuItem)
            return false;

        if (SelectedItem is SeparatorItemVm)
            return false;

        if (SelectedItem.Feature != null)
            return false;

        return true;
    }

    private void AddSeparatorMenuItem()
    {
        var newMenuItem = new SeparatorItemVm(new UiMenuItem(), this, SelectedItem, _features);

        var items = SelectedItem.Children;
        newMenuItem.Init();
        items.Add(newMenuItem);

        newMenuItem.IsSelected = true;

        ItemChanged();
        UpdateCommands();
    }

    private bool CanAddContainerMenuItem()
    {
        if (SelectedItem == null)
            return false;

        if (SelectedItem is SeparatorItemVm)
            return false;

        if (SelectedItem.Feature != null)
            return false;

        return true;
    }

    private void AddContainerMenuItem()
    {
        var newMenuItem = new MenuItemViewModel(new UiMenuItem
        {
            Header = "Новый пункт меню"
        }, this, SelectedItem, _features);

        var items = SelectedItem.Children;
        newMenuItem.Init();
        items.Add(newMenuItem);

        newMenuItem.IsSelected = true;

        ItemChanged();
        UpdateCommands();
    }

    private bool CanDeleteMenuItem() => SelectedItem != null && SelectedItem?.Header != "Конструктор" &&
                                        SelectedItem != RootMenuItem;

    private void DeleteMenuItem()
    {
        var items = SelectedItem.Parent?.Children ?? Items;

        items.Remove(SelectedItem);

        ItemChanged();
        UpdateCommands();
    }

    private bool CanMoveDownMenuItem()
    {
        if (SelectedItem == null || SelectedItem == RootMenuItem)
            return false;

        var items = SelectedItem.Parent?.Children ?? Items;

        var oldIndex = items.IndexOf(SelectedItem);
        var newIndex = oldIndex + 1;

        return !IsIndexOutOfBounds(newIndex, items);
    }

    private void MoveDownMenuItem()
    {
        var items = SelectedItem.Parent?.Children ?? Items;

        var oldIndex = items.IndexOf(SelectedItem);
        var newIndex = oldIndex + 1;
        items.Move(oldIndex, newIndex);

        ItemChanged();
        UpdateCommands();
    }

    private bool CanMoveUpMenuItem()
    {
        if (SelectedItem == null || SelectedItem == RootMenuItem)
            return false;

        var items = SelectedItem.Parent?.Children ?? Items;

        var oldIndex = items.IndexOf(SelectedItem);
        var newIndex = oldIndex - 1;

        return !IsIndexOutOfBounds(newIndex, items);
    }

    private void MoveUpMenuItem()
    {
        var items = SelectedItem.Parent?.Children ?? Items;

        var oldIndex = items.IndexOf(SelectedItem);
        var newIndex = oldIndex - 1;
        items.Move(oldIndex, newIndex);

        ItemChanged();
        UpdateCommands();
    }

    private static bool IsIndexOutOfBounds(int index, ObservableCollection<MenuItemViewModel> items) =>
        index < 0 || index > items.Count - 1;

    #endregion

    #endregion
}