using System.Windows.Media;

namespace Ariadna.Engine.Core
{
    public interface IColorFigure
    {
        /// <summary>
        /// Кисть для заливки фигуры
        /// </summary>
        Brush? Fill { get; set; }

        Brush Stroke { get; set; }
    }
}