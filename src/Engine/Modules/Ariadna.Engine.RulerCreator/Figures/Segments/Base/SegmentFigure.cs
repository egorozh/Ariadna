using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Ariadna.Engine.Core;

namespace AKIM.Engine.TwoD.RulerCreator
{
    internal abstract class SegmentFigure : Figure
    {
        #region Private Fields

        private SegmentFigure _previewSegment;

        /// <summary>
        /// Текст-блок для отображения длины сегмента
        /// </summary>
        private readonly TextBlock _lengthTextBlock;

        #endregion

        #region Public Properties

        /// <summary>
        /// Длина сегмента
        /// </summary>
        public double Length => GetLength();

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
            RulerWorkspace workspace) :
            base(workspace)
        {
            PointFigure = pointFigure;
            _previewSegment = previewSegment;

            _lengthTextBlock = new TextBlock
            {
                RenderTransformOrigin = new Point(0.5, 0.5)
            };

            _lengthTextBlock.SetZIndex(ZOrder.RulerTextBox);

            _lengthTextBlock.AddToCanvas(Workspace.RulerCreator.DrawingControl.Canvas);
        }

        #endregion

        #region Public Methods

        public override void Update()
        {
            PointFigure?.Update();
            UpdateTextBlock();
        }

        public override void Dispose()
        {
            PointFigure?.Dispose();
            HelpPointFigure?.Dispose();
            Workspace.RulerCreator.DrawingControl.Canvas.RemoveElements(_lengthTextBlock);
        }

        public override void UnDispose()
        {
            PointFigure?.UnDispose();
            HelpPointFigure?.UnDispose();
            Workspace.RulerCreator.DrawingControl.Canvas.AddElements(_lengthTextBlock);
        }

        public abstract PathSegment GetSegment();

        public virtual Point GetCenterPoint()
        {
            var point1 = PreviewPoint();
            var point2 = PointFigure.Point;

            return new Point((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2);
        }

        public abstract double GetLength();

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

        public void CalcEdgePoints(Point centerPoint, out Point prevPoint, out Point nextPoint)
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

        protected virtual void UpdateTextBlock()
        {
            _lengthTextBlock.Text = Length.ToStr();

            _lengthTextBlock.SetTextBlockOnCenter(PreviewPoint(), GetCenterPoint(), PointFigure.Point,
                Workspace.RulerCreator.CoordinateSystem);
        }

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