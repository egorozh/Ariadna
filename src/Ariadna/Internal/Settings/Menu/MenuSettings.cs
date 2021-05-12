namespace Ariadna.Settings.Menu
{
    internal class MenuSettings : BaseSettings
    {
        #region Private Fields

        private MenuSettingsViewModel _vm;

        #endregion  
        
        #region Constructor

        public MenuSettings(AriadnaApp ariadnaApp) : base(ariadnaApp)
        {
        }

        #endregion

        #region Public Methods

        public override string ToString() => "Элементы меню";

        public override void Init()
        {
            _vm = new MenuSettingsViewModel(AriadnaApp, this);

            View = new MenuSettingsControl(_vm);
        }

        public override void Accept()
        {
            base.Accept();

            _vm.Accept();
        }

        public override void Cancel()
        {
            base.Cancel();

            _vm.Cancel();
        }

        #endregion

        #region Protected Methods

        protected override string CreateId() => "6bf38633-7b37-4f4a-9fae-3874527da5a2";

        #endregion
    }
}