using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Ariadna;

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

    protected override void OnDetaching()
    {
        base.OnDetaching();

        if (AssociatedObject is ToggleButton toggleButton)
        {
            toggleButton.Checked -= Control_Checked;
            toggleButton.Unchecked -= Control_Unchecked;
        }
        else if (AssociatedObject is MenuItem menuItem)
        {
            menuItem.Checked -= Control_Checked;
            menuItem.Unchecked -= Control_Unchecked;
        }

        ToggleFeature.IsPressedChanged -= ToggleFeature_IsPressedChanged;
    }

    private void ToggleFeature_IsPressedChanged(object? sender, System.EventArgs e)
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

    private void Control_Unchecked(object? sender, RoutedEventArgs e)
    {
        ToggleFeature.IsPressed = false;

        SetIsCheckedOnControl();
    }

    private void Control_Checked(object? sender, RoutedEventArgs e) => ToggleFeature.IsPressed = true;
}