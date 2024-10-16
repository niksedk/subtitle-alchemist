using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Features.Video.TextToSpeech.Engines;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using SubtitleAlchemist.Features.Video.TextToSpeech.Voices;
using SubtitleAlchemist.Services;
using Plugin.Maui.Audio;
using SubtitleAlchemist.Features.Video.TextToSpeech.DownloadTts;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Logic.Constants;

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
    private ObservableCollection<TtsLanguage> _languages;

    [ObservableProperty]
    private TtsLanguage? _selectedLanguage;

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
    public MediaElement Player { get; set; }
    public Label LabelAudioEncodingSettings { get; set; }

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
            new AllTalk(ttsDownloadService),
            new ElevenLabs(ttsDownloadService),
        };
        _selectedEngine = _engines.FirstOrDefault();

        _voices = new ObservableCollection<Voice>();

        _languages = new ObservableCollection<TtsLanguage>();

        _voiceTestText = "Hello, how are you doing?";

        Player = new MediaElement { IsVisible = false };
        LabelAudioEncodingSettings = new();
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
                LoadSettings();
            });
            return false;
        });
    }

    private void LoadSettings()
    {
        var lastEngine = Engines.FirstOrDefault(e => e.Name == Se.Settings.Video.TextToSpeech.Engine);
        if (lastEngine != null)
        {
            SelectedEngine = lastEngine;
        }

        var lastVoice = Voices.FirstOrDefault(v => v.Name == Se.Settings.Video.TextToSpeech.Voice);
        if (lastVoice == null)
        {
            lastVoice = Voices.FirstOrDefault(p => p.Name.StartsWith("en", StringComparison.OrdinalIgnoreCase) ||
                                                        p.Name.Contains("English", StringComparison.OrdinalIgnoreCase));
            SelectedVoice = lastVoice ?? Voices.FirstOrDefault();
        }
        else
        {
            SelectedVoice = lastVoice;
        }

        VoiceTestText = Se.Settings.Video.TextToSpeech.VoiceTestText;
        DoReviewAudioClips = Se.Settings.Video.TextToSpeech.ReviewAudioClips;
        DoGenerateVideoFile = Se.Settings.Video.TextToSpeech.GenerateVideoFile;
        UseCustomAudioEncoding = Se.Settings.Video.TextToSpeech.CustomAudio;
    }

    private void SaveSettings()
    {
        Se.Settings.Video.TextToSpeech.Engine = SelectedEngine?.Name ?? string.Empty;
        Se.Settings.Video.TextToSpeech.Voice = SelectedVoice?.Name ?? string.Empty;
        Se.Settings.Video.TextToSpeech.VoiceTestText = VoiceTestText;
        Se.Settings.Video.TextToSpeech.ReviewAudioClips = DoReviewAudioClips;
        Se.Settings.Video.TextToSpeech.GenerateVideoFile = DoGenerateVideoFile;
        Se.Settings.Video.TextToSpeech.CustomAudio = UseCustomAudioEncoding;
        Se.SaveSettings();
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

        var generateParagraphAudioStatus = await GenerateParagraphAudio();
        if (!generateParagraphAudioStatus)
        {
            return;
        }

        var fixParagraphAudioSpeedStatus = await FixParagraphAudioSpeed();
        if (!fixParagraphAudioSpeedStatus)
        {
            return;
        }

        var reviewAudioClipsStatus = await ReviewAudioClips();
        if (!reviewAudioClipsStatus)
        {
            return;
        }

        var mergeAudioParagraphsResult = await MergeAudioParagraphs();
    }

    private async Task<bool> GenerateParagraphAudio()
    {
        foreach (var paragraph in _subtitle.Paragraphs)
        {

        }

        return true;
    }

    private async Task<bool> FixParagraphAudioSpeed()
    {
        foreach (var paragraph in _subtitle.Paragraphs)
        {

        }

        return true;
    }

    private async Task<bool> ReviewAudioClips()
    {
        foreach (var paragraph in _subtitle.Paragraphs)
        {

        }

        return true;
    }

    private async Task<bool> MergeAudioParagraphs()
    {
        foreach (var paragraph in _subtitle.Paragraphs)
        {

        }

        return true;
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

        SaveSettings();

        var result = await engine.Speak(VoiceTestText, voice);

        //var audioPlayer = _audioManager.CreatePlayer(result.FileName);
        //audioPlayer.Play();

        if (!File.Exists(result.FileName) && Page != null)
        {
            await Page.DisplayAlert(
                "Test voice error",
                $"Output audio file was not generated: {result.FileName}",
                "OK");
            return;
        }

        Player.Stop();
        Player.Source = null;
        Player.Source = MediaSource.FromFile(result.FileName);
        Player.Play();
    }

    [RelayCommand]
    public async Task Cancel()
    {
        SaveSettings();
        await Shell.Current.GoToAsync("..");
    }

    public void SelectedEngineChanged(object? sender, EventArgs e)
    {
        if (SelectedEngine != null)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                var voices = await SelectedEngine.GetVoices();
                Voices.Clear();
                foreach (var vo in voices)
                {
                    Voices.Add(vo);
                }
                VoiceCount = Voices.Count;

                var lastVoice = Voices.FirstOrDefault(v => v.Name == Se.Settings.Video.TextToSpeech.Voice);
                if (lastVoice == null)
                {
                    lastVoice = Voices.FirstOrDefault(p => p.Name.StartsWith("en", StringComparison.OrdinalIgnoreCase) ||
                                                           p.Name.Contains("English", StringComparison.OrdinalIgnoreCase));
                }
                SelectedVoice = lastVoice ?? Voices.First();

                HasLanguageParameter = SelectedEngine.HasLanguageParameter;
                if (HasLanguageParameter)
                {
                    var languages = await SelectedEngine.GetLanguages(SelectedVoice);
                    Languages.Clear();
                    foreach (var language in languages)
                    {
                        Languages.Add(language);
                    }

                    SelectedLanguage = Languages.FirstOrDefault();
                }
            });
        }
    }

    public void LabelAudioEncodingSettingsMouseEntered(object? sender, PointerEventArgs e)
    {
        LabelAudioEncodingSettings.TextColor = (Color)Application.Current!.Resources[ThemeNames.LinkColor];
    }

    public void LabelAudioEncodingSettingsMouseExited(object? sender, PointerEventArgs e)
    {
        LabelAudioEncodingSettings.TextColor = (Color)Application.Current!.Resources[ThemeNames.TextColor];
    }

    public void LabelAudioEncodingSettingsMouseClicked(object? sender, TappedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var result = await _popupService.ShowPopupAsync<AudioSettingsPopupModel>(CancellationToken.None);
        });
    }
}
