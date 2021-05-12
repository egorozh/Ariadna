namespace Ariadna
{
    internal class ShowLeftActionsPanelFeature : ToggleCommandFeature
    {
        public ShowLeftActionsPanelFeature(AriadnaApp ariadnaApp) : base(ariadnaApp)
        {
            this.AriadnaApp.Started += AkimApp_Started;
        }

        private void AkimApp_Started(object sender, System.EventArgs e)
        {
            IsPressed = AriadnaApp.UiManager.JsonInterface?.QuickActions.IsShowLeft ?? false;
        }

        public override string ToString() => "Базовые => Отображать левую панель быстрого доступа";

        protected override string CreateId() => "404cd3f2-9a58-4a29-b06d-528bf871eb64";

        protected override DefaultMenuProperties CreateMenuPosition()
        {
            return new DefaultMenuProperties
            {
                Header = "Левая панель б.д."
            };
        }

        protected override bool Unchecked()
        {
            AriadnaApp.UiManager.QuickActionsManager.IsShowLeft = false;
            return false;
        }

        protected override void Checked()
        {
            AriadnaApp.UiManager.QuickActionsManager.IsShowLeft = true;
        }
    }
}