namespace SubtitleAlchemist.Logic;

public interface IUndoRedoManager
{
    void Do(UndoRedoItem? action);
    UndoRedoItem? Undo();
    UndoRedoItem? Redo();
    bool CanUndo { get; }
    bool CanRedo { get; }
}