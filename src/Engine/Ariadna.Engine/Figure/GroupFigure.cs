using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Ariadna.Engine.Core;

namespace Ariadna.Engine
{
    public class GroupFigure : SelectedFigure2D, IGroupFigure
    {
        #region Public Properties

        public IReadOnlyList<IFigure2D> Figures { get; set; }

        #endregion

        #region Constructor

        public GroupFigure(IAriadnaEngine ariadnaEngine) : base(ariadnaEngine)
        {
            //Figures = figures;
            TransformAxis = new TransformAxis(true, true, true);

            foreach (var figure2D in Figures)
            {
                //if (figure2D is IGeometryFigure pathGeometryFigure)
                //    pathGeometryFigure.SetIsChild(true);
            }
        }

        #endregion

        #region Public Methods

        public override bool IsHitTest(Geometry intersectGeometry,
            IntersectSelectionMode mode = IntersectSelectionMode.Intersect)
        {
            if (mode == IntersectSelectionMode.Intersect)
            {
                for (var i = 0; i < Figures.Count; i++)
                {
                    var figure = Figures[i];

                    if (figure is ISelectedFigure2D selectedFigure && selectedFigure.IsHitTest(intersectGeometry))
                        return true;
                }
            }
            else
            {
                var count = 0;

                for (var i = 0; i < Figures.Count; i++)
                {
                    var figure = Figures[i];

                    if (figure is ISelectedFigure2D selectedFigure &&
                        selectedFigure.IsHitTest(intersectGeometry, IntersectSelectionMode.FullInside))
                        count++;
                }

                if (count == Figures.Count)
                    return true;
            }

            return false;
        }

        protected override void OnShow()
        {
            base.OnShow();

            foreach (var figure in Figures)
                figure.IsShow = true;
        }

        protected override void OnHide()
        {
            base.OnHide();

            foreach (var figure in Figures)
                figure.IsShow = false;
        }

        #endregion

        #region Internal Methods

        protected override void Update()
        {
            base.Update();

            foreach (var figure2D in Figures)
                figure2D.Update(Transform);
        }

        public override void Update(params Matrix[] transforms)
        {
            base.Update(transforms);

            var newTransforms = new[] {Transform}.ToList();
            newTransforms.AddRange(transforms);

            foreach (var figure2D in Figures)
                figure2D.Update(newTransforms.ToArray());
        }

        public override Bounds GetCanvasBorders()
        {
            var bounds = new Bounds(double.MaxValue, double.MinValue, double.MinValue, double.MaxValue);

            foreach (var figure in Figures)
            {
                var borders = figure.GetCanvasBorders();

                borders.OverrideBorders(ref bounds);
            }

            return bounds;
        }

        public override Bounds GetBorders()
        {
            var bounds = new Bounds(double.MaxValue, double.MinValue, double.MinValue, double.MaxValue);

            foreach (var figure in Figures)
            {
                var borders = figure.GetBorders();

                borders.OverrideBorders(ref bounds);
            }

            return bounds;
        }

        public override void Draw()
        {
            foreach (var figure in Figures)
                figure.Draw();

            base.Draw();
        }

        public override void Remove()
        {
            base.Remove();

            foreach (var figure in Figures)
                figure.Remove();
        }

        protected override void SelectionAction(bool isUnselectedSource)
        {
            foreach (var figure in Figures)
                if (figure is SelectedFigure2D selectedFigure2D)
                    selectedFigure2D.SetIsSelected(true, true);
        }

        public override IntersectionDetail FillHitTest(Geometry geometry)
        {
            return IntersectionDetail.NotCalculated;
        }

        public override IntersectionDetail StrokeHitTest(Geometry geometry, Pen pen)
        {
            return IntersectionDetail.NotCalculated;
        }

        protected override void UnselectionAction(bool isUnselectedSource)
        {
            foreach (var figure in Figures)
                if (figure is SelectedFigure2D selectedFigure2D)
                    selectedFigure2D.SetIsSelected(false, isUnselectedSource);
        }

        protected override void OnFrozen()
        {
            base.OnFrozen();

            foreach (var figure in Figures)
                figure.IsFrozen = true;
        }

        protected override void OnUnFrozen()
        {
            base.OnUnFrozen();

            foreach (var figure in Figures)
                figure.IsFrozen = false;
        }

        #endregion
    }
}