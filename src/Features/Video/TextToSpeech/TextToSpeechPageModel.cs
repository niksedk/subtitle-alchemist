using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Features.Video.TextToSpeech.Engines;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Core;
using SubtitleAlchemist.Features.Video.TextToSpeech.Voices;
using SubtitleAlchemist.Services;
using Plugin.Maui.Audio;
using SubtitleAlchemist.Features.Video.TextToSpeech.DownloadTts;

namespace SubtitleAlchemist.Features.Video.TextToSpeech;

public partial class TextToSpeechPageModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private ObservableCollection<ITtsEngine> _engines;

    [ObservableProperty]
    private ITtsEngine? _selectedEngine;

    [ObservableProperty]
    private ObservableCollection<Voice> _voices;

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
    private readonly IAudioManager _audioManager;
    private readonly IPopupService _popupService;

    public TextToSpeechPageModel(ITtsDownloadService ttsDownloadService, IAudioManager audioManager, IPopupService popupService)
    {
        _audioManager = audioManager;
        _popupService = popupService;
        _engines = new ObservableCollection<ITtsEngine>
        {
            new Piper(ttsDownloadService),
        };
        _selectedEngine = _engines.FirstOrDefault();

        _voices = new ObservableCollection<Voice>();

        _voiceTestText = "Hello, how are you doing?";
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
        var engine = SelectedEngine;
        if (engine == null)
        {
            return;
        }

        var isInstalled = await IsEngineInstalled(engine);
        if (!isInstalled)
        {
            return;
        }

        foreach (var paragraph in _subtitle.Paragraphs)
        {
            
        }
    }

    private async Task<bool> IsEngineInstalled(ITtsEngine engine)
    {
        if (engine.IsInstalled)
        {
            return true;
        }

        if (engine is Piper && Page != null)
        {
            var answer = await Page.DisplayAlert(
                "Download Piper?",
                $"{Environment.NewLine}\"Text to speech\" requires Piper.{Environment.NewLine}{Environment.NewLine}Download and use Piper?",
                "Yes",
                "No");

            if (!answer)
            {
                return false;
            }

            var result = await _popupService.ShowPopupAsync<DownloadTtsPopupModel>(onPresenting: viewModel => viewModel.StartDownloadPiper(), CancellationToken.None);
            return engine.IsInstalled;
        }

        return false;
    }

    [RelayCommand]
    public async Task TestVoice()
    {
        var engine = SelectedEngine;
        var voice = SelectedVoice;
        if (engine == null || voice == null)
        {
            return;
        }

        var isInstalled = await IsEngineInstalled(engine);
        if (!isInstalled)
        {
            return;
        }

        if (!engine.IsVoiceInstalled(voice) && voice.EngineVoice is PiperVoice piperVoice)
        {
            var modelFileName = Path.Combine(Piper.GetSetPiperFolder(), piperVoice.ModelShort);
            if (!File.Exists(modelFileName))
            {
                var r1 = await _popupService.ShowPopupAsync<DownloadTtsPopupModel>(onPresenting: viewModel => viewModel.StartDownloadPiperVoice(piperVoice), CancellationToken.None);
            }

            var configFileName = Path.Combine(Piper.GetSetPiperFolder(), piperVoice.ConfigShort);
            if (!File.Exists(configFileName))
            {
                var r1 = await _popupService.ShowPopupAsync<DownloadTtsPopupModel>(onPresenting: viewModel => viewModel.StartDownloadPiperVoice(piperVoice), CancellationToken.None);
            }
        }

        var result = await engine.Speak(VoiceTestText, voice);
        
        var audioPlayer = _audioManager.CreatePlayer(result.FileName);
        audioPlayer.Play();
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
