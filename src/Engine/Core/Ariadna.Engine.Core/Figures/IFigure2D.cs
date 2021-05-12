using System;
using System.Windows.Media;

namespace Ariadna.Engine.Core
{
    /// <summary>
    /// Интерфейс фигуры движка
    /// </summary>
    public interface IFigure2D
    {
        #region Properties

        /// <summary>
        /// По каким осям разрешается трансформировать фигуру
        /// </summary>
        TransformAxis TransformAxis { get; }

        /// <summary>
        /// Уникальный идентификатор фигуры
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Номер слоя фигуры
        /// </summary>
        int ZOrder { get; set; }

        /// <summary>
        /// Фигура отображается
        /// </summary>
        bool IsShow { get; set; }

        /// <summary>
        /// Трансформация фигуры
        /// </summary>
        Matrix Transform { get; set; }

        /// <summary>
        /// Фигура заморожена
        /// </summary>
        bool IsFrozen { get; set; }

        #endregion

        #region Events

        event EventHandler<FigureIsShowEventArgs> IsShowChanged;

        event EventHandler<FigureIsFrozenEventArgs> IsFrozenChanged;

        event EventHandler<TransformChangedEventArgs> TransformChanged;

        #endregion

        #region Methods

        void Update(params Matrix[] transforms);

        void Draw();
        void Remove();

        Bounds GetBorders();
        Bounds GetCanvasBorders();

        #endregion
    }

    public class FigureIsFrozenEventArgs : EventArgs
    {
    }

    public class FigureIsShowEventArgs : EventArgs
    {
    }

    public class FigureDataEventArgs : EventArgs
    {
    }

    public class TransformChangedEventArgs : EventArgs
    {
    }
}