using Ariadna.Engine.Core;
using Autofac;

namespace Ariadna.Engine.PointCreator
{
    public class PointCreatorModule : Module
    {
        protected override void Load(ContainerBuilder builder) =>
            builder.RegisterType<PointCreator>().As<IPointCreator>().As<IEngineComponent>().SingleInstance();
    }
}