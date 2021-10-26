using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;

namespace Ariadna;

public class ComboBoxFeatureBehavior : Behavior<ComboBox>
{
    #region Private Fields

    private readonly IComboboxFeature _comboboxFeature;

    #endregion

    #region Constructor

    public ComboBoxFeatureBehavior(IComboboxFeature comboboxFeature)
    {
        _comboboxFeature = comboboxFeature;
    }

    #endregion

    #region Protected Methods

    protected override void OnAttached()
    {
        AssociatedObject.ItemsSource = _comboboxFeature.Items;
        AssociatedObject.SelectedItem = _comboboxFeature.CurrentItem;
        AssociatedObject.ItemTemplate = _comboboxFeature.GetItemTemplate();
        AssociatedObject.IsEnabled = _comboboxFeature.IsEnabled;

        _comboboxFeature.IsEnabledChanged += ComboboxFeature_IsEnabledChanged;
        _comboboxFeature.CurrentChanged += ComboboxFeature_CurrentChangedEvent;

        AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        _comboboxFeature.IsEnabledChanged -= ComboboxFeature_IsEnabledChanged;
        _comboboxFeature.CurrentChanged-= ComboboxFeature_CurrentChangedEvent;

        AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
    }

    #endregion

    #region Private Methods

    private void AssociatedObject_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        _comboboxFeature.CurrentItem = AssociatedObject.SelectedItem;
    }

    private void ComboboxFeature_SelectionChanged(object? sender, System.EventArgs e)
    {
        AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;

        AssociatedObject.SelectedItem = _comboboxFeature.CurrentItem;

        AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
    }

    private void ComboboxFeature_IsEnabledChanged(object? sender, System.EventArgs e)
    {
        AssociatedObject.IsEnabled = _comboboxFeature.IsEnabled;
    }

    private void ComboboxFeature_CurrentChangedEvent(object? sender, System.EventArgs e)
    {
        AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;

        AssociatedObject.SelectedItem = _comboboxFeature.CurrentItem;

        AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
    }

    #endregion
}