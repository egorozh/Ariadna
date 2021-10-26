namespace Ariadna;

public abstract class ToggleCommandFeature : CommandFeature, IToggleCommandFeature
{
    #region Private Fields

    private bool _isPressed;

    #endregion

    #region Public Properties

    public bool IsPressed
    {
        get => _isPressed;
        set
        {
            if (value)
                Checked();
            else
            {
                var isNotUnchecked = Unchecked();

                if (isNotUnchecked)
                {
                    return;
                }
            }

            _isPressed = value;

            IsPressedChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    #endregion

    #region Events

    public event EventHandler? IsPressedChanged;

    #endregion

    #region Constructor

    protected ToggleCommandFeature(AriadnaApp ariadnaApp) : base(ariadnaApp)
    {
    }

    #endregion

    #region Protected Methods

    protected abstract bool Unchecked();

    protected abstract void Checked();

    #endregion
}