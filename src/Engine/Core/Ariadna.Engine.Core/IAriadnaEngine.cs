namespace Ariadna.Engine.Core
{
    /// <summary>
    /// Интерфейс движка
    /// </summary>
    public interface IAriadnaEngine
    {
        /// <summary>   
        /// Контрол, в котором отображаются фигуры
        /// </summary>
        IDrawingControl DrawingControl { get; }

        /// <summary>
        /// Холст с фигурами
        /// </summary>
        ICanvas Canvas { get; }

        IGridCanvas GridCanvas { get; }

        IImagesCanvas ImagesCanvas { get; }
        INavigationGrid NavigationGrid { get; }
        ISelectHelper2D SelectHelper { get; }

        /// <summary>
        /// Создатель фигур 
        /// </summary>
        IFigureCreator FigureCreator { get; }

        /// <summary>
        /// Помощник вставки
        /// </summary>
        IPasteHelper PasteHelper { get; }

        /// <summary>
        /// Создатель геометрии
        /// </summary>
        IGeometryCreator GeometryCreator { get; }
        
        
        /// <summary>
        /// Создатель линейки
        /// </summary>
        IRulerCreator RulerCreator { get; }

        /// <summary>
        /// Позволяет указать точку на плане
        /// </summary>
        IPointCreator PointCreator { get; }

        /// <summary>
        /// Менеджер трансформаций фигур
        /// </summary>
        ITransformManager TransformManager { get; }

        /// <summary>
        /// Коллекция фигур в движке
        /// </summary>
        IFigure2DCollection Figures { get; }

        /// <summary>
        /// Настройки движка
        /// </summary>
        EngineSettings Settings { get; }

        /// <summary>
        /// Координатная система
        /// </summary>
        ICoordinateSystem CoordinateSystem { get; }

        /// <summary>
        /// Менеджер поворота
        /// </summary>
        IRotationManager RotationManager { get; }

        /// <summary>
        /// Менеджер отражения
        /// </summary>
        IReflectionManager ReflectionManager { get; }

        IGridChart GridChart { get; }
    }
}