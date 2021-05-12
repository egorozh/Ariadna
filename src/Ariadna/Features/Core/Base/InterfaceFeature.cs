using System;
using System.Windows;
using System.Windows.Input;

namespace Ariadna
{
    public abstract class InterfaceFeature : Feature, IInterfaceFeature, IEscapeHandlerFeature
    {
        #region Private Fields

        private bool _isShow;

        private DefaultRibbonProperties? _defaultRibbonProperties;
        private DefaultMenuProperties? _defaultMenuProperties;

        #endregion

        #region Public Properties

        public DefaultRibbonProperties DefaultRibbonProperties => _defaultRibbonProperties ??= CreateRibbonProperties();
        public DefaultMenuProperties DefaultMenuProperties => _defaultMenuProperties ??= CreateMenuPosition();

        public bool IsShow
        {
            get => _isShow;
            set
            {
                _isShow = value;

                IsShowChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Events

        public event EventHandler IsShowChanged;

        #endregion

        #region Constructor

        protected InterfaceFeature(AriadnaApp ariadnaApp) : base(ariadnaApp)
        {
            IsShow = true;
        }

        #endregion

        #region Public Methods

        public void SubscribeEscapeHandler()
        {
            Keyboard.AddPreviewKeyDownHandler(Application.Current.MainWindow, KeyDown);
        }

        public void UnsubscribeEscapeHandler()
        {
            Keyboard.RemovePreviewKeyDownHandler(Application.Current.MainWindow, KeyDown);
        }

        public virtual void EscapePressed()
        {
        }

        #endregion

        #region Protected Methods

        protected virtual DefaultRibbonProperties CreateRibbonProperties() => null;

        protected virtual DefaultMenuProperties CreateMenuPosition() => null;

        #endregion

        #region Private Fields

        private void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) EscapePressed();
        }

        #endregion
    }

    /// <summary>
    /// Интерфейс подписки на событие нажатия клавиши Escape
    /// </summary>
    public interface IEscapeHandlerFeature
    {
        void SubscribeEscapeHandler();

        void UnsubscribeEscapeHandler();

        void EscapePressed();
    }
}