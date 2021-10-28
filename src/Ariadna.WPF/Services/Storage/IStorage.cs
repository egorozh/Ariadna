namespace Ariadna;

public interface IStorage
{
    string BasePath { get; }

    string ConfigDirectory { get; }
    
    string AvalonConfigPath { get;  }

    string AppSettingsPath { get;  }

    string IconsDirectory { get;}

    string InterfaceConfigPath { get; }

    string LogPath { get; }
}