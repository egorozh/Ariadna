using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using AKIM.Maths;
using Ariadna.Engine.Core;

namespace Ariadna.Engine
{
    /// <inheritdoc />
    internal class PasteHelper : IPasteHelper
    {
        #region Private Fields

        private readonly IAriadnaEngine _ariadnaEngine;

        private List<PathGeometry> _geometries;
        private Point _center;
        private List<Path> _previewGeometries;

        #endregion

        #region Events

        public event EventHandler<PasteResultEventArgs> PasteEnded;

        #endregion

        #region Constructor

        /// <summary>
        /// Дефолтный конструктор
        /// </summary>
        public PasteHelper(IAriadnaEngine ariadnaEngine)
        {
            _ariadnaEngine = ariadnaEngine;
        }

        #endregion

        #region Public Methods

        public void Init()
        {
        }

        public void Paste(List<string> geometries)
        {
            _ariadnaEngine.Figures.UnselectAllFigures();
            _ariadnaEngine.SelectHelper.OffSelectedHelper();

            _geometries = new List<PathGeometry>();

            foreach (var geometry in geometries)
                _geometries.Add(PathGeometry.CreateFromGeometry(Geometry.Parse(geometry)));

            _center = GetCenterPoint(_geometries);

            _previewGeometries = new List<Path>();

            foreach (var geometry in _geometries)
            {
                var shape = ShapeFactory.CreatePreviewFigure(_ariadnaEngine.CoordinateSystem.GetCanvasGeometry(geometry));

                shape.AddToCanvas(_ariadnaEngine.Canvas);
                _previewGeometries.Add(shape);
            }

            _ariadnaEngine.Canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
            _ariadnaEngine.Canvas.MouseMove += Canvas_MouseMove;

            _ariadnaEngine.CoordinateSystem.CoordinateChanged += CoordinateSystemCoordinateChanged;

            UpdatePreviewGeometries(Mouse.GetPosition(_ariadnaEngine.Canvas));
        }

        public void Cancel()
        {
            PasteEnded?.Invoke(this, new PasteResultEventArgs(false, null));

            UnSubscribe();
        }

        #endregion

        #region Private Methods

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var canvasPoint = e.GetPosition(_ariadnaEngine.Canvas);

            UpdatePreviewGeometries(canvasPoint);
        }

        private void UpdatePreviewGeometries(Point canvasPoint)
        {
            var center = _ariadnaEngine.CoordinateSystem.GetCanvasPoint(_center);

            foreach (var geometry in _previewGeometries)
                geometry.RenderTransform =
                    new TranslateTransform(canvasPoint.X - center.X, canvasPoint.Y - center.Y);
        }


        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var geometries = new List<string>();

            foreach (var path in _previewGeometries)
            {
                var translateTransform = (TranslateTransform) path.RenderTransform;

                var geometry = ((PathGeometry) path.Data).Move(translateTransform.X, translateTransform.Y);

                geometries.Add(_ariadnaEngine.CoordinateSystem.GetGlobalGeometry(geometry).ToString(CultureInfo.InvariantCulture));
            }


            PasteEnded?.Invoke(this, new PasteResultEventArgs(true, geometries));

            UnSubscribe();
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

        private void UnSubscribe()
        {
            _ariadnaEngine.Canvas.MouseLeftButtonDown -= Canvas_MouseLeftButtonDown;
            _ariadnaEngine.Canvas.MouseMove -= Canvas_MouseMove;
            _ariadnaEngine.CoordinateSystem.CoordinateChanged -= CoordinateSystemCoordinateChanged;

            _ariadnaEngine.SelectHelper.OnSelectedHelper();

            foreach (var geometry in _previewGeometries)
            {
                _ariadnaEngine.Canvas.RemoveElements(geometry);
            }
        }

        /// <summary>
        /// Происходит при изменении системы координат
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CoordinateSystemCoordinateChanged(object sender, CoordinateChangedArgs e)
        {
            if (_previewGeometries != null)
            {
                for (var index = 0; index < _previewGeometries.Count; index++)
                {
                    var geometry = _previewGeometries[index];
                    geometry.Data = _ariadnaEngine.CoordinateSystem.GetCanvasGeometry(_geometries[index]);
                }
            }
        }

        #endregion
    }
}