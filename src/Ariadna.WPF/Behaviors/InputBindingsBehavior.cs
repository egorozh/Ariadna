using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace Ariadna
{
    internal class InputBindingsBehavior : Behavior<Window>
    {
        public static readonly DependencyProperty KeyBindingsProperty = DependencyProperty.Register(
            nameof(KeyBindings), typeof(ObservableCollection<KeyBinding>),
            typeof(InputBindingsBehavior),
            new PropertyMetadata(default(ObservableCollection<KeyBinding>), KeyBindingsChangedCallback));

        private static void KeyBindingsChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is InputBindingsBehavior b)
                b.KeyBindingsChanged(e.OldValue, e.NewValue);
        }

        public ObservableCollection<KeyBinding> KeyBindings
        {
            get => (ObservableCollection<KeyBinding>) GetValue(KeyBindingsProperty);
            set => SetValue(KeyBindingsProperty, value);
        }

        private void KeyBindingsChanged(object? oldValue, object? newValue)
        {
            if (oldValue is ObservableCollection<KeyBinding> oldBindings)
            {
                AssociatedObject.InputBindings.Clear();
                oldBindings.CollectionChanged -= KeyBindingsCollectionChanged;
            }

            if (newValue is ObservableCollection<KeyBinding> keyBindings)
            {
                AssociatedObject.InputBindings.AddRange(keyBindings);

                keyBindings.CollectionChanged += KeyBindingsCollectionChanged;
            }
        }

        private void KeyBindingsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AssociatedObject.InputBindings.Add((KeyBinding) e.NewItems[0]);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    AssociatedObject.InputBindings.Remove((KeyBinding) e.OldItems[0]);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    AssociatedObject.InputBindings.Clear();
                    break;
            }
        }
    }
}