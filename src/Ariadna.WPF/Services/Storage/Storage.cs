using System.IO;

namespace Ariadna;

internal class Storage : IStorage
{
    public string BasePath { get; }

    public string ConfigDirectory { get; }
    public string IconsDirectory { get; }
    public string LogDirectory { get; }

    public string AvalonConfigPath { get; }
    public string AppSettingsPath { get; }
    public string InterfaceConfigPath { get; }
    public string LogPath { get; }

    public Storage()
    {
        BasePath = AppContext.BaseDirectory;

        ConfigDirectory = Path.Combine(BasePath, "Config");
        Directory.CreateDirectory(ConfigDirectory);

        IconsDirectory = Path.Combine(BasePath, "Icons");
        Directory.CreateDirectory(IconsDirectory);

        LogDirectory = Path.Combine(BasePath, "Logs");
        Directory.CreateDirectory(LogDirectory);

        AvalonConfigPath = Path.Combine(ConfigDirectory, "AvalonDock.config");
        AppSettingsPath = Path.Combine(ConfigDirectory, "appsettings.json");
        InterfaceConfigPath = Path.Combine(ConfigDirectory, "Interface.json");
        LogPath = Path.Combine(LogDirectory, "app_logger.txt");
    }
}