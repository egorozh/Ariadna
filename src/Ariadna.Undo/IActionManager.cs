using System;

namespace Ariadna.Undo;

public interface IActionManager
{
    IActionHistory History { get; }

    event EventHandler CollectionChanged;
    bool CanUndo { get; }
    bool CanRedo { get; }
    void RecordAction(IAction action);
    void Undo();
    void Redo();
    Transaction CreateTransaction();
    void CommitTransaction();

    void Clear();
}