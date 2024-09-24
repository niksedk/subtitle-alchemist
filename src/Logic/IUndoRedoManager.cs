namespace SubtitleAlchemist.Logic;

public interface IUndoRedoManager
{
    void Do(UndoRedoObject? action);
    UndoRedoObject? Undo();
    UndoRedoObject? Redo();
    bool CanUndo { get; }
    bool CanRedo { get; }
}