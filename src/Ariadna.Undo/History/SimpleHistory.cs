using System;
using System.Collections;
using System.Collections.Generic;

namespace Ariadna.Undo;

internal class SimpleHistory : IActionHistory
{
    #region Private Fields

    private SimpleHistoryNode _ñurrentState = new SimpleHistoryNode();

    #endregion

    #region Public Properties

    /// <summary>
    /// "Iterator" to navigate through the sequence, "Cursor"
    /// </summary>
    public SimpleHistoryNode CurrentState
    {
        get => _ñurrentState;
        set
        {
            if (value != null)
                _ñurrentState = value;
            else
                throw new ArgumentNullException("CurrentState");
        }
    }

    public SimpleHistoryNode Head { get; set; }

    public IAction LastAction { get; set; }

    public bool CanMoveForward => CurrentState.NextAction != null &&
                                  CurrentState.NextNode != null;

    public bool CanMoveBack => CurrentState.PreviousAction != null &&
                               CurrentState.PreviousNode != null;

    /// <summary>
    /// The length of Undo buffer (total number of undoable actions)
    /// </summary>
    public int Length { get; set; }

    #endregion

    #region Events

    public event EventHandler CollectionChanged;

    #endregion

    #region Constructor

    public SimpleHistory()
    {
        Init();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Adds a new action to the tail after current state. If 
    /// there exist more actions after this, they're lost (Garbage Collected).
    /// This is the only method of this class that actually modifies the linked-list.
    /// </summary>
    /// <param name="newAction">Action to be added.</param>
    /// <returns>true if action was appended, false if it was merged with the previous one</returns>
    public bool AppendAction(IAction newAction)
    {
        if (CurrentState.PreviousAction != null && CurrentState.PreviousAction.TryToMerge(newAction))
        {
            RaiseUndoBufferChanged();
            return false;
        }

        CurrentState.NextAction = newAction;
        CurrentState.NextNode = new SimpleHistoryNode(newAction, CurrentState);
        return true;
    }

    /// <summary>
    /// All existing Nodes and Actions are garbage collected.
    /// </summary>
    public void Clear()
    {
        Init();
        RaiseUndoBufferChanged();
    }

    public IEnumerable<IAction> EnumUndoableActions()
    {
        var current = Head;

        while (current != null && current != CurrentState && current.NextAction != null)
        {
            yield return current.NextAction;

            current = current.NextNode;
        }
    }

    public IEnumerable<IAction> EnumRedoableActions()
    {
        var current = CurrentState;

        while (current != null && current.NextAction != null)
        {
            yield return current.NextAction;

            current = current.NextNode;
        }
    }

    public void MoveForward()
    {
        if (!CanMoveForward)
        {
            throw new InvalidOperationException(
                "History.MoveForward() cannot execute because"
                + " CanMoveForward returned false (the current state"
                + " is the last state in the undo buffer.");
        }

        CurrentState.NextAction.Execute();
        CurrentState = CurrentState.NextNode;
        Length += 1;
        RaiseUndoBufferChanged();
    }

    public void MoveBack()
    {
        if (!CanMoveBack)
        {
            throw new InvalidOperationException(
                "History.MoveBack() cannot execute because"
                + " CanMoveBack returned false (the current state"
                + " is the last state in the undo buffer.");
        }

        CurrentState.PreviousAction.UnExecute();
        CurrentState = CurrentState.PreviousNode;
        Length -= 1;
        RaiseUndoBufferChanged();
    }

    public IEnumerator<IAction> GetEnumerator() => EnumUndoableActions().GetEnumerator();

    #endregion

    #region Protected Methods

    protected void RaiseUndoBufferChanged()
    {
        CollectionChanged?.Invoke(this, new EventArgs());
    }

    #endregion

    #region Private Methods

    private void Init()
    {
        CurrentState = new SimpleHistoryNode();
        Head = CurrentState;
    }

    #endregion

    #region IEnumerable

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion
}