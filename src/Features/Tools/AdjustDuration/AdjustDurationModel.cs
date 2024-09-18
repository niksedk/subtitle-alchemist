using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Features.Tools.AdjustDuration;

public partial class AdjustDurationModel : ObservableObject, IQueryAttributable
{

    [ObservableProperty]
    private decimal _addSeconds;

    [ObservableProperty]
    private ObservableCollection<string> _adjustViaItems;

    [ObservableProperty]
    private string _selectedAdjustViaItem;

    [ObservableProperty]
    private decimal _adjustPercentage;

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
