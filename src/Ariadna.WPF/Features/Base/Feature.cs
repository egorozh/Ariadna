namespace Ariadna;

public abstract class Feature : IFeature
{
    #region Private Fields

    private string? _id;

    #endregion

    #region Public Properties

    public string Id => _id ??= CreateId();

    #endregion

    #region Protected Methods

    /// <summary>
    /// Returns a unique identifier for the functionality
    /// </summary>
    /// <remarks>The function must return the same ID every time it is called</remarks>
    protected abstract string CreateId();

    #endregion
}