using Ariadna.Settings.Ribbon.Views;

namespace Ariadna.Settings.Ribbon
{
    internal class RibbonSettings : BaseSettings
    {
        #region Private Fields

        private RibbonSettingsViewModel _vm;

        #endregion  
        
        #region Constructor

        public RibbonSettings(AriadnaApp ariadnaApp) : base(ariadnaApp)
        {
        }

        #endregion

        #region Public Methods

        public override string ToString() => "Кнопки ленты";

        public override void Init()
        {
            _vm = new RibbonSettingsViewModel(AriadnaApp, this);

            View = new RibbonSettingsControl(_vm);
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

        protected override string CreateId() => "07b4a45e-744d-47c9-8e2e-6afae17be660";

        #endregion
    }
}