using System.Collections.Generic;
using Ariadna.Engine.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AKIM.Maths;

namespace Ariadna.Engine
{
    public class ImageFigure : SelectedFigure2D, IImageFigure
    {
        #region Private Fields

        private readonly Image _image = new()
        {
            LayoutTransform = new ScaleTransform(1, -1),
        };

        #endregion

        #region Public Properties

        public double Opacity
        {
            get => _image.Opacity;
            set => SetOpacity(value);
        }

        public ImageSource ImageSource
        {
            get => _image.Source;
            set => SetImageSource(value);
        }

        #endregion

        #region Constructor

        public ImageFigure(IAriadnaEngine ariadnaEngine) : base(ariadnaEngine)
        {
            TransformAxis = new TransformAxis(true, true, true);
        }

        #endregion

        #region Public Methods

        public PathGeometry GetTransfomedGeometry()
        {
            Point start;
            var width = _image.Source.Width;
            var height = _image.Source.Height;

            IEnumerable<Point> points = new[]
            {
                new Point(width, 0),
                new Point(width, height),
                new Point(0, height),
            };

            var imageGeometry = new PathGeometry(new[]
                {new PathFigure(start, new[] {new PolyLineSegment(points, true)}, true)});

            var geometry = GeometryExtensions.TransformedGeometry(imageGeometry, new MatrixTransform(Transform));

            return geometry;
        }

        private PathGeometry GetCanvasTransformGeometry()
        {
            Point start;
            var width = _image.Source.Width;
            var height = _image.Source.Height;

            IEnumerable<Point> points = new[]
            {
                new Point(width, 0),
                new Point(width, height),
                new Point(0, height),
            };

            var imageGeometry = new PathGeometry(new[]
                {new PathFigure(start, new[] {new PolyLineSegment(points, true)}, true)});

            var geometry =
                GeometryExtensions.TransformedGeometry(imageGeometry, new MatrixTransform(GetCanvasTransform()));

            return geometry;
        }

        public override Bounds GetCanvasBorders()
        {
            var transfGeometry = GetCanvasTransformGeometry();

            if (transfGeometry == null || transfGeometry.IsEmpty())
                return base.GetCanvasBorders();

            return new Bounds(transfGeometry.Bounds.Left, transfGeometry.Bounds.Bottom, transfGeometry.Bounds.Right,
                transfGeometry.Bounds.Top);
        }

        public override Bounds GetBorders()
        {
            var transfGeometry = GetTransfomedGeometry();

            if (transfGeometry == null || transfGeometry.IsEmpty())
                return base.GetBorders();

            return new Bounds(transfGeometry.Bounds.Left, transfGeometry.Bounds.Bottom, transfGeometry.Bounds.Right,
                transfGeometry.Bounds.Top);
        }

        public override void Draw()
        {
            _image.AddToCanvas(AriadnaEngine.ImagesCanvas);

            base.Draw();
        }

        public override void Remove()
        {
            base.Remove();

            AriadnaEngine.ImagesCanvas.RemoveElements(_image);
        }

        public override IntersectionDetail FillHitTest(Geometry gm)
        {
            //var geometry = GeometryExtensions.TransformedGeometry(_geometry, new MatrixTransform(Transform));
            Geometry geometry = GetTransfomedGeometry();

            if (geometry == null)
                return IntersectionDetail.NotCalculated;

            return geometry.FillContainsWithDetail(gm);
        }

        public override IntersectionDetail StrokeHitTest(Geometry gm, Pen pen)
        {
            //var geometry = GeometryExtensions.TransformedGeometry(_geometry, new MatrixTransform(Transform));
            Geometry geometry = GetTransfomedGeometry();
            if (geometry == null)
                return IntersectionDetail.NotCalculated;

            return geometry.StrokeContainsWithDetail(pen, gm);
        }

        #endregion

        #region Protected Methods

        protected override void OnHide()
        {
            base.OnHide();

            _image.Visibility = Visibility.Collapsed;
        }

        protected override void OnShow()
        {
            base.OnShow();

            _image.Visibility = Visibility.Visible;
        }

        protected override void OnZOrderChanged()
        {
            base.OnZOrderChanged();

            _image.SetZIndex(Core.ZOrder.ImageFigure);
        }

        protected override void Update()
        {
            base.Update();

            _image.RenderTransform = new MatrixTransform(GetCanvasTransform());
        }

        #endregion

        #region Private Methods

        private void SetOpacity(double value)
        {
            if (value <= 1 && value >= 0)
            {
                _image.Opacity = value;
            }
        }

        private void SetImageSource(ImageSource value)
        {
            _image.Source = value;
            Update();
        }

        #endregion
    }
}