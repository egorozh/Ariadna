using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Ariadna.Engine.Core;
using Vector = System.Windows.Vector;

namespace AKIM.Engine.TwoD.Rotation
{
    internal sealed class RotationWorkspace : IRotationWorkspace
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

        internal readonly RotationManager RotationManager;
        internal PointFigure StartPoint;
        internal PointFigure SecondPoint;

        #endregion
        
        #region Constructor

        public RotationWorkspace(RotationManager rotationManager, List<PathGeometry> geometries)
        {
            RotationManager = rotationManager;

            _geometries = geometries;
            Init();

            ShowMessage(HelpMessages.SetRotationCenterPointMessage);
        }

        #endregion

        #region Public Methods

        public bool CanAccessRotating()
        {
            if (StartPoint == null)
            {
                ShowMessage(HelpMessages.CreateWarningRotationWithoutCenterPointMessage, true);
                return false;
            }

            if (SecondPoint == null)
            {
                ShowMessage(HelpMessages.CreateWarningRotationWithoutAngleMessage, true);
                return false;
            }

            return true;
        }
        
        public Point GetRotationCenter()
        {
            return StartPoint.Point;
        }

        public double GetRotationAngle()
        {
            var vec1 = new Vector(RotationManager.CoordinateSystem.GetCanvasPoint(GetCenterPoint(_geometries)).X,
                RotationManager.CoordinateSystem.GetCanvasPoint(GetCenterPoint(_geometries)).Y);
            var vec2 = new Vector(RotationManager.CoordinateSystem.GetCanvasPoint(StartPoint.Point).X,
                RotationManager.CoordinateSystem.GetCanvasPoint(StartPoint.Point).Y);
            var vec3 = new Vector(_magnetPointWithoutShiftMode.X, _magnetPointWithoutShiftMode.Y);
            var vec4 = Vector.Subtract(vec2, vec1);
            var vec5 = Vector.Subtract(vec2, vec3);

            var ang = Vector.AngleBetween(vec5, vec4);

            return ang;
        }
        
        public void Dispose()
        {
            RotationManager.SelectHelper.OnSelectedHelper();

            RotationManager.DrawingControl.Canvas.MouseLeftButtonDown -= Canvas_MouseLeftButtonDown;
            RotationManager.DrawingControl.Canvas.MouseMove -= Canvas_MouseMove;
            RotationManager.CoordinateSystem.CoordinateChanged -= CoordinateSystem_CoordinateChanged;
            RotationManager.FigureCollection.UnselectAllFigures();
            DisposePreview();
            RotationManager.DrawingControl.Canvas.RemoveElements();


            ShowMessage(null);

            StartPoint?.Dispose();
            SecondPoint?.Dispose();

            RotationManager.DrawingControl.Canvas.RemoveElements(_mousePoint);
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
                    RotationManager.CoordinateSystem.GetCanvasGeometry(geometry));

                shape.AddToCanvas(RotationManager.DrawingControl.Canvas);
                _previewGeometries.Add(shape);
            }
        }

        public void DisposePreview()
        {
            if (!_previewGeometries.Any())
                return;
            foreach (var shape in _previewGeometries)
            {
                RotationManager.DrawingControl.Canvas.RemoveElements(shape);
            }

            _previewGeometries = null;
        }

        private void UpdatePreviewGeometries()
        {
            var angle = GetRotationAngle();


            foreach (var geometry in _previewGeometries)
            {
                geometry.RenderTransform =
                    new RotateTransform(-angle, RotationManager.CoordinateSystem.GetCanvasPoint(GetRotationCenter()).X,
                        RotationManager.CoordinateSystem.GetCanvasPoint(GetRotationCenter()).Y);
            }
            
        }

        #endregion

        #region Internal Methods

        internal void CreateStartPoint(Point point)
        {
            StartPoint = CreateStartPointFigure(point, this);
            StartPoint.IsEdgePoint = true;
            _startPointSetted = true;
            ShowMessage(HelpMessages.SetRotationAngleMessage);
        }

        internal void RemoveStartPoint()
        {
            _startPointSetted = false;

            StartPoint?.Dispose();
            StartPoint = null;

            _mousePoint.Visibility = Visibility.Visible;

            ShowMessage(HelpMessages.SetRotationCenterPointMessage);
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

            ShowMessage(HelpMessages.SetRotationAngleMessage);
        }

        #endregion

        #region Private Methods

        private void Init()
        {
            RotationManager.DrawingControl.Canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
            RotationManager.DrawingControl.Canvas.MouseMove += Canvas_MouseMove;
            RotationManager.CoordinateSystem.CoordinateChanged += CoordinateSystem_CoordinateChanged;
            _previewGeometries = new List<Path>();
            _mousePoint = ShapeFactory.CreateMouseCreatePoint();
            _mousePoint.AddToCanvas(RotationManager.DrawingControl.Canvas);
        }

        private void CoordinateSystem_CoordinateChanged(object sender, CoordinateChangedArgs e)
        {
            StartPoint?.Update();
            SecondPoint?.Update();

            if (_previewGeometries!= null && _previewGeometries.Any())
            {
                for (var index = 0; index < _previewGeometries.Count; index++)
                {
                    var geometry = _previewGeometries[index];
                    geometry.Data = RotationManager.CoordinateSystem.GetCanvasGeometry(_geometries[index]);
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

        private static Point GetCenterPoint(List<PathGeometry> geometries)
        {
            var selectedBorders = new Bounds(double.MaxValue, double.MinValue, double.MinValue, double.MaxValue);

            for (var i = 0; i < geometries.Count; i++)
            {
                var bounds = geometries[i].Bounds;

                var borders = new Bounds(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom);

                borders.OverrideBorders(ref selectedBorders);
            }

            var left = selectedBorders.Left;
            var right = selectedBorders.Right;
            var bottom = selectedBorders.Bottom;
            var top = selectedBorders.Top;

            return new Point((left + right) / 2, (bottom + top) / 2);
        }

        #endregion

        #region Mouse Actions

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_startPointSetted)
            {
                Actions.AddStartPoint(this, _magnetPointWithoutShiftMode);
                ShowPreview(_geometries);
            }
            else if (!_secondPointSetted)
            {
                Actions.AddSecondPoint(this, _magnetPointWithoutShiftMode);
                RotationManager.AccessRotating();
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(RotationManager.DrawingControl.Canvas);

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

        #endregion


        #region Helpers

        private void ShowMessage(string message, bool isWarning = false) =>
            RotationManager.DrawingControl.ShowMessage(message, isWarning);

        private Point CalcMagnetPoint(Point point, bool isShiftMode = false, Point prevPoint = new Point()) =>
            point.GetMagnetPoint(RotationManager.Settings, RotationManager.GridChart,
                RotationManager.FigureCollection, RotationManager.CoordinateSystem,
                isShiftMode && Keyboard.IsKeyDown(Key.LeftShift), prevPoint);

        #endregion

        #region Factory

        private static PointFigure CreateStartPointFigure(Point startPoint, RotationWorkspace workspace)
        {
            return new PointFigure(startPoint, workspace);
        }

        #endregion
    }

    internal class HelpMessages
    {
        public static string SetRotationCenterPointMessage { get; } = "Укажите центральную точку для линии отражения фигур";

        public static string SetRotationAngleMessage { get; } = "Задайте угол отражения фигур";

        public static string CreateWarningRotationWithoutCenterPointMessage { get; } =
            "Нельзя отразить фигуры, не указав центральную точку";

        public static string CreateWarningRotationWithoutAngleMessage { get; } =
            "Нельзя отразить фигуры, не задав угол отражения";
    }
}