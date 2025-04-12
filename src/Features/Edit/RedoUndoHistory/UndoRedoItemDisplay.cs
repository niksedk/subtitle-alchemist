using CommunityToolkit.Mvvm.ComponentModel;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Edit.RedoUndoHistory;

public partial class UndoRedoItemDisplay : ObservableObject
{
    [ObservableProperty] public partial string Description { get; set; }
    [ObservableProperty] public partial int SelectedLineNumber { get; set; }
    [ObservableProperty] public partial int NumberOfLines { get; set; }
    [ObservableProperty] public partial DateTime Created { get; set; }

    public UndoRedoItem Item { get; }

    public UndoRedoItemDisplay(UndoRedoItem item)
    {
        Description = item.Description;
        SelectedLineNumber = item.SelectedLines.First();
        Created = item.Created;
        NumberOfLines = item.Subtitle.Paragraphs.Count;
        Item = item;
    }
}
