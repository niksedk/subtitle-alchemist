using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubtitleAlchemist.Logic.Constants;
using Nikse.SubtitleEdit.Core.AudioToText;
using SubtitleAlchemist.Logic;
using Nikse.SubtitleEdit.Core.Common;
using CommunityToolkit.Maui.Core;
using SubtitleAlchemist.Features.Video.AudioToTextWhisper.Download;
using SubtitleAlchemist.Features.Video.AudioToTextWhisper.Engines;
using System.Text;
using Nikse.SubtitleEdit.Core.SubtitleFormats;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using Switch = Microsoft.Maui.Controls.Switch;

namespace SubtitleAlchemist.Features.Video.AudioToTextWhisper;

public partial class AudioToTextWhisperModel : ObservableObject, IQueryAttributable
{
    public float ProgressValue { get; set; }
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

    private string? _videoFileName;
    private int _audioTrackNumber;
    private readonly List<string> _filesToDelete = new();
    private bool IncompleteModel = false;
    private readonly ConcurrentBag<string> _outputText = new();
    private long _startTicks;
    private double _endSeconds;
    private double _showProgressPct = -1;
    private double _lastEstimatedMs = double.MaxValue;
    private bool _batchMode;
    private int _batchFileNumber;
    private VideoInfo _videoInfo = new();

    [ObservableProperty]
    private IWhisperEngine? _selectedWhisperEngine;

    public readonly List<IWhisperEngine> WhisperEngines = new();


    [ObservableProperty]
    private ObservableCollection<WhisperLanguage> _languages = new();

    [ObservableProperty]
    private ObservableCollection<IWhisperModel> _models = new();

    private bool _abort;

    private readonly IPopupService _popupService;
    private List<ResultText> _resultList = new();
    private bool _useCenterChannelOnly;
    private readonly Regex _timeRegexShort = new Regex(@"^\[\d\d:\d\d[\.,]\d\d\d --> \d\d:\d\d[\.,]\d\d\d\]", RegexOptions.Compiled);
    private readonly Regex _timeRegexLong = new Regex(@"^\[\d\d:\d\d:\d\d[\.,]\d\d\d --> \d\d:\d\d:\d\d[\.,]\d\d\d]", RegexOptions.Compiled);
    private readonly Regex _pctWhisper = new Regex(@"^\d+%\|", RegexOptions.Compiled);
    private readonly Regex _pctWhisperFaster = new Regex(@"^\s*\d+%\s*\|", RegexOptions.Compiled);


    public AudioToTextWhisperModel(IPopupService popupService)
    {
        _popupService = popupService;
        WhisperEngines.Add(new WhisperEngineCpp());
        WhisperEngines.Add(new WhisperEnginePurfviewFasterWhisper());
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
        var result = await _popupService.ShowPopupAsync<WhisperAdvancedPopupModel>();

        if (result is string settings)
        {
            Configuration.Settings.Tools.WhisperExtraSettings = settings;
            LabelAdvancedSettings.Text = settings;
        }
    }

    [RelayCommand]
    public async Task Cancel()
    {
        _abort = true;
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    public async Task Transcribe()
    {
        if (Page == null || SelectedWhisperEngine is null || string.IsNullOrEmpty(_videoFileName))
        {
            return;
        }

        if (PickerModel.SelectedItem is not WhisperModel model)
        {
            return;
        }

        if (PickerLanguage.SelectedItem is not WhisperLanguage language)
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

            var result = await _popupService.ShowPopupAsync<DownloadWhisperPopupModel>(CancellationToken.None);
        }

        if (!SelectedWhisperEngine.IsModelInstalled(model))
        {
            var answer = await Page.DisplayAlert(
                $"Download {model}?",
                $"Download and use {model.Name}?",
                "Yes",
                "No");

            if (!answer)
            {
                return;
            }

            SelectedWhisperEngine.DownloadModel(model.ModelFolder);
        }

        var mediaInfo = FfmpegMediaInfo.Parse(_videoFileName);
        if (mediaInfo.Tracks.Count(p => p.TrackType == FfmpegTrackType.Audio) == 0)
        {
            var answer = await Page.DisplayAlert(
                $"No audio track found",
                $"No audio track was found in {_videoFileName}",
                "OK",
                "No");

            return;
        }

        var waveFileName = GenerateWavFile(_videoFileName, _audioTrackNumber);
        if (string.IsNullOrEmpty(waveFileName))
        {
            return;
        }

        SaveSettings();

        var transcribedSubtitle = TranscribeViaWhisper(waveFileName, _videoFileName);

        var anyLinesTranscribed = transcribedSubtitle != null && transcribedSubtitle.Paragraphs.Count > 0;

        if (anyLinesTranscribed)
        {
            await Shell.Current.GoToAsync("..", new Dictionary<string, object>
            {
                { "Page", nameof(AudioToTextWhisperPage) },
                { "TranscribedSubtitle", transcribedSubtitle! },
            });
        }
    }

    public Subtitle? TranscribeViaWhisper(string waveFileName, string videoFileName)
    {
        if (SelectedWhisperEngine is null)
        {
            return null;
        }

        var engine = SelectedWhisperEngine as IWhisperEngine;

        if (PickerModel.SelectedItem is not WhisperModel model)
        {
            return null;
        }

        if (PickerLanguage.SelectedItem is not WhisperLanguage language)
        {
            return null;
        }

        _showProgressPct = -1;

        LabelProgress.Text = "Transcribing..."; // LanguageSettings.Current.AudioToText.Transcribing;
        if (_batchMode)
        {
            LabelProgress.Text = string.Format("Transcribing {0} of {1}", _batchFileNumber, 0); // TODO: listViewInputFiles.Items.Count);
        }
        else
        {
            //TODO: TaskbarList.SetProgressValue(_parentForm.Handle, 1, 100);
        }

        //Delete invalid preprocessor_config.json file
        if (Configuration.Settings.Tools.WhisperChoice is
              WhisperChoice.PurfviewFasterWhisper or
              WhisperChoice.PurfviewFasterWhisperCuda or
              WhisperChoice.PurfviewFasterWhisperXXL)
        {
            var dir = Path.Combine(WhisperHelper.GetWhisperFolder(), "_models", model.Folder);
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
            (engine.Name == WhisperEngineCpp.StaticName ||
             engine.Name == WhisperEnginePurfviewFasterWhisper.StaticName) &&
            (videoFileName.EndsWith(".mkv", StringComparison.OrdinalIgnoreCase) ||
             videoFileName.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase)) &&
            _audioTrackNumber <= 0)
        {
            inputFile = videoFileName;
        }

        var process = GetWhisperProcess(inputFile, model.Name, language.Code, SwitchTranslateToEnglish.IsToggled, OutputHandler);
        var sw = Stopwatch.StartNew();
        _outputText.Add($"Calling whisper ({Configuration.Settings.Tools.WhisperChoice}) with : {process.StartInfo.FileName} {process.StartInfo.Arguments}{Environment.NewLine}");
        _startTicks = DateTime.UtcNow.Ticks;
        _videoInfo = UiUtil.GetVideoInfo(waveFileName);
        //  timer1.Start();
        if (!_batchMode)
        {
            ShowProgressBar();
            //ProgressBar.Style = ProgressBarStyle.Marquee;
        }

        _abort = false;

        LabelProgress.Text = "Transcribing...";// LanguageSettings.Current.AudioToText.Transcribing;
        while (!process.HasExited)
        {
            Thread.Sleep(100);
            //TODO: WindowsHelper.PreventStandBy();

            if (_abort)
            {
                process.Kill(true);
                ProgressBar.IsVisible = false;
                //buttonCancel.Visible = false;
                //DialogResult = DialogResult.Cancel;

                var partialSub = new Subtitle();
                partialSub.Paragraphs.AddRange(_resultList.OrderBy(p => p.Start).Select(p => new Paragraph(p.Text, (double)p.Start * 1000.0, (double)p.End * 1000.0)).ToList());
                if (partialSub.Paragraphs.Count > 0)
                {
                    return partialSub;
                }

                return null;
            }
        }

        _outputText.Add($"Calling whisper {Configuration.Settings.Tools.WhisperChoice} done in {sw.Elapsed}{Environment.NewLine}");

        for (var i = 0; i < 10; i++)
        {
            Thread.Sleep(50);
        }

        process.Dispose();

        if (GetResultFromSrt(waveFileName, videoFileName, out var resultTexts, _outputText, _filesToDelete))
        {
            var subtitle = new Subtitle();
            subtitle.Paragraphs.AddRange(resultTexts.Select(p => new Paragraph(p.Text, (double)p.Start * 1000.0, (double)p.End * 1000.0)).ToList());
            return subtitle;
        }

        _outputText?.Add("Loading result from STDOUT" + Environment.NewLine);

        var sub = new Subtitle();
        sub.Paragraphs.AddRange(_resultList.OrderBy(p => p.Start).Select(p => new Paragraph(p.Text, (double)p.Start * 1000.0, (double)p.End * 1000.0)).ToList());
        return sub;
    }

    public static Process GetWhisperProcess(string waveFileName, string model, string language, bool translate, DataReceivedEventHandler? dataReceivedHandler = null)
    {
        var translateToEnglish = translate ? WhisperHelper.GetWhisperTranslateParameter() : string.Empty;
        if (language.ToLowerInvariant() == "english" || language.ToLowerInvariant() == "en")
        {
            language = "en";
            translateToEnglish = string.Empty;
        }

        if (Configuration.Settings.Tools.WhisperChoice == WhisperChoice.Cpp || Configuration.Settings.Tools.WhisperChoice == WhisperChoice.CppCuBlas)
        {
            if (!Configuration.Settings.Tools.WhisperExtraSettings.Contains("--print-progress"))
            {
                translateToEnglish += "--print-progress ";
            }
        }

        var outputSrt = string.Empty;
        var postParams = string.Empty;
        if (Configuration.Settings.Tools.WhisperChoice == WhisperChoice.Cpp ||
            Configuration.Settings.Tools.WhisperChoice == WhisperChoice.CppCuBlas ||
            Configuration.Settings.Tools.WhisperChoice == WhisperChoice.ConstMe)
        {
            outputSrt = "--output-srt ";
        }
        else if (Configuration.Settings.Tools.WhisperChoice == WhisperChoice.StableTs)
        {
            var srtFileName = Path.GetFileNameWithoutExtension(waveFileName);
            postParams = $" -o {srtFileName}.srt";
        }

        var w = WhisperHelper.GetWhisperPathAndFileName();
        var m = WhisperHelper.GetWhisperModelForCmdLine(model);
        var parameters = $"--language {language} --model \"{m}\" {outputSrt}{translateToEnglish}{Configuration.Settings.Tools.WhisperExtraSettings} \"{waveFileName}\"{postParams}";

        SeLogger.WhisperInfo($"{w} {parameters}");

        var process = new Process { StartInfo = new ProcessStartInfo(w, parameters) { WindowStyle = ProcessWindowStyle.Hidden, CreateNoWindow = true } };
        if (!string.IsNullOrEmpty(Configuration.Settings.General.FFmpegLocation) && process.StartInfo.EnvironmentVariables["Path"] != null)
        {
            process.StartInfo.EnvironmentVariables["Path"] = process.StartInfo.EnvironmentVariables["Path"].TrimEnd(';') + ";" + Path.GetDirectoryName(Configuration.Settings.General.FFmpegLocation);
        }

        var whisperFolder = WhisperHelper.GetWhisperFolder();
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
            process.StartInfo.EnvironmentVariables["Path"] = process.StartInfo.EnvironmentVariables["Path"].TrimEnd(';') + ";" + whisperFolder;
        }

        if (Configuration.Settings.Tools.WhisperChoice != WhisperChoice.Cpp &&
            Configuration.Settings.Tools.WhisperChoice != WhisperChoice.CppCuBlas &&
             Configuration.Settings.Tools.WhisperChoice != WhisperChoice.ConstMe)
        {
            process.StartInfo.EnvironmentVariables["PYTHONIOENCODING"] = "utf-8";
            process.StartInfo.EnvironmentVariables["PYTHONUTF8"] = "1";
            //process.StartInfo.EnvironmentVariables["PYTHONLEGACYWINDOWSSTDIO"] = "utf-8";
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

        process.Start();

        if (dataReceivedHandler != null)
        {
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        return process;
    }

    private void ShowProgressBar()
    {
        ProgressValue = 0;
        ProgressBar.IsVisible = true;
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
        var started = process.Start();
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
            System.Threading.Thread.Sleep(100);
            seconds += 0.1;
            if (seconds < 60)
            {
                LabelProgress.Text = "Seconds left: " + seconds; // string.Format(LanguageSettings.Current.AddWaveform.ExtractingSeconds, seconds);
            }
            else
            {
                LabelProgress.Text = "Minutes left: " + ((int)(seconds / 60)); // string.Format(LanguageSettings.Current.AddWaveform.ExtractingMinutes, (int)(seconds / 60), (int)(seconds % 60));
            }

            if (_abort)
            {
                process.Kill();
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
        if (!File.Exists(Configuration.Settings.General.FFmpegLocation) && Configuration.IsRunningOnWindows)
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

        var exeFilePath = Configuration.Settings.General.FFmpegLocation;
        if (!Configuration.IsRunningOnWindows)
        {
            exeFilePath = "ffmpeg";
        }

        var parameters = string.Format(fFmpegWaveTranscodeSettings, videoFileName, outWaveFile, audioParameter);
        return new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo(exeFilePath, parameters)
            {
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = false,
            }
        };
    }

    private void SaveSettings()
    {
        if (SelectedWhisperEngine is null)
        {
            return;
        }

        Configuration.Settings.Tools.WhisperChoice = SelectedWhisperEngine.Name;

        if (PickerLanguage.SelectedItem is WhisperLanguage language)
        {
            Configuration.Settings.Tools.WhisperLanguageCode = language.Code;
        }

        if (PickerModel.SelectedItem is IWhisperModel model)
        {
            Configuration.Settings.Tools.WhisperModel = model.ToString();
        }

        Configuration.Settings.Save();
    }

    public void PickerEngine_SelectedIndexChanged(object? sender, EventArgs e)
    {
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
            Models.Add(model);
        }
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query["VideoFileName"] is string videoFileName)
        {
            _videoFileName = videoFileName;
        }
    }

    [RelayCommand]
    public async Task DownloadModel()
    {
        var result = await _popupService.ShowPopupAsync<DownloadWhisperModelPopupModel>(onPresenting: viewModel => viewModel.SetModels(Models), CancellationToken.None);
    }
}