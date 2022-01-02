using System.Windows.Input;
using Ariadna;
using Microsoft.Win32;

namespace TabbedNotepad;

internal class OpenProjectFeature : CommandFeature
{
    private readonly INotepadApp _app;

    public OpenProjectFeature(AriadnaApp ariadnaApp, INotepadApp app) : base(ariadnaApp)
    {
        _app = app;
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

        var docPath = openFileDialog.FileName;

        Open(docPath);  
    }

    private void Open(string docPath)
    {
        var doc = new DocumentModel(docPath);

        _app.Projects.Add(doc);
        _app.CurrentProject = doc;
    }
}