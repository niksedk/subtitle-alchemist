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
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using SubtitleAlchemist.Logic.Media;
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
    private VideoInfo _videoInfo = new();
    private readonly TaskbarList _taskbarList;
    private readonly IntPtr _windowHandle;

    public bool Loading { get; set; } = true;

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
                RefreshDownloadStatus();;
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
    private readonly Timer _timer = new();
    private Process _whisperProcess = new();
    private Stopwatch _sw = new();

    public AudioToTextWhisperModel(IPopupService popupService, TaskbarList taskbarList)
    {
        _popupService = popupService;
        _taskbarList = taskbarList;

        _windowHandle = IntPtr.Zero;
#if WINDOWS  
		_windowHandle = ((MauiWinUIWindow)App.Current.Windows[0].Handler.PlatformView).WindowHandle;  
#endif         

        WhisperEngines.Add(new WhisperEngineCpp());
        WhisperEngines.Add(new WhisperEnginePurfviewFasterWhisper());
        WhisperEngines.Add(new WhisperEnginePurfviewFasterWhisperXxl());
        WhisperEngines.Add(new WhisperEngineOpenAi());
        WhisperEngines.Add(new WhisperEngineConstMe());

        _timer.Interval = 100;
        _timer.Elapsed += OnTimerOnElapsed;
    }

    private void OnTimerOnElapsed(object? sender, ElapsedEventArgs args)
    {
        if (_abort)
        {
            _timer.Stop();
#pragma warning disable CA1416
            _whisperProcess.Kill(true);
#pragma warning restore CA1416
            ProgressBar.IsVisible = false;

            var partialSub = new Subtitle();
            partialSub.Paragraphs.AddRange(_resultList.OrderBy(p => p.Start).Select(p => new Paragraph(p.Text, (double)p.Start * 1000.0, (double)p.End * 1000.0)).ToList());
            MakeResult(partialSub);
            return;
        }

        if (!_whisperProcess.HasExited)
        {
            var durationMs = (DateTime.UtcNow.Ticks - _startTicks) / 10_000;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LabelProgress.Text = "Transcribing...";
            });

            ElapsedText = $"Elapsed time: {new TimeCode(durationMs).ToShortDisplayString()}";
            if (_endSeconds <= 0 || _videoInfo == null)
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

        _timer.Stop();
        _outputText.Add($"Calling whisper {SeSettings.Settings.Tools.WhisperChoice} done in {_sw.Elapsed}{Environment.NewLine}");

        _whisperProcess.Dispose();

        if (GetResultFromSrt(_waveFileName, _videoFileName!, out var resultTexts, _outputText, _filesToDelete))
        {
            var subtitle = new Subtitle();
            subtitle.Paragraphs.AddRange(resultTexts.Select(p => new Paragraph(p.Text, (double)p.Start * 1000.0, (double)p.End * 1000.0)).ToList());

            MainThread.BeginInvokeOnMainThread(() =>
            {
                var postProcessedSubtitle = PostProcess(subtitle);
                MakeResult(postProcessedSubtitle);
            });

            return;
        }

        _outputText?.Add("Loading result from STDOUT" + Environment.NewLine);

        var transcribedSubtitleFromStdOut = new Subtitle();
        transcribedSubtitleFromStdOut.Paragraphs.AddRange(_resultList.OrderBy(p => p.Start).Select(p => new Paragraph(p.Text, (double)p.Start * 1000.0, (double)p.End * 1000.0)).ToList());
        MakeResult(transcribedSubtitleFromStdOut);
    }

    private void SetProgressBarPct(double pct)
    {
        var p =  pct / 100.0;

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
            _taskbarList.SetProgressValue(_windowHandle, p, 100);
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
            SeSettings.Settings.Tools.WhisperPostProcessingAddPeriods,
            SeSettings.Settings.Tools.WhisperPostProcessingMergeLines,
            SeSettings.Settings.Tools.WhisperPostProcessingFixCasing,
            SeSettings.Settings.Tools.WhisperPostProcessingFixShortDuration,
            SeSettings.Settings.Tools.WhisperPostProcessingSplitLines);

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
                using (var waveFile = new WavePeakGenerator(targetFile))
                {
                    if (!string.IsNullOrEmpty(_videoFileName) && File.Exists(_videoFileName))
                    {
                        return waveFile.GeneratePeaks(delayInMilliseconds, WavePeakGenerator.GetPeakWaveFileName(_videoFileName));
                    }
                }
            }
        }
        catch
        {
            // ignore
        }

        return null;
    }

    private static void MakeResult(Subtitle? transcribedSubtitle)
    {
        var anyLinesTranscribed = transcribedSubtitle != null && transcribedSubtitle.Paragraphs.Count > 0;

        if (anyLinesTranscribed)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.GoToAsync("..", new Dictionary<string, object>
                {
                    { "Page", nameof(AudioToTextWhisperPage) },
                    { "TranscribedSubtitle", transcribedSubtitle! },
                });
            });
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
    }

    [RelayCommand]
    public async Task Cancel()
    {
        _abort = true;
        _timer.Stop();
        await Shell.Current.GoToAsync("..");
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

        var mediaInfo = FfmpegMediaInfo2.Parse(_videoFileName);
        if (mediaInfo.Tracks.Count(p => p.TrackType == FfmpegTrackType.Audio) == 0)
        {
            var answer = await Page.DisplayAlert(
                $"No audio track found",
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
        var genWaveFile = GenerateWavFile(_videoFileName, _audioTrackNumber);
        if (string.IsNullOrEmpty(genWaveFile))
        {
            TranscribeButton.IsEnabled = true;
            return;
        }

        _waveFileName = genWaveFile;

        SaveSettings();

        var startOk = TranscribeViaWhisper(_waveFileName, _videoFileName);

        //TODO: some error message
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

        SeSettings.Settings.Tools.WhisperChoice = engine.Choice;

        _showProgressPct = -1;

        LabelProgress.Text = "Transcribing..."; // LanguageSettings.Current.AudioToText.Transcribing;
        if (_batchMode)
        {
            LabelProgress.Text = string.Format("Transcribing {0} of {1}", _batchFileNumber, 0); // TODO: listViewInputFiles.Items.Count);
        }
        else
        {
            _taskbarList.SetProgressValue(_windowHandle, 1, 100);
        }

        _useCenterChannelOnly = Configuration.Settings.General.FFmpegUseCenterChannelOnly &&
                                FfmpegMediaInfo2.Parse(_videoFileName).HasFrontCenterAudio(_audioTrackNumber);

        //Delete invalid preprocessor_config.json file
        if (SeSettings.Settings.Tools.WhisperChoice is
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
        _outputText.Add($"Calling whisper ({SeSettings.Settings.Tools.WhisperChoice}) with : {_whisperProcess.StartInfo.FileName} {_whisperProcess.StartInfo.Arguments}{Environment.NewLine}");
        _startTicks = DateTime.UtcNow.Ticks;

        _abort = false;

        LabelProgress.Text = "Transcribing...";// LanguageSettings.Current.AudioToText.Transcribing;
        _timer.Start();

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
        SeSettings.Settings.Tools.WhisperExtraSettings ??= string.Empty;

        SeSettings.Settings.Tools.WhisperExtraSettings = SeSettings.Settings.Tools.WhisperExtraSettings.Trim();
        if (SeSettings.Settings.Tools.WhisperExtraSettings == "--standard" &&
            (engine.Name != WhisperEnginePurfviewFasterWhisper.StaticName || engine.Name != WhisperEnginePurfviewFasterWhisperXxl.StaticName))
        {
            SeSettings.Settings.Tools.WhisperExtraSettings = string.Empty;
        }

        var translateToEnglish = translate ? WhisperHelper.GetWhisperTranslateParameter() : string.Empty;
        if (language.ToLowerInvariant() == "english" || language.ToLowerInvariant() == "en")
        {
            language = "en";
            translateToEnglish = string.Empty;
        }

        if (SeSettings.Settings.Tools.WhisperChoice is WhisperChoice.Cpp or WhisperChoice.CppCuBlas)
        {
            if (!SeSettings.Settings.Tools.WhisperExtraSettings.Contains("--print-progress"))
            {
                translateToEnglish += "--print-progress ";
            }
        }

        var outputSrt = string.Empty;
        var postParams = string.Empty;
        if (SeSettings.Settings.Tools.WhisperChoice is WhisperChoice.Cpp or WhisperChoice.CppCuBlas or WhisperChoice.ConstMe)
        {
            outputSrt = "--output-srt ";
        }
        else if (SeSettings.Settings.Tools.WhisperChoice == WhisperChoice.StableTs)
        {
            var srtFileName = Path.GetFileNameWithoutExtension(waveFileName);
            postParams = $" -o {srtFileName}.srt";
        }

        var w = engine.GetExecutable();
        var m = engine.GetModelForCmdLine(model);
        var parameters = $"--language {language} --model \"{m}\" {outputSrt}{translateToEnglish}{SeSettings.Settings.Tools.WhisperExtraSettings} \"{waveFileName}\"{postParams}";

        SeLogger.WhisperInfo($"{w} {parameters}");

        var process = new Process { StartInfo = new ProcessStartInfo(w, parameters) { WindowStyle = ProcessWindowStyle.Hidden, CreateNoWindow = true } };
        if (!string.IsNullOrEmpty(SeSettings.Settings.FfmpegPath) && process.StartInfo.EnvironmentVariables["Path"] != null)
        {
            process.StartInfo.EnvironmentVariables["Path"] = process.StartInfo.EnvironmentVariables["Path"]?.TrimEnd(';') + ";" + Path.GetDirectoryName(SeSettings.Settings.FfmpegPath);
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

        if (SeSettings.Settings.Tools.WhisperChoice != WhisperChoice.Cpp &&
            SeSettings.Settings.Tools.WhisperChoice != WhisperChoice.CppCuBlas &&
            SeSettings.Settings.Tools.WhisperChoice != WhisperChoice.ConstMe)
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
                ProgressValue = 0;
                ProgressBar.IsVisible = true;
            });
        }
    }

    public static bool GetResultFromSrt(string waveFileName, string videoFileName, out List<ResultText> resultTexts, ConcurrentBag<string> outputText, List<string> filesToDelete)
    {
        var srtFileName = waveFileName + ".srt";
        if (!File.Exists(srtFileName) && waveFileName.EndsWith(".wav"))
        {
            srtFileName = waveFileName.Remove(waveFileName.Length - 4) + ".srt";
        }

        var whisperFolder = WhisperHelper.GetWhisperFolder() ?? string.Empty;
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

        if (outLine.Data.Contains("running on: CUDA", StringComparison.OrdinalIgnoreCase))
        {
            RunningOnCuda = true;
        }

        _outputText.Add(outLine.Data.Trim() + Environment.NewLine);

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

    private string? GenerateWavFile(string videoFileName, int audioTrackNumber)
    {
        if (videoFileName.EndsWith(".wav"))
        {
            try
            {
                using var waveFile = new WavePeakGenerator(videoFileName);
                if (waveFile.Header != null && waveFile.Header.SampleRate == 16000)
                {
                    return videoFileName;
                }
            }
            catch
            {
                // ignore
            }
        }

        var ffmpegLog = new StringBuilder();
        var outWaveFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".wav");
        _filesToDelete.Add(outWaveFile);
        var process = GetFfmpegProcess(videoFileName, audioTrackNumber, outWaveFile);
        if (process == null)
        {
            return null;
        }

        process.ErrorDataReceived += (sender, args) =>
        {
            ffmpegLog.AppendLine(args.Data);
        };

        process.StartInfo.RedirectStandardError = true;
#pragma warning disable CA1416
        var started = process.Start();
#pragma warning restore CA1416

        process.BeginErrorReadLine();

        double seconds = 0;
        try
        {
            process.PriorityClass = ProcessPriorityClass.Normal;
        }
        catch
        {
            // ignored
        }

        _abort = false;
        string? targetDriveLetter = null;
        if (Configuration.IsRunningOnWindows)
        {
            var root = Path.GetPathRoot(outWaveFile);
            if (root is { Length: > 1 } && root[1] == ':')
            {
                targetDriveLetter = root.Remove(1);
            }
        }

        while (!process.HasExited)
        {
            Thread.Sleep(100);
            //seconds += 0.1;
            //if (seconds < 60)
            //{
            //    ElapsedText = "Estimated left: Seconds left: " + seconds; // string.Format(LanguageSettings.Current.AddWaveform.ExtractingSeconds, seconds);
            //}
            //else
            //{
            //    ElapsedText = "Minutes left: " + ((int)(seconds / 60)); // string.Format(LanguageSettings.Current.AddWaveform.ExtractingMinutes, (int)(seconds / 60), (int)(seconds % 60));
            //}

            if (_abort)
            {
#pragma warning disable CA1416
                process.Kill(true);
#pragma warning restore CA1416

                ProgressBar.IsVisible = false;
                return null;
            }

            if (targetDriveLetter != null && seconds > 1 && Convert.ToInt32(seconds) % 10 == 0)
            {
                try
                {
                    var drive = new DriveInfo(targetDriveLetter);
                    if (drive.IsReady)
                    {
                        if (drive.AvailableFreeSpace < 50 * 1000000) // 50 mb
                        {
                            //TODO:
                            //labelInfo.ForeColor = Color.Red;
                            //labelInfo.Text = LanguageSettings.Current.AddWaveform.LowDiskSpace;
                        }
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        Thread.Sleep(100);

        if (!File.Exists(outWaveFile))
        {
            SeLogger.WhisperInfo("Generated wave file not found: " + outWaveFile + Environment.NewLine +
                           "ffmpeg: " + process.StartInfo.FileName + Environment.NewLine +
                           "Parameters: " + process.StartInfo.Arguments + Environment.NewLine +
                           "OS: " + Environment.OSVersion + Environment.NewLine +
                           "64-bit: " + Environment.Is64BitOperatingSystem + Environment.NewLine +
                           "ffmpeg exit code: " + process.ExitCode + Environment.NewLine +
                           "ffmpeg log: " + ffmpegLog);
        }

        return outWaveFile;
    }

    private Process? GetFfmpegProcess(string videoFileName, int audioTrackNumber, string outWaveFile)
    {
        if (!File.Exists(SeSettings.Settings.FfmpegPath) && Configuration.IsRunningOnWindows)
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

        var exeFilePath = SeSettings.Settings.FfmpegPath;
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

        SeSettings.Settings.Tools.WhisperChoice = SelectedWhisperEngine.Choice;

        if (PickerLanguage.SelectedItem is WhisperLanguage language)
        {
            SeSettings.Settings.Tools.WhisperLanguageCode = language.Code;
        }

        if (PickerModel.SelectedItem is IWhisperModel model)
        {
            SeSettings.Settings.Tools.WhisperModel = model.ToString();
        }

        SeSettings.Settings.Tools.WhisperAutoAdjustTimings = SwitchAdjustTimings.IsToggled;
        SeSettings.Settings.Tools.VoskPostProcessing = SwitchPostProcessing.IsToggled;
        SeSettings.Settings.Tools.WhisperExtraSettings = LabelAdvancedSettings.Text;

        SeSettings.SaveSettings();
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
    }

    public void LoadSettings()
    {
        var language = Languages.FirstOrDefault(l => l.Code == SeSettings.Settings.Tools.WhisperLanguageCode);
        if (language != null)
        {
            PickerLanguage.SelectedItem = language;
        }
        else
        {
            PickerLanguage.SelectedItem = Languages.FirstOrDefault();
        }

        var model = Models.FirstOrDefault(m => m.ToString() == SeSettings.Settings.Tools.WhisperModel);
        if (model != null)
        {
            PickerModel.SelectedItem = model;
        }
        else
        {
            PickerModel.SelectedItem = Models.FirstOrDefault();
        }

        SwitchAdjustTimings.IsToggled = SeSettings.Settings.Tools.WhisperAutoAdjustTimings;
        SwitchPostProcessing.IsToggled = SeSettings.Settings.Tools.VoskPostProcessing;
        LabelAdvancedSettings.Text = SeSettings.Settings.Tools.WhisperExtraSettings;
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
                SeSettings.Settings.Tools.WhisperExtraSettings = parameters;
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
            PickerModel.SelectedItem = Models.FirstOrDefault(m=>m.Model.Name == oldModel.Model.Name);
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