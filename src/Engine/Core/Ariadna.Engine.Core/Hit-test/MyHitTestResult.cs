using System.Windows.Media;

namespace Ariadna.Engine.Core
{
    /// <summary>
    /// Результат hit-test'a
    /// </summary>
    public class MyHitTestResult
    {
        public IFigure2D Figure { get; }
        public IntersectionDetail IntersectionDetail { get; }
        public bool IsInverseSelection { get; set; }

        public MyHitTestResult(IFigure2D figure, IntersectionDetail intersectionDetail, bool isInverseSelection)
        {
            Figure = figure;
            IntersectionDetail = intersectionDetail;
            IsInverseSelection = isInverseSelection;
        }
    }

    /// <summary>
    /// Результат hit-test'a
    /// </summary>
    public class GeometryTestResult
    {
        public Geometry Geometry { get; }
        public IntersectionDetail IntersectionDetail { get; }

        public GeometryTestResult(Geometry geometry, IntersectionDetail intersectionDetail)
        {
            Geometry = geometry;
            IntersectionDetail = intersectionDetail;
        }
    }
}