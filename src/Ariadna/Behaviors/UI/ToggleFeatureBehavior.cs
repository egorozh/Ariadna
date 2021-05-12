using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Xaml.Behaviors;

namespace Ariadna
{
    public class ToggleFeatureBehavior : Behavior<Control>
    {
        public static readonly DependencyProperty ToggleFeatureProperty = DependencyProperty.Register(
            nameof(ToggleFeature), typeof(IToggleCommandFeature), typeof(ToggleFeatureBehavior),
            new PropertyMetadata(default(IToggleCommandFeature)));

        public IToggleCommandFeature ToggleFeature
        {
            get => (IToggleCommandFeature) GetValue(ToggleFeatureProperty);
            set => SetValue(ToggleFeatureProperty, value);
        }

        public ToggleFeatureBehavior(IToggleCommandFeature toggleCommandFeature)
        {
            ToggleFeature = toggleCommandFeature;
        }

        protected override void OnAttached()
        {
            if (AssociatedObject is ToggleButton toggleButton)
            {
                if (ToggleFeature.IsPressed)
                    toggleButton.IsChecked = true;

                toggleButton.Checked += Control_Checked;
                toggleButton.Unchecked += Control_Unchecked;
            }
            else if (AssociatedObject is MenuItem menuItem)
            {
                if (ToggleFeature.IsPressed)
                    menuItem.IsChecked = true;

                menuItem.Checked += Control_Checked;
                menuItem.Unchecked += Control_Unchecked;
            }

            ToggleFeature.IsPressedChanged += ToggleFeature_IsPressedChanged;
        }

        private void ToggleFeature_IsPressedChanged(object sender, System.EventArgs e)
        {
            SetIsCheckedOnControl();
        }

        private void SetIsCheckedOnControl()
        {
            if (AssociatedObject is ToggleButton toggleButton)
            {
                toggleButton.Checked -= Control_Checked;
                toggleButton.Unchecked -= Control_Unchecked;

                toggleButton.IsChecked = ToggleFeature.IsPressed;

                toggleButton.Checked += Control_Checked;
                toggleButton.Unchecked += Control_Unchecked;
            }
            else if (AssociatedObject is MenuItem menuItem)
            {
                menuItem.Checked -= Control_Checked;
                menuItem.Unchecked -= Control_Unchecked;

                menuItem.IsChecked = ToggleFeature.IsPressed;

                menuItem.Checked += Control_Checked;
                menuItem.Unchecked += Control_Unchecked;
            }
        }

        private void Control_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleFeature.IsPressed = false;

            SetIsCheckedOnControl();
        }

        private void Control_Checked(object sender, RoutedEventArgs e) => ToggleFeature.IsPressed = true;
    }

    /*
    public class ToggleCommandProperty : BaseAttachedProperty<ToggleCommandProperty, IToggleCommandFeature>
    {
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var toggleCommand = (ToggleCommandFeature) e.NewValue;

            toggleCommand.IsPressedChanged += (s, e) =>
            {
                if ((sender is ToggleButton toggleButton))
                {
                    toggleButton.Checked -= ToggleButton_Checked;
                    toggleButton.Unchecked -= ToggleButton_Unchecked;

                    toggleButton.IsChecked = toggleCommand.IsPressed;

                    toggleButton.Checked += ToggleButton_Checked;
                    toggleButton.Unchecked += ToggleButton_Unchecked;
                }
                else if (sender is MenuItem menuItem)
                {
                    menuItem.Checked -= MenuItem_Checked;
                    menuItem.Unchecked -= MenuItem_Unchecked;

                    menuItem.IsChecked = toggleCommand.IsPressed;

                    menuItem.Checked += MenuItem_Checked;
                    menuItem.Unchecked += MenuItem_Unchecked;
                }
            };

            // Вызываем методы в features:
            if ((sender is ToggleButton toggleButton))
            {
                if (toggleCommand.IsPressed)
                    toggleButton.IsChecked = true;

                toggleButton.Checked += ToggleButton_Checked;
                toggleButton.Unchecked += ToggleButton_Unchecked;
            }
            else if (sender is MenuItem menuItem)
            {
                if (toggleCommand.IsPressed)
                    menuItem.IsChecked = true;

                menuItem.Checked += MenuItem_Checked;
                menuItem.Unchecked += MenuItem_Unchecked;
            }
        }

        private void MenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem) sender;


            var feature = (ToggleCommandFeature) GetValue(menuItem);

            feature.SetIsPressed(false);
        }

        private void MenuItem_Checked(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem) sender;


            var feature = (ToggleCommandFeature) GetValue(menuItem);

            feature.SetIsPressed(true);
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            var toggleButton = (ToggleButton) sender;

            var feature = (ToggleCommandFeature) GetValue(toggleButton);

            feature.SetIsPressed(false);
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            var toggleButton = (ToggleButton) sender;

            var feature = (ToggleCommandFeature) GetValue(toggleButton);

            feature.SetIsPressed(true);
        }
    }*/
}