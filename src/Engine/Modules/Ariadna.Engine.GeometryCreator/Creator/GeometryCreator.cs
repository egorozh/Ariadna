using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using AKIM.Undo;
using Ariadna.Engine.Core;

namespace Ariadna.Engine.GeometryCreator
{
    /// <summary>
    /// Класс создания геометрии в редакторе 
    /// </summary>
    internal class GeometryCreator : IGeometryCreator
    {
        #region Private Fields

        private readonly IAriadnaEngine _ariadnaEngine;
        private CreationMode _creationMode;
        private bool _isContinue;
        private bool _isClosed;
        private bool _isCreating;

        private IGeometryWorkspace? _workspace;

        #endregion

        #region Internal Fields

        internal IDrawingControl DrawingControl;
        internal ISelectHelper2D SelectHelper;
        internal ICoordinateSystem CoordinateSystem;
        internal EngineSettings Settings;
        internal IFigure2DCollection FigureCollection;
        internal IGridChart GridChart;

        internal bool IsFilled;
     
        #endregion

        #region Public Properies

        public IActionManager ActionManager { get; } = new ActionManager();

        public bool IsClosedBlocked { get; private set; }

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

        public PathGeometry? Geometry { get; set; }

        #endregion

        #region Events

        public event EventHandler<GeometryResultArgs>? EndCreated;
        public event EventHandler<CreationModeEventArgs>? CreationModeChanged;
        public event EventHandler<IsContinueEventArgs>? IsContinueChanged;
        public event EventHandler<IsCreatedEventArgs>? IsCreatedChanged;
        public event EventHandler? SelectedChanged;

        #endregion

        #region Constructor

        public GeometryCreator(IAriadnaEngine ariadnaEngine)
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

        public void StartCreating(bool isFilled, bool isClosed, bool isClosedBlocked = false)
        {
            if (IsCreating)
                return;

            _isClosed = isClosed;
            IsFilled = isFilled;
            IsClosedBlocked = isClosedBlocked;
            IsContinue = true;
            _workspace = new GeometryWorkspace(this, null);
            _workspace.SelectedChanged += Workspace_SelectedChanged;
            Keyboard.AddPreviewKeyDownHandler(Application.Current.MainWindow, KeyDown);
            IsCreating = true;
        }
        
        public void StartEditing(PathGeometry initGeometry, bool isFilled, bool isClosedBlocked = false,
            PathGeometry? boundGeometry = null)
        {
            if (IsCreating)
                return;

            IsClosedBlocked = isClosedBlocked;
            IsContinue = false;
            IsFilled = isFilled;

            Geometry = initGeometry;

            _workspace = new GeometryWorkspace(this, boundGeometry);
            _workspace.SelectedChanged += Workspace_SelectedChanged;
            _isClosed = _workspace.IsClosed;
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

        public void AccessCreating()
        {
            if (!_workspace.CanAccessCreating())
                return;

            var data = _workspace.GetData();
            EndCreated?.Invoke(this, new GeometryResultArgs(data));
            FinalAction();
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
            if (_workspace != null)
            {
                _workspace.SelectedChanged -= Workspace_SelectedChanged;
                _workspace.Dispose();
            }

            _workspace = null;
            Geometry = null;


            ActionManager.Clear();
            Keyboard.RemovePreviewKeyDownHandler(Application.Current.MainWindow, KeyDown);
            IsCreating = false;
        }

        private void KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    AccessCreating();
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
            if (IsClosedBlocked || IsFilled)
                return;

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