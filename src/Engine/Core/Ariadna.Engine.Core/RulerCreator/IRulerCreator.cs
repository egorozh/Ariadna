using System;
using AKIM.Undo;

namespace Ariadna.Engine.Core
{
    /// <summary>
    /// Позволяет создавать линейку
    /// </summary>
    public interface IRulerCreator : IEngineComponent
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
        /// Начало создания линейки
        /// </summary>
        /// <param name="isClosed">Фигура замкнута</param>
        void StartCreating(bool isClosed);

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
        /// Отмена создания фигуры
        /// </summary>
        void CancelCreating();

        #endregion
    }
}