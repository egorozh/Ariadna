using Ariadna.Settings.QuickActions.Views;

namespace Ariadna.Settings.QuickActions
{
    internal class QuickActionsSettings : BaseSettings
    {
        #region Private Fields

        private QuickActionsSettingsViewModel _vm;

        #endregion  
        
        #region Constructor

        public QuickActionsSettings(AriadnaApp ariadnaApp) : base(ariadnaApp)
        {
        }

        #endregion

        #region Public Methods

        public override string ToString() => "Кнопки панелей быстрого доступа";

        public override void Init()
        {
            _vm = new QuickActionsSettingsViewModel(AriadnaApp, this);

            View = new QuickActionsSettingsControl(_vm);
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

        protected override string CreateId() => "8002c8d3-42a8-4110-9ec7-dcd062ef3c3f";

        #endregion
    }
}