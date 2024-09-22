using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Logic.Config;
using System.Collections.ObjectModel;

namespace SubtitleAlchemist.Features.Tools.AdjustDuration;

public partial class AdjustDurationModel : ObservableObject, IQueryAttributable
{
    internal const string ModeSeconds = "seconds";
    internal const string ModePercent = "percent";
    internal const string ModeFixed = "fixed";
    internal const string ModeRecalculate = "recalc";

    [ObservableProperty]
    private decimal _adjustSeconds;

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

    public AdjustDurationPage? Page { get; set; }
    public View ViewAdjustViaSeconds { get; set; }
    public View ViewAdjustViaPercent { get; set; }
    public View ViewAdjustViaFixed { get; set; }
    public View ViewAdjustRecalculate { get; set; }

    private Subtitle _subtitle = new();
    private List<int> _selectedIndices = new();
    private List<double> _shotChanges = new();

    public AdjustDurationModel()
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
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query["Subtitle"] is Subtitle subtitle)
        {
            _subtitle = subtitle;
        }

        if (query["SelectedIndexes"] is List<int> selectedIndices)
        {
            _selectedIndices = selectedIndices;
        }

        if (query["ShotChanges"] is List<double> shotChanges)
        {
            _shotChanges = shotChanges;
        }

        Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (Se.Settings.Tools.AdjustDurations.AdjustDurationLast == ModeSeconds)
                {
                    SelectedAdjustViaItem = AdjustViaItems[0];
                }
                else if (Se.Settings.Tools.AdjustDurations.AdjustDurationLast == ModePercent)
                {
                    SelectedAdjustViaItem = AdjustViaItems[1];
                }
                else if (Se.Settings.Tools.AdjustDurations.AdjustDurationLast == ModeFixed)
                {
                    SelectedAdjustViaItem = AdjustViaItems[2];
                }
                else if (Se.Settings.Tools.AdjustDurations.AdjustDurationLast == ModeRecalculate)
                {
                    SelectedAdjustViaItem = AdjustViaItems[3];
                }

                AdjustSeconds = Se.Settings.Tools.AdjustDurations.AdjustDurationSeconds;
                AdjustPercentage = Se.Settings.Tools.AdjustDurations.AdjustDurationPercent;
                AdjustRecalculateExtendOnly = Se.Settings.Tools.AdjustDurations.AdjustDurationExtendOnly;
                AdjustFixedValue = Se.Settings.Tools.AdjustDurations.AdjustDurationFixed;
                AdjustRecalculateMaximumCharacters = Se.Settings.Tools.AdjustDurations.AdjustDurationMaximumCps;
                AdjustRecalculateOptimalCharacters = Se.Settings.Tools.AdjustDurations.AdjustDurationOptimalCps;
                EnforceDurationLimits = Se.Settings.Tools.AdjustDurations.AdjustDurationExtendEnforceDurationLimits;
                DoNotExtendPastShotChanges = Se.Settings.Tools.AdjustDurations.AdjustDurationExtendCheckShotChanges;
            });
            return false;
        });
    }

    [RelayCommand]
    private async Task Ok()
    {
        Se.Settings.Tools.AdjustDurations.AdjustDurationSeconds = AdjustSeconds;
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
            Se.Settings.Tools.AdjustDurations.AdjustDurationLast = ModeSeconds;
            subtitle.AdjustDisplayTimeUsingSeconds((double)AdjustSeconds, _selectedIndices, _shotChanges, EnforceDurationLimits);
            info = "Subtitle durations adjusted by " + AdjustSeconds + " seconds.";
        }
        else if (SelectedAdjustViaItem == AdjustViaItems[1])
        {
            Se.Settings.Tools.AdjustDurations.AdjustDurationLast = ModePercent;
            subtitle.AdjustDisplayTimeUsingPercent(AdjustPercentage, _selectedIndices, _shotChanges, EnforceDurationLimits);
            info = "Subtitle durations adjusted by " + AdjustPercentage + "%.";
        }
        else if (SelectedAdjustViaItem == AdjustViaItems[2])
        {
            Se.Settings.Tools.AdjustDurations.AdjustDurationLast = ModeFixed;
            subtitle.SetFixedDuration(_selectedIndices, (double)AdjustFixedValue, _shotChanges);
            info = "Subtitle durations set to " + AdjustFixedValue + " seconds.";
        }
        else if (SelectedAdjustViaItem == AdjustViaItems[3])
        {
            Se.Settings.Tools.AdjustDurations.AdjustDurationLast = ModeRecalculate;
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
