using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SubtitleAlchemist.Features.Sync.AdjustAllTimes;

public partial class AdjustAllTimesPageModel : ObservableObject, IQueryAttributable
{
    public AdjustAllTimesPage? Page { get; set; }

    [ObservableProperty]
    private bool _allLines;

    [ObservableProperty]
    private bool _selectedLinesOnly;

    [ObservableProperty]
    private bool _selectedAndSubsequentLines;

    [ObservableProperty]
    private TimeSpan _adjustTime;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        
    }

    [RelayCommand]
    private async Task Ok()
    {

    }

    [RelayCommand]
    public async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }
}
