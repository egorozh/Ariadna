using System.Windows;

namespace Ariadna;

public interface ITwoStateCommandFeature : ICommandFeature
{
    State State { get; }
    
    event EventHandler StateChanged;
        
    FrameworkElement? CreateAlternativeIcon();
    DefaultMenuProperties GetAlternativeMenuProperties();
    DefaultRibbonProperties GetAlternativeRibbonProperties();
}

public enum State
{
    Main,
    Alternative
}