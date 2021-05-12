using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Ariadna.Engine.Core;

namespace Ariadna.Engine
{
    public class PointFigure : SelectedFigure2D, IPointFigure
    {
        #region Private Fields

        protected bool _animate;
        protected bool _paused;
        private Storyboard? _storyboard;
        private double _animateSpeed;
        private DispatcherTimer? _timer;
        private bool _showTimer;
        private FrameworkElement? _shape;

        #endregion

        #region Public Properties

        public double AnimateSpeed
        {
            get => _animateSpeed;
            set => SetAnimateSpeed(value);
        }

        public FrameworkElement? Shape
        {
            get => _shape;
            init => SetShape(value);
        }

        #endregion

        #region Events

        public event EventHandler? AnimationEnded;

        #endregion

        #region Constructor

        public PointFigure(IAriadnaEngine ariadnaEngine) : base(ariadnaEngine)
        {
        }

        #endregion

        #region Public Methods

        public override void Draw()
        {
            _shape?.AddToCanvas(AriadnaEngine.Canvas);

            base.Draw();
        }

        public override void Remove()
        {
            base.Remove();

            if (_shape != null)
                AriadnaEngine.Canvas.RemoveElements(_shape);
        }

        public override Bounds GetBorders()
        {
            var point = GetPosition();
            return new Bounds(point.X, point.Y, point.X, point.Y);
        }

        public override Bounds GetCanvasBorders()
        {
            var point = AriadnaEngine.CoordinateSystem.GetCanvasPoint(GetPosition());
            return new Bounds(point.X, point.Y, point.X, point.Y);
        }

        public override IntersectionDetail FillHitTest(Geometry geometry)
        {
            var width = AriadnaEngine.CoordinateSystem.GetGlobalLength(Shape.ActualWidth / 2);
            var height = AriadnaEngine.CoordinateSystem.GetGlobalLength(Shape.ActualHeight / 2);

            var point = GetPosition();

            var rect = new RectangleGeometry(new Rect(
                new Point(point.X - width, point.Y - height),
                new Point(point.X + width, point.Y + height)));

            return rect.FillContainsWithDetail(geometry);
        }

        public override IntersectionDetail StrokeHitTest(Geometry geometry, Pen pen)
        {
            var width = AriadnaEngine.CoordinateSystem.GetGlobalLength(Shape.ActualWidth / 2);
            var height = AriadnaEngine.CoordinateSystem.GetGlobalLength(Shape.ActualHeight / 2);

            var point = GetPosition();

            var rect = new RectangleGeometry(new Rect(
                new Point(point.X - width, point.Y - height),
                new Point(point.X + width, point.Y + height)));

            return rect.StrokeContainsWithDetail(pen, geometry);
        }

        public PathGeometry GetGeometry()
        {
            var radius = AriadnaEngine.CoordinateSystem.GetGlobalLength(5);
            var point = GetPosition();
            var ellipse = new EllipseGeometry(point, radius, radius);

            return PathGeometry.CreateFromGeometry(ellipse);
        }

        public void Animate(IReadOnlyList<(double, Point)> animationSpline, double animateSpeed, bool showTimer = false)
        {
            var animation = CreateAnimation(animationSpline);

            _storyboard = new Storyboard
            {
                AutoReverse = false
            };

            _storyboard.Completed += Storyboard_Completed;
            _storyboard.Children.Add(animation);

            _showTimer = showTimer;

            if (_showTimer)
            {
                if (_timer == null)
                    _timer = new DispatcherTimer();
                else
                    _timer.Tick -= Timer_Tick;

                _timer.Tick += Timer_Tick;
                _timer.Start();
            }

            _storyboard.Begin(Shape, true);
            _animate = true;

            AnimateSpeed = animateSpeed;
        }

        public void StopAnimate()
        {
            _storyboard?.Stop(Shape);
            _animate = false;

            if (_showTimer)
            {
                _timer?.Stop();
                AriadnaEngine.DrawingControl.ShowMessage(string.Empty);
            }
        }

        public void ContinueAnimate(double animateSpeed)
        {
            _storyboard?.Resume(Shape);
            _storyboard?.SetSpeedRatio(Shape, animateSpeed);
            _paused = false;

            _timer?.Start();
        }

        public void PauseAnimate()
        {
            _storyboard.Pause(Shape);
            _paused = true;

            _timer?.Stop();
        }

        protected override void OnShow()
        {
            base.OnShow();

            if (_shape != null)
                _shape.Visibility = Visibility.Visible;
        }

        protected override void OnHide()
        {
            base.OnHide();

            if (_shape != null)
                _shape.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Protected Methods

        protected Point GetPosition() => new(Transform.OffsetX, Transform.OffsetY);

        protected override void Update()
        {
            base.Update();

            if (!IsShow || _shape == null)
                return;

            if (_animate || _paused)
            {
                var lastTransform = PositionProperty.GetValue(_shape);

                var cLastPoint = AriadnaEngine.CoordinateSystem
                    .GetCanvasPoint(new Point(lastTransform.OffsetX, lastTransform.OffsetY));

                _shape.SetPosition(cLastPoint.X, cLastPoint.Y);

                return;
            }

            var point = AriadnaEngine.CoordinateSystem.GetCanvasPoint(GetPosition());

            _shape.SetPosition(point);
        }

        protected override void SelectionAction(bool isUnseisAllfiguresUnselectlectedSource)
        {
            if (Shape != null)
            {
                Shape.Opacity = 1;
                Shape.Effect = new OrangescaleEffect();
                Shape?.SetZIndex(Core.ZOrder.SelectedFigure);
            }
        }

        protected override void UnselectionAction(bool isAllfiguresUnselect)
        {
            if (Shape == null)
                return;

            Shape.SetZIndex(ZOrder);

            if (isAllfiguresUnselect)
            {
                Shape.Effect = null;
                Shape.Opacity = 1;
            }
            else
            {
                Shape.Effect = null;
                Shape.Opacity = 0.4;
            }
        }

        protected override void OnZOrderChanged()
        {
            base.OnZOrderChanged();

            _shape?.SetZIndex(ZOrder);
        }

        #endregion

        #region Private Methods

        #region Animation

        private void SetAnimateSpeed(double value)
        {
            _animateSpeed = value;

            _storyboard?.SetSpeedRatio(Shape, value);
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            _animate = false;
            AnimationEnded?.Invoke(this, EventArgs.Empty);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var time = _storyboard?.GetCurrentTime(Shape);

            if (time.HasValue && _showTimer)
            {
                var formattedTime = ToFormatTime(time.Value);

                AriadnaEngine.DrawingControl.ShowMessage(formattedTime);
            }
        }

        private static string ToFormatTime(TimeSpan time)
        {
            var hours = time.Hours;
            var minutes = time.Minutes;
            var seconds = time.Seconds;
            var ms = time.Milliseconds;

            if (hours == 0)
            {
                if (minutes == 0)
                {
                    if (seconds == 0)
                        return $"{ms} мс";

                    return $"{seconds} c {ms} мс";
                }

                return $"{minutes} мин {seconds} c {ms} мс";
            }

            return $"{hours} ч {minutes} мин {seconds} c {ms} мс";
        }

        private static MatrixAnimationUsingKeyFrames CreateAnimation(IReadOnlyList<(double, Point)> animationSpline)
        {
            var animation = new MatrixAnimationUsingKeyFrames();

            var keyFrames = new MatrixKeyFrameCollection();

            for (var i = 0; i < animationSpline.Count; i++)
            {
                var time = animationSpline[i].Item1;

                var matrix = Matrix.Identity;

                matrix.OffsetX = animationSpline[i].Item2.X;
                matrix.OffsetY = animationSpline[i].Item2.Y;

                keyFrames.Add(
                    new LinearMatrixKeyFrame
                    {
                        KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds((int) time * 1000)),
                        Value = matrix,
                    }
                );
            }

            animation.KeyFrames = keyFrames;

            Storyboard.SetTargetProperty(animation, new PropertyPath(PositionProperty.ValueProperty));

            return animation;
        }

        #endregion

        private void SetShape(FrameworkElement? value)
        {
            _shape = value;

            if (_shape != null)
            {
                _shape.SetZIndex(ZOrder);

                CoordinateSystemProperty.SetValue(_shape, AriadnaEngine.CoordinateSystem);
                PositionProperty.SetValue(_shape, Transform);
            }

            Update();
        }

        #endregion
    }

    public class LinearMatrixKeyFrame : MatrixKeyFrame
    {
        #region Constructors

        /// <summary>
        /// Creates a new LinearPointKeyFrame.
        /// </summary>
        public LinearMatrixKeyFrame()
            : base()
        {
        }

        /// <summary>
        /// Creates a new LinearPointKeyFrame.
        /// </summary>
        public LinearMatrixKeyFrame(Matrix value)
            : base(value)
        {
        }

        /// <summary>
        /// Creates a new LinearPointKeyFrame.
        /// </summary>
        public LinearMatrixKeyFrame(Matrix value, KeyTime keyTime)
            : base(value, keyTime)
        {
        }

        #endregion

        #region Freezable

        /// <summary>
        /// Implementation of <see cref="System.Windows.Freezable.CreateInstanceCore">Freezable.CreateInstanceCore</see>.
        /// </summary>
        /// <returns>The new Freezable.</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new LinearMatrixKeyFrame();
        }

        #endregion

        #region PointKeyFrame

        protected override Matrix InterpolateValueCore(Matrix baseValue, double keyFrameProgress)
        {
            if (keyFrameProgress == 0.0)
            {
                return baseValue;
            }
            else if (keyFrameProgress == 1.0)
            {
                return Value;
            }
            else
            {
                return InterpolateMatrix(baseValue, Value, keyFrameProgress);
            }
        }

        private static Matrix InterpolateMatrix(Matrix from, Matrix to, Double progress)
        {
            var fromLocation = new Point(from.OffsetX, from.OffsetY);
            var toLocation = new Point(to.OffsetX, to.OffsetY);

            var interpolatePoint = fromLocation + ((toLocation - fromLocation) * progress);

            var matrix = Matrix.Identity;

            matrix.OffsetX = interpolatePoint.X;
            matrix.OffsetY = interpolatePoint.Y;

            return matrix;
        }

        #endregion
    }
}