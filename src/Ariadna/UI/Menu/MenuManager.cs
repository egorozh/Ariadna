using System;
using Ariadna.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Ariadna
{
    public class MenuManager : BaseViewModel, IMenuManager
    {
        #region Public Properties

        public ObservableCollection<MenuItem> MenuItems { get; set; } = new();

        #endregion

        #region Public Methods

        public void InitElements(List<UiMenuItem>? uiMenuItems, List<UiIcon> uiFullIcons,
            List<UiKeyBinding> hotKeys, IReadOnlyList<IFeature> akimFeatures, AriadnaApp ariadnaApp)
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

                    AddChildrenItems(rootMenuItem, item.Children,
                        uiFullIcons, akimFeatures, hotKeys, ariadnaApp);

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
            Tree(MenuItems, i => i.ClearBehaviors());
            MenuItems.Clear();
        }

        #endregion

        #region Private Methods

        private void Tree(IEnumerable<Control> items, Action<Control> action)
        {
            foreach (var item in items)
            {
                action.Invoke(item);

                if (item is MenuItem menuItem)
                {
                    if (menuItem.ItemsSource is IEnumerable<Control> children)
                        Tree(children, action);
                }
            }
        }

        private void AddChildrenItems(MenuItem parent,
            List<UiMenuItem> children, List<UiIcon> icons, IReadOnlyList<IFeature> akimFeatures,
            List<UiKeyBinding> hotKeys, AriadnaApp ariadnaApp)
        {
            var itemsSource = parent.ItemsSource as ObservableCollection<Control>;

            foreach (var uiMenuItem in children)
            {
                if (uiMenuItem.Id != null)
                {
                    var feature = akimFeatures.FirstOrDefault(f => f.Id == uiMenuItem.Id);

                    if (feature == null)
                        continue;

                    if (feature is ICommandFeature commandFeature)
                    {
                        var enabledIcon = ariadnaApp.InterfaceHelper.GetIcon(icons, commandFeature);

                        var commandMenuItem = CreateItem(commandFeature, enabledIcon, uiMenuItem.Header, hotKeys,
                            ariadnaApp);
                        itemsSource.Add(commandMenuItem);
                    }

                    if (feature is IComboboxFeature comboboxFeature)
                    {
                        parent.AddBehavior(new ComboBoxMenuItemBehavior(comboboxFeature, itemsSource.Count));
                    }
                }
                else if (uiMenuItem.Header == "$Separator")
                {
                    var separator = new Separator();

                    itemsSource.Add(separator);
                }
                else if (uiMenuItem.Children != null)
                {
                    var menuItem = new MenuItem
                    {
                        Header = uiMenuItem.Header,
                        ItemsSource = new ObservableCollection<Control>()
                    };

                    menuItem.AddBehavior(new HiddenMenuItemBehavior());

                    AddChildrenItems(menuItem, uiMenuItem.Children, icons,
                        akimFeatures, hotKeys, ariadnaApp);

                    itemsSource.Add(menuItem);
                }
            }
        }

        #endregion

        #region Factory

        private MenuItem CreateItem(ICommandFeature commandFeature, FrameworkElement enabledIcon, string header,
            List<UiKeyBinding> hotKeys,
            AriadnaApp ariadnaApp)
        {
            var kb = ariadnaApp.InterfaceHelper.CreateKeyBinding(hotKeys, commandFeature);

            var menuItem = new MenuItem
            {
                Header = header,
                Command = commandFeature.Command,
                Icon = enabledIcon,
                InputGestureText = InterfaceHelper.GetGesture(kb)
            };

            menuItem.AddBehavior(new InterfaceFeatureBehavior(commandFeature));

            if (commandFeature is IToggleCommandFeature toggleCommandFeature)
            {
                menuItem.IsCheckable = true;
                menuItem.AddBehavior(new ToggleFeatureBehavior(toggleCommandFeature));
            }

            return menuItem;
        }

        #endregion
    }
}