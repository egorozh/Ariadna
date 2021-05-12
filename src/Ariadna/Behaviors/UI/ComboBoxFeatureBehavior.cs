using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace Ariadna
{
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
            AssociatedObject.SelectedItem = _comboboxFeature.SelectedItem;
            AssociatedObject.ItemTemplate = _comboboxFeature.ItemTemplate;
            AssociatedObject.IsEnabled = _comboboxFeature.IsEnabled;

            _comboboxFeature.IsEnabledChanged += ComboboxFeature_IsEnabledChanged;
            _comboboxFeature.SelectionChanged += ComboboxFeature_SelectionChanged;
            _comboboxFeature.CurrentChangedEvent += ComboboxFeature_CurrentChangedEvent;

            AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        }

        #endregion

        #region Private Methods

        private void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _comboboxFeature.SelectedItem = AssociatedObject.SelectedItem;
        }

        private void ComboboxFeature_SelectionChanged(object sender, System.EventArgs e)
        {
            AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;

            AssociatedObject.SelectedItem = _comboboxFeature.SelectedItem;

            AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        }

        private void ComboboxFeature_IsEnabledChanged(object sender, System.EventArgs e)
        {
            AssociatedObject.IsEnabled = _comboboxFeature.IsEnabled;
        }

        private void ComboboxFeature_CurrentChangedEvent(object sender, System.EventArgs e)
        {
            AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;

            AssociatedObject.SelectedItem = _comboboxFeature.SelectedItem;

            AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        }

        #endregion
    }
}