using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ariadna.Engine.Core
{
    public interface ICanvas : IFrameworkInputElement, IEngineComponent
    {
        event MouseButtonEventHandler MouseUp;
        event MouseButtonEventHandler MouseDown;

        UIElementCollection Children { get; }
        double ActualWidth { get; }
        double ActualHeight { get; }
        event SizeChangedEventHandler SizeChanged;

        /// <summary>
        /// Добавление элементов на <see cref="Canvas"/>
        /// </summary>
        /// <param name="elements"></param>
        void AddElements(params UIElement[] elements);

        /// <summary>
        /// Удаление элементов с <see cref="Canvas"/>
        /// </summary>
        /// <param name="elements"></param>
        void RemoveElements(params UIElement?[] elements);
    }

    public interface INavigationGrid : ICanvas
    {
    }
}