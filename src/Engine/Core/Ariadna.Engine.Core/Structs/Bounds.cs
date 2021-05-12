namespace Ariadna.Engine.Core
{
    /// <summary>
    /// Границы фигуры в глобальных координатах
    /// </summary>
    public struct Bounds
    {
        public double Left { get; set; }
        public double Top { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }

        public Bounds(double left, double top, double right, double bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }
}
