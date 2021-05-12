using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Ariadna.Engine.Core;

namespace Ariadna.Engine.Transformation
{
    /// <summary>
    /// Манипулятор для трансформирования группы фигур
    /// </summary>
    internal class GroupManipulator : ITransformManipulator
    {
        #region Private Fields

        private readonly IAriadnaEngine _ariadnaEngine;
        private readonly List<ISelectedFigure2D> _selectedFigures;
        private readonly IRectangleFigure _rectangleFigure;
        private Matrix _transform;

        #endregion

        #region Events

        public event EventHandler<TransformActionEventArgs>? TransformEnded;

        #endregion

        #region Constructor

        public GroupManipulator(IAriadnaEngine ariadnaEngine, List<ISelectedFigure2D> selectedFigures)
        {
            _ariadnaEngine = ariadnaEngine;
            _selectedFigures = selectedFigures;

            GetCanvasRectParams(selectedFigures, out var width, out var height, out var center);

            _rectangleFigure = new RectangleFigure(ariadnaEngine, true, true, width, height, center);

            _rectangleFigure.Manipulate += ManipulatorFigure_Manipulate;
            _rectangleFigure.ManipulateEnded += ManipulatorFigure_ManipulateEnded;

            _ariadnaEngine.CoordinateSystem.CoordinateChanged += CoordinateSystemCoordinateChanged;

            Update();
        }

        #endregion

        #region Public Methods

        public void Update() => _rectangleFigure.Update();

        public void Dispose()
        {
            _ariadnaEngine.CoordinateSystem.CoordinateChanged -= CoordinateSystemCoordinateChanged;

            _rectangleFigure.Manipulate -= ManipulatorFigure_Manipulate;
            _rectangleFigure.ManipulateEnded -= ManipulatorFigure_ManipulateEnded;

            _rectangleFigure.Dispose();
        }

        #endregion

        #region Private Methods

        private void ManipulatorFigure_ManipulateEnded(object? sender, ManipulateEventArgs e)
        {
            TransformEnded?.Invoke(this, new TransformActionEventArgs(e.Matrix, _selectedFigures));
        }

        private void ManipulatorFigure_Manipulate(object? sender, ManipulateEventArgs e)
        {
            _transform = AppendMatrix(_transform, e.Matrix);

            foreach (var figure in _selectedFigures)
                figure.Transform = AppendMatrix(figure.Transform, e.Matrix);

            Update();
        }

        private Matrix AppendMatrix(Matrix initMatrix, Matrix newMatrix)
        {
            initMatrix.Append(newMatrix);

            return initMatrix;
        }

        private void CoordinateSystemCoordinateChanged(object? sender, CoordinateChangedArgs e) => Update();

        private static void GetCanvasRectParams(List<ISelectedFigure2D> selectedFigures, out double width,
            out double height, out Point center)
        {
            var selectedBorders = new Bounds(double.MaxValue, double.MinValue, double.MinValue, double.MaxValue);

            for (var i = 0; i < selectedFigures.Count; i++)
            {
                var borders = (selectedFigures[i]).GetCanvasBorders();

                borders.OverrideBorders(ref selectedBorders);
            }

            var left = selectedBorders.Left;
            var right = selectedBorders.Right;
            var bottom = selectedBorders.Bottom;
            var top = selectedBorders.Top;

            Point rightBottom = new(right, bottom);
            Point leftTop = new(left, top);

            center = new((rightBottom.X + leftTop.X) / 2, (rightBottom.Y + leftTop.Y) / 2);

            width = right - left;
            height = top - bottom;
        }

        #endregion
    }
}