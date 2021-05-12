using Autofac;

namespace Ariadna
{
    internal class Bootstrapper
    {
        #region Private Fields

        private readonly AriadnaApp _ariadnaApp;
        private readonly ContainerBuilder _builder;

        #endregion

        #region Constructor

        public Bootstrapper(AriadnaApp ariadnaApp, ContainerBuilder builder)
        {
            _ariadnaApp = ariadnaApp;
            _builder = builder;
        }

        #endregion

        #region Public Methods

        public IContainer Build()
        {
            _builder.AddConfiguration(_ariadnaApp.GetRootDirectory());
            _builder.AddLogger(_ariadnaApp.LoggerObserver);

            _builder.RegisterInstance(_ariadnaApp).AsSelf().SingleInstance();
            _builder.RegisterType<ThemeManager>().As<IThemeAndAccentManager>().SingleInstance();
            _builder.RegisterType<ColorDialogService>().As<IColorDialogService>().SingleInstance();

            _builder.RegisterType<InterfaceHelper>().As<IInterfaceHelper>().SingleInstance();
            
            _builder.RegisterType<SettingsDialogViewModel>().AsSelf().SingleInstance();
            _builder.RegisterType<SettingsManager>().As<ISettingsManager>().SingleInstance();
            _builder.RegisterType<UiManager>().As<IUiManager>().SingleInstance();

            _builder.RegisterType<MainWindowViewModel>().AsSelf().SingleInstance();
            _builder.RegisterType<MainWindow>().AsSelf().SingleInstance();

            var asm = this.GetType().Assembly;

            _builder.RegisterAssemblyTypes(asm)
                .Except<IFeature>()
                .As<IFeature>().SingleInstance();

            _builder.RegisterAssemblyTypes(asm)
                .Except<ISettings>()
                .As<ISettings>().SingleInstance();

            return _builder.Build();
        }

        #endregion
    }
}