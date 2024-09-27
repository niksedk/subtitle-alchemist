using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Controls.AudioVisualizerControl;
using SubtitleAlchemist.Features.Main;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Views;

namespace SubtitleAlchemist.Features.Sync.AdjustAllTimes;

public partial class AdjustAllTimesPageModel : ObservableObject, IQueryAttributable
{
    public AdjustAllTimesPage? Page { get; set; }
    public CollectionView SubtitleList { get; internal set; } = new();
    public AudioVisualizer AudioVisualizer { get; internal set; } = new();
    public MediaElement VideoPlayer { get; set; } = new();

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
        if (Page != null &&
            query["Subtitle"] is Subtitle subtitle &&
            query["VideoFileName"] is string videoFileName &&
            query["WavePeaks"] is WavePeakData wavePeakData)
        {
            Page!.Initialize(subtitle, videoFileName, wavePeakData, this);
        }
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
