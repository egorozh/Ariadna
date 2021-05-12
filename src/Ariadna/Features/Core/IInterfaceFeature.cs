using System;

namespace Ariadna
{
    public interface IInterfaceFeature : IFeature
    {
        DefaultRibbonProperties DefaultRibbonProperties { get; }
        DefaultMenuProperties DefaultMenuProperties { get; }

        event EventHandler IsShowChanged;
        bool IsShow { get; set; }
    }
}