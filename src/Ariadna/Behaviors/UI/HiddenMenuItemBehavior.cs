using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace Ariadna
{
    public class HiddenMenuItemBehavior : Behavior<MenuItem>
    {
        protected override void OnAttached()
        {
            var itemsSource = AssociatedObject.ItemsSource;

            foreach (var item in itemsSource)
            {
                if (item is Control control)
                {
                    var commandFeature = control.GetBehavior<InterfaceFeatureBehavior>()?.Feature;

                    if (commandFeature != null)
                        commandFeature.IsShowChanged += CommandFeature_IsShowChanged;
                }
            }

            if (itemsSource is INotifyCollectionChanged notifyCollection)
                notifyCollection.CollectionChanged += NotifyCollection_CollectionChanged;

            CheckVisibility();
        }

        protected override void OnDetaching()
        {
            var itemsSource = AssociatedObject.ItemsSource;

            foreach (var item in itemsSource)
            {
                if (item is Control control)
                {
                    var commandFeature = control.GetBehavior<InterfaceFeatureBehavior>()?.Feature;

                    if (commandFeature != null)
                        commandFeature.IsShowChanged -= CommandFeature_IsShowChanged;
                }
            }

            if (itemsSource is INotifyCollectionChanged notifyCollection)
                notifyCollection.CollectionChanged -= NotifyCollection_CollectionChanged;
        }


        private void NotifyCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var addedItem = e.NewItems[0];

                if (addedItem is Control control)
                {
                    var commandFeature = control.GetBehavior<InterfaceFeatureBehavior>()?.Feature;

                    if (commandFeature != null)
                        commandFeature.IsShowChanged += CommandFeature_IsShowChanged;
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var removedItem = e.OldItems[0];

                if (removedItem is Control control)
                {
                    var commandFeature = control.GetBehavior<InterfaceFeatureBehavior>()?.Feature;

                    if (commandFeature != null)
                        commandFeature.IsShowChanged -= CommandFeature_IsShowChanged;
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                var itemsSource = AssociatedObject.ItemsSource;

                foreach (var item in itemsSource)
                {
                    if (item is Control control)
                    {
                        var commandFeature = control.GetBehavior<InterfaceFeatureBehavior>()?.Feature;

                        if (commandFeature != null)
                            commandFeature.IsShowChanged -= CommandFeature_IsShowChanged;
                    }
                }
            }

            CheckVisibility();
        }

        private void CommandFeature_IsShowChanged(object? sender, System.EventArgs e)
        {
            CheckVisibility();
        }

        private void CheckVisibility()
        {
            var itemsSource = AssociatedObject.ItemsSource;

            AssociatedObject.Visibility = Visibility.Collapsed;

            foreach (var item in itemsSource)
            {
                if (item is MenuItem control && control.Visibility == Visibility.Visible)
                {
                    AssociatedObject.Visibility = Visibility.Visible;
                    return;
                }
            }
        }
    }
}