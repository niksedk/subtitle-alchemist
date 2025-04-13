using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Features.Video.TextToSpeech;

public partial class ElevenLabSettingsPopupModel : ObservableObject
{
    public ElevenLabSettingsPopup? Popup { get; set; }

    [ObservableProperty]
    public partial double Stability { get; set; }

    [ObservableProperty]
    public partial double Similarity { get; set; }

    [ObservableProperty]
    public partial double SpeakerBoost { get; set; }

    public ElevenLabSettingsPopupModel()
    {
        Stability = Se.Settings.Video.TextToSpeech.ElevenLabsStability;
        Similarity = Se.Settings.Video.TextToSpeech.ElevenLabsSimilarity;
        SpeakerBoost = Se.Settings.Video.TextToSpeech.ElevenLabsSpeakerBoost;
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

    [RelayCommand]
    public void MoreInfo()
    {
        UiUtil.OpenUrl("https://elevenlabs.io/docs/speech-synthesis/voice-settings");
    }
}