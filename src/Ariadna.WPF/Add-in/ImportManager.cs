using System.ComponentModel.Composition;

namespace Ariadna;

/// <summary>
/// Контейнер плагинов и модулей
/// </summary>
internal class ImportManager : IPartImportsSatisfiedNotification
{
    [ImportMany(typeof(IPlugin), AllowRecomposition = true)]
    public IEnumerable<Lazy<IPlugin>> Plugins { get; private set; }
        
    public event EventHandler<ImportEventArgs>? ImportSatisfied;

    public void OnImportsSatisfied()
    {
        ImportSatisfied?.Invoke(this, new ImportEventArgs("Imports loaded!"));
    }
}

internal class ImportEventArgs : EventArgs
{
    public string Status { get; }

    public ImportEventArgs(string status)
    {
        Status = status;
    }
}