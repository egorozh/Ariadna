using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Ariadna.Engine.Core;
using Vector = System.Windows.Vector;

namespace AKIM.Engine.TwoD.Reflection
{
    internal sealed class ReflectionWorkspace : IReflectionWorkspace
    {
        #region Private Fields

        /// <summary>
        /// Примитив указателя мыши, отображаемый либо в точках привязки либо в координатах указателя мыши
        /// в зависимости от включенных режимов примагничивания
        /// </summary>
        private Rectangle _mousePoint;

        private Point _magnetPointWithoutShiftMode;
        private bool _startPointSetted;
        private bool _secondPointSetted;
        private List<Path> _previewGeometries;
        private List<PathGeometry> _geometries;

        #endregion

        #region Internal Fields

        internal readonly ReflectionManager ReflectionManager;
        internal PointFigure StartPoint;
        internal PointFigure SecondPoint;
        internal Line ReflectionLine;

        #endregion

        #region Constructor

        public ReflectionWorkspace(ReflectionManager reflectionManager, List<PathGeometry> geometries)
        {
            ReflectionManager = reflectionManager;

            _geometries = geometries;
            Init();

            ShowMessage(HelpMessages.SetReflectionCenterPointMessage);
        }

        #endregion

        #region Public Methods

        public bool CanAccessReflecting()
        {
            if (StartPoint == null)
            {
                ShowMessage(HelpMessages.CreateWarningReflectionWithoutCenterPointMessage, true);
                return false;
            }

            if (SecondPoint == null)
            {
                ShowMessage(HelpMessages.CreateWarningReflectionWithoutAngleMessage, true);
                return false;
            }

            return true;
        }

        public Point GetReflectionCenter()
        {
            return ReflectionManager.CoordinateSystem.GetCanvasPoint(StartPoint.Point);
        }

        public double GetReflectionAngle()
        {
            var vec = new Vector(0, 0);
            var vec1 = new Vector(1, 0);

            var vecToPoint = new Vector(GetReflectionCenter().X, GetReflectionCenter().Y);
            var vecToMouse = new Vector(_magnetPointWithoutShiftMode.X, _magnetPointWithoutShiftMode.Y);
            var vecOX = Vector.Subtract(vec, vec1);
            var vecFromPointToMouse = Vector.Subtract(vecToPoint, vecToMouse);

            var ang = Vector.AngleBetween(vecFromPointToMouse, vecOX);

            return ang;
        }

        public void Dispose()
        {
            ReflectionManager.SelectHelper.OnSelectedHelper();

            ReflectionManager.DrawingControl.Canvas.MouseLeftButtonDown -= Canvas_MouseLeftButtonDown;
            ReflectionManager.DrawingControl.Canvas.MouseMove -= Canvas_MouseMove;
            ReflectionManager.CoordinateSystem.CoordinateChanged -= CoordinateSystem_CoordinateChanged;
            ReflectionManager.FigureCollection.UnselectAllFigures();
            DisposePreview();
            ReflectionManager.DrawingControl.Canvas.RemoveElements();


            ShowMessage(null);

            StartPoint?.Dispose();
            SecondPoint?.Dispose();

            ReflectionManager.DrawingControl.Canvas.RemoveElements(_mousePoint);
            ReflectionManager.DrawingControl.Canvas.RemoveElements(ReflectionLine);
        }

        public void ShowPreview(List<PathGeometry> geometries)
        {
            _geometries = new List<PathGeometry>();

            foreach (var geometry in geometries)
                _geometries.Add(PathGeometry.CreateFromGeometry(geometry));

            _previewGeometries = new List<Path>();

            foreach (var geometry in _geometries)
            {
                var shape = ShapeFactory.CreatePreviewFigure(
                    ReflectionManager.CoordinateSystem.GetCanvasGeometry(geometry));

                shape.RenderTransform = new ScaleTransform(1, -1, GetReflectionCenter().X, GetReflectionCenter().Y);

                shape.AddToCanvas(ReflectionManager.DrawingControl.Canvas);
                _previewGeometries.Add(shape);
            }
        }

        public void DisposePreview()
        {
            if (!_previewGeometries.Any())
                return;
            foreach (var shape in _previewGeometries)
            {
                ReflectionManager.DrawingControl.Canvas.RemoveElements(shape);
            }

            _previewGeometries = null;
        }

        private void UpdateReflectionLine()
        {
            if (ReflectionManager.CoordinateSystem.Resolution > ReflectionManager.Settings.MinResolution &&
                Math.Abs(GetReflectionCenter().Y - _magnetPointWithoutShiftMode.Y) > 0.1)
            {
                var point1 = Intersection(GetReflectionCenter(), _magnetPointWithoutShiftMode,
                    new Point(0, 0), new Point(10, 0));
                var point2 = Intersection(GetReflectionCenter(), _magnetPointWithoutShiftMode,
                    new Point(0, 1000), new Point(10, 1000));
                ReflectionLine.SetPoints(point1.X, point1.Y, point2.X, point2.Y);
            }
            else
            {
                ReflectionLine.SetPoints(-100, GetReflectionCenter().Y, 5000, GetReflectionCenter().Y);
            }
        }

        private void UpdatePreviewGeometries()
        {
            UpdateReflectionLine();

            foreach (var geometry in _previewGeometries)
            {
                var point = GetReflectionCenter();
                double angle = GetReflectionAngle();

                RotateTransform rt1 = new RotateTransform();
                rt1.CenterX = point.X;
                rt1.CenterY = point.Y;
                rt1.Angle = angle;

                ScaleTransform st = new ScaleTransform();
                st.CenterX = point.X;
                st.CenterY = point.Y;
                st.ScaleY = -1;

                RotateTransform rt2 = new RotateTransform();
                rt2.CenterX = point.X;
                rt2.CenterY = point.Y;
                rt2.Angle = -angle;

                TransformGroup myTransformGroup = new TransformGroup();

                myTransformGroup.Children.Add(rt1);
                myTransformGroup.Children.Add(st);
                myTransformGroup.Children.Add(rt2);

                geometry.RenderTransform = myTransformGroup;
            }
        }

        #endregion

        #region Internal Methods

        internal void CreateStartPoint(Point point)
        {
            StartPoint = CreateStartPointFigure(point, this);
            StartPoint.IsEdgePoint = true;
            _startPointSetted = true;
        }

        internal void RemoveStartPoint()
        {
            _startPointSetted = false;
            StartPoint?.Dispose();
            StartPoint = null;
            _mousePoint.Visibility = Visibility.Visible;
            ShowMessage(HelpMessages.SetReflectionCenterPointMessage);
        }

        internal void CreateSecondPoint(Point point)
        {
            SecondPoint = CreateStartPointFigure(point, this);
            SecondPoint.IsEdgePoint = true;
            _mousePoint.Visibility = Visibility.Collapsed;
            _secondPointSetted = true;
        }

        internal void RemoveSecondPoint()
        {
            _secondPointSetted = false;
            SecondPoint?.Dispose();
            SecondPoint = null;
            _mousePoint.Visibility = Visibility.Visible;
            ShowMessage(HelpMessages.SetReflectionAngleMessage);
        }

        internal void CreateReflectionLine()
        {
            ReflectionLine = ShapeFactory.CreateEditLineSegment(GetReflectionCenter(), _magnetPointWithoutShiftMode);
            ReflectionLine.AddToCanvas(ReflectionManager.DrawingControl.Canvas);
            ReflectionLine.SetPoints(-1000, GetReflectionCenter().Y, 3000, GetReflectionCenter().Y);
        }

        #endregion

        #region Private Methods

        private void Init()
        {
            ReflectionManager.DrawingControl.Canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
            ReflectionManager.DrawingControl.Canvas.MouseMove += Canvas_MouseMove;
            ReflectionManager.CoordinateSystem.CoordinateChanged += CoordinateSystem_CoordinateChanged;
            _previewGeometries = new List<Path>();
            _mousePoint = ShapeFactory.CreateMouseCreatePoint();
            _mousePoint.AddToCanvas(ReflectionManager.DrawingControl.Canvas);
        }

        private void CoordinateSystem_CoordinateChanged(object sender, CoordinateChangedArgs e)
        {
            StartPoint?.Update();
            SecondPoint?.Update();

            if (_previewGeometries != null && _previewGeometries.Any())
            {
                for (var index = 0; index < _previewGeometries.Count; index++)
                {
                    var geometry = _previewGeometries[index];
                    geometry.Data = ReflectionManager.CoordinateSystem.GetCanvasGeometry(_geometries[index]);
                }

                UpdatePreviewGeometries();
            }
        }

        private void CalcMagnetPoints(Point canvasPosition)
        {
            _magnetPointWithoutShiftMode = CalcMagnetPoint(canvasPosition);

            var prevPoint = new Point();

            if (StartPoint != null)
            {
                prevPoint = StartPoint.Point;
            }
        }

        #endregion

        #region Mouse Actions

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_startPointSetted)
            {
                Actions.AddStartPoint(this, _magnetPointWithoutShiftMode);
                CreateReflectionLine();
                ShowPreview(_geometries);
            }
            else if (!_secondPointSetted)
            {
                Actions.AddSecondPoint(this, _magnetPointWithoutShiftMode);
                ReflectionManager.AccessReflecting();
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(ReflectionManager.DrawingControl.Canvas);

            // if (!_startPointSetted)
            CalcMagnetPoints(pos);
            /*else
                _magnetPointWithoutShiftMode = pos;*/

            if (!_startPointSetted || !_secondPointSetted)
                _mousePoint.SetPosition(_magnetPointWithoutShiftMode);


            if (_startPointSetted)
            {
                UpdatePreviewGeometries();
            }
        }

        public Point Intersection(Point firstLineStartPoint, Point firstLineSecondPoint, Point secondLineStartPoint,
            Point secondLineSecondPoint)
        {
            double x0 = firstLineStartPoint.X;
            double y0 = firstLineStartPoint.Y;
            double firstLineWidth = firstLineSecondPoint.X - firstLineStartPoint.X;
            double firstLineHeight = firstLineSecondPoint.Y - firstLineStartPoint.Y;

            double x1 = secondLineStartPoint.X;
            double y1 = secondLineStartPoint.Y;
            double secondLineWidth = secondLineSecondPoint.X - secondLineStartPoint.X;
            double secondLineHeight = secondLineSecondPoint.Y - secondLineStartPoint.Y;

            double x = (x0 * firstLineHeight * secondLineWidth - x1 * secondLineHeight * firstLineWidth -
                        y0 * firstLineWidth * secondLineWidth + y1 * firstLineWidth * secondLineWidth) /
                       (firstLineHeight * secondLineWidth - secondLineHeight * firstLineWidth);
            double y = (y0 * firstLineWidth * secondLineHeight - y1 * secondLineWidth * firstLineHeight -
                        x0 * firstLineHeight * secondLineHeight + x1 * firstLineHeight * secondLineWidth) /
                       (firstLineWidth * secondLineHeight - secondLineWidth * firstLineHeight);


            return new Point(x, y);
        }

        #endregion


        #region Helpers

        private void ShowMessage(string message, bool isWarning = false) =>
            ReflectionManager.DrawingControl.ShowMessage(message, isWarning);

        private Point CalcMagnetPoint(Point point, bool isShiftMode = false, Point prevPoint = new Point()) =>
            point.GetMagnetPoint(ReflectionManager.Settings, ReflectionManager.GridChart,
                ReflectionManager.FigureCollection, ReflectionManager.CoordinateSystem,
                isShiftMode && Keyboard.IsKeyDown(Key.LeftShift), prevPoint);

        #endregion

        #region Factory

        private static PointFigure CreateStartPointFigure(Point startPoint, ReflectionWorkspace workspace)
        {
            return new PointFigure(startPoint, workspace);
        }

        #endregion
    }

    internal class HelpMessages
    {
        public static string SetReflectionCenterPointMessage { get; } = "Укажите центральную точку для поворота фигур";

        public static string SetReflectionAngleMessage { get; } = "Задайте угол для поворота фигур";

        public static string CreateWarningReflectionWithoutCenterPointMessage { get; } =
            "Нельзя повернуть фигуры, не указав центральную точку";

        public static string CreateWarningReflectionWithoutAngleMessage { get; } =
            "Нельзя повернуть фигуры, не задав угол поворота";
    }
}