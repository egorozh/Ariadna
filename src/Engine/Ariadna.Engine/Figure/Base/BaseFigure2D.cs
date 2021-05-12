using System;
using System.Windows.Controls;
using System.Windows.Media;
using Ariadna.Engine.Core;

namespace Ariadna.Engine
{
    /// <summary>
    /// Базовый класс для всех фигур, реализующих <see cref="IFigure2D"/>
    /// </summary>
    public abstract class BaseFigure2D : IFigure2D
    {
        #region Private Fields

        private Matrix _transform;
        private bool _isShow = true;
        private bool _isFrozen = false;
        private int _zOrder;

        #endregion

        #region Protected Properties    

        protected IAriadnaEngine AriadnaEngine { get; }

        #endregion

        #region Public Properties

        public TransformAxis TransformAxis { get; set; } = new();

        /// <summary>
        /// Уникальный идентификатор фигуры
        /// </summary>
        public string Id { get; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Номер слоя фигуры
        /// </summary>
        public int ZOrder
        {
            get => _zOrder;
            set => SetZOrder(value);
        }

        /// <summary>
        /// Фигура отображается
        /// </summary>
        public bool IsShow
        {
            get => _isShow;
            set => SetIsShow(value);
        }

        /// <summary>
        /// Трансформация фигуры
        /// </summary>
        public Matrix Transform
        {
            get => _transform;
            set => SetTransform(value);
        }

        /// <summary>
        /// Фигура заморожена
        /// </summary>
        public bool IsFrozen
        {
            get => _isFrozen;
            set => SetIsFrozen(value);
        }

        #endregion

        #region Events

        public event EventHandler<FigureIsShowEventArgs>? IsShowChanged;
        public event EventHandler<FigureIsFrozenEventArgs>? IsFrozenChanged;
        public event EventHandler<TransformChangedEventArgs>? TransformChanged;

        #endregion

        #region Constructor

        protected BaseFigure2D(IAriadnaEngine ariadnaEngine)
        {
            AriadnaEngine = ariadnaEngine;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Получение границ фигуры в глобальных координатах
        /// </summary>
        /// <returns></returns>
        public virtual Bounds GetBorders()
            => new(-1, -1, 1, 1);

        /// <summary>
        /// Получение границ фигуры в координатах холста
        /// </summary>
        /// <returns></returns>
        public virtual Bounds GetCanvasBorders()
            => new(-1, -1, 1, 1);

        public virtual void Update(params Matrix[] transforms)
        {
            if (!IsShow)
                return;
        }

        /// <summary>
        /// Отрисовка на <see cref="Canvas"/> фигуры и её частей
        /// </summary>
        public virtual void Draw()
        {
            AriadnaEngine.CoordinateSystem.CoordinateChanged += CoordinateSystemOnPropertyChanged;
            Update();
        }

        /// <summary>
        /// Удаление фигуры с <see cref="Canvas"/> и её частей
        /// </summary>
        public virtual void Remove()
        {
            AriadnaEngine.CoordinateSystem.CoordinateChanged -= CoordinateSystemOnPropertyChanged;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Обновление фигуры при изменении параметров <see cref="ICoordinateSystem"/>
        /// </summary>
        protected virtual void Update()
        {
            if (!IsShow)
                return;
        }

        protected virtual void OnTransformChanged()
        {
            Update();
        }

        protected virtual void OnZOrderChanged()
        {
        }

        /// <summary>
        /// Показать фигуру
        /// </summary>
        protected virtual void OnShow()
        {
        }

        /// <summary>
        /// Скрыть фигуру
        /// </summary>
        protected virtual void OnHide()
        {
        }

        /// <summary>
        /// Заморозка фигуры
        /// </summary>
        protected virtual void OnFrozen()
        {
        }

        /// <summary>
        /// Разморозка фигуры
        /// </summary>
        protected virtual void OnUnFrozen()
        {
        }

        /// <summary>
        /// Получение итоговой матрицы трансформации с учетом <see cref="Transform"/>
        /// </summary>
        /// <returns></returns>
        protected Matrix GetCanvasTransform()
        {
            var transform = Transform;

            var matrix = AriadnaEngine.CoordinateSystem.GetCanvasMatrixTransform();

            transform.Append(matrix);

            return transform;
        }

        #endregion

        #region Private Methods

        private void SetTransform(Matrix value)
        {
            _transform = value;

            OnTransformChanged();

            TransformChanged?.Invoke(this, new TransformChangedEventArgs());
        }

        private void SetZOrder(int value)
        {
            _zOrder = value;

            OnZOrderChanged();
        }

        private void SetIsShow(bool isShow)
        {
            _isShow = isShow;

            if (isShow)
                OnShow();
            else
                OnHide();

            IsShowChanged?.Invoke(this, new FigureIsShowEventArgs());
        }

        private void SetIsFrozen(bool isFrozen)
        {
            _isFrozen = isFrozen;

            if (isFrozen)
                OnFrozen();
            else
                OnUnFrozen();

            IsFrozenChanged?.Invoke(this, new FigureIsFrozenEventArgs());
        }

        private void CoordinateSystemOnPropertyChanged(object? sender, CoordinateChangedArgs e)
        {
            Update();
        }

        #endregion
    }
}