using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Ariadna.Core;
using Serilog;

namespace Ariadna;

public class UiManager : BaseViewModel, IUiManager
{
    #region Private Fields

    private readonly AriadnaApp _ariadnaApp;
    private readonly IReadOnlyList<IFeature> _features;
    private readonly IInterfaceHelper _interfaceHelper;
    private readonly IReadOnlyList<ISettings> _settings;
    private readonly ILogger _logger;
    private readonly IStorage _storage;

    #endregion

    #region Public Properties

    public IRibbonManager RibbonManager { get; } = new RibbonManager();

    public ISettingsManager SettingsManager { get; }

    public IMenuManager MenuManager { get; } = new MenuManager();

    public IQuickActionsManager QuickActionsManager { get; } = new QuickActionsManager();

    public JsonInterface JsonInterface { get; private set; }

    public ObservableCollection<KeyBinding> KeyBindings { get; } = new();

    #endregion

    #region Constructor

    public UiManager(AriadnaApp ariadnaApp, ISettingsManager settingsManager,
        IReadOnlyList<IFeature> features, IInterfaceHelper interfaceHelper,
        IReadOnlyList<ISettings> settings,
        ILogger logger,
        IStorage storage)
    {
        _ariadnaApp = ariadnaApp;
        _features = features;
        _interfaceHelper = interfaceHelper;
        _settings = settings;
        _logger = logger;
        _storage = storage;
        SettingsManager = settingsManager;

        JsonInterface = InitJsonScheme();
    }

    #endregion

    #region Public Methods

    public void Init()
    {
        QuickActionsManager.IsShowChanged += QuickActionsManager_IsShowChanged;

        RibbonManager.Loaded += (s, e) => InitialItems();
    }

    public void InitialItems()
    {
        Clear();

        MenuManager.InitElements(JsonInterface.Menu, JsonInterface.Icons, JsonInterface.HotKeys,
            _features, _interfaceHelper);

        RibbonManager.InitElements(JsonInterface.Ribbon, JsonInterface.Icons, JsonInterface.HotKeys,
            JsonInterface.HelpVideos, _features, _interfaceHelper);

        QuickActionsManager.InitElements(JsonInterface.QuickActions, JsonInterface.Icons,
            JsonInterface.HotKeys, JsonInterface.HelpVideos, _interfaceHelper, _features);

        SettingsManager.InitElements(JsonInterface.Settings, _settings);

        InitKeyBindings(JsonInterface.HotKeys);

        RibbonManager.SelectTabItem();

        CreateSettingsButton(JsonInterface);
    }

    public void SetNewRibbonItems(List<UiTabRibbon> tabs)
    {
        JsonInterface.Ribbon = tabs;

        JsonInterface.SaveJsonScheme(JsonInterface, GetJsonPath(), _logger);
        RibbonManager.InitElements(JsonInterface.Ribbon, JsonInterface.Icons, JsonInterface.HotKeys,
            JsonInterface.HelpVideos, _features, _interfaceHelper);
    }

    public void SetNewMenuItems(List<UiMenuItem> menus)
    {
        JsonInterface.Menu = menus;

        JsonInterface.SaveJsonScheme(JsonInterface, GetJsonPath(), _logger);
        MenuManager.InitElements(JsonInterface.Menu, JsonInterface.Icons, JsonInterface.HotKeys,
            _features, _interfaceHelper);
    }

    public void SetNewQuickActions(UiQuickActions quickActions)
    {
        JsonInterface.QuickActions = quickActions;

        JsonInterface.SaveJsonScheme(JsonInterface, GetJsonPath(), _logger);
        QuickActionsManager.InitElements(JsonInterface.QuickActions, JsonInterface.Icons,
            JsonInterface.HotKeys, JsonInterface.HelpVideos, _interfaceHelper, _features);
    }

    public void SetNewIcons(List<UiIcon> icons)
    {
        JsonInterface.Icons = icons;

        JsonInterface.SaveJsonScheme(JsonInterface, GetJsonPath(), _logger);

        RibbonManager.InitElements(JsonInterface.Ribbon, JsonInterface.Icons, JsonInterface.HotKeys,
            JsonInterface.HelpVideos, _features, _interfaceHelper);
        MenuManager.InitElements(JsonInterface.Menu, JsonInterface.Icons, JsonInterface.HotKeys,
            _features, _interfaceHelper);
        QuickActionsManager.InitElements(JsonInterface.QuickActions, JsonInterface.Icons,
            JsonInterface.HotKeys, JsonInterface.HelpVideos, _interfaceHelper, _features);
    }

    public void SetNewHotKeys(List<UiKeyBinding> hotKeys)
    {
        InitKeyBindings(hotKeys);

        JsonInterface.HotKeys = hotKeys;

        JsonInterface.SaveJsonScheme(JsonInterface, GetJsonPath(), _logger);

        RibbonManager.InitElements(JsonInterface.Ribbon, JsonInterface.Icons, JsonInterface.HotKeys,
            JsonInterface.HelpVideos, _features, _interfaceHelper);
        MenuManager.InitElements(JsonInterface.Menu, JsonInterface.Icons, JsonInterface.HotKeys,
            _features, _interfaceHelper);
        QuickActionsManager.InitElements(JsonInterface.QuickActions, JsonInterface.Icons,
            JsonInterface.HotKeys, JsonInterface.HelpVideos, _interfaceHelper, _features);
    }

    public ContextMenu GetContextMenu(List<UiMenuItem> items)
    {
        ContextMenu cm = new()
        {
            ItemsSource = new ObservableCollection<Control>()
        };

        ContextMenuManager.SetItems(cm, items, JsonInterface.Icons, JsonInterface.HotKeys,
            _features, _interfaceHelper);

        //cm.Unloaded += CmOnUnloaded;

        return cm;
    }

    public void DeactivateContextMenu(ContextMenu contextMenu)
    {
        ContextMenuManager.Tree(new[] {contextMenu}, i => i.ClearBehaviors());
    }

    private void CmOnUnloaded(object sender, RoutedEventArgs e)
    {
        if (sender is not ContextMenu cm)
            return;

        cm.Unloaded -= CmOnUnloaded;

        ContextMenuManager.Tree(new[] {cm}, i => i.ClearBehaviors());
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

        JsonInterface.SaveJsonScheme(JsonInterface, GetJsonPath(), _logger);
    }

    private string GetJsonPath() => _storage.InterfaceConfigPath;

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
            _logger.Error(e.Message);
        }

        return jsonInterface;
    }

    private void InitKeyBindings(List<UiKeyBinding> uiKeyBindings)
    {
        KeyBindings.Clear();

        var keyBindings = new List<KeyBinding>();

        foreach (var commandFeature in _features.OfType<ICommandFeature>())
        {
            var kb = _interfaceHelper.CreateKeyBinding(uiKeyBindings, commandFeature);

            if (kb != null)
                keyBindings.Add(kb);
        }

        foreach (var keyBinding in keyBindings) 
            KeyBindings.Add(keyBinding);
    }

    private void CreateSettingsButton(JsonInterface jsonScheme)
    {
        var settingsFeature =
            _features.FirstOrDefault(f => f.Id == "267d8733-06d9-4c5a-b438-b44a5c63f7a1");

        if (settingsFeature is not ICommandFeature commandFeature)
            return;

        var button = new Button
        {
            Content = commandFeature.GetDefaultIcon(),
            Command = commandFeature
        };

        var ribSet = commandFeature.GetDefaultRibbonProperties();

        var header = ribSet.Header ?? "Настройки";
        var description = ribSet.Description ?? "";
        var disableReason = ribSet.DisableReason ?? "";

        var kb = commandFeature.GetDefaultKeyBinding();

        if (jsonScheme != null)
        {
            if (jsonScheme.Icons != null)
                button.Content = _interfaceHelper.GetIcon(jsonScheme.Icons, commandFeature);

            if (jsonScheme.HotKeys != null)
                kb = _interfaceHelper.CreateKeyBinding(jsonScheme.HotKeys, commandFeature);

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

        button.ToolTip = _interfaceHelper.CreateToolTip(commandFeature, jsonScheme?.HelpVideos,
            jsonScheme?.HotKeys, header, description, disableReason);

        button.AddBehavior(new InterfaceFeatureBehavior(commandFeature));

        _ariadnaApp.SettingsButton = button;
    }

    #endregion
}