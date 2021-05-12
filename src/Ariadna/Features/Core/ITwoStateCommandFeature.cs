using System;
using System.Windows;

namespace Ariadna
{
    public interface ITwoStateCommandFeature : ICommandFeature
    {
        State State { get; set; }

        DefaultMenuProperties AlternativeMenuProperties { get; }
        DefaultRibbonProperties AlternativeRibbonProperties { get; }
      
        event EventHandler StateChanged;

        FrameworkElement CreateAlternativeIcon();
    }

    public enum State
    {
        Main,
        Alternative
    }
}