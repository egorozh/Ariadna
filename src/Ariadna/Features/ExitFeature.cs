using System.Windows;
using System.Windows.Input;

namespace Ariadna
{
    internal class ExitFeature : CommandFeature
    {
        #region Constructor

        /// <summary>
        /// Дефолтный конструктор
        /// </summary>
        public ExitFeature(AriadnaApp ariadnaApp) : base(ariadnaApp)
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Выход из приложения
        /// </summary>
        protected override void Execute() => Application.Current.Shutdown();

        protected override DefaultMenuProperties CreateMenuPosition() =>
            new DefaultMenuProperties
            {
               Header = "Выход"
            };

        protected override KeyBinding CreateDefaultKeyBinding() => new KeyBinding(Command, Key.F4, ModifierKeys.Alt);


        protected override string CreateId() => "c25d0f42-686d-4f66-aa71-75c77f067f03";

        #endregion

        #region Public Methods

        public override string ToString() => "Базовые => Выйти из программы";

        #endregion
    }
}