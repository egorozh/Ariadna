using Ariadna.Core;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Ariadna;

public class MenuManager : BaseViewModel, IMenuManager
{
    #region Public Properties

    public ObservableCollection<MenuItem> MenuItems { get; set; } = new();

    #endregion

    #region Public Methods

    public void InitElements(List<UiMenuItem>? uiMenuItems, List<UiIcon> uiFullIcons,
        List<UiKeyBinding> hotKeys, IReadOnlyList<IFeature> features,
        IInterfaceHelper interfaceHelper)
    {
        if (uiMenuItems != null)
        {
            Clear();

            // Root menu items:
            foreach (var item in uiMenuItems)
            {
                var rootMenuItem = new MenuItem
                {
                    Header = item.Header,
                    ItemsSource = new ObservableCollection<Control>()
                };

                rootMenuItem.AddBehavior(new HiddenMenuItemBehavior());

                MenuItemFactory.AddChildrenItems(rootMenuItem, item.Children,
                    uiFullIcons, features, hotKeys, interfaceHelper);

                MenuItems.Add(rootMenuItem);
            }
        }
    }

    public void BlockAllRootMenuItems(string[] rootMenuHeaders = null)
    {
        foreach (var menuItem in MenuItems)
            if (rootMenuHeaders == null)
                menuItem.IsEnabled = false;
            else
                foreach (var tabHeader in rootMenuHeaders)
                    if (tabHeader != (string) menuItem.Header)
                        menuItem.IsEnabled = false;
    }

    public void UnBlockAllRootMenuItems(string[] rootMenuHeaders = null)
    {
        foreach (var menuItem in MenuItems)
            if (rootMenuHeaders == null)
                menuItem.IsEnabled = true;
            else
                foreach (var tabHeader in rootMenuHeaders)
                    if (tabHeader != (string) menuItem.Header)
                        menuItem.IsEnabled = true;
    }

    public void Clear()
    {
        ContextMenuManager.Tree(MenuItems, i => i.ClearBehaviors());
        MenuItems.Clear();
    }

    #endregion

    #region Private Methods

       
        
    #endregion
}