using System.Windows;

namespace Ariadna.Engine.Core
{
    /// <summary>
    /// Отрисовывает координатную сетку
    /// </summary>
    public interface IGridChart : IEngineComponent
    {
        /// <summary>
        /// Поиск <see cref="Point"/>, ближайшей к какому-то узлу сетки
        /// </summary>
        /// <param name="canvasPoint">Позиция указателя мыши</param>
        /// <returns></returns>
        Point SearchClosePointToGridNode(Point canvasPoint);
    }
}