using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Logic;

public class UndoRedoObject
{
    public Subtitle Subtitle { get; set; }
    public List<int> SelectedIndices { get; set; }
    public int CaretIndex { get; set; }
    public int SelectionLength { get; set; }

    public UndoRedoObject(Subtitle subtitle, List<int> selectedIndices, int caretIndex, int selectionLength)
    {
        Subtitle = subtitle;
        SelectedIndices = selectedIndices;
        CaretIndex = caretIndex;
        SelectionLength = selectionLength;
    }
}
