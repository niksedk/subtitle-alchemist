using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Features.Video.TextToSpeech.Engines;
using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Maui.Views;
using SubtitleAlchemist.Features.Video.TextToSpeech.Voices;
using SubtitleAlchemist.Services;
using SubtitleAlchemist.Features.Video.TextToSpeech.DownloadTts;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Logic.Constants;
using SubtitleAlchemist.Logic.Media;

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
    private bool _hasApiKey;

    [ObservableProperty]
    private string _apiKey;

    [ObservableProperty]
    private bool _hasRegion;

    [ObservableProperty]
    private ObservableCollection<string> _regions;

    [ObservableProperty]
    private string? _selectedRegion;

    [ObservableProperty]
    private bool _hasModel;

    [ObservableProperty]
    private ObservableCollection<string> _models;

    [ObservableProperty]
    private string? _selectedModel;

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

    [ObservableProperty]
    private string _doneOrCancelText;

    public TextToSpeechPage? Page { get; set; }
    public MediaElement Player { get; set; }
    public Label LabelAudioEncodingSettings { get; set; }

    private Subtitle _subtitle = new();
    private readonly IPopupService _popupService;
    private readonly IFileHelper _fileHelper;
    private readonly string _waveFolder;
    private CancellationTokenSource _cancellationTokenSource;
    private CancellationToken _cancellationToken;
    private WavePeakData _wavePeakData;
    private FfmpegMediaInfo2? _mediaInfo;
    private string _videoFileName = string.Empty;

    public TextToSpeechPageModel(ITtsDownloadService ttsDownloadService, IPopupService popupService, IFileHelper fileHelper)
    {
        _popupService = popupService;
        _fileHelper = fileHelper;
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
        for (var i = 0; i < int.MaxValue; i++)
        {
            _waveFolder = Path.Combine(Path.GetTempPath(), $"Tts_{i}");
            if (!Directory.Exists(_waveFolder))
            {
                Directory.CreateDirectory(_waveFolder);
                break;
            }
        }

        _cancellationTokenSource = new();
        _cancellationToken = _cancellationTokenSource.Token;

        Player = new MediaElement { IsVisible = false };
        LabelAudioEncodingSettings = new();
        _wavePeakData = new WavePeakData(1, new List<WavePeak>());

        _apiKey = string.Empty;
        _regions = new ObservableCollection<string>();
        _models = new ObservableCollection<string>();

        _doneOrCancelText = "Done";
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        var page = query["Page"].ToString();

        if (page == nameof(ReviewSpeechPage))
        {
            if (query.ContainsKey("StepResult") && query["StepResult"] is TtsStepResult[] stepResult && Page != null)
            {
                Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
                {
                    ProgressText = string.Empty;
                    ProgressValue = 0;
                    IsGenerating = true;
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await MergeAndAddToVideo(stepResult);
                    });

                    return false;
                });
            }

            return;
        }

        if (query["Subtitle"] is Subtitle subtitle)
        {
            _subtitle = new Subtitle(subtitle, false);
        }

        if (query.ContainsKey("WavePeaks") && query["WavePeaks"] is WavePeakData wavePeakData)
        {
            _wavePeakData = wavePeakData;
        }

        if (query.ContainsKey("VideoFileName") && query["VideoFileName"] is string videoFileName && !string.IsNullOrEmpty(videoFileName))
        {
            if (File.Exists(videoFileName))
            {
                _videoFileName = videoFileName;
                Task.Run(() => { _mediaInfo = FfmpegMediaInfo2.Parse(videoFileName); }, _cancellationToken);
            }
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

        if (SelectedEngine is AzureSpeech)
        {
            ApiKey = Se.Settings.Video.TextToSpeech.AzureApiKey;
            SelectedRegion = Se.Settings.Video.TextToSpeech.AzureRegion;
        }
        else if (SelectedEngine is ElevenLabs)
        {
            ApiKey = Se.Settings.Video.TextToSpeech.ElevenLabsApiKey;
            SelectedModel = Se.Settings.Video.TextToSpeech.ElevenLabsModel;
        }
    }

    private void SaveSettings()
    {
        Se.Settings.Video.TextToSpeech.Engine = SelectedEngine?.Name ?? string.Empty;
        Se.Settings.Video.TextToSpeech.Voice = SelectedVoice?.Name ?? string.Empty;
        Se.Settings.Video.TextToSpeech.VoiceTestText = VoiceTestText;
        Se.Settings.Video.TextToSpeech.ReviewAudioClips = DoReviewAudioClips;
        Se.Settings.Video.TextToSpeech.GenerateVideoFile = DoGenerateVideoFile;
        Se.Settings.Video.TextToSpeech.CustomAudio = UseCustomAudioEncoding;

        if (SelectedEngine is AzureSpeech)
        {
            Se.Settings.Video.TextToSpeech.AzureApiKey = ApiKey;
            Se.Settings.Video.TextToSpeech.AzureRegion = SelectedRegion ?? string.Empty;
        }
        else if (SelectedEngine is ElevenLabs)
        {
            Se.Settings.Video.TextToSpeech.ElevenLabsApiKey = ApiKey;
            Se.Settings.Video.TextToSpeech.ElevenLabsModel = SelectedModel ?? string.Empty;
        }

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
        _cancellationToken = _cancellationTokenSource.Token;
        ProgressValue = 0;
        ProgressText = string.Empty;
        IsGenerating = true;
        DoneOrCancelText = "Cancel";
        SaveSettings();

        // Generate
        var generateSpeechResult = await GenerateSpeech(_cancellationToken);
        if (generateSpeechResult == null)
        {
            DoneOrCancelText = "Done";
            IsGenerating = false;
            return;
        }

        // Fix speed
        var fixSpeedResult = await FixSpeed(generateSpeechResult, _cancellationToken);
        if (fixSpeedResult == null)
        {
            DoneOrCancelText = "Done";
            IsGenerating = false;
            return;
        }

        // Review audio clips
        if (DoReviewAudioClips)
        {
            var reviewAudioClipsResult = await ReviewAudioClips(fixSpeedResult);
            if (reviewAudioClipsResult == null)
            {
                return;
            }
        }

        await MergeAndAddToVideo(fixSpeedResult);
    }

    private async Task MergeAndAddToVideo(TtsStepResult[] fixSpeedResult)
    {
        // Merge audio paragraphs
        var mergedAudioFileName = await MergeAudioParagraphs(fixSpeedResult, _cancellationToken);
        if (mergedAudioFileName == null)
        {
            DoneOrCancelText = "Done";
            IsGenerating = false;
            return;
        }

        // Choose folder
        var result = await FolderPicker.Default.PickAsync(_cancellationToken);
        if (!result.IsSuccessful)
        {
            DoneOrCancelText = "Done";
            IsGenerating = false;
            return;
        }
        var outputFolder = result.Folder.Path;
        var audioFileName = Path.Combine(outputFolder, GetBestFileName(".wav"));

        File.Move(mergedAudioFileName, audioFileName);

        // Add audio to video file
        await HandleAddToVideo(audioFileName, outputFolder, _cancellationToken);

        DoneOrCancelText = "Done";
        IsGenerating = false;
    }

    private string GetBestFileName(string extension)
    {
        if (!string.IsNullOrEmpty(_videoFileName))
        {
            return Path.GetFileNameWithoutExtension(_videoFileName) + extension;
        }

        if (!string.IsNullOrEmpty(_subtitle.FileName))
        {
            return Path.GetFileNameWithoutExtension(_subtitle.FileName) + extension;
        }

        return Guid.NewGuid() + extension;
    }

    [RelayCommand]
    public async Task Import()
    {
        if (Page == null)
        {
            return;
        }

        var fileName = await _fileHelper.PickAndShowFile("Open SubtitleEditTts.json file", "*.json");
        if (string.IsNullOrEmpty(fileName))
        {
            return;
        }

        var json = await File.ReadAllTextAsync(fileName, _cancellationToken);
        var importExport = JsonSerializer.Deserialize<TtsImportExport>(json);
        if (importExport == null)
        {
            await Page.DisplayAlert(
                "Text to speech",
                "Nothing to import",
                "OK");

            return;
        }

        var stepResults = new List<TtsStepResult>();
        for (var index = 0; index < importExport.Items.Count; index++)
        {
            var item = importExport.Items[index];
            var paragraph = new Paragraph(item.Text, item.StartMs, item.EndMs) { Number = index + 1 };
            stepResults.Add(new TtsStepResult
            {
                Text = item.Text,
                CurrentFileName = item.AudioFileName,
                Paragraph = paragraph,
                SpeedFactor = 1.0f,
                Voice = Voices.FirstOrDefault(v => v.Name == item.VoiceName),
            });
        }

        await Shell.Current.GoToAsync(nameof(ReviewSpeechPage), new Dictionary<string, object>
        {
            { "Page", nameof(TextToSpeechPage) },
            { "StepResult", stepResults.ToArray() },
            { "Engines", Engines.ToArray() },
            { "Engine", SelectedEngine ?? Engines.First() },
            { "Voices", Voices.ToArray() },
            { "Voice", SelectedVoice ?? Voices.First() },
            { "WavePeaks", _wavePeakData }, // TODO
            { "WaveFolder", _waveFolder },
            { "VideoFileName", importExport.VideoFileName },
        });
    }

    private async Task HandleAddToVideo(string mergedAudioFileName, string outputFolder, CancellationToken cancellationToken)
    {
        if (DoGenerateVideoFile && !string.IsNullOrEmpty(_videoFileName))
        {
            var outputFileName = await AddAudioToVideoFile(mergedAudioFileName, outputFolder, cancellationToken);
            if (!string.IsNullOrEmpty(outputFileName) && Page != null)
            {
                await Page.DisplayAlert(
                    "Text to speech",
                    $"Video file generated: {outputFileName}",
                    "OK");

                UiUtil.OpenFolderFromFileName(outputFileName);
            }
        }
        else
        {
            UiUtil.OpenFolderFromFileName(mergedAudioFileName);
        }
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
        ProgressValue = 0;
        for (var index = 0; index < _subtitle.Paragraphs.Count; index++)
        {
            ProgressText = $"Generating speech: segment {index + 1} of {_subtitle.Paragraphs.Count}";
            var paragraph = _subtitle.Paragraphs[index];
            var speakResult = await engine.Speak(paragraph.Text, _waveFolder, voice, SelectedLanguage, SelectedRegion, SelectedModel, _cancellationToken);
            resultList.Add(new TtsStepResult
            {
                Text = paragraph.Text,
                CurrentFileName = speakResult.FileName,
                Paragraph = paragraph,
                SpeedFactor = 1.0f,
                Voice = voice,
            });
            ProgressValue = (double)(index + 1) / _subtitle.Paragraphs.Count;
        }
        ProgressValue = 1;

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
        ProgressValue = 0;
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
            if (mediaInfo.Duration.TotalMilliseconds <= p.DurationTotalMilliseconds + addDuration)
            {
                resultList.Add(new TtsStepResult
                {
                    Paragraph = p,
                    Text = item.Text,
                    CurrentFileName = outputFileName1,
                    SpeedFactor = 1.0f,
                    Voice = item.Voice,
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
                    Voice = item.Voice,
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
                Voice = item.Voice,
            });

            var mergeProcess = VideoPreviewGenerator.ChangeSpeed(outputFileName1, outputFileName2, (float)factor);
#pragma warning disable CA1416 // Validate platform compatibility
            _ = mergeProcess.Start();
#pragma warning restore CA1416 // Validate platform compatibility
            await mergeProcess.WaitForExitAsync(cancellationToken);

            ProgressValue = (double)(index + 1) / _subtitle.Paragraphs.Count;
        }
        ProgressValue = 1;

        return resultList.ToArray();
    }

    private async Task<TtsStepResult[]?> ReviewAudioClips(TtsStepResult[] previousStepResult)
    {
        if (!DoReviewAudioClips)
        {
            return previousStepResult;
        }

        var engine = SelectedEngine;
        var voice = SelectedVoice;
        if (engine == null || voice == null)
        {
            return previousStepResult;
        }

        await Shell.Current.GoToAsync(nameof(ReviewSpeechPage), new Dictionary<string, object>
        {
            { "Page", nameof(TextToSpeechPage) },
            { "StepResult", previousStepResult },
            { "Engines", Engines.ToArray() },
            { "Engine", engine },
            { "Voices", Voices.ToArray() },
            { "Voice", voice },
            { "WavePeaks", _wavePeakData },
            { "WaveFolder", _waveFolder },
            { "VideoFileName", _videoFileName },
        });

        return null;
    }

    private async Task<string?> MergeAudioParagraphs(TtsStepResult[] previousStepResult, CancellationToken cancellationToken)
    {
        var engine = SelectedEngine;
        var voice = SelectedVoice;
        if (engine == null || voice == null)
        {
            return null;
        }

        var silenceFileName = await GenerateSilenceWaveFile(cancellationToken);

        var outputFileName = string.Empty;
        var inputFileName = silenceFileName;
        ProgressValue = 0;
        for (var index = 0; index < previousStepResult.Length; index++)
        {
            ProgressText = $"Merging audio: segment {index + 1} of {_subtitle.Paragraphs.Count}";

            var item = previousStepResult[index];
            outputFileName = Path.Combine(_waveFolder, $"silence{index}.wav");
            var mergeProcess = VideoPreviewGenerator.MergeAudioTracks(inputFileName, item.CurrentFileName, outputFileName, (float)item.Paragraph.StartTime.TotalSeconds);
            var fileNameToDelete = inputFileName;
            inputFileName = outputFileName;
#pragma warning disable CA1416 // Validate platform compatibility
            _ = mergeProcess.Start();
#pragma warning restore CA1416 // Validate platform compatibility
            await mergeProcess.WaitForExitAsync(cancellationToken);

            ProgressValue = (double)(index + 1) / previousStepResult.Length;

            DeleteFileNoError(fileNameToDelete);
        }
        ProgressValue = 1;

        return outputFileName;
    }

    private async Task<string?> AddAudioToVideoFile(string audioFileName, string outputFolder, CancellationToken cancellationToken)
    {
        var videoExt = ".mkv";
        if (_videoFileName.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase))
        {
            videoExt = ".mp4";
        }

        ProgressText = "Adding audio to video file...";
        var outputFileName = Path.Combine(outputFolder, Path.GetFileNameWithoutExtension(audioFileName) + videoExt);

        var audioEncoding = Se.Settings.Video.TextToSpeech.CustomAudioEncoding;
        if (string.IsNullOrWhiteSpace(audioEncoding) || !UseCustomAudioEncoding)
        {
            audioEncoding = string.Empty;
        }

        bool? stereo = null;
        if (Se.Settings.Video.TextToSpeech.CustomAudioStereo && UseCustomAudioEncoding)
        {
            stereo = true;
        }

        var addAudioProcess = VideoPreviewGenerator.AddAudioTrack(_videoFileName, audioFileName, outputFileName, audioEncoding, stereo);
#pragma warning disable CA1416 // Validate platform compatibility
        var _ = addAudioProcess.Start();
#pragma warning restore CA1416 // Validate platform compatibility
        await addAudioProcess.WaitForExitAsync(cancellationToken);

        ProgressText = string.Empty;
        return outputFileName;
    }

    private async Task<string> GenerateSilenceWaveFile(CancellationToken cancellationToken)
    {
        ProgressText = "Preparing merge...";
        ProgressValue = 0;
        var silenceFileName = Path.Combine(_waveFolder, "silence.wav");
        var silenceIdx = 0;
        while (File.Exists(silenceFileName))
        {
            silenceIdx++;
            silenceFileName = Path.Combine(_waveFolder, $"silence_{silenceIdx}.wav");
        }

        var durationInSeconds = 10f;
        if (_mediaInfo != null)
        {
            durationInSeconds = (float)_mediaInfo.Duration.TotalSeconds;
        }
        else if (_subtitle.Paragraphs.Count > 0)
        {
            durationInSeconds = (float)_subtitle.Paragraphs.Max(p => p.EndTime.TotalSeconds);
        }

        var silenceProcess = VideoPreviewGenerator.GenerateEmptyAudio(silenceFileName, durationInSeconds);
#pragma warning disable CA1416 // Validate platform compatibility
        _ = silenceProcess.Start();
#pragma warning restore CA1416 // Validate platform compatibility
        await silenceProcess.WaitForExitAsync(cancellationToken);
        return silenceFileName;
    }

    private async Task<bool> IsEngineInstalled(ITtsEngine engine)
    {
        if (await engine.IsInstalled(SelectedRegion) || Page == null)
        {
            return true;
        }

        if (engine is Piper)
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

            await _popupService.ShowPopupAsync<DownloadTtsPopupModel>(onPresenting: viewModel => viewModel.StartDownloadPiper(), CancellationToken.None);
            return await engine.IsInstalled(SelectedRegion);
        }

        if (engine is AllTalk)
        {
            var answer = await Page.DisplayAlert(
                "AllTalk not started?",
                $"\"AllTalk\" text to speech requires a running local AllTalk web server.{Environment.NewLine}{Environment.NewLine}Read more?",
                "Yes",
                "No");

            if (!answer)
            {
                return false;
            }

            await UiUtil.OpenUrlAsync("https://github.com/erew123/alltalk_tts");
            return await engine.IsInstalled(SelectedRegion);
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
                await _popupService.ShowPopupAsync<DownloadTtsPopupModel>(onPresenting: viewModel => viewModel.StartDownloadPiperVoice(piperVoice), CancellationToken.None);
            }

            var configFileName = Path.Combine(Piper.GetSetPiperFolder(), piperVoice.ConfigShort);
            if (!File.Exists(configFileName))
            {
                await _popupService.ShowPopupAsync<DownloadTtsPopupModel>(onPresenting: viewModel => viewModel.StartDownloadPiperVoice(piperVoice), CancellationToken.None);
            }
        }

        SaveSettings();

        var result = await engine.Speak(VoiceTestText, _waveFolder, voice, SelectedLanguage, SelectedRegion, SelectedModel, _cancellationToken);
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
    public async Task DoneOrCancel()
    {
        if (IsGenerating)
        {
            _cancellationTokenSource.Cancel(false);
            return;
        }

        SaveSettings();
        Cleanup();
        await Shell.Current.GoToAsync("..");
    }

    public void SelectedEngineChanged(object? sender, EventArgs e)
    {
        var engine = SelectedEngine;
        if (engine == null)
        {
            return;
        }

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var voices = await engine.GetVoices();
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

            HasLanguageParameter = engine.HasLanguageParameter;
            HasApiKey = engine.HasApiKey;
            HasRegion = engine.HasRegion;
            HasModel = engine.HasModel;

            if (HasLanguageParameter)
            {
                var languages = await engine.GetLanguages(SelectedVoice, SelectedModel);
                Languages.Clear();
                foreach (var language in languages)
                {
                    Languages.Add(language);
                }

                SelectedLanguage = Languages.FirstOrDefault();
            }

            if (HasRegion)
            {
                var regions = await engine.GetRegions();
                Regions.Clear();
                foreach (var region in regions)
                {
                    Regions.Add(region);
                }

                SelectedRegion = Regions.FirstOrDefault();
            }

            if (HasModel)
            {
                var models = await engine.GetModels();
                Models.Clear();
                foreach (var model in models)
                {
                    Models.Add(model);
                }

                SelectedModel = Models.FirstOrDefault();
            }

            if (SelectedEngine is AzureSpeech)
            {
                ApiKey = Se.Settings.Video.TextToSpeech.AzureApiKey;
                SelectedRegion = Se.Settings.Video.TextToSpeech.AzureRegion;
                if (string.IsNullOrEmpty(SelectedRegion))
                {
                    SelectedRegion = "westeurope";
                }
            }
            else if (SelectedEngine is ElevenLabs)
            {
                ApiKey = Se.Settings.Video.TextToSpeech.ElevenLabsApiKey;
                SelectedModel = Se.Settings.Video.TextToSpeech.ElevenLabsModel;
                if (string.IsNullOrEmpty(SelectedModel))
                {
                    SelectedModel = Models.First();
                }
            }
        });
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
            await _popupService.ShowPopupAsync<AudioSettingsPopupModel>(CancellationToken.None);
        });
    }

    public void Cleanup()
    {
        Task.Run(() =>
        {
            var files = Directory.GetFiles(_waveFolder, "*.*");
            foreach (var file in files)
            {
                DeleteFileNoError(file);
            }
        }, _cancellationToken);
    }

    private static void DeleteFileNoError(string file)
    {
        try
        {
            File.Delete(file);
        }
        catch
        {
            // ignored
        }
    }

    public void SelectedModelChanged(object? sender, EventArgs e)
    {
        var engine = SelectedEngine;
        var voice = SelectedVoice;
        var model = SelectedModel;
        if (engine == null || voice == null || model == null)
        {
            return;
        }

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (engine.HasLanguageParameter)
            {
                var languages = await engine.GetLanguages(voice, model);
                Languages.Clear();
                foreach (var language in languages)
                {
                    Languages.Add(language);
                }

                SelectedLanguage = Languages.FirstOrDefault(p => p.Name == Se.Settings.Video.TextToSpeech.ElevenLabsLanguage);
                if (SelectedLanguage == null)
                {
                    SelectedLanguage = Languages.FirstOrDefault(p => p.Code == "en");
                }
            }
        });
    }
}
