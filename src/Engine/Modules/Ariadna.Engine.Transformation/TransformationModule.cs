using Ariadna.Engine.Core;
using Autofac;

namespace Ariadna.Engine.Transformation
{
    public class TransformationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TransformManager>().As<ITransformManager>().As<IEngineComponent>().SingleInstance();
        }
    }
}