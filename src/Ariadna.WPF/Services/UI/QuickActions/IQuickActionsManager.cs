namespace Ariadna;

public interface IQuickActionsManager
{
    bool IsShowLeft { get; set; }
    bool IsShowTop { get; set; }
    bool IsShowRight { get; set; }

    void Clear();

    void InitElements(UiQuickActions jsonSchemeQuickActions, List<UiIcon> icons,
        List<UiKeyBinding> hotKeys, List<UiHelpVideo> helpVideos,
        IInterfaceHelper interfaceHelper, IReadOnlyList<IFeature> features);

    event EventHandler<IsShowChangedEventArgs> IsShowChanged;
}

public class IsShowChangedEventArgs : EventArgs
{
    public IsShowChangedEventArgs(string propertyName)
    {
        PropertyName = propertyName;
    }

    public string PropertyName { get; }
}