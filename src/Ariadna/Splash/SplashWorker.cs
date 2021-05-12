using System;
using System.Threading;

namespace Ariadna
{
    internal class SplashWorker
    {
        #region Private Fields

        private readonly Func<ISplashScreen> _splashScreenFactory;
        private ManualResetEvent _resetSplashCreated;
        private Thread _splashThread;
        private ISplashScreen _screen;

        #endregion

        #region Constructor

        public SplashWorker(Func<ISplashScreen> splashScreenFactory)
        {
            _splashScreenFactory = splashScreenFactory;
        }

        #endregion

        #region Public Methods

        public void Start()
        {
            if (_splashScreenFactory == null)
                return;

            // ManualResetEvent acts as a block.  It waits for a signal to be set.
            _resetSplashCreated = new ManualResetEvent(false);

            // Create a new thread for the splash screen to run on
            _splashThread = new Thread(ShowSplash);
            _splashThread.SetApartmentState(ApartmentState.STA);
            _splashThread.IsBackground = true;
            _splashThread.Name = "Splash Screen";
            _splashThread.Start();

            // Wait for the blocker to be signaled before continuing. This is essentially the same as: while(ResetSplashCreated.NotSet) {}
            _resetSplashCreated.WaitOne();
        }

        public void Close()
        {
            if (_screen == null)
                return;

            _screen.LoadComplete();
        }

        public void SendMessage(string message)
        {
            if (_screen == null)
                return;

            _screen.AddMessage(message);
        }

        #endregion

        #region Private Methods

        private void ShowSplash()
        {
            if (_splashScreenFactory == null)
                return;

            _screen = _splashScreenFactory.Invoke();

            if (_screen == null)
                return;

            _screen.Show();

            _resetSplashCreated.Set();
            System.Windows.Threading.Dispatcher.Run();
        }

        #endregion
    }
}