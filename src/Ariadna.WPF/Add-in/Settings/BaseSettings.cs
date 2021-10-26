using System.Windows;

namespace Ariadna;

public abstract class BaseSettings : ISettings
{
    #region Private Fields

    private string? _id;
    private bool _hasChanges;

    #endregion

    #region Public Properties
    
    public string Id => _id ??= CreateId();

    public FrameworkElement? View { get; protected set; }

    public event EventHandler? HasChangesChanged;

    public bool HasChanges
    {
        get => _hasChanges;
        set
        {
            _hasChanges = value;
            HasChangesChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    #endregion
    
    public abstract void Init();

    public virtual void Accept()
    {
        HasChanges = false;
    }

    public virtual void Cancel()
    {
        HasChanges = false;
    }

    #region Protected Methods

    /// <summary>
    /// Возвращает уникальный идентификатор настройки
    /// </summary>
    /// <remarks>Функция должна возвращать один и тот же ID при каждом вызове</remarks>
    /// <returns>Уникальный идентификатор настройки</returns>
    protected abstract string CreateId();

    #endregion
}