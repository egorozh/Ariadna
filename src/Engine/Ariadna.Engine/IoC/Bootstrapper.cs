using AKIM.Engine.TwoD.Reflection;
using AKIM.Engine.TwoD.Rotation;
using AKIM.Engine.TwoD.RulerCreator;
using Ariadna.Engine.Core;
using Ariadna.Engine.GeometryCreator;
using Ariadna.Engine.PointCreator;
using Ariadna.Engine.Transformation;
using Autofac;

namespace Ariadna.Engine
{
    internal static class Bootstrapper
    {
        public static IContainer Build(EngineSettings settings, AriadnaEngine2D ariadnaEngine2D)
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(ariadnaEngine2D).As<IAriadnaEngine>().SingleInstance();

            builder.RegisterInstance(settings).As<EngineSettings>().As<IEngineComponent>().SingleInstance();
            builder.RegisterType<FigureCollection>().As<IFigure2DCollection>().As<IEngineComponent>().SingleInstance();

            builder.RegisterType<AkimGridCanvas>().As<IGridCanvas>().As<IEngineComponent>().SingleInstance();
            builder.RegisterType<AkimImagesCanvas>().As<IImagesCanvas>().As<IEngineComponent>().SingleInstance();
            builder.RegisterType<AkimCanvas>().As<ICanvas>().As<IEngineComponent>().SingleInstance();
            builder.RegisterType<AkimNavigationGrid>().As<INavigationGrid>().As<IEngineComponent>().SingleInstance();

            builder.RegisterType<CoordinateSystem>().As<ICoordinateSystem>().As<IEngineComponent>().SingleInstance();

            builder.RegisterType<HelpPanelViewModel>().AsSelf().SingleInstance();

            builder.RegisterType<DrawingControl>().As<IDrawingControl>().As<IEngineComponent>().SingleInstance();

            builder.RegisterType<GridChart>().As<IGridChart>().As<IEngineComponent>().SingleInstance();
            builder.RegisterType<SelectHelper2D>().As<ISelectHelper2D>().As<IEngineComponent>().SingleInstance();

            builder.RegisterType<FigureCreator>().As<IFigureCreator>().As<IEngineComponent>().SingleInstance();
            builder.RegisterModule<PointCreatorModule>();
            builder.RegisterModule<GeometryCreatorModule>();
            builder.RegisterModule<RulerCreatorModule>();
            builder.RegisterModule<RotationModule>();
            builder.RegisterModule<ReflectionModule>();

            builder.RegisterType<PasteHelper>().As<IPasteHelper>().As<IEngineComponent>().SingleInstance();

            if (settings.IsEditing)
                builder.RegisterModule<TransformationModule>();

            return builder.Build();
        }
    }
}