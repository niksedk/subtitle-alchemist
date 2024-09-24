namespace SubtitleAlchemist.Logic;

public class UndoRedoManager : IUndoRedoManager
{
    private readonly List<UndoRedoObject?> _undoList = new();
    private readonly List<UndoRedoObject?> _redoList = new();
    private const int MaxUndoItems = 100;

    public void Do(UndoRedoObject? action)
    {
        _undoList.Add(action);
        if (_undoList.Count > MaxUndoItems)
        {
            _undoList.RemoveAt(0);
        }

        _redoList.Clear();
    }

    public UndoRedoObject? Undo()
    {
        if (_undoList.Count > 0)
        {
            var action = _undoList[^1];
            _undoList.RemoveAt(_undoList.Count - 1);
            _redoList.Add(action);
            return action;
        }

        return null;
    }

    public UndoRedoObject? Redo()
    {
        if (_redoList.Count > 0)
        {
            var action = _redoList[^1];
            _redoList.RemoveAt(_redoList.Count - 1);
            _undoList.Add(action);
            return action;
        }

        return null;
    }

    public bool CanUndo => _undoList.Count > 0;
    public bool CanRedo => _redoList.Count > 0;
}
