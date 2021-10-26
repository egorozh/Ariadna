using System.Windows;

namespace Ariadna;

public abstract class TwoStateCommandFeature : CommandFeature, ITwoStateCommandFeature
{
    #region Private Fields
    
    private State _state;

    #endregion

    #region Public Properties

    public State State
    {
        get => _state;
        set
        {
            _state = value;

            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    #endregion

    #region Events

    public event EventHandler? StateChanged;

    #endregion
    
    #region Constructor

    protected TwoStateCommandFeature(AriadnaApp ariadnaApp) : base(ariadnaApp)
    {
    }

    #endregion
    
    #region Public Methods

    public FrameworkElement? CreateAlternativeIcon() => null;

    public DefaultMenuProperties GetAlternativeMenuProperties() => new();

    public DefaultRibbonProperties GetAlternativeRibbonProperties() => new();

    #endregion
}