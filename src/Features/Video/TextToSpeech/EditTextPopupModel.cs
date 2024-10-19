using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SubtitleAlchemist.Features.Video.TextToSpeech;

public partial class EditTextPopupModel : ObservableObject
{
    public EditTextPopup? Popup { get; set; }

    [ObservableProperty]
    private string? _text;

    public EditTextPopupModel()
    {
        _text = string.Empty;
    }

    private void Close(EditTextPopupResult? result)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Popup?.Close(result);
        });
    }

    public void Initialize(string text)
    {
        Popup?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Text = text;
            });

            return false;
        });
    }

    [RelayCommand]
    public void Ok()
    {
        Close(new EditTextPopupResult { Text = Text ?? string.Empty, Regenerate = false });
    }

    [RelayCommand]
    public void OkAndRegenerate()
    {
        Close(new EditTextPopupResult { Text = Text ?? string.Empty, Regenerate = true });
    }

    [RelayCommand]
    public void Cancel()
    {
        Close(null);
    }
}