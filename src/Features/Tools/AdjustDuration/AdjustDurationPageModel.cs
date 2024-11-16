using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Logic.Config;
using System.Collections.ObjectModel;
using System.Timers;
using SubtitleAlchemist.Features.Main;

namespace SubtitleAlchemist.Features.Tools.AdjustDuration;

public partial class AdjustDurationPageModel : ObservableObject, IQueryAttributable
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

    public AdjustDurationPage? Page { get; set; }
    public View ViewAdjustViaSeconds { get; set; }
    public View ViewAdjustViaPercent { get; set; }
    public View ViewAdjustViaFixed { get; set; }
    public View ViewAdjustRecalculate { get; set; }

    private Subtitle _subtitle = new();
    private List<int> _selectedIndices = new();
    private List<double> _shotChanges = new();
    private readonly System.Timers.Timer _previewTimer;
    private int _previewLastHash = -1;

    public AdjustDurationPageModel()
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
        _previewTimer.Elapsed += PreviewTimerElapsed;
    }

    private void PreviewTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        _previewTimer.Stop();

        var subtitle = new Subtitle(_subtitle, false);
        var idx = AdjustViaItems.IndexOf(SelectedAdjustViaItem);
        switch (idx)
        {
            case 0: // seconds
                subtitle.AdjustDisplayTimeUsingSeconds(AdjustSeconds.TotalSeconds, null, _shotChanges, EnforceDurationLimits);
                UpdateParagraphs(subtitle);
                break;
            case 1: // percent
                subtitle.AdjustDisplayTimeUsingPercent(AdjustPercentage, null, _shotChanges, EnforceDurationLimits);
                UpdateParagraphs(subtitle);
                break;
            case 2: // fixed
                subtitle.SetFixedDuration(null, (double)AdjustFixedValue, _shotChanges);
                UpdateParagraphs(subtitle);
                break;
            case 3: // recalculate
                subtitle.RecalculateDisplayTimes(
                    (double)AdjustRecalculateMaximumCharacters,
                    _selectedIndices,
                    (double)AdjustRecalculateOptimalCharacters,
                    AdjustRecalculateExtendOnly,
                    _shotChanges);
                UpdateParagraphs(subtitle);
                break;
        }

        _previewTimer.Start();
    }

    private void UpdateParagraphs(Subtitle subtitle)
    {
        if (Paragraphs.Count != subtitle.Paragraphs.Count)
        {
            Paragraphs = new ObservableCollection<DisplayParagraph>(subtitle.Paragraphs.Select(p => new DisplayParagraph(p)));
        }
        else
        {
            var hash = subtitle.GetFastHashCode(string.Empty);
            if (_previewLastHash == hash)
            {
                return; // no changes
            }

            double totalDurationMs = 0;
            double originalTotalDurationMs = 0;
            Page?.BatchBegin();
            for (var i = 0; i < subtitle.Paragraphs.Count; i++)
            {
                Paragraphs[i].End = subtitle.Paragraphs[i].EndTime.TimeSpan;
                Paragraphs[i].Duration = subtitle.Paragraphs[i].Duration.TimeSpan;
                totalDurationMs += subtitle.Paragraphs[i].Duration.TotalMilliseconds;
                originalTotalDurationMs += _subtitle.Paragraphs[i].Duration.TotalMilliseconds;
            }
            Page?.BatchCommit();

            _previewLastHash = hash;

            if (subtitle.Paragraphs.Count > 0)
            {
                var averageDurationMs = totalDurationMs / subtitle.Paragraphs.Count;
                var originalAverageDurationMs = originalTotalDurationMs / subtitle.Paragraphs.Count;
                PreviewInfo = $"New average duration: {new TimeCode(averageDurationMs).ToShortDisplayString()}, old average duration: {new TimeCode(originalAverageDurationMs).ToShortDisplayString()}";
            }
            else
            {
                PreviewInfo = string.Empty;
            }
        }
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query["Subtitle"] is Subtitle subtitle)
        {
            _subtitle = new Subtitle(subtitle, false);
            Paragraphs = new ObservableCollection<DisplayParagraph>(subtitle.Paragraphs.Select(p => new DisplayParagraph(p)));
        }

        if (query["SelectedIndexes"] is List<int> selectedIndices)
        {
            _selectedIndices = selectedIndices;
        }

        if (query["ShotChanges"] is List<double> shotChanges)
        {
            _shotChanges = shotChanges;
        }

        Page?.Initialize(_subtitle, this);

        Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (Se.Settings.Tools.AdjustDurations.AdjustDurationLast == AdjustDurationType.Seconds.ToString())
                {
                    SelectedAdjustViaItem = AdjustViaItems[0];
                }
                else if (Se.Settings.Tools.AdjustDurations.AdjustDurationLast == AdjustDurationType.Percent.ToString())
                {
                    SelectedAdjustViaItem = AdjustViaItems[1];
                }
                else if (Se.Settings.Tools.AdjustDurations.AdjustDurationLast == AdjustDurationType.Fixed.ToString())
                {
                    SelectedAdjustViaItem = AdjustViaItems[2];
                }
                else if (Se.Settings.Tools.AdjustDurations.AdjustDurationLast == AdjustDurationType.Recalculate.ToString())
                {
                    SelectedAdjustViaItem = AdjustViaItems[3];
                }

                AdjustSeconds = TimeSpan.FromSeconds(Se.Settings.Tools.AdjustDurations.AdjustDurationSeconds);
                AdjustPercentage = Se.Settings.Tools.AdjustDurations.AdjustDurationPercent;
                AdjustRecalculateExtendOnly = Se.Settings.Tools.AdjustDurations.AdjustDurationExtendOnly;
                AdjustFixedValue = Se.Settings.Tools.AdjustDurations.AdjustDurationFixed;
                AdjustRecalculateMaximumCharacters = Se.Settings.Tools.AdjustDurations.AdjustDurationMaximumCps;
                AdjustRecalculateOptimalCharacters = Se.Settings.Tools.AdjustDurations.AdjustDurationOptimalCps;
                EnforceDurationLimits = Se.Settings.Tools.AdjustDurations.AdjustDurationExtendEnforceDurationLimits;
                DoNotExtendPastShotChanges = Se.Settings.Tools.AdjustDurations.AdjustDurationExtendCheckShotChanges;

                _previewTimer.Start();
            });
            return false;
        });
    }

    [RelayCommand]
    private async Task Ok()
    {
        Se.Settings.Tools.AdjustDurations.AdjustDurationSeconds = AdjustSeconds.TotalSeconds;
        Se.Settings.Tools.AdjustDurations.AdjustDurationPercent = AdjustPercentage;
        Se.Settings.Tools.AdjustDurations.AdjustDurationFixed = AdjustFixedValue;
        Se.Settings.Tools.AdjustDurations.AdjustDurationMaximumCps = AdjustRecalculateMaximumCharacters;
        Se.Settings.Tools.AdjustDurations.AdjustDurationOptimalCps = AdjustRecalculateOptimalCharacters;
        Se.Settings.Tools.AdjustDurations.AdjustDurationExtendOnly = AdjustRecalculateExtendOnly;
        Se.Settings.Tools.AdjustDurations.AdjustDurationExtendEnforceDurationLimits = EnforceDurationLimits;
        Se.Settings.Tools.AdjustDurations.AdjustDurationExtendCheckShotChanges = DoNotExtendPastShotChanges;

        var subtitle = new Subtitle(_subtitle, false);
        var info = string.Empty;

        if (SelectedAdjustViaItem == AdjustViaItems[0])
        {
            Se.Settings.Tools.AdjustDurations.AdjustDurationLast = AdjustDurationType.Seconds.ToString();
            subtitle.AdjustDisplayTimeUsingSeconds(AdjustSeconds.TotalSeconds, _selectedIndices, _shotChanges, EnforceDurationLimits);
            info = "Subtitle durations adjusted by " + AdjustSeconds + " seconds.";
        }
        else if (SelectedAdjustViaItem == AdjustViaItems[1])
        {
            Se.Settings.Tools.AdjustDurations.AdjustDurationLast = AdjustDurationType.Percent.ToString();
            subtitle.AdjustDisplayTimeUsingPercent(AdjustPercentage, _selectedIndices, _shotChanges, EnforceDurationLimits);
            info = "Subtitle durations adjusted by " + AdjustPercentage + "%.";
        }
        else if (SelectedAdjustViaItem == AdjustViaItems[2])
        {
            Se.Settings.Tools.AdjustDurations.AdjustDurationLast = AdjustDurationType.Fixed.ToString();
            subtitle.SetFixedDuration(_selectedIndices, (double)AdjustFixedValue, _shotChanges);
            info = "Subtitle durations set to " + AdjustFixedValue + " seconds.";
        }
        else if (SelectedAdjustViaItem == AdjustViaItems[3])
        {
            Se.Settings.Tools.AdjustDurations.AdjustDurationLast = AdjustDurationType.Recalculate.ToString();
            subtitle.RecalculateDisplayTimes(
                (double)AdjustRecalculateMaximumCharacters,
                _selectedIndices,
                (double)AdjustRecalculateOptimalCharacters,
                AdjustRecalculateExtendOnly, 
                _shotChanges);
            info = "Subtitle durations recalculated.";
        }

        Se.SaveSettings();

        await Shell.Current.GoToAsync("..", new Dictionary<string, object>
        {
            { "Subtitle", subtitle },
            { "Status", info },
        });
    }

    [RelayCommand]
    public async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }

    public void PickerAdjustVia_SelectedIndexChanged(object? sender, EventArgs e)
    {
        ViewAdjustViaSeconds.IsVisible = false;
        ViewAdjustViaPercent.IsVisible = false;
        ViewAdjustViaFixed.IsVisible = false;
        ViewAdjustRecalculate.IsVisible = false;

        if (SelectedAdjustViaItem == AdjustViaItems[0])
        {
            ViewAdjustViaSeconds.Opacity = 0;
            ViewAdjustViaSeconds.IsVisible = true;
            ViewAdjustViaSeconds.FadeTo(1, 200);
        }
        else if (SelectedAdjustViaItem == AdjustViaItems[1])
        {
            ViewAdjustViaPercent.Opacity = 0;
            ViewAdjustViaPercent.IsVisible = true;
            ViewAdjustViaPercent.FadeTo(1, 200);
        }
        else if (SelectedAdjustViaItem == AdjustViaItems[2])
        {
            ViewAdjustViaFixed.Opacity = 0;
            ViewAdjustViaFixed.IsVisible = true;
            ViewAdjustViaFixed.FadeTo(1, 200);
        }
        else if (SelectedAdjustViaItem == AdjustViaItems[3])
        {
            ViewAdjustRecalculate.Opacity = 0;
            ViewAdjustRecalculate.IsVisible = true;
            ViewAdjustRecalculate.FadeTo(1, 200);
        }
    }
}
