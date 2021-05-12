using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Ariadna.Core;
using Fluent;
using Button = Fluent.Button;
using MenuItem = Fluent.MenuItem;
using ToggleButton = Fluent.ToggleButton;

namespace Ariadna
{
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
            List<UiHelpVideo> helpVideos,
            AriadnaApp akimApp)
        {
            var index = SelectedTabIndex;

            Clear();

            var features = akimApp.Features.OfType<IInterfaceFeature>();

            foreach (var uiTabRibbon in tabs)
                CreateRibbonItem(uiTabRibbon, icons, hotKeys, helpVideos, akimApp, features);

            SelectedTabIndex = index;
        }

        public string GetTabName(IFeature akimFeature)
        {
            foreach (var ribbonTabItem in _tabs)
            {
                if (ribbonTabItem is IRibbonTabItem tab)
                {
                    if (tab.Contains(akimFeature))
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
            _tabs.Clear();
        }

        #endregion

        #region Private Methods

        private void CreateRibbonItem(UiTabRibbon ribbonTabItem, List<UiIcon> icons,
            List<UiKeyBinding> hotKeys, List<UiHelpVideo> helpVideos, AriadnaApp akimApp,
            IEnumerable<IInterfaceFeature> features)
        {
            foreach (var ribbonGroup in ribbonTabItem.Boxes)
            {
                foreach (var ribbonItem in ribbonGroup.Items)
                {
                    var feature = features.FirstOrDefault(f => f.Id == ribbonItem.Id);

                    if (feature == null)
                        continue;

                    var control = feature switch
                    {
                        ICommandFeature commandFeature => CreateButton(ribbonItem.Size, ribbonItem.Header,
                            commandFeature, hotKeys, akimApp.InterfaceHelper, ribbonItem, icons, helpVideos,
                            ribbonItem.SplitButtonItem, akimApp),
                        IComboboxFeature comboboxFeature => CreateComboBox(comboboxFeature, akimApp.InterfaceHelper,
                            ribbonItem, helpVideos),
                        _ => null
                    };

                    if (control == null)
                        continue;

                    control.AddBehavior(new InterfaceFeatureBehavior(feature));

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

        #region Factory

        private static Control CreateComboBox(IComboboxFeature feature, IInterfaceHelper interfaceHelper,
            UiQuickActionItem item, List<UiHelpVideo> videos)
        {
            var ribbonComboBox = new RibbonComboBox
            {
                ToolTip = interfaceHelper.CreateToolTip(feature, videos, null, item.Header, item.Description,
                    item.DisableReason),
                TextBlock = {Text = item.Header},
            };

            ribbonComboBox.ComboBox.AddBehavior(new ComboBoxFeatureBehavior(feature));

            return ribbonComboBox;
        }

        private static Control CreateButton(RibbonItemSize size, string header, ICommandFeature feature,
            List<UiKeyBinding> hotKeys, IInterfaceHelper interfaceHelper, UiQuickActionItem item, List<UiIcon> icons,
            List<UiHelpVideo> videos, UiSplitButtonItem? splitButtonItem, AriadnaApp ariadnaApp)
        {
            if (splitButtonItem != null)
            {
                return CreateSplitButton(size, header, feature, hotKeys, interfaceHelper, item, icons, videos,
                    splitButtonItem, ariadnaApp);
            }

            var icon = interfaceHelper.GetIcon(icons, feature);

            ButtonBase button = feature switch
            {
                IToggleCommandFeature _ => new ToggleButton(),
                _ => new Button(),
            };

            var kb = interfaceHelper.CreateKeyBinding(hotKeys, feature);

            button.Command = feature.Command;
            button.Focusable = false;
            button.Margin = new Thickness(5, 2, 5, 2);
            button.ToolTip = interfaceHelper.CreateToolTip(feature, videos, hotKeys, item.Header, item.Description,
                item.DisableReason);

            if (button is IRibbonControl ribbonControl)
            {
                ribbonControl.SizeDefinition = InterfaceHelper.ConvertDefinitions(size);
                ribbonControl.Header = header;
                ribbonControl.Icon = icon;
            }

            if (button is ILargeIconProvider largeIconProvider)
                largeIconProvider.LargeIcon = icon;


            if (feature is IToggleCommandFeature toggleCommandFeature)
                button.AddBehavior(new ToggleFeatureBehavior(toggleCommandFeature));

            if (feature is ITwoStateCommandFeature twoStateCommandFeature)
            {
                var ribbonPropAlt = twoStateCommandFeature.AlternativeRibbonProperties;

                if (ribbonPropAlt != null)
                {
                    button.AddBehavior(new TwoStateFeatureBehavior(twoStateCommandFeature, header, ribbonPropAlt.Name,
                        icon, twoStateCommandFeature.CreateAlternativeIcon(), kb, item));
                }
            }


            return button;
        }

        private static Control CreateSplitButton(RibbonItemSize size, string header, ICommandFeature feature,
            List<UiKeyBinding> hotKeys, IInterfaceHelper interfaceHelper, UiQuickActionItem item, List<UiIcon> icons,
            List<UiHelpVideo> videos, UiSplitButtonItem splitButtonItem, AriadnaApp ariadnaApp)
        {
            var icon = interfaceHelper.GetIcon(icons, feature);
            var kb = interfaceHelper.CreateKeyBinding(hotKeys, feature);


            SplitButton splitButton = new()
            {
                SizeDefinition = InterfaceHelper.ConvertDefinitions(size),
                Header = splitButtonItem.Header,
                Icon = icon,
                LargeIcon = icon,
                Command = feature.Command,
                Focusable = false,
                Margin = new Thickness(5, 2, 5, 2),
                ToolTip = interfaceHelper.CreateToolTip(feature, videos, hotKeys, item.Header, item.Description,
                    item.DisableReason),
            };


            if (feature is IToggleCommandFeature toggleCommandFeature)
                splitButton.AddBehavior(new ToggleFeatureBehavior(toggleCommandFeature));

            if (feature is ITwoStateCommandFeature twoStateCommandFeature)
            {
                var ribbonPropAlt = twoStateCommandFeature.AlternativeRibbonProperties;

                if (ribbonPropAlt != null)
                {
                    splitButton.AddBehavior(new TwoStateFeatureBehavior(twoStateCommandFeature, header,
                        ribbonPropAlt.Name,
                        icon, twoStateCommandFeature.CreateAlternativeIcon(), kb, item));
                }
            }

            splitButton.ItemsSource = CreateSplitButtonItems(splitButtonItem.Items, hotKeys, ariadnaApp, icons);


            return splitButton;
        }

        private static ObservableCollection<MenuItem> CreateSplitButtonItems(List<UiRibbonItem> splitButtonItems,
            List<UiKeyBinding> hotKeys, AriadnaApp ariadnaApp, List<UiIcon> icons)
        {
            ObservableCollection<MenuItem> items = new();

            foreach (var item in splitButtonItems)
            {
                var feature = ariadnaApp.Features.FirstOrDefault(f => f.Id == item.Id);

                if (feature is not ICommandFeature commandFeature)
                    continue;

                var icon = ariadnaApp.InterfaceHelper.GetIcon(icons, commandFeature);

                items.Add(CreateItem(commandFeature, icon, item.Header, hotKeys, ariadnaApp));
            }

            return items;
        }

        private static MenuItem CreateItem(ICommandFeature commandFeature, FrameworkElement enabledIcon, string header,
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

        #endregion
    }
}