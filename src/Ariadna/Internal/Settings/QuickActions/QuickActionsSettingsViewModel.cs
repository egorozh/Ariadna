using AKIM.Plugins.Base;
using Ariadna.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Ariadna.Settings.QuickActions
{
    internal class QuickActionsSettingsViewModel : BaseViewModel
    {
        #region Private Fields

        private readonly AriadnaApp _akimApp;
        private readonly ISettings _settings;

        #endregion

        #region Public Properties

        public ObservableCollection<QuickActionsPanelViewModel> Panels { get; set; } =
            new ObservableCollection<QuickActionsPanelViewModel>();

        public QuickActionsPanelViewModel SelectedPanel { get; set; }

        #endregion

        #region Constructor

        public QuickActionsSettingsViewModel(AriadnaApp akimApp, ISettings settings)
        {
            _akimApp = akimApp;
            _settings = settings;

            Panels.CollectionChanged += Tabs_CollectionChanged;

            Update();
        }

        #endregion

        #region Public Methods

        public void Accept()
        {
            var quickManager = _akimApp.UiManager.QuickActionsManager;

            var quickActions = new UiQuickActions
            {
                Left = GetQuickActions(Panels[0]),
                Top = GetQuickActions(Panels[1]),
                Right = GetQuickActions(Panels[2]),
                IsShowLeft = quickManager.IsShowLeft,
                IsShowTop = quickManager.IsShowTop,
                IsShowRight = quickManager.IsShowRight
            };

            _akimApp.UiManager.SetNewQuickActions(quickActions);
        }

        private List<UiQuickActionsGroup> GetQuickActions(QuickActionsPanelViewModel panel)
        {
            var boxes = new List<UiQuickActionsGroup>();

            foreach (var quickActionsGroupViewModel in panel.Groups)
            {
                var items = new List<UiQuickActionItem>();

                foreach (var button in quickActionsGroupViewModel.Buttons)
                {
                    var uiItem = new UiQuickActionItem
                    {
                        Description = button.Description,
                        Header = button.Header,
                        DisableReason = button.DisableReason,
                        Id = button.Feature?.Id,
                    };
                    items.Add(uiItem);
                }

                var uiBox = new UiQuickActionsGroup
                {
                    Items = items,
                    Header = quickActionsGroupViewModel.Header
                };

                boxes.Add(uiBox);
            }

            return boxes;
        }

        public void Cancel()
        {
            Update();
        }

        public IInterfaceFeature SelectFeature()
        {
            var featureBrowser = new FeaturesBrowserDialog();

            var allCommandFeatures = _akimApp.Features.OfType<IInterfaceFeature>().ToList();

            var noRibbonFeature = allCommandFeatures
                .Where(f => !ContainsInQuickActions(f))
                .ToList();

            var filters = new Dictionary<string, IEnumerable<IInterfaceFeature>>
            {
                {"Не задействованные в панели быстрого доступа", noRibbonFeature}
            };

            var feature = featureBrowser.ShowDialog(allCommandFeatures, filters);
            return feature;
        }

        #endregion

        #region Private Methods

        private void Update()
        {
            var jsonScheme = _akimApp.UiManager.JsonInterface;

            if (jsonScheme?.QuickActions == null)
                return;

            Panels.Clear();

            var leftVm =
                new QuickActionsPanelViewModel(jsonScheme.QuickActions.Left, this, "Левая панель быстрого доступа",
                    _akimApp);

            var topVm =
                new QuickActionsPanelViewModel(jsonScheme.QuickActions.Top, this, "Верхняя панель быстрого доступа",
                    _akimApp);

            var rightVm =
                new QuickActionsPanelViewModel(jsonScheme.QuickActions.Right, this, "Правая панель быстрого доступа",
                    _akimApp);

            leftVm.ItemChanged += TabVm_ItemChanged;
            topVm.ItemChanged += TabVm_ItemChanged;
            rightVm.ItemChanged += TabVm_ItemChanged;

            Panels.Add(leftVm);
            Panels.Add(topVm);
            Panels.Add(rightVm);

            SelectedPanel = Panels.FirstOrDefault();

            _settings.HasChanges = false;
        }

        private void TabVm_ItemChanged(object sender, EventArgs e)
        {
            _settings.HasChanges = true;
        }

        private bool ContainsInQuickActions(IInterfaceFeature commandFeature)
        {
            foreach (var tab in Panels)
            {
                foreach (var @group in tab.Groups)
                {
                    foreach (var button in @group.Buttons)
                    {
                        if (button.Feature == commandFeature)
                            return true;
                    }
                }
            }

            return false;
        }

        private void Tabs_CollectionChanged(object sender,
            NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var newTab = e.NewItems[0] as QuickActionsPanelViewModel;
                    newTab.ItemChanged += TabVm_ItemChanged;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    var oldTab = e.OldItems[0] as QuickActionsPanelViewModel;
                    oldTab.ItemChanged -= TabVm_ItemChanged;
                    break;
                case NotifyCollectionChangedAction.Reset:

                    if (e.OldItems != null)
                    {
                        foreach (QuickActionsPanelViewModel button in e.OldItems)
                            button.ItemChanged -= TabVm_ItemChanged;
                    }

                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:

                    break;
            }

            _settings.HasChanges = true;
        }

        #endregion
    }
}