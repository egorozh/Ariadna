using System.Windows;
using Ariadna.Core;

namespace Ariadna
{
    public class ColorProperty : BaseAttachedProperty<ColorProperty, string>
    {
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}