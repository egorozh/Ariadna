using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Controls;
using Ariadna.Core;
using Prism.Commands;

namespace Ariadna.Settings;

internal class RibbonSettingsViewModel : BaseViewModel
{
    #region Private Fields

    private readonly ISettings _settings;
    private readonly IUiManager _uiManager;
    private readonly IReadOnlyList<IFeature> _features;
    private DelegateCommand _moveUpTabCommand;
    private DelegateCommand _moveDownTabCommand;
    private DelegateCommand _renameTabCommand;
    private DelegateCommand _deleteTabCommand;

    #endregion

    #region Public Properties

    public ObservableCollection<TabViewModel> Tabs { get; set; } = new();

    public TabViewModel SelectedTab { get; set; }

    public ContextMenu TabsContextMenu { get; set; }

    #endregion

    #region Constructor

    public RibbonSettingsViewModel(RibbonSettings settings,
        IUiManager uiManager, IReadOnlyList<IFeature> features)
    {
        _settings = settings;
        _uiManager = uiManager;
        _features = features;

        CreateTabsContextMenu();

        PropertyChanged += RibbonSettingsViewModel_PropertyChanged;

        Tabs.CollectionChanged += Tabs_CollectionChanged;

        Update();
    }

    #endregion

    #region Public Methods

    public void Accept()
    {
        var tabs = new List<UiTabRibbon>();

        foreach (var tab in Tabs)
        {
            var boxes = new List<UiRibbonGroup>();

            foreach (var @group in tab.Groups)
            {
                var items = new List<UiRibbonItem>();

                foreach (var button in @group.Items)
                {
                    var uiItem = new UiRibbonItem
                    {
                        Description = button.Description,
                        Header = button.Header,
                        DisableReason = button.DisableReason,
                        Id = button.Feature?.Id,
                    };

                    if (button is ButtonViewModel buttonViewModel)
                        uiItem.Size = buttonViewModel.Size;

                    if (button is SplitButtonViewModel splitButtonViewModel)
                    {
                        List<UiRibbonItem> splitItems = new();

                        foreach (var b in splitButtonViewModel.Buttons)
                        {
                            var it = new UiRibbonItem
                            {
                                Description = b.Description,
                                Header = b.Header,
                                DisableReason = b.DisableReason,
                                Id = b.Feature?.Id,
                                Size = b.Size
                            };

                            splitItems.Add(it);
                        }

                        uiItem.SplitButtonItem = new UiSplitButtonItem()
                        {
                            Header = splitButtonViewModel.SplitHeader,
                            Items = splitItems
                        };
                    }

                    items.Add(uiItem);
                }

                var uiBox = new UiRibbonGroup
                {
                    Header = @group.Header,
                    Items = items
                };

                boxes.Add(uiBox);
            }

            var uiTab = new UiTabRibbon
            {
                Header = tab.Header,
                Boxes = boxes,
            };

            tabs.Add(uiTab);
        }

        _uiManager.SetNewRibbonItems(tabs);
    }

    public void Cancel() => Update();

    public IInterfaceFeature SelectFeature()
    {
        var featureBrowser = new FeaturesBrowserDialog();

        var allCommandFeatures = _features.OfType<IInterfaceFeature>().ToList();

        var noRibbonFeature = allCommandFeatures
            .Where(f => !ContainsInRibbon(f))
            .ToList();

        var filters = new Dictionary<string, IEnumerable<IInterfaceFeature>>
        {
            {"Не задействованные в ленте", noRibbonFeature}
        };

        var feature = featureBrowser.ShowDialog(allCommandFeatures, filters);
        return feature;
    }

    #endregion

    #region Private Methods

    private void RibbonSettingsViewModel_PropertyChanged(object sender,
        System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectedTab))
        {
            _moveUpTabCommand.RaiseCanExecuteChanged();
            _moveDownTabCommand.RaiseCanExecuteChanged();

            _renameTabCommand.RaiseCanExecuteChanged();
            _deleteTabCommand.RaiseCanExecuteChanged();
        }
    }
        
    private void Update()
    {
        var jsonScheme = _uiManager.JsonInterface;

        if (jsonScheme?.Ribbon == null)
            return;

        Tabs.Clear();

        foreach (var tabRibbon in jsonScheme.Ribbon)
        {
            var tabVm = new TabViewModel(tabRibbon, this, _features);

            Tabs.Add(tabVm);
        }

        SelectedTab = Tabs.FirstOrDefault();

        _settings.HasChanges = false;
    }

    private void TabVm_ItemChanged(object? sender, EventArgs e)
    {
        _settings.HasChanges = true;
    }

    private bool ContainsInRibbon(IInterfaceFeature feature)
    {
        foreach (var tab in Tabs)
        {
            foreach (var @group in tab.Groups)
            {
                foreach (var button in @group.Items)
                {
                    if (button.Feature == feature)
                        return true;
                }
            }
        }

        return false;
    }

    private void CreateTabsContextMenu()
    {
        TabsContextMenu = new ContextMenu();

        _moveUpTabCommand = new DelegateCommand(MoveUpTab, CanMoveUpTab);
        _moveDownTabCommand = new DelegateCommand(MoveDownTab, CanMoveDownTab);

        _renameTabCommand = new DelegateCommand(RenameTab, CanRenameTab);
        _deleteTabCommand = new DelegateCommand(DeleteTab, CanDeleteTab);

        TabsContextMenu = new ContextMenu
        {
            ItemsSource = new ObservableCollection<Control>
            {
                new MenuItem
                {
                    Header = "Переименовать",
                    Command = _renameTabCommand
                },
                new Separator(),
                new MenuItem
                {
                    Header = "Добавить вкладку",
                    Command = new DelegateCommand(AddTab)
                },
                new MenuItem
                {
                    Header = "Удалить вкладку",
                    Command = _deleteTabCommand
                },
                new Separator(),
                new MenuItem
                {
                    Header = "Переместить вверх",
                    Command = _moveUpTabCommand
                },
                new MenuItem
                {
                    Header = "Переместить вниз",
                    Command = _moveDownTabCommand
                },
            }
        };
    }

    #region ContextMenu Command Methods

    private void AddTab()
    {
        var tbDialog = new TextBoxDialog
        {
            Title = "Создать вкладку",
            Text = "Новая вкладка"
        };

        tbDialog.SelectAll();

        var res = tbDialog.ShowDialog();

        if (res == false)
            return;

        var newTab = new TabViewModel(new UiTabRibbon
        {
            Header = tbDialog.Text
        }, this, _features);

        var index = Tabs.IndexOf(SelectedTab) + 1;

        Tabs.Insert(index, newTab);

        SelectedTab = newTab;
    }

    private bool CanDeleteTab() => SelectedTab != null && SelectedTab?.Header != "Конструктор";

    private void DeleteTab()
    {
        Tabs.Remove(SelectedTab);
        SelectedTab = Tabs.FirstOrDefault();
    }

    private bool CanMoveUpTab()
    {
        var oldIndex = Tabs.IndexOf(SelectedTab);
        var newIndex = oldIndex - 1;

        return !IsIndexOutOfBounds(newIndex, Tabs);
    }

    private bool CanMoveDownTab()
    {
        var oldIndex = Tabs.IndexOf(SelectedTab);
        var newIndex = oldIndex + 1;

        return !IsIndexOutOfBounds(newIndex, Tabs);
    }

    private static bool IsIndexOutOfBounds(int index, ObservableCollection<TabViewModel> Tabs) =>
        index < 0 || index > Tabs.Count - 1;

    private void MoveDownTab()
    {
        var oldIndex = Tabs.IndexOf(SelectedTab);
        var newIndex = oldIndex + 1;
        Tabs.Move(oldIndex, newIndex);
    }

    private void MoveUpTab()
    {
        var oldIndex = Tabs.IndexOf(SelectedTab);
        var newIndex = oldIndex - 1;
        Tabs.Move(oldIndex, newIndex);
    }

    private bool CanRenameTab() => SelectedTab != null && SelectedTab?.Header != "Конструктор";

    private void RenameTab()
    {
        var tbDialog = new TextBoxDialog
        {
            Title = "Переименовать вкладку",
            Text = SelectedTab.Header
        };

        tbDialog.SelectAll();

        var res = tbDialog.ShowDialog();

        if (res == false)
            return;

        SelectedTab.Header = tbDialog.Text;
        _settings.HasChanges = true;
    }

    #endregion

    private void Tabs_CollectionChanged(object sender,
        NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                var newTab = e.NewItems[0] as TabViewModel;
                newTab.ItemChanged += TabVm_ItemChanged;
                break;
            case NotifyCollectionChangedAction.Remove:
                var oldTab = e.OldItems[0] as TabViewModel;
                oldTab.ItemChanged -= TabVm_ItemChanged;
                break;
            case NotifyCollectionChangedAction.Reset:

                if (e.OldItems != null)
                {
                    foreach (TabViewModel button in e.OldItems)
                        button.ItemChanged -= TabVm_ItemChanged;
                }

                break;
            case NotifyCollectionChangedAction.Replace:
                break;
            case NotifyCollectionChangedAction.Move:

                _moveUpTabCommand.RaiseCanExecuteChanged();
                _moveDownTabCommand.RaiseCanExecuteChanged();

                _renameTabCommand.RaiseCanExecuteChanged();
                _deleteTabCommand.RaiseCanExecuteChanged();

                break;
        }

        _settings.HasChanges = true;
    }

    #endregion
}