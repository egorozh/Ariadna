using System.Collections.Generic;

namespace Ariadna
{
    public interface IUiManager
    {
        IRibbonManager RibbonManager { get; }
        ISettingsManager SettingsManager { get; }
        IMenuManager MenuManager { get; }
        IQuickActionsManager QuickActionsManager { get; }
        ITipPopupManager TipPopupManager { get; }
        
        JsonInterface JsonInterface { get; }
     
        #region Events
        
        #endregion

        void InitialItems();

        void SetNewRibbonItems(List<UiTabRibbon> tabs);

        void SetNewMenuItems(List<UiMenuItem> menus);
        void SetNewQuickActions(UiQuickActions quickActions);

        void SetNewIcons(List<UiIcon> icons);
        void SetNewHotKeys(List<UiKeyBinding> hotKeys);
    }
}