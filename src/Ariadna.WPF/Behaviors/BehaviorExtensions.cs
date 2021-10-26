using Microsoft.Xaml.Behaviors;
using System.Windows;

namespace Ariadna;

public static class BehaviorExtensions
{
    public static void AddBehavior(this DependencyObject dependencyObject, Behavior behavior)
    {
        var behaviors = Interaction.GetBehaviors(dependencyObject);

        behaviors.Add(behavior);
    }

    public static T? GetBehavior<T>(this DependencyObject dependencyObject) where T : Behavior
    {
        var behaviors = Interaction.GetBehaviors(dependencyObject);

        return behaviors.FirstOrDefault(b => b.GetType() == typeof(T)) as T;
    }

    public static void ClearBehaviors(this DependencyObject dependencyObject)
    {
        var behaviors = Interaction.GetBehaviors(dependencyObject);

        behaviors.Clear();
    }
}