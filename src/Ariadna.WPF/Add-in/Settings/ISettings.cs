using System.Windows;

namespace Ariadna;

public interface ISettings
{
    /// <summary>
    /// Id
    /// </summary>
    string Id { get; }

    FrameworkElement? View { get; }

    public event EventHandler? HasChangesChanged;

    ///// <summary>
    ///// Есть изменения
    ///// </summary>
    bool HasChanges { get; set; }

    void Init();

    void Accept();

    void Cancel();
}