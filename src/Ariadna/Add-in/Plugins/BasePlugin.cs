using System.ComponentModel.Composition;
using Autofac;

namespace Ariadna
{
    [Export(typeof(IPlugin))]
    public abstract class BasePlugin : IPlugin
    {
        #region Public Properties

        public string Name { get; protected set; }

        #endregion

        #region Public Methods

        public virtual void Init(ContainerBuilder builder)
        {
            var asm = this.GetType().Assembly;

            builder.RegisterAssemblyTypes(asm)
                .Except<IFeature>()
                .As<IFeature>().SingleInstance();

            builder.RegisterAssemblyTypes(asm)
                .Except<IToolViewModel>()
                .As<IToolViewModel>().SingleInstance();

            builder.RegisterAssemblyTypes(asm)
                .Except<ISettings>()
                .As<ISettings>().SingleInstance();
        }

        public virtual void Init<TParameter>(ContainerBuilder builder, TParameter parameter) where TParameter : class
        {
            builder.RegisterInstance(parameter).As<TParameter>().SingleInstance();

            Init(builder);
        }

        public virtual string GetResourceUri()
        {
            return null;
        }

        #endregion
    }
}