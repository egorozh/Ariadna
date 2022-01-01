using Autofac;
using System.Reflection;

namespace Ariadna;

public class StartupOptions
{
    private readonly ContainerBuilder _builder;

    #region Internal Fields

    internal Func<ISplashScreen> SplashScreenFactory;

    internal List<IPlugin> Plugins = new List<IPlugin>();
    
    #endregion

    internal StartupOptions(ContainerBuilder builder)
    {
        _builder = builder;
    }

    #region Public Methods

    #region Splash Screen

    public void AddSplashScreen<T>() where T : ISplashScreen
    {
        SplashScreenFactory = () =>
            (ISplashScreen) Activator.CreateInstance(typeof(T));
    }

    public void AddSplashScreen(Func<ISplashScreen> createFactory) => SplashScreenFactory = createFactory;

    #endregion

    #region SingleProjectApp

    public void AddApp<T, TAs1, TAs2>() where T : IMultiProjectApp
    {
        _builder.RegisterType<T>().As<TAs1>().As<TAs2>().As<IMultiProjectApp>()
            .SingleInstance();
    }

    public void AddApp<T, TAs1>() where T : IMultiProjectApp
    {
        _builder.RegisterType<T>().As<TAs1>().As<IMultiProjectApp>()
            .SingleInstance();
    }

    #endregion

    public void AddPlugin<T>() where T : IPlugin
    {
        if (Activator.CreateInstance(typeof(T)) is IPlugin plugin)
        {
            Plugins.Add(plugin);

            plugin.Init(_builder);
        }
    }

    public void AddPlugin<T, TParameter>(TParameter parameter) where T : IPlugin where TParameter : class
    {
        if (Activator.CreateInstance(typeof(T)) is IPlugin plugin)
        {
            Plugins.Add(plugin);

            plugin.Init(_builder, parameter);
        }
    }

    public void AddFeatures(Assembly asm)
    {
        _builder.RegisterAssemblyTypes(asm)
            .Except<IFeature>()
            .As<IFeature>().SingleInstance();
    }

    public void AddServices(Action<ContainerBuilder> action)
    {
        action?.Invoke(_builder);
    }
    
    #endregion
}