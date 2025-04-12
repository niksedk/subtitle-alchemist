using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Edit.RedoUndoHistory;

public partial class UndoRedoHistoryPageModel : ObservableObject, IQueryAttributable
{
    public UndoRedoHistoryPage? Page { get; set; }

    [ObservableProperty] public partial ObservableCollection<UndoRedoItemDisplay> UndoRedoItems { get; set; } = new();
    [ObservableProperty] public partial UndoRedoItemDisplay? SelectedUndoRedoItem { get; set; }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query["UndoRedoManager"] is UndoRedoManager undoRedoManager)
        {
            foreach (var undoRedoItem in undoRedoManager.UndoList)
            {
                UndoRedoItems.Add(new UndoRedoItemDisplay(undoRedoItem));
            }
        }
    }

    [RelayCommand]
    public async Task CompareHistoryItems()
    {

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    public async Task CompareWithCurrent()
    {

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    public async Task Cancel()
    {
       
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    public async Task RollbackTo()
    {
        if (SelectedUndoRedoItem is not UndoRedoItemDisplay item)
        {
            return;
        }

        await Shell.Current.GoToAsync("..", new Dictionary<string, object>
        {
            { "Page", nameof(UndoRedoHistoryPage) },
            { "UndoRedoItem", item.Item },
        });
    }

    public void SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        
    }
}
