using System;
using System.Windows;
using System.Windows.Input;
using Ariadna.Engine.Core;

namespace Ariadna.Engine.PointCreator
{
    /// <summary>
    /// Класс создания точки в редакторе 
    /// </summary>
    internal class PointCreator : IPointCreator
    {
        private readonly IAriadnaEngine _ariadnaEngine;

        #region Private Fields

        private bool _isCreating;

        private IPointWorkspace _workspace;

        #endregion

        #region Internal Fields

        internal IDrawingControl DrawingControl;
        internal ISelectHelper2D SelectHelper;
        internal ICoordinateSystem CoordinateSystem;
        internal EngineSettings Settings;
        internal IFigure2DCollection FigureCollection;
        internal IGridChart GridChart;

        #endregion

        #region Public Properies

        public bool IsCreating
        {
            get => _isCreating;
            private set => SetIsCreating(value);
        }

        #endregion

        #region Events

        public event EventHandler<PointGeometryResultArgs> EndCreated;

        public event EventHandler<IsPointCreatedEventArgs> IsCreatedChanged;
        public event EventHandler SelectedChanged;

        #endregion

        #region Constructor

        public PointCreator(IAriadnaEngine ariadnaEngine)
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

        public void StartCreating()
        {
            if (IsCreating)
                return;

            _workspace = new PointWorkspace(this);

            Keyboard.AddPreviewKeyDownHandler(Application.Current.MainWindow, KeyDown);

            IsCreating = true;
        }

        public void AccessCreating()
        {
            if (!_workspace.CanAccessCreating())
                return;

            var point = _workspace.GetPoint();

            FinalAction();
            EndCreated?.Invoke(this, new PointGeometryResultArgs(point));
        }

        public void CancelCreating()
        {
            FinalAction();
            EndCreated?.Invoke(this, new PointGeometryResultArgs(new Point(), true));
        }

        #endregion

        #region Private Methods

        private void FinalAction()
        {
            _workspace.Dispose();
            _workspace = null;

            Keyboard.RemovePreviewKeyDownHandler(Application.Current.MainWindow, KeyDown);

            IsCreating = false;
        }

        private void KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    CancelCreating();
                    break;
            }
        }

        #region Set Properties

        private void SetIsCreating(bool value)
        {
            _isCreating = value;

            IsCreatedChanged?.Invoke(this, new IsPointCreatedEventArgs(value));
        }

        #endregion

        #endregion
    }
}