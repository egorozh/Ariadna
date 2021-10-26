using Ariadna.Core;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Ariadna;

internal class QuickActionsManager : BaseViewModel, IQuickActionsManager
{
    #region Public Properties

    public ObservableCollection<ObservableCollection<Control>> TopToolBar { get; } =
        new();

    public ObservableCollection<ObservableCollection<Control>> LeftToolBar { get; } =
        new();

    public ObservableCollection<ObservableCollection<Control>> RightToolBar { get; } =
        new();

    public bool IsShowLeft { get; set; }
    public bool IsShowTop { get; set; }
    public bool IsShowRight { get; set; }

    #endregion

    #region Events

    public event EventHandler<IsShowChangedEventArgs>? IsShowChanged;

    #endregion

    #region Constructor

    public QuickActionsManager()
    {
        PropertyChanged += QuickActionsManager_PropertyChanged;
    }

    #endregion

    #region Public Methods

    public void Clear()
    {
        ClearToolBar(TopToolBar);
        ClearToolBar(LeftToolBar);
        ClearToolBar(RightToolBar);
    }

    public void InitElements(UiQuickActions? quickActions, List<UiIcon> icons, List<UiKeyBinding> hotKeys,
        List<UiHelpVideo> helpVideos, 
        IInterfaceHelper interfaceHelper,
        IReadOnlyList<IFeature> features)
    {
        Clear();

        if (quickActions == null)
            return;

        InitPanel(quickActions.Left, icons, hotKeys, helpVideos, LeftToolBar, interfaceHelper, features);
        InitPanel(quickActions.Top, icons, hotKeys, helpVideos, TopToolBar, interfaceHelper, features);
        InitPanel(quickActions.Right, icons, hotKeys, helpVideos, RightToolBar, interfaceHelper, features);

        IsShowLeft = quickActions.IsShowLeft;
        IsShowTop = quickActions.IsShowTop;
        IsShowRight = quickActions.IsShowRight;
    }

    private void QuickActionsManager_PropertyChanged(object? sender,
        System.ComponentModel.PropertyChangedEventArgs e)
    {
        var propertyName = e.PropertyName;

        switch (propertyName)
        {
            case nameof(IsShowLeft):
            case nameof(IsShowTop):
            case nameof(IsShowRight):
                IsShowChanged?.Invoke(this, new IsShowChangedEventArgs(propertyName));
                break;
        }
    }

    #endregion

    #region Private Methods

    private static void ClearToolBar(ObservableCollection<ObservableCollection<Control>> toolbar)
    {
        foreach (var group in toolbar)
        foreach (var control in group)
            control.ClearBehaviors();

        toolbar.Clear();
    }

    private void InitPanel(List<UiQuickActionsGroup> groups, List<UiIcon> icons, List<UiKeyBinding> hotKeys,
        List<UiHelpVideo> helpVideos, ObservableCollection<ObservableCollection<Control>> toolBar,
        IInterfaceHelper interfaceHelper,
        IReadOnlyList<IFeature> features)
    {
        foreach (var actionsGroup in groups)
        {
            var group = GetItems(actionsGroup, icons, hotKeys, helpVideos,
                interfaceHelper, features);

            toolBar.Add(group);
        }
    }

    private static ObservableCollection<Control> GetItems(
        UiQuickActionsGroup actionsGroup,
        List<UiIcon> icons,
        List<UiKeyBinding> hotKeys,
        List<UiHelpVideo> helpVideos,
        IInterfaceHelper interfaceHelper,
        IReadOnlyList<IFeature> features)
    {
        var items = new ObservableCollection<Control>();

        foreach (var item in actionsGroup.Items)
        {
            var feature = features.FirstOrDefault(f => f.Id == item.Id);

            if (feature is not IInterfaceFeature interfaceFeature)
                continue;

            var control = ItemFactory.CreateItem(features, item, interfaceFeature, hotKeys,
                icons, helpVideos, interfaceHelper, false);

            if (control == null)
                continue;

            items.Add(control);
        }

        return items;
    }

    #endregion
}