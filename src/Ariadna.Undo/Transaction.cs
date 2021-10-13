using System;
using System.Collections.Generic;
using System.Linq;

namespace Ariadna.Undo;

public sealed class Transaction : IAction, IDisposable
{
    #region Private Fields

    private readonly List<IAction> _actions = new();
    private readonly ActionManager _actionManager;

    #endregion

    #region Private Properties

    private bool Aborted { get; set; }

    #endregion

    #region Public Properties

    public bool AllowToMergeWithPrevious { get; set; }
    public string Notice { get; set; }
    public bool IsDelayed { get; set; }

    #endregion

    #region Constructor

    private Transaction(ActionManager actionManager, bool delayed)
    {
        _actionManager = actionManager;
        actionManager.OpenTransaction(this);
        IsDelayed = delayed;
    }

    #endregion

    #region Public Methods

    public static Transaction Create(ActionManager actionManager, bool delayed)
    {
        if (actionManager == null)
        {
            throw new ArgumentNullException("actionManager");
        }

        return new Transaction(actionManager, delayed);
    }

    /// <summary>
    /// By default, the actions are delayed and executed only after
    /// the top-level transaction commits.
    /// </summary>
    /// <remarks>
    /// Make sure to dispose of the transaction once you're done - it will actually call Commit for you
    /// </remarks>
    /// <example>
    /// Recommended usage: using (Transaction.Create(actionManager)) { DoStuff(); }
    /// </example>
    public static Transaction Create(ActionManager actionManager) => Create(actionManager, true);

    public void Commit() => _actionManager.CommitTransaction();

    public void Rollback()
    {
        _actionManager.RollBackTransaction();
        Aborted = true;
    }

    public void Dispose()
    {
        if (!Aborted)
        {
            Commit();
        }
    }

    public void Add(IAction actionToAppend)
    {
        if (actionToAppend == null)
        {
            throw new ArgumentNullException("actionToAppend");
        }

        _actions.Add(actionToAppend);
    }

    public bool HasActions() => _actions.Count != 0;

    public void Remove(IAction actionToCancel)
    {
        if (actionToCancel == null)
        {
            throw new ArgumentNullException("actionToCancel");
        }

        _actions.Remove(actionToCancel);
    }

    #region IAction implementation

    public void Execute()
    {
        if (!IsDelayed)
        {
            IsDelayed = true;
            return;
        }

        foreach (var action in _actions)
        {
            action.Execute();
        }
    }

    public void UnExecute()
    {
        foreach (var action in Enumerable.Reverse(_actions))
        {
            action.UnExecute();
        }
    }

    public bool CanExecute()
    {
        foreach (var action in _actions)
        {
            if (!action.CanExecute())
            {
                return false;
            }
        }

        return true;
    }

    public bool CanUnExecute()
    {
        foreach (var action in Enumerable.Reverse(_actions))
        {
            if (!action.CanUnExecute())
            {
                return false;
            }
        }

        return true;
    }

    public bool TryToMerge(IAction followingAction) => false;

    #endregion

    #endregion
}