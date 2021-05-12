using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Xaml.Behaviors;

namespace Ariadna
{
    public class DropDownButtonBehavior : Behavior<Button>
    {
        private bool _isContextMenuOpen;
            
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AddHandler(Button.ClickEvent, new RoutedEventHandler(AssociatedObject_Click), true);
        }

        private void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button?.ContextMenu != null)
            {
                if (!_isContextMenuOpen)
                {
                    // Add handler to detect when the ContextMenu closes
                    button.ContextMenu.AddHandler(ContextMenu.ClosedEvent, new RoutedEventHandler(ContextMenu_Closed), true);
                   
                    // If there is a drop-down assigned to this button, then position and display it 
                    button.ContextMenu.PlacementTarget = button;
                    button.ContextMenu.Placement = PlacementMode.Bottom;
                    button.ContextMenu.IsOpen = true;
                    _isContextMenuOpen = true;
                }
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.RemoveHandler(Button.ClickEvent, new RoutedEventHandler(AssociatedObject_Click));
        }

        private void ContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            _isContextMenuOpen = false;

            if (sender is ContextMenu contextMenu)
            {
                contextMenu.RemoveHandler(ContextMenu.ClosedEvent, new RoutedEventHandler(ContextMenu_Closed));
            }
        }
    }
}
