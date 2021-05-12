using System.Windows;
using System.Windows.Input;
using Ariadna.Core;

namespace Ariadna
{
    /// <summary>
    /// Смена фокуса при нажатии Enter
    /// </summary>
    public class EnterKeyProperty : BaseAttachedProperty<EnterKeyProperty, bool>
    {
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is bool isEnterKey)) return;
            if (!(sender is FrameworkElement frameworkElement)) return;

            if (isEnterKey)
            {
                frameworkElement.Unloaded += Unloaded;
                frameworkElement.PreviewKeyDown += PreviewKeyDown;
            }
            else
            {
                frameworkElement.PreviewKeyDown -= PreviewKeyDown;
            }
        }

        private void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var frameworkElement = e.OriginalSource as FrameworkElement;

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                frameworkElement?.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private void Unloaded(object sender, RoutedEventArgs e)
        {
            if (!(sender is FrameworkElement frameworkElement)) return;

            frameworkElement.Unloaded -= Unloaded;
            frameworkElement.PreviewKeyDown -= PreviewKeyDown;
        }
    }
}