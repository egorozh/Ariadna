using Ariadna.Engine.Core;
using Autofac;

namespace AKIM.Engine.TwoD.RulerCreator
{
    public class RulerCreatorModule : Module
    {
        protected override void Load(ContainerBuilder builder) =>
            builder.RegisterType<RulerCreator>().As<IRulerCreator>().As<IEngineComponent>().SingleInstance();
    }
}