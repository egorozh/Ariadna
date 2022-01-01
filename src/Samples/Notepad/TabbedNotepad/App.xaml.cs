using Ariadna;
using System.Windows;

namespace TabbedNotepad;

public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        var app = new AriadnaApp(e.Args, this, true,
            options =>
            {
                options.AddApp<NotepadApp, INotepadApp>();

                options.AddFeatures(GetType().Assembly);
            });
    }
}