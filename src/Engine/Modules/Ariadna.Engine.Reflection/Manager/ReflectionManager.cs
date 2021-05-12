using AKIM.Undo;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Ariadna.Engine.Core;

namespace AKIM.Engine.TwoD.Reflection
{
    internal class ReflectionManager : IReflectionManager
    {
        private readonly IAriadnaEngine _ariadnaEngine;

        #region Private Fields

        internal IDrawingControl DrawingControl;
        internal ISelectHelper2D SelectHelper;
        internal ICoordinateSystem CoordinateSystem;
        internal EngineSettings Settings;
        internal IFigure2DCollection FigureCollection;
        internal IGridChart GridChart;

        private IReflectionWorkspace _reflectionWorkspace;
        private bool _isReflecting;

        public event EventHandler<ReflectionGeometryResultArgs> ReflectionEnded;

        #endregion

        #region Public Properies

        public IActionManager ActionManager { get; } = new ActionManager();

        public bool IsReflecting
        {
            get => _isReflecting;
            private set => SetIsReflecting(value);
        }

        #endregion

        #region Constructor

        public ReflectionManager(IAriadnaEngine ariadnaEngine)
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

        public void StartReflecting(List<PathGeometry> geometries)
        {
            if (IsReflecting)
                return;

            IsReflecting = true;
            _reflectionWorkspace = new ReflectionWorkspace(this, geometries);

            Keyboard.AddPreviewKeyDownHandler(Application.Current.MainWindow, KeyDown);
        }

        public void AccessReflecting()
        {
            if (!_reflectionWorkspace.CanAccessReflecting())
                return;

            var center = CoordinateSystem.GetGlobalPoint(_reflectionWorkspace.GetReflectionCenter());

            var angle = _reflectionWorkspace.GetReflectionAngle();

            FinalAction();
            ReflectionEnded?.Invoke(this, new ReflectionGeometryResultArgs(angle, center));
        }

        public void CancelReflecting()
        {
            FinalAction();
            ReflectionEnded?.Invoke(this, new ReflectionGeometryResultArgs(400.0d, new Point(0, 0)));
        }

        #endregion

        #region Private Methods

        private void FinalAction()
        {
            _reflectionWorkspace.Dispose();
            _reflectionWorkspace = null;
            ActionManager.Clear();
            Keyboard.RemovePreviewKeyDownHandler(Application.Current.MainWindow, KeyDown);
            FigureCollection.UnselectAllFigures();
            IsReflecting = false;
        }

        private void KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    CancelReflecting();
                    break;
            }
        }

        #region Set Properties

        private void SetIsReflecting(bool value)
        {
            _isReflecting = value;
        }

        #endregion

        #endregion
    }
}