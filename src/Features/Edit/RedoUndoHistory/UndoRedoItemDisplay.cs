using CommunityToolkit.Mvvm.ComponentModel;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Edit.RedoUndoHistory;

public partial class UndoRedoItemDisplay : ObservableObject
{
    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    private int _selectedLineNumber;

    [ObservableProperty]
    private int _numberOfLines;

    [ObservableProperty]
    private DateTime _created;

    public UndoRedoItem Item { get; }

    public UndoRedoItemDisplay(UndoRedoItem item)
    {
        _description = item.Description;
        _selectedLineNumber = item.SelectedLines.First();
        _created = item.Created;
        _numberOfLines = item.Subtitle.Paragraphs.Count;
        Item = item;
    }
}
