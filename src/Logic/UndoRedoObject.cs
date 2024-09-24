using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Logic;

public class UndoRedoObject
{
    public string Text { get; set; }
    public List<int> SelectedIndices { get; set; }
    public int CaretIndex { get; set; }
    public int SelectionLength { get; set; }
    public Subtitle Subtitle { get; set; }
}
