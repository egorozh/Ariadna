using Ariadna.Settings.Icons.Views;

namespace Ariadna.Settings.Icons
{
    internal class IconsSettings : BaseSettings
    {
        #region Private Fields

        private IconsSettingsViewModel _vm;

        #endregion

        #region Constructor

        public IconsSettings(AriadnaApp ariadnaApp) : base(ariadnaApp)
        {
        }

        #endregion
        
        #region Public Methods

        public override string ToString() => "Значки";

        public override void Init()
        {
            _vm = new IconsSettingsViewModel(AriadnaApp, this);

            View = new IconsSettingsControl(_vm);
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

        protected override string CreateId() => "7ad7d3d7-e2ed-4dec-971c-dc738d14fd64";

        #endregion
    }
}