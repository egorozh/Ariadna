namespace Ariadna
{
    internal class ShowRibbonFeature : ToggleCommandFeature
    {
        private readonly IMagicOptions _magicOptions;
        private const string ConfigKey = "ShowRibbon";

        public ShowRibbonFeature(AriadnaApp ariadnaApp, IMagicOptions magicOptions) : base(ariadnaApp)
        {
            _magicOptions = magicOptions;
            IsPressed = magicOptions.IsShowRibbon;

            ariadnaApp.Started += AkimApp_Started;
        }

        private void AkimApp_Started(object sender, System.EventArgs e)
        {
            AriadnaApp.UiManager.RibbonManager.Visible = IsPressed;
        }

        public override string ToString() => "Базовые => Отображать ленту";


        protected override string CreateId() => "c31f36db-3654-4f75-ad2a-f3738455deec";

        protected override DefaultMenuProperties CreateMenuPosition()
        {
            return new DefaultMenuProperties
            {
                Header = "Коммандная лента"
            };
        }

        protected override bool Unchecked()
        {
            AriadnaApp.UiManager.RibbonManager.Visible = false;
            _magicOptions.IsShowRibbon = false;

            return false;
        }

        protected override void Checked()
        {
            AriadnaApp.UiManager.RibbonManager.Visible = true;
            _magicOptions.IsShowRibbon = true;
        }
    }
}