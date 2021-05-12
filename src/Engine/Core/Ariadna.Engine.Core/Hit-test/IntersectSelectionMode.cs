namespace Ariadna.Engine.Core
{
    /// <summary>
    /// Режим захвата фигур при выделении
    /// </summary>
    public enum IntersectSelectionMode
    {
        /// <summary>
        /// Если фигура попадает частично в прямоугольник
        /// она выделяется
        /// </summary>
        Intersect,

        /// <summary>
        /// Если фигура попадает полностью в прямоугольник,
        /// она выделяется
        /// </summary>
        FullInside
    }
}