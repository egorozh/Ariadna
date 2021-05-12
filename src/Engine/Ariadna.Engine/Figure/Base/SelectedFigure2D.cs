using System.Windows.Media;
using Ariadna.Engine.Core;

namespace Ariadna.Engine
{
    /// <summary>
    /// Фигура, которую можно выделить
    /// </summary>
    public abstract class SelectedFigure2D : BaseFigure2D, ISelectedFigure2D
    {
        #region Private Fields

        private bool _selectable = true;

        #endregion
        
        #region Public Properties

        /// <summary>
        /// Выделена ли фигура
        /// </summary>
        public bool IsSelected { get; private set; }

        /// <summary>
        /// Фигура замкнута?
        /// </summary>
        public bool IsFilled { get; set; }

        #endregion

        #region Constructor

        protected SelectedFigure2D(IAriadnaEngine ariadnaEngine) : base(ariadnaEngine)
        {
        }

        #endregion

        #region Public Methods

        public virtual bool IsHitTest(Geometry intersectGeometry,
            IntersectSelectionMode mode = IntersectSelectionMode.Intersect)
        {
            var detail = FillHitTest(intersectGeometry);

            if (!IsFilled)
            {
                detail = StrokeHitTest(intersectGeometry, new Pen(Brushes.Red, 0.01));

                if (detail == IntersectionDetail.Empty ||
                    detail == IntersectionDetail.NotCalculated)
                    return false;

                if (mode == IntersectSelectionMode.FullInside)
                    return detail == IntersectionDetail.FullyInside;

                return true;
            }

            if (detail == IntersectionDetail.Empty ||
                detail == IntersectionDetail.NotCalculated)
                return false;

            if (mode == IntersectSelectionMode.FullInside)
                return detail == IntersectionDetail.FullyInside;

            return true;
        }

        public abstract IntersectionDetail FillHitTest(Geometry geometry);

        public abstract IntersectionDetail StrokeHitTest(Geometry geometry, Pen pen);

        #endregion

        #region Internal Methods

        internal void SetIsSelected(bool isSelected, bool isAllfiguresUnselect)
        {
            if (!_selectable)
                return;

            IsSelected = isSelected;

            if (isSelected)
                SelectionAction(isAllfiguresUnselect);
            else
                UnselectionAction(isAllfiguresUnselect);
        }

        #endregion

        #region Protected Methods

        protected virtual void UnselectionAction(bool isAllfiguresUnselect = false)
        {
        }

        protected virtual void SelectionAction(bool isAllfiguresUnselect = false)
        {
        }

        #endregion
    }
}