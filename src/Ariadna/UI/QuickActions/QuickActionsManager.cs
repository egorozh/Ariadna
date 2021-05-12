using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Ariadna.Core;
using Button = System.Windows.Controls.Button;
using ComboBox = System.Windows.Controls.ComboBox;
using ToggleButton = System.Windows.Controls.Primitives.ToggleButton;

namespace Ariadna
{
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
            List<UiHelpVideo> helpVideos, AriadnaApp ariadnaApp)
        {
            Clear();

            if (quickActions == null)
                return;

            InitPanel(quickActions.Left, icons, hotKeys, helpVideos, LeftToolBar, quickActions.Size, ariadnaApp);
            InitPanel(quickActions.Top, icons, hotKeys, helpVideos, TopToolBar, quickActions.Size, ariadnaApp, true);
            InitPanel(quickActions.Right, icons, hotKeys, helpVideos, RightToolBar, quickActions.Size, ariadnaApp);

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
            RibbonItemSize size, AriadnaApp ariadnaApp, bool includeComboBoxes = false)
        {
            foreach (var actionsGroup in groups)
            {
                var group = GetItems(actionsGroup, icons, hotKeys, helpVideos, size, includeComboBoxes, ariadnaApp);

                toolBar.Add(group);
            }
        }

        private ObservableCollection<Control> GetItems(UiQuickActionsGroup actionsGroup, List<UiIcon> icons,
            List<UiKeyBinding> hotKeys, List<UiHelpVideo> helpVideos, RibbonItemSize size, bool includeComboBoxes,
            AriadnaApp ariadnaApp)
        {
            var features = ariadnaApp.Features.OfType<IInterfaceFeature>();

            var items = new ObservableCollection<Control>();

            foreach (var item in actionsGroup.Items)
            {
                var feature = features.FirstOrDefault(f => f.Id == item.Id);
                if (feature == null)
                    continue;

                if (feature is ICommandFeature commandFeature)
                {
                    var enabledIcon = ariadnaApp.InterfaceHelper.GetIcon(icons, commandFeature);
                    var button = CreateButton(item, commandFeature, enabledIcon, hotKeys, ariadnaApp.InterfaceHelper,
                        helpVideos);

                    items.Add(button);
                }

                if (feature is IComboboxFeature comboboxFeature && includeComboBoxes)
                {
                    var comboBox = CreateComboBox(item, comboboxFeature, ariadnaApp.InterfaceHelper, helpVideos);

                    items.Add(comboBox);
                }
            }

            return items;
        }

        #endregion

        #region Factory

        private ComboBox CreateComboBox(UiQuickActionItem item, IComboboxFeature feature,
            IInterfaceHelper interfaceHelper, List<UiHelpVideo> helpVideos)
        {
            var comboBox = new ComboBox
            {
                ToolTip = interfaceHelper.CreateToolTip(feature, helpVideos, null, item.Header, item.Description,
                    item.DisableReason),
                Margin = new Thickness(5, 2, 5, 2)
            };

            comboBox.AddBehavior(new InterfaceFeatureBehavior(feature));
            comboBox.AddBehavior(new ComboBoxFeatureBehavior(feature));

            return comboBox;
        }

        private ButtonBase CreateButton(UiQuickActionItem item, ICommandFeature feature, object icon,
            List<UiKeyBinding> hotKeys, IInterfaceHelper interfaceHelper, List<UiHelpVideo> helpVideos)
        {
            ButtonBase button = feature switch
            {
                IToggleCommandFeature _ => new ToggleButton(),
                _ => new Button(),
            };

            button.Command = feature.Command;
            button.Focusable = false;
            button.Margin = new Thickness(5, 2, 5, 2);


            button.ToolTip = interfaceHelper.CreateToolTip(feature, helpVideos, hotKeys, item.Header, item.Description,
                item.DisableReason);

            button.Width = 22;
            button.Height = 22;

            button.Content = icon;

            button.AddBehavior(new InterfaceFeatureBehavior(feature));

            if (feature is IToggleCommandFeature toggleCommandFeature)
                button.AddBehavior(new ToggleFeatureBehavior(toggleCommandFeature));

            return button;
        }

        #endregion
    }
}