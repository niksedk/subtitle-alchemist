using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.AudioToText;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.ContainerFormats.Matroska;
using Nikse.SubtitleEdit.Core.SubtitleFormats;
using SubtitleAlchemist.Features.Video.AudioToTextWhisper.Download;
using SubtitleAlchemist.Features.Video.AudioToTextWhisper.Engines;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Logic.Constants;
using SubtitleAlchemist.Logic.Media;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using Switch = Microsoft.Maui.Controls.Switch;
using Timer = System.Timers.Timer;

namespace SubtitleAlchemist.Features.Video.AudioToTextWhisper;

public partial class AudioToTextWhisperModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private float _progressValue;

    [ObservableProperty]
    private string _elapsedText = string.Empty;

    [ObservableProperty]
    private string _estimatedText = string.Empty;

    public Label TitleLabel { get; set; } = new();
    public Picker PickerEngine { get; set; } = new();
    public Picker PickerLanguage { get; set; } = new();
    public Picker PickerModel { get; set; } = new();
    public Button ButtonModel { get; set; } = new();
    public Switch SwitchTranslateToEnglish { get; set; } = new();
    public Switch SwitchAdjustTimings { get; set; } = new();
    public Switch SwitchPostProcessing { get; set; } = new();
    public Label LabelProgress { get; set; } = new();
    public Label LabelAdvancedSettings { get; set; } = new();

    public AudioToTextWhisperPage? Page { get; set; }
    public ProgressBar ProgressBar { get; set; } = new();

    public bool RunningOnCuda { get; set; }

    public bool UnknownArgument { get; set; }
    public Button TranscribeButton { get; set; } = new();
    public Label LinkLabelProcessingSettings { get; set; } = new();

    private string? _videoFileName;
    private string _waveFileName = string.Empty;
    private int _audioTrackNumber;
    private readonly List<string> _filesToDelete = new();
    private bool IncompleteModel;
    private readonly ConcurrentBag<string> _outputText = new();
    private long _startTicks = 0;
    private double _endSeconds;
    private double _showProgressPct = -1;
    private double _lastEstimatedMs = double.MaxValue;
    private bool _batchMode;
    private int _batchFileNumber;
    private readonly VideoInfo _videoInfo = new();
    private readonly TaskbarList _taskbarList;
    private IntPtr _windowHandle;

    public bool Loading { get; set; } = true;
    public Editor ConsoleText { get; set; } = new();
    public ScrollView ConsoleTextScrollView { get; set; } = new();

    [ObservableProperty]
    private IWhisperEngine? _selectedWhisperEngine;

    public readonly List<IWhisperEngine> WhisperEngines = new();


    [ObservableProperty]
    private ObservableCollection<WhisperLanguage> _languages = new();

    [ObservableProperty]
    private ObservableCollection<WhisperModelDisplay> _models = new();

    public class WhisperModelDisplay
    {
        public WhisperModel Model { get; set; } = new WhisperModel();
        public string? Display { get; set; }
        public IWhisperEngine Engine { get; set; } = new WhisperEngineCpp();

        public override string ToString()
        {
            if (Display == null)
            {
                RefreshDownloadStatus(); ;
            }

            return Display!;
        }

        private string IsInstalled()
        {
            if (!Engine.IsModelInstalled(Model))
            {
                return ", not installed";
            }

            return string.Empty;
        }

        public void RefreshDownloadStatus()
        {
            Display = Model.Name;

            if (!string.IsNullOrEmpty(Model.Size))
            {
                Display += $" ({Model.Size}{IsInstalled()})";
            }
            else
            {
                Display += $" ({IsInstalled().TrimStart(',').TrimStart()})";
            }
        }
    }

    private bool _abort;

    private readonly IPopupService _popupService;
    private List<ResultText> _resultList = new();
    private bool _useCenterChannelOnly;
    private readonly Regex _timeRegexShort = new Regex(@"^\[\d\d:\d\d[\.,]\d\d\d --> \d\d:\d\d[\.,]\d\d\d\]", RegexOptions.Compiled);
    private readonly Regex _timeRegexLong = new Regex(@"^\[\d\d:\d\d:\d\d[\.,]\d\d\d --> \d\d:\d\d:\d\d[\.,]\d\d\d]", RegexOptions.Compiled);
    private readonly Regex _pctWhisper = new Regex(@"^\d+%\|", RegexOptions.Compiled);
    private readonly Regex _pctWhisperFaster = new Regex(@"^\s*\d+%\s*\|", RegexOptions.Compiled);
    private readonly Timer _timerWhisper = new();
    private Process _whisperProcess = new();

    private Process _waveExtractProcess = new();
    private readonly Timer _timerWaveExtract = new();

    private Stopwatch _sw = new();
    private StringBuilder _ffmpegLog = new StringBuilder();

    public AudioToTextWhisperModel(IPopupService popupService, TaskbarList taskbarList)
    {
        _popupService = popupService;
        _taskbarList = taskbarList;

        _windowHandle = IntPtr.Zero;

        WhisperEngines.Add(new WhisperEngineCpp());
        WhisperEngines.Add(new WhisperEnginePurfviewFasterWhisper());
        WhisperEngines.Add(new WhisperEnginePurfviewFasterWhisperXxl());
        WhisperEngines.Add(new WhisperEngineOpenAi());
        WhisperEngines.Add(new WhisperEngineConstMe());

        _timerWhisper.Interval = 100;
        _timerWhisper.Elapsed += OnTimerWhisperOnElapsed;

        _timerWaveExtract.Interval = 100;
        _timerWaveExtract.Elapsed += OnTimerWaveExtractOnElapsed;
    }

    private void OnTimerWaveExtractOnElapsed(object? sender, ElapsedEventArgs e)
    {
        if (_abort)
        {
            _timerWaveExtract.Stop();
#pragma warning disable CA1416
            _waveExtractProcess.Kill(true);
#pragma warning restore CA1416
            ProgressBar.IsVisible = false;

            TranscribeButton.IsEnabled = true;
            return;
        }

        if (!_waveExtractProcess.HasExited)
        {
            var durationMs = (DateTime.UtcNow.Ticks - _startTicks) / 10_000;
            ElapsedText = $"Time elapsed: {new TimeCode(durationMs).ToShortDisplayString()}";

            return;
        }

        _timerWaveExtract.Stop();

        if (!File.Exists(_waveFileName))
        {
            SeLogger.WhisperInfo("Generated wave file not found: " + _waveFileName + Environment.NewLine +
                                 "ffmpeg: " + _waveExtractProcess.StartInfo.FileName + Environment.NewLine +
                                 "Parameters: " + _waveExtractProcess.StartInfo.Arguments + Environment.NewLine +
                                 "OS: " + Environment.OSVersion + Environment.NewLine +
                                 "64-bit: " + Environment.Is64BitOperatingSystem + Environment.NewLine +
                                 "ffmpeg exit code: " + _waveExtractProcess.ExitCode + Environment.NewLine +
                                 "ffmpeg log: " + _ffmpegLog);
            TranscribeButton.IsEnabled = true;
            return;
        }

        var startOk = TranscribeViaWhisper(_waveFileName, _videoFileName);
    }

    private void OnTimerWhisperOnElapsed(object? sender, ElapsedEventArgs args)
    {
        if (Page == null)
        {
            return;
        }

        if (_abort)
        {
            _timerWhisper.Stop();
#pragma warning disable CA1416
            _whisperProcess.Kill(true);
#pragma warning restore CA1416

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                ProgressBar.IsVisible = false;
                var partialSub = new Subtitle();
                partialSub.Paragraphs.AddRange(_resultList.OrderBy(p => p.Start).Select(p => new Paragraph(p.Text, (double)p.Start * 1000.0, (double)p.End * 1000.0)).ToList());

                if (partialSub.Paragraphs.Count > 0)
                {
                    var answer = await Page.DisplayAlert(
                        $"Keep partial transcription?",
                        $"Do you want to keep {partialSub.Paragraphs.Count} lines?",
                        "Yes",
                        "No");

                    if (!answer)
                    {
                        await Shell.Current.GoToAsync("..");
                        return;
                    }
                }

                await MakeResult(partialSub);
            });

            return;
        }

        if (!_whisperProcess.HasExited)
        {
            var durationMs = (DateTime.UtcNow.Ticks - _startTicks) / 10_000;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LabelProgress.Text = "Transcribing...";
            });

            ElapsedText = $"Time elapsed: {new TimeCode(durationMs).ToShortDisplayString()}";
            if (_endSeconds <= 0)
            {
                if (_showProgressPct > 0)
                {
                    SetProgressBarPct(_showProgressPct);
                }

                return;
            }

            ShowProgressBar();

            _videoInfo.TotalSeconds = Math.Max(_endSeconds, _videoInfo.TotalSeconds);
            var msPerFrame = durationMs / (_endSeconds * 1000.0);
            var estimatedTotalMs = msPerFrame * _videoInfo.TotalMilliseconds;
            var msEstimatedLeft = estimatedTotalMs - durationMs;
            if (msEstimatedLeft > _lastEstimatedMs)
            {
                msEstimatedLeft = _lastEstimatedMs;
            }
            else
            {
                _lastEstimatedMs = msEstimatedLeft;
            }

            if (_showProgressPct > 0)
            {
                SetProgressBarPct(_showProgressPct);
            }
            else
            {
                SetProgressBarPct(_endSeconds * 100.0 / _videoInfo.TotalSeconds);
            }

            EstimatedText = ProgressHelper.ToProgressTime(msEstimatedLeft);

            return;
        }

        _timerWhisper.Stop();

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await ProgressBar.ProgressTo(1, 500, Easing.Linear);

            LogToConsole($"Whisper ({Se.Settings.Tools.WhisperChoice}) done in {_sw.Elapsed}{Environment.NewLine}");

            _whisperProcess.Dispose();

            if (GetResultFromSrt(_waveFileName, _videoFileName!, out var resultTexts, _outputText, _filesToDelete))
            {
                var subtitle = new Subtitle();
                subtitle.Paragraphs.AddRange(resultTexts
                    .Select(p => new Paragraph(p.Text, (double)p.Start * 1000.0, (double)p.End * 1000.0)).ToList());

                var postProcessedSubtitle = PostProcess(subtitle);
                await MakeResult(postProcessedSubtitle);

                return;
            }

            _outputText.Add("Loading result from STDOUT" + Environment.NewLine);

            var transcribedSubtitleFromStdOut = new Subtitle();
            transcribedSubtitleFromStdOut.Paragraphs.AddRange(_resultList.OrderBy(p => p.Start)
                .Select(p => new Paragraph(p.Text, (double)p.Start * 1000.0, (double)p.End * 1000.0)).ToList());
            await MakeResult(transcribedSubtitleFromStdOut);
        });
    }

    private void LogToConsole(string s)
    {
        _outputText.Add(s);
        MainThread.BeginInvokeOnMainThread(() =>
        {
            ConsoleText.Text += s + "\n";
            ConsoleTextScrollView.ScrollToAsync(0, ConsoleText.Height, true);
        });
    }

    private void SetProgressBarPct(double pct)
    {
        var p = pct / 100.0;

        if (p > 1)
        {
            p = 1;
        }

        if (p < 0)
        {
            p = 0;
        }

        MainThread.BeginInvokeOnMainThread(() =>
        {
            ProgressValue = (float)p;
            _taskbarList.SetProgressValue(_windowHandle, Math.Max(0, Math.Min((int)pct, 100)), 100);
        });
    }

    private Subtitle PostProcess(Subtitle transcript)
    {
        if (PickerLanguage.SelectedItem is not WhisperLanguage language)
        {
            return transcript;
        }

        if (SwitchAdjustTimings.IsToggled || SwitchPostProcessing.IsToggled)
        {
            LabelProgress.Text = "Post-processing...";
        }

        var postProcessor = new AudioToTextPostProcessor(SwitchTranslateToEnglish.IsToggled ? "en" : language.Code)
        {
            ParagraphMaxChars = Configuration.Settings.General.SubtitleLineMaximumLength * 2,
        };

        WavePeakData? wavePeaks = null;
        if (SwitchAdjustTimings.IsToggled)
        {
            wavePeaks = MakeWavePeaks();
        }

        if (SwitchAdjustTimings.IsToggled && wavePeaks != null)
        {
            transcript = WhisperTimingFixer.ShortenLongDuration(transcript);
            transcript = WhisperTimingFixer.ShortenViaWavePeaks(transcript, wavePeaks);
        }

        transcript = postProcessor.Fix(
            AudioToTextPostProcessor.Engine.Whisper,
            transcript,
            SwitchPostProcessing.IsToggled,
            Se.Settings.Tools.WhisperPostProcessingAddPeriods,
            Se.Settings.Tools.WhisperPostProcessingMergeLines,
            Se.Settings.Tools.WhisperPostProcessingFixCasing,
            Se.Settings.Tools.WhisperPostProcessingFixShortDuration,
            Se.Settings.Tools.WhisperPostProcessingSplitLines);

        return transcript;
    }

    private WavePeakData? MakeWavePeaks()
    {
        if (string.IsNullOrEmpty(_videoFileName) || !File.Exists(_videoFileName))
        {
            return null;
        }

        var targetFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".wav");
        try
        {
            var process = GetFfmpegProcess(_videoFileName, _audioTrackNumber, targetFile);
            if (process == null)
            {
                return null;
            }

#pragma warning disable CA1416
            process.Start();
#pragma warning restore CA1416

            while (!process.HasExited)
            {
                Task.Delay(100);
            }

            // check for delay in matroska files
            var delayInMilliseconds = 0;
            var audioTrackNames = new List<string>();
            var mkvAudioTrackNumbers = new Dictionary<int, int>();
            if (_videoFileName.ToLowerInvariant().EndsWith(".mkv", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    using (var matroska = new MatroskaFile(_videoFileName))
                    {
                        if (matroska.IsValid)
                        {
                            foreach (var track in matroska.GetTracks())
                            {
                                if (track.IsAudio)
                                {
                                    if (track.CodecId != null && track.Language != null)
                                    {
                                        audioTrackNames.Add("#" + track.TrackNumber + ": " + track.CodecId.Replace("\0", string.Empty) + " - " + track.Language.Replace("\0", string.Empty));
                                    }
                                    else
                                    {
                                        audioTrackNames.Add("#" + track.TrackNumber);
                                    }

                                    mkvAudioTrackNumbers.Add(mkvAudioTrackNumbers.Count, track.TrackNumber);
                                }
                            }

                            if (mkvAudioTrackNumbers.Count > 0)
                            {
                                delayInMilliseconds = (int)matroska.GetAudioTrackDelayMilliseconds(mkvAudioTrackNumbers[0]);
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    SeLogger.Error(exception, $"Error getting delay from mkv: {_videoFileName}");
                }
            }

            if (File.Exists(targetFile))
            {
                using var waveFile = new WavePeakGenerator(targetFile);
                if (!string.IsNullOrEmpty(_videoFileName) && File.Exists(_videoFileName))
                {
                    return waveFile.GeneratePeaks(delayInMilliseconds, WavePeakGenerator.GetPeakWaveFileName(_videoFileName));
                }
            }
        }
        catch
        {
            // ignore
        }

        return null;
    }

    private async Task MakeResult(Subtitle? transcribedSubtitle)
    {
        var sbLog = new StringBuilder();
        foreach (var s in _outputText)
        {
            sbLog.AppendLine(s);
        }

        Se.WriteWhisperLog(sbLog.ToString().Trim());

        var anyLinesTranscribed = transcribedSubtitle != null && transcribedSubtitle.Paragraphs.Count > 0;

        if (anyLinesTranscribed)
        {
            await Shell.Current.GoToAsync("..", new Dictionary<string, object>
                {
                    { "Page", nameof(AudioToTextWhisperPage) },
                    { "TranscribedSubtitle", transcribedSubtitle! },
                });
        }
        else if (_abort)
        {
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            TranscribeButton.IsEnabled = true;

            if (IncompleteModel)
            {
                Page?.DisplayAlert("Incomplete model", "The model is incomplete. Please download the full model.", "OK");
            }
            else if (UnknownArgument && !string.IsNullOrEmpty(Se.Settings.Tools.WhisperCustomCommandLineArguments))
            {
                Page?.DisplayAlert($"Unknown argument: {Se.Settings.Tools.WhisperCustomCommandLineArguments}", "Unknown argument. Please check the advanced settings.", "OK");
            }
            else
            {
                Page?.DisplayAlert("No result", "No result from whisper. Please check the log", "OK");
            }
        }
    }

    public void MouseEnteredPoweredBy()
    {
        TitleLabel.TextColor = (Color)Application.Current!.Resources[ThemeNames.LinkColor];
    }

    public void MouseExitedPoweredBy()
    {
        TitleLabel.TextColor = (Color)Application.Current!.Resources[ThemeNames.TextColor];
    }

    public void MouseClickedPoweredBy(object? sender, TappedEventArgs e)
    {
        if (PickerEngine.SelectedItem is not IWhisperEngine engine)
        {
            return;
        }

        UiUtil.OpenUrl(engine.Url);
    }

    [RelayCommand]
    public async Task ShowAdvancedWhisperSettings()
    {
        if (PickerEngine.SelectedItem is not IWhisperEngine engine)
        {
            return;
        }

        await Shell.Current.GoToAsync(nameof(WhisperAdvancedPage), new Dictionary<string, object>
        {
            { "Page",  nameof(AudioToTextWhisperPage) },
            { "WhisperEngine", engine.Name },
        });

        var isPurfview = engine.Name == WhisperEnginePurfviewFasterWhisper.StaticName ||
                         engine.Name == WhisperEnginePurfviewFasterWhisperXxl.StaticName;
        if (isPurfview)
        {
            if (string.IsNullOrWhiteSpace(Se.Settings.Tools.WhisperCustomCommandLineArguments))
            {
                Se.Settings.Tools.WhisperCustomCommandLineArgumentsPurfviewBlank = true;
            }
        }

        SaveSettings();
    }

    [RelayCommand]
    public async Task Cancel()
    {
        _abort = true;

        if (TranscribeButton.IsEnabled)
        {
            await Shell.Current.GoToAsync("..");
        }
    }

    [RelayCommand]
    public async Task ShowWhisperLog()
    {
        if (Page == null)
        {
            return;
        }

        var whisperLogFile = Se.GetWhisperLogFilePath();

        if (!File.Exists(whisperLogFile))
        {
            await Page.DisplayAlert("Whisper log", "No Whisper log file yet.", "OK");
        }

        UiUtil.OpenFile(Se.GetWhisperLogFilePath());
    }

    [RelayCommand]
    public async Task Transcribe()
    {
        if (Page == null || SelectedWhisperEngine is null || string.IsNullOrEmpty(_videoFileName))
        {
            return;
        }

        if (PickerModel.SelectedItem is not WhisperModelDisplay model)
        {
            return;
        }

        if (PickerLanguage.SelectedItem is not WhisperLanguage language)
        {
            return;
        }

        if (SelectedWhisperEngine is not { } engine)
        {
            return;
        }

        if (!SelectedWhisperEngine.IsEngineInstalled())
        {
            var answer = await Page.DisplayAlert(
                $"Download {SelectedWhisperEngine.Name}?",
                $"Download and use {SelectedWhisperEngine.Name}?",
                "Yes",
                "No");

            if (!answer)
            {
                return;
            }

            var result = await _popupService.ShowPopupAsync<DownloadWhisperPopupModel>(onPresenting: viewModel =>
            {
                viewModel.Engine = engine;
                viewModel.StartDownload();
            }, CancellationToken.None);
        }

        if (!SelectedWhisperEngine.IsModelInstalled(model.Model))
        {
            var answer = await Page.DisplayAlert(
                $"Download {model}?",
                $"Download and use {model.Model.Name}?",
                "Yes",
                "No");

            if (!answer)
            {
                return;
            }

            var result = await _popupService.ShowPopupAsync<DownloadWhisperModelPopupModel>(onPresenting: viewModel =>
            {
                viewModel.SetModels(Models, SelectedWhisperEngine, model);
                viewModel.StartDownload();
            }, CancellationToken.None);

            RefreshDownloadStatus(result as WhisperModel);

            return;
        }

        TranscribeButton.IsEnabled = false;
        ConsoleText.Text = string.Empty;

        var mediaInfo = FfmpegMediaInfo2.Parse(_videoFileName);
        if (mediaInfo.Tracks.Count(p => p.TrackType == FfmpegTrackType.Audio) == 0)
        {
            var answer = await Page.DisplayAlert(
                "No audio track found",
                $"No audio track was found in {_videoFileName}",
                "OK",
                "No");

            TranscribeButton.IsEnabled = true;
            return;
        }


        _videoInfo.TotalMilliseconds = mediaInfo.Duration.TotalMilliseconds;
        _videoInfo.TotalSeconds = mediaInfo.Duration.TotalSeconds;
        _videoInfo.Width = mediaInfo.Dimension.Width;
        _videoInfo.Height = mediaInfo.Dimension.Height;

        LabelProgress.IsVisible = true;
        LabelProgress.Text = "Generating wav file...";
        _startTicks = DateTime.UtcNow.Ticks;

        var startGenerateWaveFileOk = GenerateWavFile(_videoFileName, _audioTrackNumber);

        //TODO: some error handling
    }

    public bool TranscribeViaWhisper(string waveFileName, string videoFileName)
    {
        if (SelectedWhisperEngine is not { } engine)
        {
            return false;
        }

        if (_videoFileName == null)
        {
            return false;
        }

        if (PickerModel.SelectedItem is not WhisperModelDisplay model)
        {
            return false;
        }

        if (PickerLanguage.SelectedItem is not WhisperLanguage language)
        {
            return false;
        }

        Se.Settings.Tools.WhisperChoice = engine.Choice;

        _showProgressPct = -1;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            LabelProgress.Text = "Transcribing..."; // LanguageSettings.Current.AudioToText.Transcribing;
        });

        //if (_batchMode)
        //{

        //    LabelProgress.Text = string.Format("Transcribing {0} of {1}", _batchFileNumber, 0); // TODO: listViewInputFiles.Items.Count);
        //}

        _useCenterChannelOnly = Configuration.Settings.General.FFmpegUseCenterChannelOnly &&
                                FfmpegMediaInfo2.Parse(_videoFileName).HasFrontCenterAudio(_audioTrackNumber);

        //Delete invalid preprocessor_config.json file
        if (Se.Settings.Tools.WhisperChoice is
              WhisperChoice.PurfviewFasterWhisper or
              WhisperChoice.PurfviewFasterWhisperCuda or
              WhisperChoice.PurfviewFasterWhisperXXL)
        {
            var dir = Path.Combine(engine.GetAndCreateWhisperModelFolder(model.Model), model.Model.Folder);
            if (Directory.Exists(dir))
            {
                try
                {
                    var jsonFileName = Path.Combine(dir, "preprocessor_config.json");
                    if (File.Exists(jsonFileName))
                    {
                        var text = FileUtil.ReadAllTextShared(jsonFileName, Encoding.UTF8);
                        if (text.StartsWith("Entry not found", StringComparison.OrdinalIgnoreCase))
                        {
                            File.Delete(jsonFileName);
                        }
                    }

                    jsonFileName = Path.Combine(dir, "vocabulary.json");
                    if (File.Exists(jsonFileName))
                    {
                        var text = FileUtil.ReadAllTextShared(jsonFileName, Encoding.UTF8);
                        if (text.StartsWith("Entry not found", StringComparison.OrdinalIgnoreCase))
                        {
                            File.Delete(jsonFileName);
                        }
                    }
                }
                catch
                {
                    // ignore
                }
            }
        }

        _resultList.Clear();

        var inputFile = waveFileName;
        if (!_useCenterChannelOnly &&
            (engine.Name == WhisperEnginePurfviewFasterWhisper.StaticName ||
             engine.Name == WhisperEnginePurfviewFasterWhisperXxl.StaticName) &&
            (videoFileName.EndsWith(".mkv", StringComparison.OrdinalIgnoreCase) ||
             videoFileName.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase)) &&
            _audioTrackNumber <= 0)
        {
            inputFile = videoFileName;
        }

        _whisperProcess = GetWhisperProcess(engine, inputFile, model.Model.Name, language.Code, SwitchTranslateToEnglish.IsToggled, OutputHandler);
        _sw = Stopwatch.StartNew();
        LogToConsole($"Calling whisper ({Se.Settings.Tools.WhisperChoice}) with : {_whisperProcess.StartInfo.FileName} {_whisperProcess.StartInfo.Arguments}{Environment.NewLine}");

        _abort = false;

        LabelProgress.Text = "Transcribing...";// LanguageSettings.Current.AudioToText.Transcribing;
        _timerWhisper.Start();

        return true;
    }

    public static Process GetWhisperProcess(
        IWhisperEngine engine,
        string waveFileName,
        string model,
        string language,
        bool translate,
        DataReceivedEventHandler? dataReceivedHandler = null)
    {
        Se.Settings.Tools.WhisperCustomCommandLineArguments = Se.Settings.Tools.WhisperCustomCommandLineArguments.Trim();
        if (Se.Settings.Tools.WhisperCustomCommandLineArguments == "--standard" &&
            (engine.Name != WhisperEnginePurfviewFasterWhisper.StaticName && engine.Name != WhisperEnginePurfviewFasterWhisperXxl.StaticName))
        {
            Se.Settings.Tools.WhisperCustomCommandLineArguments = string.Empty;
        }

        var translateToEnglish = translate ? WhisperHelper.GetWhisperTranslateParameter() : string.Empty;
        if (language.ToLowerInvariant() == "english" || language.ToLowerInvariant() == "en")
        {
            language = "en";
            translateToEnglish = string.Empty;
        }

        if (Se.Settings.Tools.WhisperChoice is WhisperChoice.Cpp or WhisperChoice.CppCuBlas)
        {
            if (!Se.Settings.Tools.WhisperCustomCommandLineArguments.Contains("--print-progress"))
            {
                translateToEnglish += "--print-progress ";
            }
        }

        var outputSrt = string.Empty;
        var postParams = string.Empty;
        if (Se.Settings.Tools.WhisperChoice is WhisperChoice.Cpp or WhisperChoice.CppCuBlas or WhisperChoice.ConstMe)
        {
            outputSrt = "--output-srt ";
        }
        else if (Se.Settings.Tools.WhisperChoice == WhisperChoice.StableTs)
        {
            var srtFileName = Path.GetFileNameWithoutExtension(waveFileName);
            postParams = $" -o {srtFileName}.srt";
        }

        var w = engine.GetExecutable();
        var m = engine.GetModelForCmdLine(model);
        var parameters = $"--language {language} --model \"{m}\" {outputSrt}{translateToEnglish}{Se.Settings.Tools.WhisperCustomCommandLineArguments} \"{waveFileName}\"{postParams}";

        SeLogger.WhisperInfo($"{w} {parameters}");

        var process = new Process { StartInfo = new ProcessStartInfo(w, parameters) { WindowStyle = ProcessWindowStyle.Hidden, CreateNoWindow = true } };
        if (!string.IsNullOrEmpty(Se.Settings.FfmpegPath) && process.StartInfo.EnvironmentVariables["Path"] != null)
        {
            process.StartInfo.EnvironmentVariables["Path"] = process.StartInfo.EnvironmentVariables["Path"]?.TrimEnd(';') + ";" + Path.GetDirectoryName(Se.Settings.FfmpegPath);
        }

        var whisperFolder = engine.GetAndCreateWhisperFolder();
        if (!string.IsNullOrEmpty(whisperFolder))
        {
            if (File.Exists(whisperFolder))
            {
                whisperFolder = Path.GetDirectoryName(whisperFolder);
            }

            if (whisperFolder != null)
            {
                process.StartInfo.WorkingDirectory = whisperFolder;
            }
        }

        if (!string.IsNullOrEmpty(whisperFolder) && process.StartInfo.EnvironmentVariables["Path"] != null)
        {
            process.StartInfo.EnvironmentVariables["Path"] = process.StartInfo.EnvironmentVariables["Path"]?.TrimEnd(';') + ";" + whisperFolder;
        }

        if (Se.Settings.Tools.WhisperChoice != WhisperChoice.Cpp &&
            Se.Settings.Tools.WhisperChoice != WhisperChoice.CppCuBlas &&
            Se.Settings.Tools.WhisperChoice != WhisperChoice.ConstMe)
        {
            process.StartInfo.EnvironmentVariables["PYTHONIOENCODING"] = "utf-8";
            process.StartInfo.EnvironmentVariables["PYTHONUTF8"] = "1";
        }

        if (dataReceivedHandler != null)
        {
            process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.OutputDataReceived += dataReceivedHandler;
            process.ErrorDataReceived += dataReceivedHandler;
        }

#pragma warning disable CA1416
        process.Start();
#pragma warning restore CA1416

        if (dataReceivedHandler != null)
        {
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        return process;
    }

    private void ShowProgressBar()
    {
        if (!ProgressBar.IsVisible)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
#if WINDOWS
                _windowHandle = ((MauiWinUIWindow)App.Current.Windows[0].Handler.PlatformView).WindowHandle;
                //_windowHandle = (TitleLabel.Window.Handler.PlatformView as MauiWinUIWindow).WindowHandle;  
                //var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(_windowHandle);
#endif

                ProgressValue = 0;
                ProgressBar.IsVisible = true;
            });
        }
    }

    public bool GetResultFromSrt(string waveFileName, string videoFileName, out List<ResultText> resultTexts, ConcurrentBag<string> outputText, List<string> filesToDelete)
    {
        if (PickerEngine.SelectedItem is not IWhisperEngine engine)
        {
            resultTexts = new List<ResultText>();
            return false;
        }

        var srtFileName = waveFileName + ".srt";
        if (!File.Exists(srtFileName) && waveFileName.EndsWith(".wav"))
        {
            srtFileName = waveFileName.Remove(waveFileName.Length - 4) + ".srt";
        }

        var whisperFolder = engine.GetAndCreateWhisperFolder();
        if (!string.IsNullOrEmpty(whisperFolder) && !File.Exists(srtFileName) && !string.IsNullOrEmpty(videoFileName))
        {
            srtFileName = Path.Combine(whisperFolder, Path.GetFileNameWithoutExtension(videoFileName)) + ".srt";
        }

        if (!File.Exists(srtFileName))
        {
            srtFileName = Path.Combine(whisperFolder, Path.GetFileNameWithoutExtension(waveFileName)) + ".srt";
        }

        var vttFileName = Path.Combine(whisperFolder, Path.GetFileName(waveFileName) + ".vtt");
        if (!File.Exists(vttFileName))
        {
            vttFileName = Path.Combine(whisperFolder, Path.GetFileNameWithoutExtension(waveFileName)) + ".vtt";
        }

        if (!File.Exists(srtFileName) && !File.Exists(vttFileName))
        {
            resultTexts = new List<ResultText>();
            return false;
        }

        var sub = new Subtitle();
        if (File.Exists(srtFileName))
        {
            var rawText = FileUtil.ReadAllLinesShared(srtFileName, Encoding.UTF8);
            new SubRip().LoadSubtitle(sub, rawText, srtFileName);
            outputText?.Add($"Loading result from {srtFileName}{Environment.NewLine}");
        }
        else
        {
            var rawText = FileUtil.ReadAllLinesShared(srtFileName, Encoding.UTF8);
            new WebVTT().LoadSubtitle(sub, rawText, srtFileName);
            outputText?.Add($"Loading result from {vttFileName}{Environment.NewLine}");
        }

        sub.RemoveEmptyLines();

        var results = new List<ResultText>();
        foreach (var p in sub.Paragraphs)
        {
            results.Add(new ResultText
            {
                Start = (decimal)p.StartTime.TotalSeconds,
                End = (decimal)p.EndTime.TotalSeconds,
                Text = p.Text
            });
        }

        resultTexts = results;

        if (File.Exists(srtFileName))
        {
            filesToDelete?.Add(srtFileName);
        }

        if (File.Exists(vttFileName))
        {
            filesToDelete?.Add(vttFileName);
        }

        return true;
    }

    private void OutputHandler(object sendingProcess, System.Diagnostics.DataReceivedEventArgs outLine)
    {
        if (string.IsNullOrWhiteSpace(outLine.Data))
        {
            return;
        }

        if (PickerLanguage.SelectedItem is not WhisperLanguage language)
        {
            return;
        }

        if (outLine.Data.Contains("not all tensors loaded from model file"))
        {
            IncompleteModel = true;
        }

        if (outLine.Data.Contains("error: unknown argument: ", StringComparison.OrdinalIgnoreCase))
        {
            UnknownArgument = true;
        }
        else if (outLine.Data.Contains("error: unrecognized argument: ", StringComparison.OrdinalIgnoreCase))
        {
            UnknownArgument = true;
        }
        else if (outLine.Data.Contains("error: unrecognized arguments: ", StringComparison.OrdinalIgnoreCase))
        {
            UnknownArgument = true;
        }

        if (outLine.Data.Contains("running on: CUDA", StringComparison.OrdinalIgnoreCase))
        {
            RunningOnCuda = true;
        }

        LogToConsole(outLine.Data.Trim() + Environment.NewLine);

        foreach (var line in outLine.Data.SplitToLines())
        {
            if (_timeRegexShort.IsMatch(line))
            {
                var start = line.Substring(1, 10);
                var end = line.Substring(14, 10);
                var text = line.Remove(0, 25).Trim();
                var rt = new ResultText
                {
                    Start = GetSeconds(start),
                    End = GetSeconds(end),
                    Text = Utilities.AutoBreakLine(text, language.Code),
                };

                if (_showProgressPct < 0)
                {
                    _endSeconds = (double)rt.End;
                }

                _resultList.Add(rt);
            }
            else if (_timeRegexLong.IsMatch(line))
            {
                var start = line.Substring(1, 12);
                var end = line.Substring(18, 12);
                var text = line.Remove(0, 31).Trim();
                var rt = new ResultText
                {
                    Start = GetSeconds(start),
                    End = GetSeconds(end),
                    Text = Utilities.AutoBreakLine(text, language.Code),
                };

                if (_showProgressPct < 0)
                {
                    _endSeconds = (double)rt.End;
                }

                _resultList.Add(rt);
            }
            else if (line.StartsWith("whisper_full: progress =", StringComparison.OrdinalIgnoreCase))
            {
                var arr = line.Split('=');
                if (arr.Length == 2)
                {
                    var pctString = arr[1].Trim().TrimEnd('%').TrimEnd();
                    if (double.TryParse(pctString, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var pct))
                    {
                        _endSeconds = _videoInfo.TotalSeconds * pct / 100.0;
                        _showProgressPct = pct;
                    }
                }
            }
            else if (_pctWhisper.IsMatch(line.TrimStart()))
            {
                var arr = line.Split('%');
                if (arr.Length > 1 && double.TryParse(arr[0], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var pct))
                {
                    _endSeconds = _videoInfo.TotalSeconds * pct / 100.0;
                    _showProgressPct = pct;
                }
            }
            else if (_pctWhisperFaster.IsMatch(line))
            {
                var arr = line.Split('%');
                if (arr.Length > 1 && double.TryParse(arr[0].Trim(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var pct))
                {
                    _endSeconds = _videoInfo.TotalSeconds * pct / 100.0;
                    _showProgressPct = pct;
                }
            }
        }
    }

    private static decimal GetSeconds(string timeCode)
    {
        return (decimal)(TimeCode.ParseToMilliseconds(timeCode) / 1000.0);
    }

    private bool GenerateWavFile(string videoFileName, int audioTrackNumber)
    {
        if (videoFileName.EndsWith(".wav"))
        {
            try
            {
                using var waveFile = new WavePeakGenerator(videoFileName);
                if (waveFile.Header != null && waveFile.Header.SampleRate == 16000)
                {
                    _videoFileName = videoFileName;
                    var startOk = TranscribeViaWhisper(_waveFileName, _videoFileName);
                    return startOk;
                }
            }
            catch
            {
                // ignore
            }
        }

        _ffmpegLog = new StringBuilder();
        _waveFileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".wav");
        _filesToDelete.Add(_waveFileName);
        _waveExtractProcess = GetFfmpegProcess(videoFileName, audioTrackNumber, _waveFileName);
        if (_waveExtractProcess == null)
        {
            return false;
        }

        _waveExtractProcess.ErrorDataReceived += (sender, args) =>
        {
            _ffmpegLog.AppendLine(args.Data);
        };

        _waveExtractProcess.StartInfo.RedirectStandardError = true;
#pragma warning disable CA1416
        var started = _waveExtractProcess.Start();
#pragma warning restore CA1416

        _waveExtractProcess.BeginErrorReadLine();
        _abort = false;
        _timerWaveExtract.Start();
        return true;
    }

    private Process? GetFfmpegProcess(string videoFileName, int audioTrackNumber, string outWaveFile)
    {
        if (!File.Exists(Se.Settings.FfmpegPath) && Configuration.IsRunningOnWindows)
        {
            return null;
        }

        var audioParameter = string.Empty;
        if (audioTrackNumber > 0)
        {
            audioParameter = $"-map 0:a:{audioTrackNumber}";
        }

        //TODo:    labelFC.Text = string.Empty;
        var fFmpegWaveTranscodeSettings = "-i \"{0}\" -vn -ar 16000 -ac 1 -ab 32k -af volume=1.75 -f wav {2} \"{1}\"";
        if (_useCenterChannelOnly)
        {
            fFmpegWaveTranscodeSettings = "-i \"{0}\" -vn -ar 16000 -ab 32k -af volume=1.75 -af \"pan=mono|c0=FC\" -f wav {2} \"{1}\"";
            //TODO:  labelFC.Text = "FC";
        }

        //-i indicates the input
        //-vn means no video output
        //-ar 44100 indicates the sampling frequency.
        //-ab indicates the bit rate (in this example 160kb/s)
        //-af volume=1.75 will boot volume... 1.0 is normal
        //-ac 2 means 2 channels
        // "-map 0:a:0" is the first audio stream, "-map 0:a:1" is the second audio stream

        var exeFilePath = Se.Settings.FfmpegPath;
        if (!Configuration.IsRunningOnWindows)
        {
            exeFilePath = "ffmpeg";
        }

        var parameters = string.Format(fFmpegWaveTranscodeSettings, videoFileName, outWaveFile, audioParameter);
        return new Process
        {
            StartInfo = new ProcessStartInfo(exeFilePath, parameters)
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = false,
            }
        };
    }

    private void SaveSettings()
    {
        if (Loading)
        {
            return;
        }

        if (SelectedWhisperEngine is null)
        {
            return;
        }

        Se.Settings.Tools.WhisperChoice = SelectedWhisperEngine.Choice;

        if (PickerLanguage.SelectedItem is WhisperLanguage language)
        {
            Se.Settings.Tools.WhisperLanguageCode = language.Code;
        }

        if (PickerModel.SelectedItem is IWhisperModel model)
        {
            Se.Settings.Tools.WhisperModel = model.ToString();
        }

        Se.Settings.Tools.WhisperAutoAdjustTimings = SwitchAdjustTimings.IsToggled;
        Se.Settings.Tools.VoskPostProcessing = SwitchPostProcessing.IsToggled;
        Se.Settings.Tools.WhisperCustomCommandLineArguments = LabelAdvancedSettings.Text;

        Se.SaveSettings();
    }

    public void PickerEngine_SelectedIndexChanged(object? sender, EventArgs e)
    {
        SaveSettings();

        if (PickerEngine.SelectedItem is not IWhisperEngine engine)
        {
            return;
        }

        SelectedWhisperEngine = engine;
        TitleLabel.Text = engine.Name;

        Languages.Clear();
        foreach (var language in engine.Languages)
        {
            Languages.Add(language);
        }

        Models.Clear();
        foreach (var model in engine.Models)
        {
            Models.Add(new WhisperModelDisplay
            {
                Model = model,
                Engine = engine,
            });
        }

        LoadSettings();

        var isPurfview = engine.Name == WhisperEnginePurfviewFasterWhisper.StaticName ||
                         engine.Name == WhisperEnginePurfviewFasterWhisperXxl.StaticName;
        if (isPurfview &&
            string.IsNullOrWhiteSpace(Se.Settings.Tools.WhisperCustomCommandLineArguments) &&
            !Se.Settings.Tools.WhisperCustomCommandLineArgumentsPurfviewBlank)
        {
            Se.Settings.Tools.WhisperCustomCommandLineArguments = "--standard";
            LabelAdvancedSettings.Text = Se.Settings.Tools.WhisperCustomCommandLineArguments;
        }
    }

    public void LoadSettings()
    {
        var language = Languages.FirstOrDefault(l => l.Code == Se.Settings.Tools.WhisperLanguageCode);
        if (language != null)
        {
            PickerLanguage.SelectedItem = language;
        }
        else
        {
            PickerLanguage.SelectedItem = Languages.FirstOrDefault();
        }

        var model = Models.FirstOrDefault(m => m.ToString() == Se.Settings.Tools.WhisperModel);
        if (model != null)
        {
            PickerModel.SelectedItem = model;
        }
        else
        {
            PickerModel.SelectedItem = Models.FirstOrDefault();
        }

        SwitchAdjustTimings.IsToggled = Se.Settings.Tools.WhisperAutoAdjustTimings;
        SwitchPostProcessing.IsToggled = Se.Settings.Tools.VoskPostProcessing;
        LabelAdvancedSettings.Text = Se.Settings.Tools.WhisperCustomCommandLineArguments;
        ProgressBar.IsVisible = false;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.ContainsKey("Page"))
        {
            throw new ArgumentException("\"Page\" not found in shell query attributes");
        }

        if (query["Page"] is not string page)
        {
            throw new ArgumentException("\"Page\" shell query attribute is not a string");
        }

        if (page == nameof(WhisperAdvancedPage))
        {
            if (query.ContainsKey("Parameters") && query["Parameters"] is string parameters)
            {
                Se.Settings.Tools.WhisperCustomCommandLineArguments = parameters;
                LabelAdvancedSettings.Text = parameters;
                SaveSettings();
            }
        }

        if (query.ContainsKey("VideoFileName") && query["VideoFileName"] is string videoFileName)
        {
            _videoFileName = videoFileName;
        }

        if (query.ContainsKey("AudioTrackNumber") && query["AudioTrackNumber"] is int audioTrackNumber)
        {
            _audioTrackNumber = audioTrackNumber;
        }
    }

    [RelayCommand]
    public async Task DownloadModel()
    {
        if (PickerEngine.SelectedItem is not IWhisperEngine engine)
        {
            return;
        }

        if (PickerModel.SelectedItem is not WhisperModelDisplay model)
        {
            return;
        }

        var result = await _popupService
        .ShowPopupAsync<DownloadWhisperModelPopupModel>(onPresenting: viewModel =>
        {
            viewModel.SetModels(Models, engine, model);
        }, CancellationToken.None);
        RefreshDownloadStatus(result as WhisperModel);

        SaveSettings();
    }

    private void RefreshDownloadStatus(WhisperModel? result)
    {
        if (PickerEngine.SelectedItem is not IWhisperEngine engine)
        {
            return;
        }

        if (PickerModel.SelectedItem is not WhisperModelDisplay oldModel)
        {
            return;
        }

        Models.Clear();
        foreach (var model in engine.Models)
        {
            Models.Add(new WhisperModelDisplay
            {
                Model = model,
                Engine = engine,
            });
        }

        if (result != null)
        {
            PickerModel.SelectedItem = Models.FirstOrDefault(m => m.Model.Name == result.Name);
        }
        else
        {
            PickerModel.SelectedItem = Models.FirstOrDefault(m => m.Model.Name == oldModel.Model.Name);
        }
    }

    public void MouseEnteredPostProcessingSettings(object obj)
    {
        LinkLabelProcessingSettings.TextColor = (Color)Application.Current!.Resources[ThemeNames.LinkColor];
    }

    public void MouseExitedProcessingSettings(object obj)
    {
        LinkLabelProcessingSettings.TextColor = (Color)Application.Current!.Resources[ThemeNames.TextColor];
    }

    public async Task MouseClickedProcessingSettings(object? sender, TappedEventArgs e)
    {
        await _popupService.ShowPopupAsync<WhisperPostProcessingPopupModel>(onPresenting: viewModel => viewModel.LoadSettings(), CancellationToken.None);
    }
}