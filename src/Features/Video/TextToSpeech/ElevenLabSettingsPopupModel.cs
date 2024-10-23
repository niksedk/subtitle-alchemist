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

    [ObservableProperty]
    private double _speakerBoost;

    public ElevenLabSettingsPopupModel()
    {
        _stability = Se.Settings.Video.TextToSpeech.ElevenLabsStability;
        _similarity = Se.Settings.Video.TextToSpeech.ElevenLabsSimilarity;
        _speakerBoost = Se.Settings.Video.TextToSpeech.ElevenLabsSpeakerBoost;
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
        Se.Settings.Video.TextToSpeech.ElevenLabsSpeakerBoost = SpeakerBoost;

        Close();
    }

    [RelayCommand]
    public void Cancel()
    {
        Close();
    }
}