using System.Collections.Generic;
using Ariadna.Engine.Core;
using Autofac;

namespace Ariadna.Engine
{
    /// <summary>
    /// Класс 2D движка, основанном на технологии WPF
    /// </summary>
    public sealed class AriadnaEngine2D : IAriadnaEngine
    {
        #region Internal Fields

        /// <summary>
        /// IoC контейнер
        /// </summary>
        internal readonly IContainer IoC;

        #endregion

        #region Public Properties

        /// <summary>
        /// Настройки движка
        /// </summary>
        public EngineSettings Settings { get; }

        /// <summary>
        /// Координатная система
        /// </summary>
        public ICoordinateSystem CoordinateSystem => IoC.Resolve<ICoordinateSystem>();

        /// <summary>
        /// Контрол, в котором отображаются фигуры
        /// </summary>
        public IDrawingControl DrawingControl => IoC.Resolve<IDrawingControl>();

        public IImagesCanvas ImagesCanvas => IoC.Resolve<IImagesCanvas>();
        public INavigationGrid NavigationGrid => IoC.Resolve<INavigationGrid>();
        public ISelectHelper2D SelectHelper => IoC.Resolve<ISelectHelper2D>();

        /// <summary>
        /// Холст с фигурами
        /// </summary>
        public ICanvas Canvas => IoC.Resolve<ICanvas>();

        public IGridCanvas GridCanvas => IoC.Resolve<IGridCanvas>();

        /// <summary>
        /// Создатель фигур 
        /// </summary>
        public IFigureCreator FigureCreator => IoC.Resolve<IFigureCreator>();

        /// <summary>
        /// Создатель геометрии
        /// </summary>
        public IGeometryCreator GeometryCreator => IoC.Resolve<IGeometryCreator>();

        /// <summary>
        /// Создатель линейки
        /// </summary>
        public IRulerCreator RulerCreator => IoC.Resolve<IRulerCreator>();

        /// <summary>
        /// Выбор точки
        /// </summary>
        public IPointCreator PointCreator => IoC.Resolve<IPointCreator>();

        /// <summary>
        /// Помощник вставки
        /// </summary>
        public IPasteHelper PasteHelper => IoC.Resolve<IPasteHelper>();

        /// <summary>
        /// Менеджер трансформаций фигур
        /// </summary>
        public ITransformManager TransformManager => IoC.Resolve<ITransformManager>();

        /// <summary>
        /// Коллекция фигур в движке
        /// </summary>
        public IFigure2DCollection Figures => IoC.Resolve<IFigure2DCollection>();

        /// <summary>
        /// Менеджер поворота
        /// </summary>
        public IRotationManager RotationManager => IoC.Resolve<IRotationManager>();

        /// <summary>
        /// Менеджер поворота
        /// </summary>
        public IReflectionManager ReflectionManager => IoC.Resolve<IReflectionManager>();

        public IGridChart GridChart => IoC.Resolve<IGridChart>();

        #endregion

        #region Constructors

        /// <summary>
        /// Основной конструктор
        /// </summary>
        /// <param name="engineSettings"></param>
        public AriadnaEngine2D(EngineSettings engineSettings)
        {
            Settings = engineSettings;
            IoC = Bootstrapper.Build(Settings, this);

            foreach (var component in IoC.Resolve<IEnumerable<IEngineComponent>>())
                component.Init();
        }

        #endregion
    }
}