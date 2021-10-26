namespace Ariadna;

public interface IInterfaceFeature : IFeature
{
    bool IsShow { get; set; }
        
    event EventHandler IsShowChanged;

    DefaultRibbonProperties GetDefaultRibbonProperties();
    DefaultMenuProperties GetDefaultMenuProperties();
}