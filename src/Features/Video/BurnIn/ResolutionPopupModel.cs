using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubtitleAlchemist.Logic.Constants;
using Label = Microsoft.Maui.Controls.Label;

namespace SubtitleAlchemist.Features.Video.BurnIn;

public partial class ResolutionPopupModel : ObservableObject
{
    public ResolutionPopup? Popup { get; set; }

    [ObservableProperty]
    private ObservableCollection<ResolutionItem> _resolutionItems;

    [ObservableProperty]
    private ResolutionItem? _selectedResolution;

    public ResolutionPopupModel()
    {
        _resolutionItems = new ObservableCollection<ResolutionItem>(ResolutionItem.GetResolutions());
    }

    [RelayCommand]
    private void Cancel()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Popup?.Close();
        });
    }

    public void PointerEntered(object? sender, PointerEventArgs e)
    {
        if (sender is Label { BindingContext: ResolutionItem item } && item.ItemType != ResolutionItemType.Separator)
        {
            item.BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.ActiveBackgroundColor];
        }
    }

    public void PointerExited(object? sender, PointerEventArgs e)
    {
        if (sender is Label { BindingContext: ResolutionItem item })
        {
            item.BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.BackgroundColor];
        }
    }

    public void TappedSingle(object? sender, TappedEventArgs e)
    {
        if (sender is Label { BindingContext: ResolutionItem item } && item.ItemType != ResolutionItemType.Separator)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Popup?.Close(item);
            });
        }
    }
}