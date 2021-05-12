using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using Ariadna.Engine.Core;

namespace Ariadna.Engine.PointCreator
{
    internal sealed class PointWorkspace : IPointWorkspace
    {
        #region Private Fields

        /// <summary>
        /// Примитив указателя мыши, отображаемый либо в точках привязки либо в координатах указателя мыши
        /// в зависимости от включенных режимов примагничивания
        /// </summary>
        private Rectangle _mousePoint;

        private Point _magnetPointWithoutShiftMode;
        private bool _startPointSetted;

        #endregion

        #region Internal Fields

        internal readonly PointCreator PointCreator;
        internal PointFigure StartPoint;

        #endregion
        
        #region Constructor

        public PointWorkspace(PointCreator PointCreator)
        {
            this.PointCreator = PointCreator;

            Init();

            ShowMessage(HelpMessages.SetStartPointMessage);
        }

        #endregion

        #region Public Methods

        public bool CanAccessCreating()
        {
            if (StartPoint == null)
            {
                ShowMessage(HelpMessages.CreateWarningWithoutStartPointMessage, true);
                return false;
            }

            return true;
        }

        public Point GetPoint() => StartPoint.Point;

        public void Dispose()
        {
            PointCreator.SelectHelper.OnSelectedHelper();

            PointCreator.DrawingControl.Canvas.MouseLeftButtonDown -= Canvas_MouseLeftButtonDown;
            PointCreator.DrawingControl.Canvas.MouseMove -= Canvas_MouseMove;
            PointCreator.CoordinateSystem.CoordinateChanged -= CoordinateSystem_CoordinateChanged;

            ShowMessage(null);

            StartPoint?.Dispose();

            PointCreator.DrawingControl.Canvas.RemoveElements(_mousePoint);
        }

        #endregion

        #region Internal Methods

        internal void CreateStartPoint(Point point)
        {
            StartPoint = CreateStartPointFigure(point, this);

            StartPoint.IsEdgePoint = true;

            _mousePoint.Visibility = Visibility.Collapsed;
            _startPointSetted = true;
        }

        #endregion

        #region Private Methods

        private void Init()
        {
            PointCreator.FigureCollection.UnselectAllFigures();

            PointCreator.SelectHelper.OffSelectedHelper();

            PointCreator.DrawingControl.Canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
            PointCreator.DrawingControl.Canvas.MouseMove += Canvas_MouseMove;

            PointCreator.CoordinateSystem.CoordinateChanged += CoordinateSystem_CoordinateChanged;

            _mousePoint = ShapeFactory.CreateMouseCreatePoint();
            _mousePoint.AddToCanvas(PointCreator.DrawingControl.Canvas);
        }

        private void CoordinateSystem_CoordinateChanged(object sender, CoordinateChangedArgs e)
        {
            StartPoint?.Update();
        }

        private void CalcMagnetPoints(Point canvasPosition)
        {
            _magnetPointWithoutShiftMode = CalcMagnetPoint(canvasPosition);
        }

        #endregion

        #region Mouse Actions

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_startPointSetted)
            {
                Actions.AddStartPoint(this, _magnetPointWithoutShiftMode);
                PointCreator.AccessCreating();
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(PointCreator.DrawingControl.Canvas);

            CalcMagnetPoints(pos);

            if (!_startPointSetted)
                _mousePoint.SetPosition(_magnetPointWithoutShiftMode);
        }

        #endregion
        
        #region Helpers

        private void ShowMessage(string message, bool isWarning = false) =>
            PointCreator.DrawingControl.ShowMessage(message, isWarning);

        private Point CalcMagnetPoint(Point point, bool isShiftMode = false, Point prevPoint = new Point()) =>
            point.GetMagnetPoint(PointCreator.Settings, PointCreator.GridChart,
                PointCreator.FigureCollection, PointCreator.CoordinateSystem,
                isShiftMode && Keyboard.IsKeyDown(Key.LeftShift), prevPoint);

        #endregion

        #region Factory

        private static PointFigure CreateStartPointFigure(Point startPoint, PointWorkspace workspace) => new PointFigure(startPoint, workspace);

        #endregion
    }
}