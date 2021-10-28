using Ariadna.Core;
using Autofac;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using SingleInstanceHelper;
using System.ComponentModel;
using System.Windows;
using IContainer = Autofac.IContainer;
using ILogger = Serilog.ILogger;

namespace Ariadna;

public class AriadnaApp : BaseViewModel
{
    #region Private Fields

    private readonly Application _application;

    #endregion

    public IContainer Host { get; }
    
    public string? Title { get; set; }

    public object? AppView { get; set; }

    public string[] Args { get; }

    public bool IsSingleInstance { get; }

    public LogObserver LoggerObserver { get; set; } = new();

    public IMultiProjectApp App => Host.Resolve<IMultiProjectApp>();

    #region Events

    public event EventHandler<IReadOnlyCollection<string>>? NextInstanceRunned;

    public event EventHandler? Started;

    public event EventHandler? Closed;

    public event EventHandler<CancelEventArgs>? Closing;

    #endregion

    #region Constructor

    public AriadnaApp(string[] args, Application application, bool isSingleInstance = false,
        Action<StartupOptions>? optionsAction = null)
    {
        _application = application;
        Args = args;
        IsSingleInstance = isSingleInstance;

        if (IsSingleInstance)
        {
            var first = ApplicationActivator.LaunchOrReturn(OnNextInstanceRunned, Args);

            if (!first)
            {
                _application.Shutdown();
                return;
            }
        }

        var builder = new ContainerBuilder();
        var options = new StartupOptions(builder);
        optionsAction?.Invoke(options);

        var splashWorker = new SplashWorker(options.SplashScreenFactory);
        splashWorker.Start();

        #region Start

        InitPlugins(options);

        Host = Build(builder);

        var vm = Host.Resolve<MultiProjectViewModel>();

        AppView = new MultiProjectView(vm);
        
        Host.Resolve<IUiManager>().Init(Host.Resolve<IReadOnlyList<IFeature>>(), Host.Resolve<IReadOnlyList<ISettings>>());
        
        App.Init(Host.Resolve<IReadOnlyList<IToolViewModel>>());

        #endregion

        var window = Host.Resolve<MainWindow>();

        SetMainWindow(window);

        window.ContentRendered += (_, _) =>
        {
            splashWorker.Close();
            Started?.Invoke(this, EventArgs.Empty);
        };
    }

    #endregion

    #region Internal Methods

    internal bool OnClosing()
    {
        CancelEventArgs cancelArgs = new();
        Closing?.Invoke(this, cancelArgs);

        return cancelArgs.Cancel;
    }

    internal void ClosedApp() => Closed?.Invoke(this, EventArgs.Empty);

    internal async Task CatchUnhandledException(Exception exception)
    {
        Host.Resolve<ILogger>().Error(exception, "Ошибка");

        await Host.Resolve<IDialogService>()
            .ShowMessageBoxAsync("Ошибка", "Что-то пошло не так. Обратитесь к разработчикам!");
    }

    #endregion

    #region Private Methods

    private IContainer Build(ContainerBuilder builder)
    {
        AddConfiguration(builder);
        AddLogger(builder, LoggerObserver);

        builder.RegisterInstance(this).AsSelf().SingleInstance();

        #region Services

        builder.RegisterType<ColorDialogService>().As<IColorDialogService>().SingleInstance();
        builder.RegisterType<ImageHelpers>().As<IImageHelpers>().SingleInstance();
        builder.RegisterType<InterfaceHelper>().As<IInterfaceHelper>().SingleInstance();
        builder.RegisterType<UiOptions>().As<IUiOptions>().SingleInstance();
        builder.RegisterType<Storage>().As<IStorage>().SingleInstance();
        builder.RegisterType<SvgHelper>().As<ISvgHelper>().SingleInstance();
        builder.RegisterType<ThemeManager>().As<IThemeAndAccentManager>().SingleInstance();
        builder.RegisterType<UiManager>().As<IUiManager>().SingleInstance();

        #endregion

        builder.RegisterType<SettingsManager>().As<ISettingsManager>().SingleInstance();

        builder.RegisterType<MainWindowViewModel>().AsSelf().As<IDialogService>().SingleInstance();
        builder.RegisterType<MultiProjectViewModel>().AsSelf().SingleInstance();
        builder.RegisterType<MainWindow>().AsSelf().SingleInstance();

        var asm = this.GetType().Assembly;

        builder.RegisterAssemblyTypes(asm)
            .Except<IFeature>()
            .As<IFeature>().SingleInstance();

        builder.RegisterAssemblyTypes(asm)
            .Except<ISettings>()
            .As<ISettings>().SingleInstance();

        return builder.Build();
    }

    private static void AddConfiguration(ContainerBuilder builder)
    {
#if RELEASE
        const string environmentName = "Production";
#else
        const string environmentName = "Development";
#endif
        builder.Register(c =>
        {
            var basePath = c.Resolve<IStorage>().ConfigDirectory;
            var cBuilder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{environmentName}.json", true, true);
            return cBuilder.Build();
        }).As<IConfiguration>().SingleInstance();

        builder.RegisterType<UiOptions>().As<IUiOptions>().SingleInstance();
    }

    private static void AddLogger(ContainerBuilder builder, IObserver<LogEvent> observer)
    {
        builder.Register(c =>
            {
                var path = c.Resolve<IStorage>().LogPath;

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.File(path, rollingInterval: RollingInterval.Day)
                    .WriteTo.Observers(events =>
                        events.Subscribe(observer))
                    .CreateLogger();


                return Log.Logger;
            }
        ).As<ILogger>().SingleInstance();
    }

    private void SetMainWindow(MainWindow mainWindow)
    {
        _application.MainWindow = mainWindow;

        mainWindow.Show();

        mainWindow.WindowState = WindowState.Minimized;
        mainWindow.WindowState = WindowState.Normal;

        mainWindow.Activate();
    }

    private void InitPlugins(StartupOptions options)
    {
        foreach (var plugin in options.Plugins)
        {
            var resourceUri = plugin.GetResourceUri();
            AddResource(resourceUri);
        }
    }

    private void AddResource(string uri)
    {
        if (string.IsNullOrEmpty(uri))
            return;
#if DEBUG
        var moduleResource = new ResourceDictionary
        {
            Source = new Uri(uri, UriKind.RelativeOrAbsolute)
        };
        _application.Resources.MergedDictionaries.Add(moduleResource);
#endif
#if RELEASE
        try
        {
            var moduleResource = new ResourceDictionary
            {
                Source = new Uri(uri, UriKind.RelativeOrAbsolute)
            };
            _application.Resources.MergedDictionaries.Add(moduleResource);
        }
        catch (Exception e)
        {
            Host.Resolve<ILogger>()
                .Error(e, $"Словарь ресурсов, определенный в {uri} не существует");
        }
#endif
    }

    private void OnNextInstanceRunned(string[] args)
    {
        NextInstanceRunned?.Invoke(this, args);

        _application?.MainWindow?.Activate();
    }

    #endregion
}