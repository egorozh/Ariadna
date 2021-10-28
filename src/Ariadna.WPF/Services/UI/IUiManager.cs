using System.Windows.Controls;

namespace Ariadna;

public interface IUiManager
{
    IRibbonManager RibbonManager { get; }
    ISettingsManager SettingsManager { get; }
    IMenuManager MenuManager { get; }
    IQuickActionsManager QuickActionsManager { get; }
    
    JsonInterface JsonInterface { get; }

    #region Events

    #endregion

    void InitialItems();    

    void SetNewRibbonItems(List<UiTabRibbon> tabs);

    void SetNewMenuItems(List<UiMenuItem> menus);
    void SetNewQuickActions(UiQuickActions quickActions);

    void SetNewIcons(List<UiIcon> icons);
    void SetNewHotKeys(List<UiKeyBinding> hotKeys);

    ContextMenu GetContextMenu(List<UiMenuItem> items);
    void DeactivateContextMenu(ContextMenu contextMenu);

    void Init(IReadOnlyList<IFeature> features, IReadOnlyList<ISettings> settings);
}