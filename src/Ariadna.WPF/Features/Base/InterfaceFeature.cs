namespace Ariadna;

public abstract class InterfaceFeature : Feature, IInterfaceFeature
{
    #region Private Fields

    private bool _isShow = true;

    #endregion

    #region Public Properties

    public bool IsShow
    {
        get => _isShow;
        set
        {
            _isShow = value;

            IsShowChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    #endregion

    #region Events

    public event EventHandler? IsShowChanged;

    #endregion

    #region Public Methods

    public virtual DefaultRibbonProperties GetDefaultRibbonProperties() => new();

    public virtual DefaultMenuProperties GetDefaultMenuProperties() => new();

    #endregion
}