using Ariadna.Engine.Core;
using Autofac;

namespace AKIM.Engine.TwoD.Rotation
{
    public class RotationModule : Module
    {
        protected override void Load(ContainerBuilder builder) =>
            builder.RegisterType<RotationManager>().As<IRotationManager>().As<IEngineComponent>().SingleInstance();
    }
}