using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows.Input;
using Ariadna.Core;

namespace Ariadna
{
    /// <summary>
    /// Менеджер интерфейса основного приложения
    /// </summary>
    public class UiManager : BaseViewModel, IUiManager
    {
        #region Private Fields

        private readonly AriadnaApp _ariadnaApp;

        #endregion

        #region Public Properties

        public IRibbonManager RibbonManager { get; } = new RibbonManager();

        public ISettingsManager SettingsManager { get; }

        public IMenuManager MenuManager { get; } = new MenuManager();

        public IQuickActionsManager QuickActionsManager { get; } = new QuickActionsManager();

        public ITipPopupManager TipPopupManager { get; } = new TipPopupManager();

        public JsonInterface JsonInterface { get; private set; }

        /// <summary>
        /// Горячие клавиши
        /// </summary>
        public KeyBindingsCollection KeyBindings { get; } = new KeyBindingsCollection();

        #endregion

        #region Constructor

        public UiManager(AriadnaApp ariadnaApp, ISettingsManager settingsManager)
        {
            _ariadnaApp = ariadnaApp;
            SettingsManager = settingsManager;

            JsonInterface = InitJsonScheme();

            QuickActionsManager.IsShowChanged += QuickActionsManager_IsShowChanged;

            RibbonManager.Loaded += (s, e) => InitialItems();
        }

        #endregion

        #region Public Methods

        public void InitialItems()
        {
            Clear();

            MenuManager.InitElements(JsonInterface.Menu, JsonInterface.Icons, JsonInterface.HotKeys,
                _ariadnaApp.Features, _ariadnaApp);

            RibbonManager.InitElements(JsonInterface.Ribbon, JsonInterface.Icons, JsonInterface.HotKeys,
                JsonInterface.HelpVideos,
                _ariadnaApp);

            QuickActionsManager.InitElements(JsonInterface.QuickActions, JsonInterface.Icons,
                JsonInterface.HotKeys, JsonInterface.HelpVideos, _ariadnaApp);

            SettingsManager.InitElements(JsonInterface.Settings, _ariadnaApp.Settings);

            InitKeyBindings(JsonInterface.HotKeys);

            RibbonManager.SelectTabItem();

            CreateSettingsButton(JsonInterface);
        }

        public void SetNewRibbonItems(List<UiTabRibbon> tabs)
        {
            JsonInterface.Ribbon = tabs;

            JsonInterface.SaveJsonScheme(JsonInterface, GetJsonPath(), _ariadnaApp.Logger);
            RibbonManager.InitElements(JsonInterface.Ribbon, JsonInterface.Icons, JsonInterface.HotKeys,
                JsonInterface.HelpVideos,
                _ariadnaApp);
        }

        public void SetNewMenuItems(List<UiMenuItem> menus)
        {
            JsonInterface.Menu = menus;

            JsonInterface.SaveJsonScheme(JsonInterface, GetJsonPath(), _ariadnaApp.Logger);
            MenuManager.InitElements(JsonInterface.Menu, JsonInterface.Icons, JsonInterface.HotKeys,
                _ariadnaApp.Features, _ariadnaApp);
        }

        public void SetNewQuickActions(UiQuickActions quickActions)
        {
            JsonInterface.QuickActions = quickActions;

            JsonInterface.SaveJsonScheme(JsonInterface, GetJsonPath(), _ariadnaApp.Logger);
            QuickActionsManager.InitElements(JsonInterface.QuickActions, JsonInterface.Icons,
                JsonInterface.HotKeys, JsonInterface.HelpVideos, _ariadnaApp);
        }

        public void SetNewIcons(List<UiIcon> icons)
        {
            JsonInterface.Icons = icons;

            JsonInterface.SaveJsonScheme(JsonInterface, GetJsonPath(), _ariadnaApp.Logger);

            RibbonManager.InitElements(JsonInterface.Ribbon, JsonInterface.Icons, JsonInterface.HotKeys,
                JsonInterface.HelpVideos,
                _ariadnaApp);
            MenuManager.InitElements(JsonInterface.Menu, JsonInterface.Icons, JsonInterface.HotKeys,
                _ariadnaApp.Features, _ariadnaApp);
            QuickActionsManager.InitElements(JsonInterface.QuickActions, JsonInterface.Icons,
                JsonInterface.HotKeys, JsonInterface.HelpVideos, _ariadnaApp);
        }

        public void SetNewHotKeys(List<UiKeyBinding> hotKeys)
        {
            InitKeyBindings(hotKeys);

            JsonInterface.HotKeys = hotKeys;

            JsonInterface.SaveJsonScheme(JsonInterface, GetJsonPath(), _ariadnaApp.Logger);

            RibbonManager.InitElements(JsonInterface.Ribbon, JsonInterface.Icons, JsonInterface.HotKeys,
                JsonInterface.HelpVideos,
                _ariadnaApp);
            MenuManager.InitElements(JsonInterface.Menu, JsonInterface.Icons, JsonInterface.HotKeys,
                _ariadnaApp.Features, _ariadnaApp);
            QuickActionsManager.InitElements(JsonInterface.QuickActions, JsonInterface.Icons,
                JsonInterface.HotKeys, JsonInterface.HelpVideos, _ariadnaApp);
        }

        #endregion

        #region Internal Methods

        internal void Clear()
        {
            RibbonManager.Clear();
            MenuManager.Clear();
            QuickActionsManager.Clear();
            SettingsManager.Clear();
        }

        #endregion

        #region Private Methods

        private void QuickActionsManager_IsShowChanged(object? sender, IsShowChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IQuickActionsManager.IsShowLeft):
                    JsonInterface.QuickActions.IsShowLeft = QuickActionsManager.IsShowLeft;
                    break;
                case nameof(IQuickActionsManager.IsShowTop):
                    JsonInterface.QuickActions.IsShowTop = QuickActionsManager.IsShowTop;
                    break;
                case nameof(IQuickActionsManager.IsShowRight):
                    JsonInterface.QuickActions.IsShowRight = QuickActionsManager.IsShowRight;
                    break;
            }

            JsonInterface.SaveJsonScheme(JsonInterface, GetJsonPath(), _ariadnaApp.Logger);
        }

        private string GetJsonPath() => Path.Combine(_ariadnaApp.GetCongigDirectory(), "Interface.json");

        private JsonInterface InitJsonScheme()
        {
            var jsonInterface = new JsonInterface();

            var path = GetJsonPath();

            try
            {
                if (File.Exists(path))
                {
                    var json = File.ReadAllText(path);
                    jsonInterface = JsonSerializer.Deserialize<JsonInterface>(json);
                }
            }
            catch (Exception e)
            {
                _ariadnaApp.Logger.Error(e.Message);
            }

            return jsonInterface;
        }

        private void InitKeyBindings(List<UiKeyBinding> uiKeyBindings)
        {
            KeyBindings.Clear();

            var keyBindings = new List<KeyBinding>();

            foreach (var commandFeature in _ariadnaApp.Features.OfType<ICommandFeature>())
            {
                var kb = _ariadnaApp.InterfaceHelper.CreateKeyBinding(uiKeyBindings, commandFeature);

                if (kb != null)
                    keyBindings.Add(kb);
            }

            KeyBindings.AddRange(keyBindings);
        }

        private void CreateSettingsButton(JsonInterface jsonScheme)
        {
            var settingsFeature =
                _ariadnaApp.Features.FirstOrDefault(f => f.Id == "267d8733-06d9-4c5a-b438-b44a5c63f7a1");

            if (!(settingsFeature is ICommandFeature commandFeature))
                return;

            var button = new Button
            {
                Content = commandFeature.CreateDefaultIcon(),
                Command = commandFeature.Command
            };

            var ribSet = commandFeature.DefaultRibbonProperties;

            var header = ribSet.Name ?? "Настройки";
            var description = ribSet.Description ?? "";
            var disableReason = ribSet.DisableReason ?? "";

            var kb = commandFeature.KeyBinding;

            if (jsonScheme != null)
            {
                if (jsonScheme.Icons != null)
                    button.Content = _ariadnaApp.InterfaceHelper.GetIcon(jsonScheme.Icons, commandFeature);

                if (jsonScheme.HotKeys != null)
                    kb = _ariadnaApp.InterfaceHelper.CreateKeyBinding(jsonScheme.HotKeys, commandFeature);

                if (jsonScheme.Ribbon != null)
                {
                    var uiSettingsProp = jsonScheme.Ribbon.SelectMany(ui => ui.Boxes).SelectMany(b => b.Items)
                        .FirstOrDefault(item => item.Id == commandFeature.Id);

                    if (uiSettingsProp != null)
                    {
                        header = uiSettingsProp.Header ?? "Настройки";
                        description = uiSettingsProp.Description ?? "";
                        disableReason = uiSettingsProp.DisableReason ?? "";
                    }
                }
            }

            button.ToolTip = _ariadnaApp.InterfaceHelper.CreateToolTip(commandFeature, jsonScheme?.HelpVideos,
                jsonScheme?.HotKeys, header, description, disableReason);

            button.AddBehavior(new InterfaceFeatureBehavior(commandFeature));

            _ariadnaApp.SettingsButton = button;
        }

        #endregion
    }
}