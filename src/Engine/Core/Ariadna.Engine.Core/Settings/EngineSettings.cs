using System.Runtime.Serialization;
using Ariadna.Core;

namespace Ariadna.Engine.Core
{
    /// <summary>
    /// Класс с настройками 2D движка
    /// </summary>
    [DataContract]
    public class EngineSettings : BaseViewModel, IEngineComponent
    {
        #region Public Properties

        /// <summary>
        /// Названии единицы измерения в которых измеряются координаты фигур в движке
        /// </summary>
        [DataMember]
        public string UnitsName { get; set; } = "м.";
        
        /// <summary>
        /// Минимальное значение масштабирования
        /// </summary>
        [DataMember]
        public decimal MinResolution { get; set; } = 0.001m;

        /// <summary>
        /// Максимальное значение масштабирования
        /// </summary>
        [DataMember]
        public decimal MaxResolution { get; set; } = 500;

        /// <summary>
        /// Начальное значения масштаба
        /// </summary>
        [DataMember]
        public decimal InitResolution { get; set; } = 1M;

        /// <summary>
        /// Угол поворота системы координат
        /// </summary>
        [DataMember]
        public double InitAngle { get; set; }

        /// <summary>
        /// Начальное положение центра координат в dp по X
        /// </summary>
        [DataMember]
        public double InitDeltaX { get; set; } = 400.0;

        /// <summary>
        /// Начальное положение центра координат в dp по Y
        /// </summary>
        [DataMember]
        public double InitDeltaY { get; set; } = 240.0;

        /// <summary>
        /// Количество маленьких квадратов в сетке
        /// </summary>
        [DataMember]
        public int CountLittleRects { get; set; } = 4;

        /// <summary>
        /// Цвет выделенной фигуры
        /// </summary>
        [DataMember]
        public string SelectedColor { get; set; } = "#FF8C00";

        /// <summary>
        /// Радиус окружности, в область которой попадают элементы при выделении по нажатию ЛКМ
        /// </summary>
        [DataMember]
        public double SelectionPointRadius { get; set; } = 10;

        /// <summary>
        /// Режим примагничивания
        /// </summary>
        [DataMember]
        public bool MagnetMode { get; set; } = true;

        /// <summary>
        /// Режим примагничивания к узлам сетки
        /// </summary>
        [DataMember]
        public bool MagnetGridMode { get; set; } = true;

        /// <summary>
        /// Отображение сетки
        /// </summary>
        [DataMember]
        public bool ShowGrid { get; set; } = true;

        /// <summary>
        /// Трансформация выделенных элементов, с помощью прямоугольника с узловыми точками
        /// </summary>
        [DataMember]
        public bool IsEditing { get; set; } = true;

        /// <summary>
        /// Трансформация выделенных элементов, с помощью прямоугольника с узловыми точками
        /// </summary>
        [DataMember]
        public bool IsShowHelpPanel { get; set; } = true;
        
        #endregion

        #region Public Methods   

        public void Init()
        {
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ShowGrid))
                {
                    if (!ShowGrid) MagnetGridMode = false;
                }
            };
        }

        #endregion
    }
}