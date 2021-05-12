using System.Windows;
using System.Windows.Media;

namespace Ariadna.Engine.GeometryCreator
{
    internal abstract class SegmentFigure : Figure
    {
        #region Private Fields

        private SegmentFigure _previewSegment;

        #endregion

        #region Public Properties

        /// <summary>
        /// Конечная точка сегмента
        /// </summary>
        public PointFigure PointFigure { get; }

        /// <summary>
        /// Вспомогательная точка сегмента
        /// </summary>
        public HelpPointFigure HelpPointFigure { get; protected set; }

        /// <summary>
        /// Предыдущий сегмент
        /// </summary>
        public SegmentFigure PreviewSegment
        {
            get => _previewSegment;
            set => SetPreviewSegment(value);
        }

        #endregion

        #region Constructor

        protected SegmentFigure(PointFigure pointFigure,
            SegmentFigure previewSegment,
            GeometryWorkspace workspace) :
            base(workspace)
        {
            PointFigure = pointFigure;

            _previewSegment = previewSegment;
        }

        #endregion

        #region Public Methods

        public override void Update()
        {
            PointFigure?.Update();
        }

        public override void Dispose()
        {
            PointFigure?.Dispose();
            HelpPointFigure?.Dispose();
        }

        public override void UnDispose()
        {
            PointFigure?.UnDispose();
            HelpPointFigure?.UnDispose();
        }

        public abstract PathSegment GetSegment();

        public virtual Point GetCenterPoint()
        {
            var point1 = PreviewPoint();
            var point2 = PointFigure.Point;

            return new Point((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2);
        }

        public virtual void SetCenterPoint(Point centerPoint)
        {
            var oldCenterPoint = GetCenterPoint();

            var vector = centerPoint - oldCenterPoint;

            PointFigure.Point += vector;

            if (PreviewSegment == null)
            {
                Workspace.StartPoint.Point += vector;
            }
            else
            {
                PreviewSegment.PointFigure.Point += vector;
            }

            Update();
        }

        public virtual void CalcEdgePoints(Point centerPoint, out Point prevPoint, out Point nextPoint)
        {
            var oldCenterPoint = GetCenterPoint();

            var vector = centerPoint - oldCenterPoint;

            nextPoint = PointFigure.Point + vector;

            if (PreviewSegment == null)
            {
                prevPoint = Workspace.StartPoint.Point + vector;
            }
            else
            {
                prevPoint = PreviewSegment.PointFigure.Point + vector;
            }
        }

        #endregion

        #region Protected Methods

        protected Point PreviewPoint() => PreviewSegment?.PointFigure.Point ?? Workspace.StartPoint.Point;

        #endregion

        #region Private Methods

        private void SetPreviewSegment(SegmentFigure value)
        {
            _previewSegment = value;

            Update();
        }

        #endregion
    }
}