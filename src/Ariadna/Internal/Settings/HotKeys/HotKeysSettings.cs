using Ariadna.Settings.HotKeys.Views;

namespace Ariadna.Settings.HotKeys
{
    internal class HotKeysSettings : BaseSettings
    {
        #region Private Fields

        private HotKeysSettingsViewModel _vm;

        #endregion  
        
        #region Constructor

        public HotKeysSettings(AriadnaApp ariadnaApp) : base(ariadnaApp)
        {
        }

        #endregion

        #region Public Methods

        public override string ToString() => "Горячие клавиши";

        public override void Init()
        {
            _vm = new HotKeysSettingsViewModel(AriadnaApp, this);

            View = new HotKeysSettingsControl(_vm);
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

        protected override string CreateId() => "fc660ec8-c685-4c8f-bc69-3ddcabcf57cd";

        #endregion
    }
}