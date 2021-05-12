using System.Windows;
using Fluent;
using Microsoft.Xaml.Behaviors;

namespace Ariadna
{
    internal class RibbonBehavior : Behavior<Ribbon>
    {
        public static readonly DependencyProperty RibbonManagerProperty = DependencyProperty.Register(
            nameof(RibbonManager), typeof(RibbonManager), typeof(RibbonBehavior),
            new PropertyMetadata(default(RibbonManager)));
        
        public RibbonManager RibbonManager
        {
            get => (RibbonManager) GetValue(RibbonManagerProperty);
            set => SetValue(RibbonManagerProperty, value);
        }
        
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            RibbonManager.Init(AssociatedObject.Tabs);
        }
    }
}