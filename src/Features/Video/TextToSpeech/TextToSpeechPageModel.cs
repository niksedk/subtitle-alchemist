using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Features.Main;

namespace SubtitleAlchemist.Features.Video.TextToSpeech;

public partial class TextToSpeechPageModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private ObservableCollection<DisplayParagraph> _paragraphs = new();

    [ObservableProperty]
    private TimeSpan _adjustSeconds;

    [ObservableProperty]
    private ObservableCollection<string> _adjustViaItems;

    [ObservableProperty]
    private string _selectedAdjustViaItem;

    [ObservableProperty]
    private int _adjustPercentage;

    [ObservableProperty]
    private decimal _adjustFixedValue;

    [ObservableProperty]
    private decimal _adjustRecalculateMaximumCharacters;

    [ObservableProperty]
    private decimal _adjustRecalculateOptimalCharacters;

    [ObservableProperty]
    private bool _adjustRecalculateExtendOnly;

    [ObservableProperty]
    private bool _enforceDurationLimits;

    [ObservableProperty]
    private bool _doNotExtendPastShotChanges;

    [ObservableProperty]
    private string _previewInfo = string.Empty;

    public TextToSpeechPage? Page { get; set; }
    public View ViewAdjustViaSeconds { get; set; }
    public View ViewAdjustViaPercent { get; set; }
    public View ViewAdjustViaFixed { get; set; }
    public View ViewAdjustRecalculate { get; set; }

    private Subtitle _subtitle = new();
    private List<int> _selectedIndices = new();
    private List<double> _shotChanges = new();
    private readonly System.Timers.Timer _previewTimer;

    public TextToSpeechPageModel()
    {
        _adjustViaItems = new ObservableCollection<string>
        {
            "Seconds",
            "Percent",
            "Fixed",
            "Recalculate"
        };

        _selectedAdjustViaItem = _adjustViaItems[0];

        ViewAdjustViaSeconds = new BoxView();
        ViewAdjustViaPercent = new BoxView();
        ViewAdjustViaFixed = new BoxView();
        ViewAdjustRecalculate = new BoxView();

        _previewTimer = new System.Timers.Timer(1000);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query["Subtitle"] is Subtitle subtitle)
        {
            _subtitle = new Subtitle(subtitle, false);
        }

        Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                
            });
            return false;
        });
    }

    [RelayCommand]
    public async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }

}
