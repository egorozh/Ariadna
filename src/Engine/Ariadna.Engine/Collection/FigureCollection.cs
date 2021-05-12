using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Ariadna.Engine.Core;

namespace Ariadna.Engine
{
    /// <summary>
    /// Коллекция фигур движка
    /// </summary>
    internal class FigureCollection : ObservableCollection<IFigure2D>, IFigure2DCollection
    {
        #region Public Properties

        /// <summary>
        /// Коллекция выделенных фигур
        /// </summary>
        public IList<ISelectedFigure2D> SelectedFigures
        {
            get
            {
                var selectedFigures = new List<ISelectedFigure2D>();

                for (var i = 0; i < this.Count; i++)
                {
                    var figure = this[i];
                    if (figure is ISelectedFigure2D selectedFigure && selectedFigure.IsSelected)
                        selectedFigures.Add(selectedFigure);
                }

                return selectedFigures;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Извещает о последней выделенной фигуре
        /// </summary>
        public event EventHandler<FigureSelectedEventArgs> FigureSelected;

        public event EventHandler<FigureSelectedEventArgs> InternalFigureSelected;

        public event EventHandler<FigureIsShowEventArgs> IsShowChanged;

        public event EventHandler<FigureDataEventArgs> DataChanged;

        public event EventHandler<TransformChangedEventArgs> TransformChanged;

        public event EventHandler<FigureIsFrozenEventArgs> IsFrozenChanged;

        #endregion

        #region Public Methods

        public void Init()
        {
        }

        protected override void ClearItems()
        {
            foreach (var figure in this)
            {
                figure.IsShowChanged -= Figure_IsShowChanged;
                figure.IsFrozenChanged -= Figure_IsFrozenChanged;
                figure.TransformChanged -= Figure_TransformChanged;

                if (figure is IGeometryFigure pathGeometryFigure)
                    pathGeometryFigure.DataChanged -= Figure_DataChanged;

                figure.Remove();
            }

            base.ClearItems();
        }

        /// <summary>
        /// Добавить фигуру
        /// </summary>
        /// <param name="figure"></param>
        /// <param name="selectionSubscribe"></param>
        public void AddFigure(IFigure2D figure, bool selectionSubscribe = true)
        {
            Add(figure);
            figure.Draw();


            figure.IsShowChanged += Figure_IsShowChanged;
            figure.IsFrozenChanged += Figure_IsFrozenChanged;
            figure.TransformChanged += Figure_TransformChanged;

            if (figure is IGeometryFigure pathGeometryFigure)
                pathGeometryFigure.DataChanged += Figure_DataChanged;
        }

        /// <summary>
        /// Удалить фигуру
        /// </summary>
        /// <param name="figure"></param>
        public void RemoveFigure(IFigure2D figure)
        {
            if (figure != null)
            {
                figure.IsShowChanged -= Figure_IsShowChanged;
                figure.IsFrozenChanged -= Figure_IsFrozenChanged;
                figure.TransformChanged -= Figure_TransformChanged;

                if (figure is IGeometryFigure pathGeometryFigure)
                    pathGeometryFigure.DataChanged -= Figure_DataChanged;
                
                figure.Remove();
                Remove(figure);
            }
        }

        /// <summary>
        /// Сбросить выделение со всех фигур
        /// </summary>
        public void UnselectAllFigures()
        {
            foreach (var figure in this)
                if (figure is SelectedFigure2D selectedFigure)
                    selectedFigure.SetIsSelected(false, true);

            FigureSelected?.Invoke(this,
                new FigureSelectedEventArgs(null, this.OfType<ISelectedFigure2D>().ToList()));
        }

        public void GroupSelect(List<ISelectedFigure2D> selectedFigures, List<ISelectedFigure2D> unSelectedFigures,
            object syncToken)
        {
            var action = SelectAction(selectedFigures, unSelectedFigures);

            InternalFigureSelected?.Invoke(this, action);
            FigureSelected?.Invoke(this, action);
        }

        internal void GroupSelect(List<ISelectedFigure2D> selectedFigures, List<ISelectedFigure2D> unSelectedFigures)
        {
            var action = SelectAction(selectedFigures, unSelectedFigures);

            FigureSelected?.Invoke(this, action);
        }

        private FigureSelectedEventArgs SelectAction(List<ISelectedFigure2D> selectedFigures,
            List<ISelectedFigure2D> unSelectedFigures)
        {
            if (selectedFigures != null)
            {
                for (var index = 0; index < selectedFigures.Count; index++)
                {
                    ((SelectedFigure2D) selectedFigures[index]).SetIsSelected(true, false);
                }
            }

            if (unSelectedFigures != null)
            {
                for (var index = 0; index < unSelectedFigures.Count; index++)
                {
                    ((SelectedFigure2D) unSelectedFigures[index]).SetIsSelected(false,
                        !(selectedFigures != null && selectedFigures.Count > 0));
                }
            }

            return new FigureSelectedEventArgs(selectedFigures, unSelectedFigures, this);
        }

        #endregion

        #region Private Methods

        private void Figure_IsFrozenChanged(object sender, FigureIsFrozenEventArgs e)
        {
            IsFrozenChanged?.Invoke(sender, e);
        }

        private void Figure_IsShowChanged(object sender, FigureIsShowEventArgs e)
        {
            IsShowChanged?.Invoke(sender, e);
        }

        private void Figure_DataChanged(object sender, FigureDataEventArgs e)
        {
            DataChanged?.Invoke(sender, e);
        }

        private void Figure_TransformChanged(object sender, TransformChangedEventArgs e)
        {
            TransformChanged?.Invoke(sender, e);
        }

        #endregion
    }
}