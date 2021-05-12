using System;
using System.Collections.Generic;
using System.Windows;
using Ariadna.Engine.Core;

namespace Ariadna.Engine.Transformation
{
    /// <summary>
    /// Манипулятор для одиночно-выделенной фигуры
    /// </summary>
    internal class SingleManipulator : ITransformManipulator
    {
        #region Private Fields

        private readonly IAriadnaEngine _ariadnaEngine;
        private readonly ISelectedFigure2D _figure;
        private readonly IManipulatorFigure _manipulatorFigure;
        private readonly IRectangleFigure _rectFigure;

        #endregion

        #region Events

        public event EventHandler<TransformActionEventArgs>? TransformEnded;

        #endregion

        #region Constructor

        public SingleManipulator(IAriadnaEngine ariadnaEngine, ISelectedFigure2D selectedFigure)
        {
            _ariadnaEngine = ariadnaEngine;
            _figure = selectedFigure;

            _manipulatorFigure = new ManipulatorFigure(_ariadnaEngine, selectedFigure.TransformAxis.IsRotate);

            GetCanvasRectParams(selectedFigure, out var width, out var height, out var center);

            _rectFigure = new RectangleFigure(_ariadnaEngine, !(selectedFigure is IPointFigure),false, width, height, center);

            _manipulatorFigure.Manipulate += ManipulatorFigure_Manipulate;
            _manipulatorFigure.ManipulateEnded += ManipulatorFigure_ManipulateEnded;

            _rectFigure.Manipulate += ManipulatorFigure_Manipulate;
            _rectFigure.ManipulateEnded += ManipulatorFigure_ManipulateEnded;

            _ariadnaEngine.CoordinateSystem.CoordinateChanged += CoordinateSystemCoordinateChanged;

            Update();
        }

        #endregion

        #region Public Methods

        public void Update()
        {
            _manipulatorFigure.Update(_figure.Transform);
            _rectFigure.Update();
        }

        public void Dispose()
        {
            _ariadnaEngine.CoordinateSystem.CoordinateChanged -= CoordinateSystemCoordinateChanged;

            _manipulatorFigure.Manipulate -= ManipulatorFigure_Manipulate;
            _manipulatorFigure.ManipulateEnded -= ManipulatorFigure_ManipulateEnded;

            _rectFigure.Manipulate -= ManipulatorFigure_Manipulate;
            _rectFigure.ManipulateEnded -= ManipulatorFigure_ManipulateEnded;

            _manipulatorFigure.Dispose();
            _rectFigure.Dispose();
        }

        #endregion

        #region Private Methods

        private void CoordinateSystemCoordinateChanged(object? sender, CoordinateChangedArgs e) => Update();

        private void ManipulatorFigure_ManipulateEnded(object? sender, ManipulateEventArgs e)
        {
            TransformEnded?.Invoke(this, new TransformActionEventArgs(e.Matrix,
                new List<ISelectedFigure2D> {_figure}));
        }

        private void ManipulatorFigure_Manipulate(object? sender, ManipulateEventArgs e)
        {
            var newMatrix = _figure.Transform;
            newMatrix.Append(e.Matrix);

            _figure.Transform = newMatrix;
            Update();

            if (sender is IManipulatorFigure)
                _rectFigure.ChangeTransform(e.Matrix);
        }


        private static void GetCanvasRectParams(ISelectedFigure2D figure, out double width,
            out double height, out Point center)
        {
            var borders = figure.GetCanvasBorders();

            var left = borders.Left;
            var right = borders.Right;
            var bottom = borders.Bottom;
            var top = borders.Top;

            Point rightBottom = new(right, bottom);
            Point leftTop = new(left, top);

            center = new((rightBottom.X + leftTop.X) / 2, (rightBottom.Y + leftTop.Y) / 2);

            width = right - left;
            height = top - bottom;
        }

        #endregion
    }
}