using Ariadna.Core;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Ariadna;

public class RibbonManager : BaseViewModel, IRibbonManager
{
    #region Private Fields

    private ObservableCollection<Fluent.RibbonTabItem> _tabs;

    #endregion

    #region Public Properties

    public event EventHandler? Loaded;

    public bool Visible { get; set; } = true;

    public int SelectedTabIndex { get; set; }

    #endregion

    #region Public Methods

    public void Init(ObservableCollection<Fluent.RibbonTabItem> tabs)
    {
        _tabs = tabs;
        Loaded?.Invoke(this, EventArgs.Empty);
    }

    public void InitElements(List<UiTabRibbon> tabs, List<UiIcon> icons, List<UiKeyBinding> hotKeys,
        List<UiHelpVideo> helpVideos, IReadOnlyList<IFeature> features, IInterfaceHelper interfaceHelper)
    {
        var index = SelectedTabIndex;

        Clear();
        
        foreach (var uiTabRibbon in tabs)
            CreateRibbonItem(uiTabRibbon, icons, hotKeys, helpVideos, features, interfaceHelper);

        SelectedTabIndex = index;
    }

    public string GetTabName(IFeature feature)
    {
        foreach (var ribbonTabItem in _tabs)
        {
            if (ribbonTabItem is IRibbonTabItem tab)
            {
                if (tab.Contains(feature))
                    return tab.Header as string;
            }
        }

        return null;
    }

    public void SelectTabItem(string tabHeader)
    {
        if (tabHeader == null)
        {
            SelectedTabIndex = 0;
            return;
        }

        var index = _tabs.IndexOf(_tabs.First(tabItem => (string) tabItem.Header == tabHeader));
        SelectedTabIndex = index;
    }

    public void BlockAllTabItems(string[] tabHeaders = null)
    {
        foreach (var ribbonsTabItem in _tabs)
        {
            if (ribbonsTabItem is IRibbonTabItem tab)
            {
                if (tabHeaders == null)
                    tab.Block();
                else
                    foreach (var tabHeader in tabHeaders)
                        if (tabHeader != (string) ribbonsTabItem.Header)
                            tab.Block();
            }
        }
    }

    public void UnBlockAllTabItems(string[] tabHeaders = null)
    {
        foreach (var ribbonsTabItem in _tabs)
            if (ribbonsTabItem is IRibbonTabItem tab)
            {
                if (tabHeaders == null)
                    tab.UnBlock();
                else
                    foreach (var tabHeader in tabHeaders)
                        if (tabHeader != (string) ribbonsTabItem.Header)
                            tab.UnBlock();
            }
    }

    public void Clear()
    {
        foreach (var tabItem in _tabs)
        foreach (var groupBox in tabItem.Groups)
        foreach (var item in groupBox.ItemsSource)
        {
            if (item is DependencyObject f)
                f.ClearBehaviors();
        }

        _tabs.Clear();
    }

    #endregion

    #region Private Methods

    private void CreateRibbonItem(UiTabRibbon ribbonTabItem, List<UiIcon> icons,
        List<UiKeyBinding> hotKeys, List<UiHelpVideo> helpVideos,
        IReadOnlyList<IFeature> features, IInterfaceHelper interfaceHelper)
    {
        foreach (var ribbonGroup in ribbonTabItem.Boxes)
        {
            foreach (var ribbonItem in ribbonGroup.Items)
            {
                var feature = features.FirstOrDefault(f => f.Id == ribbonItem.Id);

                if (feature is not IInterfaceFeature interfaceFeature)
                    continue;

                var control = ItemFactory.CreateItem(features, ribbonItem, interfaceFeature, hotKeys,
                    icons, helpVideos, interfaceHelper, true, ribbonItem.Size);

                if (control == null)
                    continue;

                AddRibbonItem(control, ribbonTabItem.Header, ribbonGroup.Header);
            }
        }
    }

    private void AddRibbonItem(Control control, string tabHeader, string boxHeader)
    {
        if (control == null)
            return;

        var rootTabItem = GetTabItem(tabHeader);

        if (rootTabItem == null)
        {
            rootTabItem = new RibbonTabItem()
            {
                Header = tabHeader,
            };
            _tabs.Add((Fluent.RibbonTabItem) rootTabItem);

            rootTabItem.AddRibbonItem(control, boxHeader);
        }
        else
        {
            rootTabItem.AddRibbonItem(control, boxHeader);
        }
    }

    private IRibbonTabItem GetTabItem(string tabHeader)
    {
        foreach (var tab in _tabs)
        {
            if (tab.Header as string == tabHeader)
                return (IRibbonTabItem) tab;
        }

        return null;
    }

    #endregion
}