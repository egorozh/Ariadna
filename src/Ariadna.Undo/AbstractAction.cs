namespace Ariadna.Undo;

public abstract class AbstractAction : IAction
{
    #region Protected Properties

    protected int ExecuteCount { get; set; }

    #endregion

    #region Public Properties

    /// <summary>
    /// Defines if the action can be merged with the previous one in the Undo buffer
    /// This is useful for long chains of consecutive operations of the same type,
    /// e.g. dragging something or typing some text
    /// </summary>
    public bool AllowToMergeWithPrevious { get; set; }

    public string Notice { get; set; }

    #endregion

    #region Public Methods

    public virtual void Execute()
    {
        if (!CanExecute())
        {
            return;
        }

        ExecuteCore();
        ExecuteCount++;
    }

    public virtual void UnExecute()
    {
        if (!CanUnExecute())
        {
            return;
        }

        UnExecuteCore();
        ExecuteCount--;
    }

    public virtual bool CanExecute() => ExecuteCount == 0;

    public virtual bool CanUnExecute() => !CanExecute();

    /// <summary>
    /// If the last action can be joined with the followingAction,
    /// the following action isn't added to the Undo stack,
    /// but rather mixed together with the current one.
    /// </summary>
    /// <param name="followingAction"></param>
    /// <returns>true if the FollowingAction can be merged with the
    /// last action in the Undo stack</returns>
    public virtual bool TryToMerge(IAction followingAction)
    {
        return false;
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Override execute core to provide your logic that actually performs the action
    /// </summary>
    protected abstract void ExecuteCore();

    /// <summary>
    /// Override this to provide the logic that undoes the action
    /// </summary>
    protected abstract void UnExecuteCore();

    #endregion
}