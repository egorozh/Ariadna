using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ariadna.Engine.Core
{
    /// <summary>
    /// Предоставляет методы по работе с системой координат
    /// </summary>
    public interface ICoordinateSystem : INotifyPropertyChanged, IEngineComponent
    {
        #region Properties

        /// <summary>
        /// Нынешняя позиция курсора мыши в глобальных координатах
        /// </summary>
        Point MouseGlobalPosition { get; }

        /// <summary>
        /// Расстояние до центра координат по X в координатах холста
        /// </summary>
        double DeltaX { get; }

        /// <summary>
        /// Расстояние до центра координат по Y в координатах холста
        /// </summary>
        double DeltaY { get; }

        /// <summary>
        /// Разрешение в unit/dp
        /// </summary>
        decimal Resolution { get; }

        /// <summary>
        /// Угол поворота системы координат
        /// </summary>
        double Angle { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Происходит при изменении масштаба и сдвига начала координат
        /// </summary>
        event EventHandler<CoordinateChangedArgs> CoordinateChanged;

        #endregion

        #region Global Converter Methods

        /// <summary>
        /// Получение матрицы трансформации для перевода в глобальную систему координат
        /// </summary>
        /// <returns></returns>
        Matrix GetGlobalMatrixTransform();

        /// <summary>
        /// Получение глобального значения длины
        /// </summary>
        /// <param name="canvasLength">Длина в dp <see cref="Canvas"/></param>
        /// <returns></returns>
        double GetGlobalLength(double canvasLength);

        /// <summary>
        /// Получение <see cref="Point"/> с глобальными координатами
        /// </summary>
        /// <param name="canvasPoint"><see cref="Point"/> с координатами <see cref="Canvas"/></param>
        /// <returns></returns>
        Point GetGlobalPoint(Point canvasPoint);

        /// <summary>
        /// Получение <see cref="Geometry"/> c глобальными координатами
        /// </summary>
        /// <param name="geometry"><see cref="Geometry"/> c координатами <see cref="Canvas"/></param>
        /// <returns></returns>
        Geometry GetGlobalGeometry(Geometry geometry);

        #endregion

        #region Canvas Converter Methods

        /// <summary>
        /// Получение матрицы трансформации для перевода в систему координат <see cref="Canvas"/>
        /// </summary>
        Matrix GetCanvasMatrixTransform();

        /// <summary>
        /// Получение длины в dp <see cref="Canvas"/>
        /// </summary>
        /// <param name="localLength">Глобальное значение длины</param>
        /// <returns></returns>
        double GetCanvasLength(double localLength);

        /// <summary>
        /// Получение <see cref="Point"/> с координатами <see cref="Canvas"/>
        /// </summary>
        /// <param name="localPoint"><see cref="Point"/> с глобальными координатами</param>
        /// <returns></returns>
        Point GetCanvasPoint(Point localPoint);

        /// <summary>
        /// Получение <see cref="PointCollection"/> c координатами <see cref="Canvas"/>
        /// </summary>
        /// <param name="localPoints">Коллекция точек с глобальными координатами</param>
        /// <returns></returns>
        PointCollection GetCanvasPoints(ICollection<Point> localPoints);


        /// <summary>
        /// Получение <see cref="PathGeometry"/> c координатами <see cref="Canvas"/>
        /// </summary>
        /// <param name="pathGeometry"><see cref="PathGeometry"/> с глобальными координатами</param>
        /// <returns></returns>
        PathGeometry GetCanvasGeometry(PathGeometry geometry);

        /// <summary>
        /// Получение <see cref="Size"/> с координатами для <see cref="Canvas"/>
        /// </summary>
        /// <param name="globalSize"><see cref="Size"/> с глобальными координатами</param>
        /// <returns></returns>
        Size GetCanvasSize(Size globalSize);

        #endregion

        #region Moving and Scale Methods

        /// <summary>
        /// Центрирование системы координат
        /// </summary>
        void Centering();

        /// <summary>
        /// Центрирование относительно точки
        /// </summary>
        /// <param name="position"></param>
        void Centering(Point position);

        /// <summary>
        /// Включение режима перемещения и масштабирования
        /// </summary>
        void OnMoving();

        /// <summary>
        /// Отключение режима перемещения и масштабирования
        /// </summary>
        void OffMoving();

        #endregion
    }
}