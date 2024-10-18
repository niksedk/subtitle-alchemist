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

    private void Close(string? text)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Popup?.Close(text);
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
        Close(Text);
    }

    [RelayCommand]
    public void Cancel()
    {
        Close(null);
    }
}