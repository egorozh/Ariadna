namespace Ariadna.Engine.Core
{
    /// <summary>
    /// Позволяет выделять фигуры одиночным и групповым выбором
    /// </summary>
    public interface ISelectHelper2D : IEngineComponent
    {
        /// <summary>
        /// Отключение режима выделения фигур
        /// </summary>
        void OffSelectedHelper();

        /// <summary>
        /// Включение режима выделения фигур
        /// </summary>
        void OnSelectedHelper();
    }
}