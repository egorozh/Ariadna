using Ariadna.Engine.Core;
using Autofac;

namespace Ariadna.Engine.GeometryCreator
{
    public class GeometryCreatorModule : Module
    {
        protected override void Load(ContainerBuilder builder) =>
            builder.RegisterType<GeometryCreator>().As<IGeometryCreator>().As<IEngineComponent>().SingleInstance();
    }
}