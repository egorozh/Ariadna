namespace Ariadna;

public interface IRibbonManager
{
    event EventHandler Loaded;

    bool Visible { get; set; }

    void SelectTabItem(string tabHeader = null);

    /// <summary>
    /// Заблокировать все вкладки ленты
    /// </summary>
    /// <param name="tabHeaders">Исключения</param>
    void BlockAllTabItems(string[] tabHeaders = null);

    /// <summary>
    /// Разблокировать все вкладки ленты
    /// </summary>
    /// <param name="tabHeaders">Исключения</param>
    void UnBlockAllTabItems(string[] tabHeaders = null);

    void Clear();

    void InitElements(List<UiTabRibbon> tabs, List<UiIcon> icons, List<UiKeyBinding> jsonConfigHotKeys,
        List<UiHelpVideo> helpVideos, IReadOnlyList<IFeature> features, IInterfaceHelper interfaceHelper);

    string GetTabName(IFeature akimFeature);
}