using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Ariadna.Engine.Core;

namespace Ariadna.Engine.Transformation
{
    /// <summary>
    /// Фабрика создания примитивов для движка
    /// </summary>
    internal class ShapeFactory
    {
        /// <summary>
        /// Создание главного прямоугольника для редактирования фигуры
        /// </summary>
        /// <returns></returns>
        public static Path CreateEditMainRectangle()
        {
            Path shape = new()
            {
                Stroke = Brushes.DeepSkyBlue,
                StrokeThickness = 1,
                Fill = Brushes.Transparent,
                RenderTransformOrigin = new Point(0.5, 0.5),
            };
            shape.SetZIndex((int) ZOrder.EditRectangle);

            return shape;
        }

        /// <summary>
        /// Создание центральной точки внутри прямоугольника для редактирования фигуры
        /// </summary>
        /// <returns></returns>
        public static Ellipse CreateEditCenterPoint()
        {
            var shape = new Ellipse()
            {
                Stroke = Brushes.DeepSkyBlue,
                Fill = Brushes.White,
                StrokeThickness = 1,
                Width = 6,
                Height = 6,
                RenderTransformOrigin = new Point(0.5, 0.5)
            };
            shape.SetZIndex((int) ZOrder.EditRectanglePoint);

            return shape;
        }

        /// <summary>
        /// Создание точки для редактирования точечных фигур
        /// </summary>
        /// <returns></returns>
        public static Ellipse CreateEditPoint()
        {
            var shape = new Ellipse()
            {
                Stroke = Brushes.DeepSkyBlue,
                Fill = Brushes.White,
                StrokeThickness = 1,
                Width = 10,
                Height = 10,
                RenderTransformOrigin = new Point(0.5, 0.5)
            };
            shape.SetZIndex((int) ZOrder.EditRectanglePoint);

            return shape;
        }

        /// <summary>
        /// Создание угловых точек прямоугольника для редактирования фигуры
        /// </summary>
        /// <returns></returns>
        public static Rectangle CreateEditRectanglePoint()
        {
            var shape = new Rectangle
            {
                Stroke = Brushes.DeepSkyBlue,
                Fill = Brushes.White,
                StrokeThickness = 1,
                Width = 8,
                Height = 8,
                RenderTransformOrigin = new Point(0.5, 0.5)
            };
            shape.SetZIndex((int) ZOrder.EditRectanglePoint);

            return shape;
        }

        /// <summary>
        /// Создание фигуры предпросмотра при трансформации фигуры
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static Path CreatePreviewFigure(Geometry geometry)
        {
            var shape = new Path
            {
                Stroke = Brushes.LawnGreen,
                StrokeLineJoin = PenLineJoin.Bevel,

                Data = geometry,
            };

            shape.SetZIndex((int) ZOrder.EditPreviewFigure);

            return shape;
        }

        /// <summary>
        /// Создание курсора поворота
        /// </summary>
        /// <returns></returns>
        public static Path CreateRotateCursor()
        {
            var cursor = new Path
            {
                Data = Geometry.Parse(
                    "M3.2,1.8L5,0L0,0L0,5L1.8,3.2A7.5,7.5,0,0,1,1.8,16.8L0,15L0,20L5,20L3.2,18.2A9.2,9.2,0,0,0,3.2,1.8z"),
                Fill = Brushes.White,
                Stroke = Brushes.Black,
                Width = 8.229,
                Height = 20,
                StrokeThickness = 0.5,
                RenderTransformOrigin = new(0.5, 0.5),
                Visibility = Visibility.Collapsed,
            };

            Panel.SetZIndex(cursor, 2000);

            return cursor;
        }

        public static Path CreateScaleCursor()
        {
            var cursor = new Path
            {
                Data = Geometry.Parse(
                    "M0,5L5,0L5,4L22,4L22,0L27,5L22,10L22,6L5,6L5,10z"),
                Fill = Brushes.White,
                Stroke = Brushes.Black,
                Width = 27,
                Height = 10,
                StrokeThickness = 0.5,
                RenderTransformOrigin = new(0.5, 0.5),
                Visibility = Visibility.Collapsed,
            };

            Panel.SetZIndex(cursor, 2000);

            return cursor;
        }

        public static Ellipse CreatePreviewPointShape(double x, double y)
        {
            var shape = new Ellipse()
            {
                Stroke = Brushes.LawnGreen,
                Fill = Brushes.LightGreen,
                StrokeThickness = 1,
                Width = 12,
                Height = 12,
                RenderTransformOrigin = new Point(0.5, 0.5)
            };
            shape.SetPosition(x, y);

            shape.SetZIndex((int) ZOrder.EditPreviewFigure);

            return shape;
        }

        public static Path CreateArrow()
        {
            var arrow = new Path
            {
                Data = Geometry.Parse(
                    "M 0,-1 L0,1 L22,1 L22,4 L30,0 L22,-4 L22,-1 z"),
                Fill = Brushes.White,
                Stroke = Brushes.Black,
                StrokeThickness = 0.5,
            };

            Panel.SetZIndex(arrow, 2000);

            return arrow;
        }

        public static Path CreateRotateArc()
        {
            var rotateArc = new Path
            {
                Data = Geometry.Parse(
                    "M1,-20A20,20,0,0,1,20,-1L18,-1A18,18,0,0,0,1,-18"),
                Fill = Brushes.White,
                Stroke = Brushes.Black,
                StrokeThickness = 0.5,
            };

            Panel.SetZIndex(rotateArc, 2000);

            return rotateArc;
        }

        public static Path CreateXTriangle()
        {
            var arrow = new Path
            {
                Data = Geometry.Parse(
                    "M 30,-30 L30,-22 L22,-22 Z"),
                Fill = Brushes.White,
                Stroke = Brushes.Black,
                StrokeThickness = 0,
            };

            Panel.SetZIndex(arrow, 2000);

            return arrow;
        }

        public static Path CreateYTriangle()
        {
            var arrow = new Path
            {
                Data = Geometry.Parse(
                    "M 30,-30 L22,-30 L22,-22 Z"),
                Fill = Brushes.White,
                Stroke = Brushes.Black,
                StrokeThickness = 0,
            };

            Panel.SetZIndex(arrow, 2000);

            return arrow;
        }
    }
}