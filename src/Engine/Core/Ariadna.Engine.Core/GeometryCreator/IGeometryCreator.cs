using System;
using System.Globalization;
using System.Windows.Media;
using AKIM.Undo;

namespace Ariadna.Engine.Core
{
    /// <summary>
    /// Позволяет создавать и редактировать геометрию фигур
    /// </summary>
    public interface IGeometryCreator : IEngineComponent
    {
        #region Properties

        /// <summary>
        /// Текущий режим редактирования
        /// </summary>
        CreationMode CreationMode { get; set; }

        /// <summary>
        /// Action Manager
        /// </summary>
        IActionManager ActionManager { get; }

        /// <summary>
        /// Фигура замкнута
        /// </summary>
        bool IsClosed { get; set; }

        /// <summary>
        /// Продолжать делать сегменты в конце геометрии
        /// </summary>
        bool IsContinue { get; set; }

        /// <summary>
        /// Конструктор активен 
        /// </summary>
        bool IsCreating { get; }

        /// <summary>
        /// Нельзя устанавливать замкнутость фигуры
        /// </summary>
        bool IsClosedBlocked { get; }

        #endregion

        #region Events

        /// <summary>
        /// Происходит по окончании редактирования
        /// </summary>
        event EventHandler<GeometryResultArgs> EndCreated;

        /// <summary>
        /// Происходит при смене режима редактирования
        /// </summary>
        event EventHandler<CreationModeEventArgs> CreationModeChanged;

        /// <summary>
        /// Происходит при переключении режима продолжения геометрии
        /// </summary>
        event EventHandler<IsContinueEventArgs> IsContinueChanged;

        /// <summary>
        /// Происходит при начале и окончании редактирования
        /// </summary>
        event EventHandler<IsCreatedEventArgs> IsCreatedChanged;


        /// <summary>
        /// Происходит при выделении узлов
        /// </summary>
        event EventHandler SelectedChanged;

        #endregion

        #region Methods

        /// <summary>
        /// Начало создания фигуры
        /// </summary>
        /// <param name="isFilled">Фигура с заливкой</param>
        /// <param name="isClosed">Фигура замкнута</param>
        /// <param name="isClosedBlocked">Запретить </param>
        void StartCreating(bool isFilled, bool isClosed, bool isClosedBlocked = false);

        /// <summary>
        /// Начало редактирования элемента
        /// </summary>
        /// <param name="initGeometry">Исходная геометрия</param>
        /// <param name="isFilled">Фигура с заливкой</param>
        /// <param name="isClosedBlocked"></param>
        /// <param name="boundGeometry"></param>
        void StartEditing(PathGeometry initGeometry, bool isFilled, bool isClosedBlocked = false,
            PathGeometry? boundGeometry = null);

        /// <summary>
        /// Удаление выделенного узла
        /// </summary>
        void DeleteSelectedPoint();

        /// <summary>
        /// Можно ли удалить узел
        /// </summary>
        /// <returns></returns>
        bool CanDeleteSelectedPoint();

        /// <summary>
        /// Превращение выделенного дугового сегмента в линейный
        /// </summary>
        void ConvertSelectedArcSegment();

        /// <summary>
        /// Можно ли превратить выделенный дуговой сегмент в линейный
        /// </summary>
        bool CanConvertSelectedArcSegment();

        /// <summary>
        /// Можно ли активировать режим "Продолжить"
        /// </summary>
        /// <returns></returns>
        bool CanSetIsContinue();

        /// <summary>
        /// Принять создание фигуры
        /// </summary>
        /// <returns></returns>
        void AccessCreating();

        /// <summary>
        /// Отмена создания фигуры
        /// </summary>
        void CancelCreating();

        #endregion
    }

    public class IsCreatedEventArgs
    {
        public bool IsCreated { get; }

        public IsCreatedEventArgs(bool isCreated)
        {
            IsCreated = isCreated;
        }
    }

    public class IsContinueEventArgs
    {
        public bool IsContinue { get; }

        public IsContinueEventArgs(bool isContinue)
        {
            IsContinue = isContinue;
        }
    }

    public class GeometryResultArgs : EventArgs
    {
        //public string Data => Geometry.ToString(CultureInfo.InvariantCulture);

        public PathGeometry? Geometry { get; }

        public bool IsCancel => Geometry == null;

        public GeometryResultArgs(PathGeometry? geometry)
        {
            Geometry = geometry;
        }
    }

    public class CreationModeEventArgs : EventArgs
    {
        public CreationMode CreationMode { get; }

        public CreationModeEventArgs(CreationMode creationMode)
        {
            CreationMode = creationMode;
        }
    }
}