using System.Windows.Input;
using Ariadna;
using Microsoft.Win32;

namespace TabbedNotepad
{
    internal class OpenProjectFeature : CommandFeature
    {
        public OpenProjectFeature(AriadnaApp ariadnaApp) : base(ariadnaApp)
        {
        }

        public override DefaultMenuProperties GetDefaultMenuProperties() => new()
        {
            Header = "Open file"
        };

        public override DefaultRibbonProperties GetDefaultRibbonProperties() => new()
        {
            Header = "Open"
        };


        public override KeyBinding GetDefaultKeyBinding() => new(this, Key.O, ModifierKeys.Control | ModifierKeys.Shift);

        protected override string CreateId() => "031965a0-cbc8-4b6e-a342-4938b1fc4469";

        protected override void Execute()
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Открыть проект",
                Filter = "Text documents |*.*"
            };

            var res = openFileDialog.ShowDialog();

            if (res == false) return;

            var projectPath = openFileDialog.FileName;

            Open(projectPath);
        }

        private void Open(string projectPath)
        {
            
        }
    }
}
