using System.Windows;
using System.Windows.Media;
using Ariadna.Engine.Core;

namespace Ariadna.Engine
{
    public class PositionProperty : BaseAttachedProperty<PositionProperty, Matrix>
    {
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var cs = CoordinateSystemProperty.GetValue(sender);

            if (cs != null)
            {
                if (sender is FrameworkElement frameworkElement && e.NewValue is Matrix matrix)
                {
                    frameworkElement.SetPosition(cs.GetCanvasPoint(new Point(matrix.OffsetX, matrix.OffsetY)));
                }
            }
        }
    }

    internal class CoordinateSystemProperty : BaseAttachedProperty<CoordinateSystemProperty, ICoordinateSystem>
    {
    }
}