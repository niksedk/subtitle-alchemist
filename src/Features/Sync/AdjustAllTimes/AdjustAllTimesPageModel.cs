using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.Settings;
using SubtitleAlchemist.Features.Main;

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

    [ObservableProperty]
    private string _totalAdjustmentInfo = string.Empty;

    [ObservableProperty]
    private ObservableCollection<DisplayParagraph> _paragraphs = new();

    private long _totalAdjustmentMs;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query["Subtitle"] is Subtitle subtitle)
        {
            Initialize(subtitle);
        }
    }

    private void Initialize(Subtitle subtitle)
    {
        Paragraphs = new ObservableCollection<DisplayParagraph>(subtitle.Paragraphs.Select(p => new DisplayParagraph(p)));
    }

    [RelayCommand]
    private async Task ShowEarlier()
    {
        _totalAdjustmentMs -= (long)Math.Round(AdjustTime.TotalMilliseconds, MidpointRounding.AwayFromZero);
        ShowTotalAdjustment();
    }

    [RelayCommand]
    private async Task ShowLater()
    {
        _totalAdjustmentMs += (long)Math.Round(AdjustTime.TotalMilliseconds, MidpointRounding.AwayFromZero);
        ShowTotalAdjustment();
    }

    private void ShowTotalAdjustment()
    {
        var timeCode = new TimeCode(_totalAdjustmentMs);
        TotalAdjustmentInfo = $"Total adjustment: {timeCode.ToShortDisplayString()}";
    }

    [RelayCommand]
    public async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }

    public void SubtitlesViewSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
    }
}
