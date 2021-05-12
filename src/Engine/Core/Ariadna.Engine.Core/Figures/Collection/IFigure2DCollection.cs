using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Ariadna.Engine.Core
{
    /// <summary>
    /// Коллекция фигур движка
    /// </summary>
    public interface IFigure2DCollection : IList<IFigure2D>, INotifyCollectionChanged, IEngineComponent
    {
        #region Properties

        /// <summary>
        /// Коллекция выделенных фигур
        /// </summary>
        IList<ISelectedFigure2D> SelectedFigures { get; }

        #endregion

        #region Events

        /// <summary>
        /// Извещает о последней выделенной фигуре
        /// </summary>
        event EventHandler<FigureSelectedEventArgs> FigureSelected;

        event EventHandler<FigureIsShowEventArgs> IsShowChanged;

        event EventHandler<FigureDataEventArgs> DataChanged;

        event EventHandler<TransformChangedEventArgs> TransformChanged;

        event EventHandler<FigureIsFrozenEventArgs> IsFrozenChanged;

        event EventHandler<FigureSelectedEventArgs> InternalFigureSelected;

        #endregion

        #region Methods

        /// <summary>
        /// Добавить фигуру
        /// </summary>
        /// <param name="figure"></param>
        /// <param name="selectionSubscribe"></param>
        void AddFigure(IFigure2D figure, bool selectionSubscribe = true);

        /// <summary>
        /// Удалить фигуру
        /// </summary>
        /// <param name="figure"></param>
        void RemoveFigure(IFigure2D figure);

        /// <summary>
        /// Сбросить выделение со всех фигур
        /// </summary>
        void UnselectAllFigures();

        /// <summary>
        /// Выделить фигуры или снять выделение
        /// </summary>
        /// <param name="selectedFigures"></param>
        /// <param name="unSelectedFigures"></param>
        /// <param name="syncToken"></param>
        void GroupSelect(List<ISelectedFigure2D>? selectedFigures, List<ISelectedFigure2D>? unSelectedFigures,
            object syncToken);

        #endregion
    }
}