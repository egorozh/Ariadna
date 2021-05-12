using Ariadna.Core;
using Autofac;
using MahApps.Metro.Controls;
using Serilog;
using SingleInstanceHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Serilog.Events;

namespace Ariadna
{
    public class AriadnaApp : BaseViewModel
    {
        #region Instance

        public static AriadnaApp? Instance { get; private set; }

        #endregion

        #region Private Fields

        private readonly Application _application;
        private readonly ContainerBuilder _builder;

        #endregion

        #region Public Properties

        public DataTemplate? IconTemplate { get; set; }

        public IContainer Host { get; }

        /// <summary>
        /// Заголовок программы
        /// </summary>
        public string? Title { get; set; }

        public object? AppView { get; set; }

        public string[] Args { get; }

        public MetroWindow MainWindow => Host.Resolve<MainWindow>();

        public IApp App => Host.Resolve<IApp>();

        /// <summary>
        /// Менеджер интерфейса
        /// </summary>
        public IUiManager UiManager => Host.Resolve<IUiManager>();

        public IInterfaceHelper InterfaceHelper => Host.Resolve<IInterfaceHelper>();

        /// <summary>
        /// Конвертация svg-изображений
        /// </summary>
        public ISvgHelper SvgHelper { get; } = new SvgHelper();

        /// <summary>
        /// Все команды (функции), зарегистрированные в приложении
        /// </summary>
        public IReadOnlyList<IFeature> Features { get; }

        /// <summary>
        /// Сервис color-picker'a
        /// </summary>
        public IColorDialogService ColorDialogService => Host.Resolve<IColorDialogService>();

        /// <summary>
        /// Все плагины программы, зарегистрированные в приложении
        /// </summary>
        public IReadOnlyList<IPlugin> Plugins { get; } = new ObservableCollection<IPlugin>();

        /// <summary>
        /// Логгер приложения
        /// </summary>
        public ILogger Logger => Host.Resolve<ILogger>();

        public Button? SettingsButton { get; set; }

        public IEnumerable<IToolViewModel> Tools => Host.Resolve<IEnumerable<IToolViewModel>>();

        public IEnumerable<ISettings> Settings => Host.Resolve<IEnumerable<ISettings>>();

        public bool IsSingleInstance { get; }

        public LogObserver LoggerObserver { get; set; } = new();

        #endregion

        #region Events

        public event EventHandler<IReadOnlyCollection<string>> NextInstanceRunned;

        /// <summary>
        /// Уведомляет о готовности приложения
        /// </summary>
        public event EventHandler Started = (s, e) => { };

        /// <summary>
        /// Уведомляет о закрытии приложения
        /// </summary>
        public event EventHandler Closed;

        #endregion

        #region Constructor

        private AriadnaApp(string[] args, Application application, Action<AriadnaOptions> optionsAction = null,
            bool isSingleInstance = false)
        {
            Args = args;
            IsSingleInstance = isSingleInstance;

            _application = application;

            _builder = new ContainerBuilder();

            var options = new AriadnaOptions(_builder);
            optionsAction?.Invoke(options);

            var splashWorker = new SplashWorker(options.SplashScreenFactory);
            splashWorker.Start();

            #region Start

            var bootstrapper = new Bootstrapper(this, _builder);

            InitPlugins(options);

            Host = bootstrapper.Build();

            SetAppView();

            SetMainIcon(options.MainIconResourceKey);

            Features = new List<IFeature>(Host.Resolve<IEnumerable<IFeature>>());

            #endregion

            splashWorker.Close();

            SetMainWindow();

            Started?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Public Methods

        public static void Run(string[] args, Application application, Action<AriadnaOptions> options = null)
        {
            Instance = new AriadnaApp(args, application, options);
        }

        public static void RunSingle(string[] args, Application application, Action<AriadnaOptions> options = null)
        {
            var first = ApplicationActivator.LaunchOrReturn(
                commandArgs => { Instance?.OnNextInstanceRunned(commandArgs); }
                , args);

            if (!first)
                application.Shutdown();

            Instance = new AriadnaApp(args, application, options, true);
        }

        private void OnNextInstanceRunned(string[] args)
        {
            NextInstanceRunned?.Invoke(this, args);

            Application.Current?.MainWindow?.Activate();
        }

        public DirectoryInfo GetRootDirectory() => new(AppDomain.CurrentDomain.BaseDirectory);

        public string GetCongigDirectory()
        {
            return Path.Combine(GetRootDirectory().FullName, "Config");
        }

        /// <summary>
        /// Вызов Message Box приложения
        /// </summary>
        public async Task<AriadnaMessageDialogResult> ShowMessageBoxAsync(string title,
            string message,
            AriadnaMessageDialogStyle style = AriadnaMessageDialogStyle.Affirmative) =>
            await Host.Resolve<MainWindowViewModel>().ShowMessageBoxAsync(title, message, style);

        /// <summary>
        /// Добавление словаря ресурсов
        /// </summary>
        /// <param name="uri"></param>
        public void AddResource(string uri)
        {
            if (string.IsNullOrEmpty(uri)) return;

#if RELEASE
            try
            {
#endif
            var moduleResource = new ResourceDictionary
            {
                Source = new Uri(uri, UriKind.RelativeOrAbsolute)
            };
            Application.Current.Resources.MergedDictionaries.Add(moduleResource);
#if RELEASE
            }
            catch (Exception e)
            {
                //Logger.Log($"Словарь ресурсов, определенный в {sourceName} не существует\r\n" + e.Message,
                //    LogLevel.Error);
            }
#endif
        }

        #endregion

        #region Internal Methods

        internal void ClosedApp()
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Вызывается при закрытии окна приложения
        /// </summary>
        internal bool Close()
        {
            var res = false;

            foreach (var feature in Features)
            {
                var isCancelClose = feature.Closing();

                if (isCancelClose)
                    res = true;
            }

            return res;
        }

        internal async Task LoadAsync()
        {
        }

        #endregion

        #region Private Methods

        private void SetMainIcon(object mainIconResourceKey)
        {
            if (mainIconResourceKey != null)
                IconTemplate = _application.TryFindResource(mainIconResourceKey) as DataTemplate;
        }

        private void SetAppView()
        {
            switch (Host.Resolve<IApp>())
            {
                case IMultiProjectApp multiProjectApp:

                    AppView = new MultiProjectView(new MultiProjectViewModel(multiProjectApp), this);

                    break;
                case ISingleProjectApp singleProjectApp:

                    AppView = singleProjectApp.View;

                    break;
            }
        }

        private void SetMainWindow()
        {
            _application.MainWindow = MainWindow;
            MainWindow.Show();

            MainWindow.WindowState = WindowState.Minimized;
            MainWindow.WindowState = WindowState.Normal;

            MainWindow.Activate();
        }

        private void InitPlugins(AriadnaOptions options)
        {
            foreach (var plugin in options.Plugins)
            {
                ((ObservableCollection<IPlugin>) Plugins).Add(plugin);

                var resourceUri = plugin.GetResourceUri();
                AddResource(resourceUri);
            }
        }

        #endregion
    }

    public class LogObserver : IObserver<LogEvent>
    {
        public event Action<LogEvent>? LogEventPushed; 

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(LogEvent value)
        {
            LogEventPushed?.Invoke(value);
        }
    }
}