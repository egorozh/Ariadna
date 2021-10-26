namespace Ariadna;

public interface IStorage
{
    string BasePath { get; }

    string ConfigDirectory { get; }
    
    string AvalonConfigPath { get;  }

    string AppSettingsPath { get;  }
}