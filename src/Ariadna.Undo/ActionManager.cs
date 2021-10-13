using System;
using System.Collections.Generic;

namespace Ariadna.Undo;

public sealed class ActionManager : IActionManager
{
    #region Private Fields

    private IActionHistory _history;

    #endregion

    #region Public Properties

    public IActionHistory History
    {
        get => _history;
        set
        {
            if (_history != null)
                _history.CollectionChanged -= RaiseUndoBufferChanged;

            _history = value;
            if (_history != null)
                _history.CollectionChanged += RaiseUndoBufferChanged;
        }
    }

    #endregion

    #region Events

    /// <summary>
    /// Listen to this event to be notified when a new action is added, executed, undone or redone
    /// </summary>
    public event EventHandler CollectionChanged;

    #endregion

    #region Constructor

    public ActionManager()
    {
        History = new SimpleHistory();
    }

    #endregion

    #region Public Methods

    #region RecordAction

    #region Running

    /// <summary>
    /// Currently running action (during an Undo or Redo process)
    /// </summary>
    /// <remarks>null if no Undo or Redo is taking place</remarks>
    public IAction CurrentAction { get; internal set; }

    /// <summary>
    /// Checks if we're inside an Undo or Redo operation
    /// </summary>
    public bool ActionIsExecuting => CurrentAction != null;

    #endregion

    /// <summary>
    /// Defines whether we should record an action to the Undo buffer and then execute,
    /// or just execute it without it becoming a part of history
    /// </summary>
    public bool ExecuteImmediatelyWithoutRecording { get; set; }

    /// <summary>
    /// Central method to add and execute a new action.
    /// </summary>
    /// <param name="existingAction">An action to be recorded in the buffer and executed</param>
    public void RecordAction(IAction action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(
                "ActionManager.RecordAction: the action argument is null");
        }

        // make sure we're not inside an Undo or Redo operation
        CheckNotRunningBeforeRecording(action);

        // if we don't want to record actions, just run and forget it
        if (ExecuteImmediatelyWithoutRecording && action.CanExecute())
        {
            action.Execute();
            return;
        }

        // Check if we're inside a transaction that is being recorded
        Transaction currentTransaction = RecordingTransaction;
        if (currentTransaction != null)
        {
            // if we're inside a transaction, just add the action to the transaction's list
            currentTransaction.Add(action);
            if (!currentTransaction.IsDelayed)
                action.Execute();
        }
        else
        {
            RunActionDirectly(action);
        }
    }

    #endregion

    #region Transactions

    public Transaction CreateTransaction() => Transaction.Create(this);

    public Transaction CreateTransaction(bool delayed) => Transaction.Create(this, delayed);

    public Stack<Transaction> TransactionStack { get; } = new Stack<Transaction>();

    public Transaction RecordingTransaction => TransactionStack.Count > 0 ? TransactionStack.Peek() : null;

    public void OpenTransaction(Transaction t) => TransactionStack.Push(t);

    public void CommitTransaction()
    {
        if (TransactionStack.Count == 0)
        {
            throw new InvalidOperationException(
                "ActionManager.CommitTransaction was called"
                + " when there is no open transaction (TransactionStack is empty)."
                + " Please examine the stack trace of this exception to find code"
                + " which called CommitTransaction one time too many."
                + " Normally you don't call OpenTransaction and CommitTransaction directly,"
                + " but use using(var t = Transaction.Create(Root)) instead.");
        }

        var committing = TransactionStack.Pop();

        if (committing.HasActions())
            RecordAction(committing);
    }

    public void RollBackTransaction()
    {
        if (TransactionStack.Count != 0)
        {
            var topLevelTransaction = TransactionStack.Peek();

            topLevelTransaction?.UnExecute();

            TransactionStack.Clear();
        }
    }

    #endregion

    #region Undo, Redo

    public void Undo()
    {
        if (!CanUndo)
            return;

        if (ActionIsExecuting)
        {
            throw new InvalidOperationException("ActionManager is currently busy" +
                                                $" executing a transaction ({CurrentAction}). This transaction has called Undo()" +
                                                " which is not allowed until the transaction ends." +
                                                " Please examine the stack trace of this exception to see" +
                                                " what part of your code called Undo.");
        }

        CurrentAction = History.CurrentState.PreviousAction;
        History.MoveBack();
        CurrentAction = null;
    }

    public void Redo()
    {
        if (!CanRedo)
            return;

        if (ActionIsExecuting)
        {
            throw new InvalidOperationException("ActionManager is currently busy" +
                                                $" executing a transaction ({CurrentAction}). This transaction has called Redo()" +
                                                " which is not allowed until the transaction ends." +
                                                " Please examine the stack trace of this exception to see" +
                                                " what part of your code called Redo.");
        }

        CurrentAction = History.CurrentState.NextAction;
        History.MoveForward();
        CurrentAction = null;
    }

    public bool CanUndo => History.CanMoveBack;

    public bool CanRedo => History.CanMoveForward;

    #endregion

    #region Buffer

    public void Clear()
    {
        History.Clear();
        CurrentAction = null;
    }

    public IEnumerable<IAction> EnumUndoableActions() => History.EnumUndoableActions();

    public IEnumerable<IAction> EnumRedoableActions() => History.EnumRedoableActions();

    #endregion

    #endregion

    #region Private Methods

    private void CheckNotRunningBeforeRecording(IAction candidate)
    {
        if (CurrentAction != null)
        {
            string candidateActionName = candidate != null ? candidate.ToString() : "";
            throw new InvalidOperationException
            (
                "ActionManager.RecordActionDirectly: the ActionManager is currently running " +
                $"or undoing an action ({CurrentAction.ToString()}), and this action (while being executed) attempted " +
                $"to recursively record another action ({candidateActionName}), which is not allowed. " +
                "You can examine the stack trace of this exception to see what the " +
                "executing action did wrong and change this action not to influence the " +
                "Undo stack during its execution. Checking if ActionManager.ActionIsExecuting == true " +
                "before launching another transaction might help to avoid the problem. Thanks and sorry for the inconvenience."
            );
        }
    }

    /// <summary>
    /// Adds the action to the buffer and runs it
    /// </summary>
    private void RunActionDirectly(IAction actionToRun)
    {
        CheckNotRunningBeforeRecording(actionToRun);

        CurrentAction = actionToRun;
        try
        {
            if (History.AppendAction(actionToRun))
                History.MoveForward();
        }
        finally
        {
            CurrentAction = null;
        }
    }


    private void RaiseUndoBufferChanged(object sender, EventArgs e) => CollectionChanged?.Invoke(this, e);

    #endregion
}