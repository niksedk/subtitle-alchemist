using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using SubtitleAlchemist.Logic.Config;
using SharpHook.Native;

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
        });
    }

    [RelayCommand]
    private async Task Ok()
    {
        Se.Settings.Tools.AdjustDurations.AdjustDurationSeconds = AdjustSeconds;
        Se.Settings.Tools.AdjustDurations.AdjustDurationPercent = AdjustPercentage;
        Se.Settings.Tools.AdjustDurations.AdjustDurationExtendOnly = AdjustRecalculateExtendOnly;
        Se.Settings.Tools.AdjustDurations.AdjustDurationExtendEnforceDurationLimits = EnforceDurationLimits;
        Se.Settings.Tools.AdjustDurations.AdjustDurationExtendCheckShotChanges = DoNotExtendPastShotChanges;

        if (SelectedAdjustViaItem == AdjustViaItems[0])
        {
            Se.Settings.Tools.AdjustDurations.AdjustDurationLast = ModeSeconds;
        }
        else if (SelectedAdjustViaItem == AdjustViaItems[1])
        {
            Se.Settings.Tools.AdjustDurations.AdjustDurationLast = ModePercent;
        }
        else if (SelectedAdjustViaItem == AdjustViaItems[2])
        {
            Se.Settings.Tools.AdjustDurations.AdjustDurationLast = ModeFixed;
        }
        else if (SelectedAdjustViaItem == AdjustViaItems[3])
        {
            Se.Settings.Tools.AdjustDurations.AdjustDurationLast = ModeRecalculate;
        }

        Se.SaveSettings();

        await Shell.Current.GoToAsync("..", new Dictionary<string, object>
        {
            { "addSeconds", AdjustSeconds },
            { "adjustPercentage", AdjustPercentage },
            { "adjustFixedValue", AdjustFixedValue },
            { "adjustRecalculateMaximumCharacters", AdjustRecalculateMaximumCharacters },
            { "adjustRecalculateOptimalCharacters", AdjustRecalculateOptimalCharacters },
            { "adjustRecalculateExtendOnly", AdjustRecalculateExtendOnly },
            { "enforceDurationLimits", EnforceDurationLimits },
            { "doNotExtendPastShotChanges", DoNotExtendPastShotChanges }
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
