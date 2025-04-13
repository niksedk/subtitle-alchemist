using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SubtitleAlchemist.Features.Video.TextToSpeech;

public partial class AudioSettingsPopupModel : ObservableObject
{
    public AudioSettingsPopup? Popup { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<string> Encodings { get; set; }

    [ObservableProperty]
    public partial string? SelectedEncoding { get; set; }

    [ObservableProperty]
    public partial bool IsStereo { get; set; }

    public AudioSettingsPopupModel()
    {
        Encodings = new ObservableCollection<string>
        {
            "copy",
            "aac",
            "ac3",
            "eac3",
            "truehd",
            "libvorbis",
            "libmp3lame",
            "libopus",
        };
        SelectedEncoding = Encodings[0];
        IsStereo = false;
    }

    private void Close()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Popup?.Close();
        });
    }

    [RelayCommand]
    public void Ok()
    {
        Close();
    }

    [RelayCommand]
    public void Cancel()
    {
        Close();
    }
}