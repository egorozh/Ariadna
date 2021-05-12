using AKIM.Undo;
using System;
using System.Windows;
using System.Windows.Input;
using Ariadna.Engine.Core;

namespace AKIM.Engine.TwoD.RulerCreator
{
    /// <summary>
    /// Класс создания геометрии в редакторе 
    /// </summary>
    internal class RulerCreator : IRulerCreator
    {
        private readonly IAriadnaEngine _ariadnaEngine;

        #region Private Fields

        private CreationMode _creationMode;
        private bool _isContinue;
        private bool _isClosed;
        private bool _isCreating;

        private IRulerWorkspace _workspace;

        #endregion

        #region Internal Fields

        internal IDrawingControl DrawingControl;
        internal ISelectHelper2D SelectHelper;
        internal ICoordinateSystem CoordinateSystem;
        internal EngineSettings Settings;
        internal IFigure2DCollection FigureCollection;
        internal IGridChart GridChart;
        internal IGeometryFigure Figure;

        #endregion

        #region Public Properies

        public IActionManager ActionManager { get; } = new ActionManager();
        
        public CreationMode CreationMode
        {
            get => _creationMode;
            set => SetCreationMode(value);
        }

        public bool IsClosed
        {
            get => _isClosed;
            set => SetIsClosed(value);
        }

        public bool IsContinue
        {
            get => _isContinue;
            set => SetIsContinue(value);
        }

        public bool IsCreating
        {
            get => _isCreating;
            private set => SetIsCreating(value);
        }

        #endregion

        #region Events

        public event EventHandler<GeometryResultArgs> EndCreated;
        public event EventHandler<CreationModeEventArgs> CreationModeChanged;
        public event EventHandler<IsContinueEventArgs> IsContinueChanged;
        public event EventHandler<IsCreatedEventArgs> IsCreatedChanged;
        public event EventHandler SelectedChanged;

        #endregion

        #region Constructor

        public RulerCreator(IAriadnaEngine ariadnaEngine)
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

        public void StartCreating(bool isClosed)
        {
            if (IsCreating)
                return;
            _isClosed = isClosed;
          
            IsContinue = true;
            _workspace = new RulerWorkspace(this);
            _workspace.SelectedChanged += Workspace_SelectedChanged;
            Keyboard.AddPreviewKeyDownHandler(Application.Current.MainWindow, KeyDown);
            IsCreating = true;
        }

        public void DeleteSelectedPoint() => _workspace?.DeleteSelectedPoint();

        public bool CanDeleteSelectedPoint()
        {
            if (_workspace != null)
                return _workspace.CanDeleteSelectedPoint();
            return false;
        }

        public void ConvertSelectedArcSegment() => _workspace?.ConvertSelectedArcSegment();

        public bool CanConvertSelectedArcSegment()
        {
            if (_workspace != null)
                return _workspace.CanConvertSelectedArcSegment();
            return false;
        }

        public bool CanSetIsContinue()
        {
            if (IsContinue)
                return true;
            if (_workspace != null)
                return _workspace.CanContinue();
            return false;
        }
        
        public void CancelCreating()
        {
            EndCreated?.Invoke(this, new GeometryResultArgs(null));
            FinalAction();
        }

        #endregion

        #region Private Methods

        private void FinalAction()
        {
            _workspace.SelectedChanged -= Workspace_SelectedChanged;
            _workspace.Dispose();
            _workspace = null;
            if (Figure != null)
            {
                if (Figure is ISelectedFigure2D selectedFigure2D)
                    selectedFigure2D.IsShow = true;

                Figure = null;
            }

            ActionManager.Clear();
            Keyboard.RemovePreviewKeyDownHandler(Application.Current.MainWindow, KeyDown);
            IsCreating = false;
        }

        private void KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    CancelCreating();
                    break;
                case Key.Escape:
                    CancelCreating();
                    break;
                case Key.Delete:
                    DeleteSelectedPoint();
                    break;
            }
        }

        private void Workspace_SelectedChanged(object sender, EventArgs e) => SelectedChanged?.Invoke(sender, e);

        #region Set Properties

        private void SetCreationMode(CreationMode value)
        {
            _creationMode = value;
            _workspace?.SetCreationMode(value);
            CreationModeChanged?.Invoke(this, new CreationModeEventArgs(value));
        }

        private void SetIsContinue(bool value)
        {
            if (_workspace != null && !_workspace.CanContinue())
                return;
            _isContinue = value;
            _workspace?.Continue();
            IsContinueChanged?.Invoke(this, new IsContinueEventArgs(value));
        }

        private void SetIsClosed(bool value)
        {
            _isClosed = value;
            _workspace?.SetIsClosed(value);
        }

        private void SetIsCreating(bool value)
        {
            _isCreating = value;
            IsCreatedChanged?.Invoke(this, new IsCreatedEventArgs(value));
        }

        #endregion

        #endregion
    }
}