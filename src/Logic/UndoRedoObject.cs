using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Logic;

public class UndoRedoObject
{
    public string Description { get; set; }
    public Subtitle Subtitle { get; set; }
    public string SubtitleFileName { get; set; }
    public int[] SelectedIndices { get; set; }
    public int CaretIndex { get; set; }
    public int SelectionLength { get; set; }

    public UndoRedoObject(string description, Subtitle subtitle, string subtitleFileName, int[] selectedIndices, int caretIndex, int selectionLength)
    {
        Description = description;
        Subtitle = subtitle;
        SubtitleFileName = subtitleFileName;
        SelectedIndices = selectedIndices;
        CaretIndex = caretIndex;
        SelectionLength = selectionLength;
    }
}
