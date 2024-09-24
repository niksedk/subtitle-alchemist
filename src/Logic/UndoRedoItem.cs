using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Logic;

public class UndoRedoItem
{
    public string Description { get; set; }
    public Subtitle Subtitle { get; set; }
    public string SubtitleFileName { get; set; }
    public int[] SelectedLines { get; set; }
    public int CaretIndex { get; set; }
    public int SelectionLength { get; set; }
    public DateTime Created { get; set; }

    public UndoRedoItem(
        string description, 
        Subtitle subtitle, 
        string subtitleFileName, 
        int[] selectedLines, 
        int caretIndex, 
        int selectionLength)
    {
        Description = description;
        Subtitle = subtitle;
        SubtitleFileName = subtitleFileName;
        SelectedLines = selectedLines;
        CaretIndex = caretIndex;
        SelectionLength = selectionLength;
        Created = DateTime.Now;
    }
}
