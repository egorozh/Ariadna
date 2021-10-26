using System.IO;

namespace Ariadna;

internal class Storage : IStorage
{
    public string BasePath { get; }

    public string ConfigDirectory { get; }
    public string AvalonConfigPath { get; }
    public string AppSettingsPath { get; }

    public Storage()
    {
        BasePath = AppContext.BaseDirectory;

        ConfigDirectory = Path.Combine(BasePath, "Config");
        Directory.CreateDirectory(ConfigDirectory);

        AvalonConfigPath = Path.Combine(ConfigDirectory, "AvalonDock.config");
        AppSettingsPath = Path.Combine(ConfigDirectory, "appsettings.json");
    }
}