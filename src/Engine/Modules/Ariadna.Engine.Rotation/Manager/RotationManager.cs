using AKIM.Undo;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Ariadna.Engine.Core;

namespace AKIM.Engine.TwoD.Rotation
{
    internal class RotationManager : IRotationManager
    {
        private readonly IAriadnaEngine _ariadnaEngine;

        #region Private Fields

        internal IDrawingControl DrawingControl;
        internal ISelectHelper2D SelectHelper;
        internal ICoordinateSystem CoordinateSystem;
        internal EngineSettings Settings;
        internal IFigure2DCollection FigureCollection;
        internal IGridChart GridChart;

        private IRotationWorkspace _rotationWorkspace;
        private bool _isRotating;

        public event EventHandler<RotationGeometryResultArgs> RotationEnded;

        #endregion

        #region Public Properies

        public IActionManager ActionManager { get; } = new ActionManager();

        public bool IsRotating
        {
            get => _isRotating;
            private set => SetIsRotating(value);
        }

        #endregion

        #region Constructor

        public RotationManager(IAriadnaEngine ariadnaEngine)
        {
            _ariadnaEngine = ariadnaEngine;
        }

        #endregion

        #region Public Methods

        public void Init()
        {
            DrawingControl = _ariadnaEngine.DrawingControl;
            SelectHelper = _ariadnaEngine.SelectHelper;
            CoordinateSystem = _ariadnaEngine.CoordinateSystem;
            Settings = _ariadnaEngine.Settings;
            FigureCollection = _ariadnaEngine.Figures;
            GridChart = _ariadnaEngine.GridChart;
        }

        public void StartRotating(List<PathGeometry> geometries)
        {
            if (IsRotating)
                return;

            IsRotating = true;
            _rotationWorkspace = new RotationWorkspace(this, geometries);

            Keyboard.AddPreviewKeyDownHandler(Application.Current.MainWindow, KeyDown);
        }

        public void AccessRotating()
        {
            if (!_rotationWorkspace.CanAccessRotating())
                return;

            var center = _rotationWorkspace.GetRotationCenter();

            var angle = _rotationWorkspace.GetRotationAngle();

            FinalAction();
            RotationEnded?.Invoke(this, new RotationGeometryResultArgs(-angle, center));
        }

        public void CancelRotating()
        {
            FinalAction();
            RotationEnded?.Invoke(this, new RotationGeometryResultArgs(0.0d, new Point(0, 0)));
        }

        #endregion

        #region Private Methods

        private void FinalAction()
        {
            _rotationWorkspace.Dispose();
            _rotationWorkspace = null;
            ActionManager.Clear();
            Keyboard.RemovePreviewKeyDownHandler(Application.Current.MainWindow, KeyDown);
            FigureCollection.UnselectAllFigures();
            IsRotating = false;
        }

        private void KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    CancelRotating();
                    break;
            }
        }

        #region Set Properties

        private void SetIsRotating(bool value)
        {
            _isRotating = value;
        }

        #endregion

        #endregion
    }
}