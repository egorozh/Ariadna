using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ariadna.Engine.Core
{
    public interface IGridCanvas : IEngineComponent
    {
        double ActualWidth { get; }
        double ActualHeight { get; }

        void ShowGrid();
        void HideGrid();

        double DpiInOneSquare { get; }

        event SizeChangedEventHandler SizeChanged;

        void UpdateGridBrush(double deltaX, double deltaY, EngineSettings engineSettings, decimal resolution,
            ICoordinateSystem coordinateSystem, double angle);
        
        /// <summary>
        /// Добавление элементов на <see cref="Canvas"/>
        /// </summary>
        /// <param name="elements"></param>
        void AddElements(params UIElement[] elements);

        /// <summary>
        /// Удаление элементов с <see cref="Canvas"/>
        /// </summary>
        /// <param name="elements"></param>
        void RemoveElements(params UIElement[] elements);

        void UpdateMajorPen(SolidColorBrush majorGridLineColor);
        void UpdateMinorPen(SolidColorBrush minorGridLineColor);
    }
}