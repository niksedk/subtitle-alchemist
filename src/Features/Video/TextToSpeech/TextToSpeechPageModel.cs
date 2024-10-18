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
using SubtitleAlchemist.Logic.Media;
using System;

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

    [ObservableProperty]
    private bool _isGenerating;

    [ObservableProperty]
    private string _progressText;

    [ObservableProperty]
    private double _progressValue;

    public TextToSpeechPage? Page { get; set; }
    public MediaElement Player { get; set; }
    public Label LabelAudioEncodingSettings { get; set; }

    private Subtitle _subtitle = new();
    private readonly IAudioManager _audioManager;
    private readonly IPopupService _popupService;
    private readonly string _waveFolder;
    private CancellationTokenSource _cancellationTokenSource = new();

    public TextToSpeechPageModel(ITtsDownloadService ttsDownloadService, IAudioManager audioManager, IPopupService popupService)
    {
        _audioManager = audioManager;
        _popupService = popupService;
        _engines = new ObservableCollection<ITtsEngine>
        {
            new Piper(ttsDownloadService),
            new AllTalk(ttsDownloadService),
            new ElevenLabs(ttsDownloadService),
            new AzureSpeech(ttsDownloadService),
        };
        _selectedEngine = _engines.FirstOrDefault();

        _voices = new ObservableCollection<Voice>();

        _languages = new ObservableCollection<TtsLanguage>();

        _voiceTestText = "Hello, how are you doing?";

        _progressText = string.Empty;

        _waveFolder = string.Empty;
        for (var i=0; i < int.MaxValue; i++)
        {
            _waveFolder = Path.Combine(Path.GetTempPath(), $"Tts_{i}");
            if (!Directory.Exists(_waveFolder))
            {
                Directory.CreateDirectory(_waveFolder);
                break;
            }
        }

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
            MainThread.BeginInvokeOnMainThread(LoadSettings);
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

        _cancellationTokenSource = new();
        var cancellationToken = _cancellationTokenSource.Token;
        ProgressValue = 0;
        ProgressText = string.Empty;
        IsGenerating = true;

        var generateSpeechResult = await GenerateSpeech(cancellationToken);
        if (generateSpeechResult == null)
        {
            IsGenerating = false;
            return;
        }

        var fixSpeedResult = await FixSpeed(generateSpeechResult, cancellationToken);
        if (fixSpeedResult == null)
        {
            IsGenerating = false;
            return;
        }

        var reviewAudioClipsResult = await ReviewAudioClips(fixSpeedResult, cancellationToken);
        if (reviewAudioClipsResult == null)
        {
            IsGenerating = false;
            return;
        }

        var mergeAudioParagraphsResult = await MergeAudioParagraphs(reviewAudioClipsResult, cancellationToken);
    }

    private async Task<TtsStepResult[]?> GenerateSpeech(CancellationToken cancellationToken)
    {
        var engine = SelectedEngine;
        var voice = SelectedVoice;
        if (engine == null || voice == null) 
        {
            return null;
        }

        var resultList = new List<TtsStepResult>();
        for (var index = 0; index < _subtitle.Paragraphs.Count; index++)
        {
            ProgressText = $"Generating speech: segment {index + 1} of {_subtitle.Paragraphs.Count}";
            ProgressValue = (double)index / _subtitle.Paragraphs.Count;
            var paragraph = _subtitle.Paragraphs[index];
            var speakResult = await engine.Speak(paragraph.Text, voice);
            resultList.Add(new TtsStepResult()
            {
                Text = paragraph.Text,
                CurrentFileName = speakResult.FileName,
                Paragraph = paragraph,
            });
        }

        return resultList.ToArray();
    }

    private async Task<TtsStepResult[]?> FixSpeed(TtsStepResult[] previousStepResult, CancellationToken cancellationToken)
    {
        var engine = SelectedEngine;
        var voice = SelectedVoice;
        if (engine == null || voice == null)
        {
            return null;
        }

        var resultList = new List<TtsStepResult>();
        for (var index = 0; index < previousStepResult.Length; index++)
        {
            ProgressText = $"Adjusting speed: segment {index + 1} of {_subtitle.Paragraphs.Count}";
            ProgressValue = (double)index / _subtitle.Paragraphs.Count;

            var item = previousStepResult[index];
            var p = item.Paragraph;
            var next = index + 1 < previousStepResult.Length ? previousStepResult[index + 1] : null;
            var outputFileName1 = Path.Combine(Path.GetDirectoryName(item.CurrentFileName), Guid.NewGuid() + ".wav");
            var trimProcess = VideoPreviewGenerator.TrimSilenceStartAndEnd(item.CurrentFileName, outputFileName1);
#pragma warning disable CA1416 // Validate platform compatibility
            _ = trimProcess.Start();
#pragma warning restore CA1416 // Validate platform compatibility
            await trimProcess.WaitForExitAsync(cancellationToken);

            var addDuration = 0d;
            if (next != null && p.EndTime.TotalMilliseconds < next.Paragraph.StartTime.TotalMilliseconds)
            {
                var diff = next.Paragraph.StartTime.TotalMilliseconds - p.EndTime.TotalMilliseconds;
                addDuration = Math.Min(1000, diff);
                if (addDuration < 0)
                {
                    addDuration = 0;
                }
            }

            var mediaInfo = FfmpegMediaInfo2.Parse(outputFileName1);
            if (mediaInfo.Duration.TotalMilliseconds  <= p.DurationTotalMilliseconds + addDuration)
            {
                resultList.Add(new TtsStepResult
                {
                    Paragraph = p,
                    Text = item.Text,
                    CurrentFileName = outputFileName1,
                    SpeedFactor = 1.0f,
                });
                continue;
            }

            var divisor = (decimal)(p.DurationTotalMilliseconds + addDuration);
            if (divisor <= 0)
            {
                resultList.Add(new TtsStepResult
                {
                    Paragraph = p,
                    Text = item.Text,
                    CurrentFileName = item.CurrentFileName,
                    SpeedFactor = 1.0f,
                });

                SeLogger.Error($"TextToSpeech: Duration is zero (skipping): {item.CurrentFileName}, {p}");
                continue;
            }

            var ext = ".wav";
            var factor = (decimal)mediaInfo.Duration.TotalMilliseconds / divisor;
            var outputFileName2 = Path.Combine(_waveFolder, $"{index}_{Guid.NewGuid()}{ext}");
            var overrideFileName = string.Empty;
            if (!string.IsNullOrEmpty(overrideFileName) && File.Exists(Path.Combine(_waveFolder, overrideFileName)))
            {
                outputFileName2 = Path.Combine(_waveFolder, $"{Path.GetFileNameWithoutExtension(overrideFileName)}_{Guid.NewGuid()}{ext}");
            }

            resultList.Add(new TtsStepResult
            {
                Paragraph = p,
                Text = item.Text,
                CurrentFileName = outputFileName2,
                SpeedFactor = (float)factor,
            });

            var mergeProcess = VideoPreviewGenerator.ChangeSpeed(outputFileName1, outputFileName2, (float)factor);
#pragma warning disable CA1416 // Validate platform compatibility
            _ = mergeProcess.Start();
#pragma warning restore CA1416 // Validate platform compatibility
            await mergeProcess.WaitForExitAsync(cancellationToken);
        }

        return resultList.ToArray();
    }

    private async Task<TtsStepResult[]?> ReviewAudioClips(TtsStepResult[] previousStepResult, CancellationToken cancellationToken)
    {
        if (!DoReviewAudioClips)
        {
            return previousStepResult;
        }

        var engine = SelectedEngine;
        var voice = SelectedVoice;
        if (engine == null || voice == null)
        {
            return null;
        }

        await Shell.Current.GoToAsync(nameof(ReviewSpeechPage), new Dictionary<string, object>
        {
            { "Page", nameof(TextToSpeechPage) },
            { "StepResult", previousStepResult },
            { "Engine", engine },
            { "Voice", voice },
        });

        return previousStepResult;

        var resultList = new List<TtsStepResult>();
        return resultList.ToArray();
    }

    private async Task<TtsStepResult[]?> MergeAudioParagraphs(TtsStepResult[] prevoiusStepResult,
        CancellationToken cancellationToken)
    {
        var engine = SelectedEngine;
        var voice = SelectedVoice;
        if (engine == null || voice == null)
        {
            return null;
        }


        var resultList = new List<TtsStepResult>();
        for (var index = 0; index < prevoiusStepResult.Length; index++)
        {
            ProgressText = $"Merging audio: segment {index + 1} of {_subtitle.Paragraphs.Count}";
            ProgressValue = (double)index / _subtitle.Paragraphs.Count;

            var item = prevoiusStepResult[index];
        }

        return resultList.ToArray();
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
