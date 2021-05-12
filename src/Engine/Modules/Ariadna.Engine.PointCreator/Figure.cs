using System;
using System.Windows.Media;

namespace Ariadna.Engine.PointCreator
{
    internal abstract class Figure : IDisposable
    {
        #region Private Fields

        private bool _isSelected;

        #endregion

        #region Protected Fields    

        protected PointWorkspace Workspace;

        #endregion

        #region Public Properties

        public bool IsSelected
        {
            get => _isSelected;
            set => SetIsSelected(value);
        }

        #endregion

        #region Constructor 

        protected Figure(PointWorkspace workspace)
        {
            Workspace = workspace;
        }

        #endregion

        #region Public Methods

        public abstract Geometry GetGeometry();

        public abstract void Update();

        public abstract void Dispose();

        public abstract void UnDispose();

        public abstract void Highlight(bool isHighlight);

        #endregion

        #region Protected Methods

        protected abstract void SelectChanged(bool isSelected);

        #endregion

        #region Private Methods 

        private void SetIsSelected(bool isSelected)
        {
            _isSelected = isSelected;

            SelectChanged(isSelected);
        }

        #endregion
    }
}