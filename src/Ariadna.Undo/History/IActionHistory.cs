using System;
using System.Collections.Generic;

namespace Ariadna.Undo;

/// <summary>
/// A notion of the buffer. Instead of two stacks, it's a state machine
/// with the current state. It can move one state back or one state forward.
/// Allows for non-linear buffers, where you can choose one of several actions to redo.
/// </summary>
public interface IActionHistory : IEnumerable<IAction>
{
    #region Properties

    bool CanMoveBack { get; }
    bool CanMoveForward { get; }
    int Length { get; }

    SimpleHistoryNode CurrentState { get; }

    #endregion

    #region Events

    event EventHandler CollectionChanged;

    #endregion

    #region Methods

    /// <summary>
    /// Appends an action to the end of the Undo buffer.
    /// </summary>
    /// <param name="newAction">An action to append.</param>
    /// <returns>false if merged with previous, else true</returns>
    bool AppendAction(IAction newAction);

    void Clear();

    void MoveBack();
    void MoveForward();
        
    IEnumerable<IAction> EnumUndoableActions();
    IEnumerable<IAction> EnumRedoableActions();

    #endregion
}