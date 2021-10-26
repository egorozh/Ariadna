using Microsoft.Xaml.Behaviors;
using System.Windows;

namespace Ariadna;

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
        base.OnAttached();

        AssociatedObject.Visibility = Feature.IsShow ? Visibility.Visible : Visibility.Collapsed;

        Feature.IsShowChanged += Feature_IsShowChanged;
    }


    protected override void OnDetaching()
    {
        base.OnDetaching();

        Feature.IsShowChanged -= Feature_IsShowChanged;
    }

    #endregion

    #region Private Methods
        
    private void Feature_IsShowChanged(object? sender, EventArgs e)
    {
        AssociatedObject.Visibility = Feature.IsShow ? Visibility.Visible : Visibility.Collapsed;
    }

    #endregion
}