using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace Ariadna
{
    public class InterfaceFeatureBehavior : Behavior<FrameworkElement>
    {
        #region Public Properties

        public IInterfaceFeature Feature { get; }

        #endregion
        
        #region Constructor

        public InterfaceFeatureBehavior(IInterfaceFeature feature) => Feature = feature;

        #endregion

        #region Protected Methods

        protected override void OnAttached()
        {
            AssociatedObject.Visibility = Feature.IsShow ? Visibility.Visible : Visibility.Collapsed;

            Feature.IsShowChanged += Feature_IsShowChanged;

            AssociatedObject.Loaded += AssociatedObjectOnLoaded;
        }

        protected override void OnDetaching()
        {
            Feature.IsShowChanged -= Feature_IsShowChanged;
            AssociatedObject.Loaded -= AssociatedObjectOnLoaded;
        }

        #endregion

        #region Private Methods

        private void AssociatedObjectOnLoaded(object sender, RoutedEventArgs e)
        {
            if (Feature is ICommandFeature commandFeature)
                commandFeature.Update();
        }

        private void Feature_IsShowChanged(object? sender, System.EventArgs e)
        {
            AssociatedObject.Visibility = Feature.IsShow ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion
    }
}