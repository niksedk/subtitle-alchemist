using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Features.Video.TextToSpeech;

public partial class ElevenLabSettingsPopupModel : ObservableObject
{
    public ElevenLabSettingsPopup? Popup { get; set; }

    [ObservableProperty]
    private double _stability;

    [ObservableProperty]
    private double _similarity;

    public ElevenLabSettingsPopupModel()
    {
        _stability = Se.Settings.Video.TextToSpeech.ElevenLabsStability;
        _similarity = Se.Settings.Video.TextToSpeech.ElevenLabsSimilarity;
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
        Se.Settings.Video.TextToSpeech.ElevenLabsStability = Stability;
        Se.Settings.Video.TextToSpeech.ElevenLabsSimilarity = Similarity;

        Close();
    }

    [RelayCommand]
    public void Cancel()
    {
        Close();
    }
}