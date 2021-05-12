using Ariadna.Engine.Core;
using Autofac;

namespace AKIM.Engine.TwoD.Reflection
{
    public class ReflectionModule : Module
    {
        protected override void Load(ContainerBuilder builder) =>
            builder.RegisterType<ReflectionManager>().As<IReflectionManager>().As<IEngineComponent>().SingleInstance();
    }
}