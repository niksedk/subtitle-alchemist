using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Features.Video.TextToSpeech.Engines;
using System.Collections.ObjectModel;
using SubtitleAlchemist.Features.Video.TextToSpeech.Voices;

namespace SubtitleAlchemist.Features.Video.TextToSpeech;

public partial class TextToSpeechPageModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private ObservableCollection<ITtsEngine> _engines = new();

    [ObservableProperty]
    private ITtsEngine? _selectedEngine;

    [ObservableProperty]
    private ObservableCollection<Voice> _voices = new();

    [ObservableProperty]
    private Voice? _selectedVoice;

    [ObservableProperty]
    private bool _hasLanguageParameter;

    [ObservableProperty]
    private int _voiceCount;

    [ObservableProperty]
    private string _voiceTestText;

    [ObservableProperty]
    private bool _doReviewAudioClips;

    [ObservableProperty]
    private bool _doGenerateVideoFile;

    [ObservableProperty]
    private bool _useCustomAudioEncoding;


    public TextToSpeechPage? Page { get; set; }


    private Subtitle _subtitle = new();


    public TextToSpeechPageModel()
    {
        _engines = new ObservableCollection<ITtsEngine>
        {
            new Piper(),
        };
        _selectedEngine = _engines.FirstOrDefault();

        _voices = new ObservableCollection<Voice>();

        _voiceTestText = "Hello, how are you today?";
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query["Subtitle"] is Subtitle subtitle)
        {
            _subtitle = new Subtitle(subtitle, false);
        }

        Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {

            });
            return false;
        });
    }

    [RelayCommand]
    public async Task GenerateTts()
    {
    }

    [RelayCommand]
    public async Task TestVoice()
    {
    }

    [RelayCommand]
    public async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }

    public void SelectedEngineChanged(object? sender, EventArgs e)
    {
        if (SelectedEngine != null)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                var voices = await SelectedEngine.GetVoices();
                Voices = new ObservableCollection<Voice>(voices);
                VoiceCount = Voices.Count;
                SelectedVoice = Voices.FirstOrDefault();
                HasLanguageParameter = SelectedEngine.HasLanguageParameter;
            });
        }

    }
}
