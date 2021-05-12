﻿using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Ariadna.Engine.Core;

namespace AKIM.Engine.TwoD.Rotation
{
    /// <summary>
    /// Фабрика создания примитивов для движка
    /// </summary>
    internal class ShapeFactory
    {
        /// <summary>
        /// Создание значка указателя мыши для режима создания фигур
        /// </summary>
        /// <returns></returns>
        public static Rectangle CreateMouseCreatePoint()
        {
            var point = new Rectangle()
            {
                Fill = new SolidColorBrush(Color.FromArgb(70, 0, 0, 170)),
                Stroke = Brushes.DeepSkyBlue,
                Width = 10,
                Height = 10
            };

            point.SetZIndex((int) ZOrder.SelectedRectangle);

            return point;
        }

        public static Ellipse CreateEditNodePoint()
        {
            var shape = new Ellipse()
            {
                Stroke = Brushes.DodgerBlue,
                Fill = Brushes.DodgerBlue,
                StrokeThickness = 1,
                Width = 10,
                Height = 10
            };
            shape.SetZIndex((int) ZOrder.EditNodePoint);

            return shape;
        }
        public static Line CreateEditLineSegment(Point startPoint,Point endPoint)
        {
            var line = new Line
            {
                X1 = startPoint.X,
                Y1 = startPoint.Y,
                X2 = endPoint.X,
                Y2 = endPoint.Y,
                Stroke = Brushes.DodgerBlue,
                StrokeThickness = 1.5,
                StrokeDashArray = new DoubleCollection() {4, 4}
            };
            
            line.SetZIndex((int) ZOrder.EditSegment);
            return line;
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

            shape.SetZIndex((int)ZOrder.EditPreviewFigure);

            return shape;
        }
    }
}