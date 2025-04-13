using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.SubtitleFormats;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SubtitleAlchemist.Controls.ColorPickerControl;
using SubtitleAlchemist.Features.Video.BurnIn;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Logic.Constants;
using SubtitleAlchemist.Logic.Media;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using SubtitleAlchemist.Features.Shared.PickSubtitleLine;
using SubtitleAlchemist.Features.Shared.PickVideoPosition;
using Timer = System.Timers.Timer;

namespace SubtitleAlchemist.Features.Video.TransparentSubtitles;

public partial class TransparentSubPageModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    public partial string VideoFileName { get; set; }

    [ObservableProperty]
    public partial string VideoFileSize { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<string> FontNames { get; set; }

    [ObservableProperty]
    public partial string SelectedFontName { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<double> FontFactors { get; set; }

    [ObservableProperty]
    public partial double SelectedFontFactor { get; set; }

    [ObservableProperty]
    public partial string FontSizeText { get; set; }

    [ObservableProperty]
    public partial bool FontIsBold { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<decimal> FontOutlines { get; set; }

    [ObservableProperty]
    public partial decimal SelectedFontOutline { get; set; }

    [ObservableProperty]
    public partial string FontOutlineText { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<decimal> FontShadowWidths { get; set; }

    [ObservableProperty]
    public partial decimal SelectedFontShadowWidth { get; set; }

    [ObservableProperty]
    public partial string FontShadowText { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<FontBoxItem> FontBoxTypes { get; set; }

    [ObservableProperty]
    public partial FontBoxItem SelectedFontBoxType { get; set; }

    [ObservableProperty]
    public partial Color FontTextColor { get; set; }

    [ObservableProperty]
    public partial Color FontBoxColor { get; set; }

    [ObservableProperty]
    public partial Color FontOutlineColor { get; set; }

    [ObservableProperty]
    public partial Color FontShadowColor { get; set; }

    [ObservableProperty]
    public partial int FontMarginHorizontal { get; set; }

    [ObservableProperty]
    public partial int FontMarginVertical { get; set; }

    [ObservableProperty]
    public partial bool FontFixRtl { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<AlignmentItem> FontAlignments { get; set; }

    [ObservableProperty]
    public partial AlignmentItem SelectedFontAlignment { get; set; }

    [ObservableProperty]
    public partial string FontAssaInfo { get; set; }

    [ObservableProperty]
    public partial int VideoWidth { get; set; }

    [ObservableProperty]
    public partial int VideoHeight { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<double> FrameRates { get; set; }

    [ObservableProperty]
    public partial double SelectedFrameRate { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<string> VideoExtension { get; set; }

    [ObservableProperty]
    public partial int SelectedVideoExtension { get; set; }

    [ObservableProperty]
    public partial string OutputSourceFolder { get; set; }

    [ObservableProperty]
    public partial bool UseOutputFolderVisible { get; set; }

    [ObservableProperty]
    public partial bool UseSourceFolderVisible { get; set; }

    [ObservableProperty]
    public partial bool IsCutActive { get; set; }

    [ObservableProperty]
    public partial TimeSpan CutFrom { get; set; }

    [ObservableProperty]
    public partial TimeSpan CutTo { get; set; }

    [ObservableProperty]
    public partial bool UseTargetFileSize { get; set; }

    [ObservableProperty]
    public partial int TargetFileSize { get; set; }

    [ObservableProperty]
    public partial string ButtonModeText { get; set; }

    [ObservableProperty]
    public partial string ProgressText { get; set; }

    [ObservableProperty]
    public partial double ProgressValue { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<BurnInJobItem> JobItems { get; set; }

    [ObservableProperty]
    public partial BurnInJobItem? SelectedJobItem { get; set; }

    [ObservableProperty]
    public partial bool IsSubtitleLoaded { get; set; }
    public TransparentSubPage? Page { get; set; }
    public MediaElement VideoPlayer { get; set; }
    public Label LabelHelp { get; set; }
    public Button ButtonGenerate { get; set; }
    public Button ButtonOk { get; set; }
    public Button ButtonMode { get; internal set; }
    public ProgressBar ProgressBar { get; set; }
    public Image ImagePreview { get; set; }
    public Border BatchView { get; set; }
    public Label LabelOutputFolder { get; set; }
    public Button ButtonResolution { get; set; }
    public Entry EntryHeight { get; set; }
    public Entry EntryWidth { get; set; }
    public Label LabelX { get; set; }
    public Label LabelVideoFileName { get; set; }
    public Label LabelVideoFileSize { get; set; }
    public Border FontAssaView { get; set; }
    public Border FontPropertiesView { get; set; }

    private Subtitle _subtitle = new();
    private bool _loading = true;
    private readonly StringBuilder _log;
    private static readonly Regex FrameFinderRegex = new(@"[Ff]rame=\s*\d+", RegexOptions.Compiled);
    private long _startTicks;
    private long _processedFrames;
    private Process? _ffmpegProcess;
    private readonly Timer _timerAnalyze;
    private readonly Timer _timerGenerate;
    private bool _doAbort;
    private bool _isBatchMode;
    private int _jobItemIndex = -1;
    private FfmpegMediaInfo2? _mediaInfo;
    private bool _useSourceResolution;
    private SubtitleFormat? _subtitleFormat;

    private readonly IPopupService _popupService;
    private readonly IFileHelper _fileHelper;

    public TransparentSubPageModel(IPopupService popupService, IFileHelper fileHelper)
    {
        _popupService = popupService;
        _fileHelper = fileHelper;
        FontBoxColor = Colors.Wheat;
        FontOutlineColor = Colors.Black;
        FontShadowColor = Colors.Black;
        VideoExtension = new();
        OutputSourceFolder = string.Empty;
        ProgressText = string.Empty;
        VideoPlayer = new();
        LabelHelp = new();
        ButtonGenerate = new();
        ButtonOk = new();
        ButtonMode = new();
        ButtonResolution = new();
        ProgressValue = 0;
        ProgressBar = new();
        ImagePreview = new();
        BatchView = new();
        LabelOutputFolder = new();
        VideoFileName = string.Empty;
        VideoFileSize = string.Empty;
        EntryWidth = new();
        EntryHeight = new();
        LabelX = new();
        LabelVideoFileName = new();
        LabelVideoFileSize = new();
        FontAssaView = new();
        FontPropertiesView = new();
        FrameRates = new ObservableCollection<double>()
        {
            23.976,
            24,
            25,
            29.97,
            30,
            48,
            59.94,
            60,
            120,
        };
        SelectedFrameRate = 24;
        FontNames = new ObservableCollection<string>(FontHelper.GetSystemFonts());
        SelectedFontName = FontNames.First();
        FontFactors = new ObservableCollection<double>(
            Enumerable.Range(200, 1000)
            .Select(i => Math.Round(i * 0.0005, 3))
            .ToList().Distinct());
        SelectedFontFactor = 0.4;
        FontSizeText = string.Empty;
        FontTextColor = Colors.WhiteSmoke;
        FontOutlineText = "Outline";
        FontOutlines = new ObservableCollection<decimal>(Enumerable.Range(0, 50).Select(p => (decimal)p));
        SelectedFontOutline = 2.0m;
        FontShadowText = "Shadow";
        FontShadowWidths = new ObservableCollection<decimal>(Enumerable.Range(0, 50).Select(p => (decimal)p));
        SelectedFontShadowWidth = 2.0m;
        FontBoxTypes = new ObservableCollection<FontBoxItem>
        {
            new(FontBoxType.None, "None"),
            new(FontBoxType.OneBox, "One box"),
            new(FontBoxType.BoxPerLine, "Box per line"),
        };
        SelectedFontBoxType = FontBoxTypes[0];
        FontMarginHorizontal = 10;
        FontMarginVertical = 10;
        FontAlignments = new ObservableCollection<AlignmentItem>(AlignmentItem.Alignments);
        SelectedFontAlignment = AlignmentItem.Alignments[7];
        FontAssaInfo = string.Empty;
        VideoWidth = 1920;
        VideoHeight = 1080;
        JobItems = new ObservableCollection<BurnInJobItem>();
        ButtonModeText = "Batch mode";

        _log = new StringBuilder();

        _timerGenerate = new();
        _timerGenerate.Elapsed += TimerGenerateElapsed;
        _timerGenerate.Interval = 100;

        _timerAnalyze = new();
        _timerAnalyze.Elapsed += TimerAnalyzeElapsed;
        _timerAnalyze.Interval = 100;
    }

    private void TimerAnalyzeElapsed(object? sender, ElapsedEventArgs e)
    {
        if (_ffmpegProcess == null)
        {
            return;
        }

        if (_doAbort)
        {
            _timerAnalyze.Stop();
#pragma warning disable CA1416
            _ffmpegProcess.Kill(true);
#pragma warning restore CA1416
            return;
        }

        if (!_ffmpegProcess.HasExited)
        {
            var percentage = (int)Math.Round((double)_processedFrames / JobItems[_jobItemIndex].TotalFrames * 100.0, MidpointRounding.AwayFromZero);
            percentage = Math.Clamp(percentage, 0, 100);

            var durationMs = (DateTime.UtcNow.Ticks - _startTicks) / 10_000;
            var msPerFrame = (float)durationMs / _processedFrames;
            var estimatedTotalMs = msPerFrame * JobItems[_jobItemIndex].TotalFrames;
            var estimatedLeft = ProgressHelper.ToProgressTime(estimatedTotalMs - durationMs);

            if (JobItems.Count == 1)
            {
                ProgressText = $"Analyzing video... {percentage}%     {estimatedLeft}";
            }
            else
            {
                ProgressText = $"Analyzing video {_jobItemIndex + 1}/{JobItems.Count}... {percentage}%     {estimatedLeft}";
            }

            return;
        }

        _timerAnalyze.Stop();

        var jobItem = JobItems[_jobItemIndex];
        _ffmpegProcess = GetFfmpegProcess(jobItem);
#pragma warning disable CA1416 // Validate platform compatibility
        _ffmpegProcess.Start();
#pragma warning restore CA1416 // Validate platform compatibility
        _ffmpegProcess.BeginOutputReadLine();
        _ffmpegProcess.BeginErrorReadLine();

        _timerGenerate.Start();
    }

    private void TimerGenerateElapsed(object? sender, ElapsedEventArgs e)
    {
        if (_ffmpegProcess == null)
        {
            return;
        }

        if (_doAbort)
        {
            _timerGenerate.Stop();
#pragma warning disable CA1416
            _ffmpegProcess.Kill(true);
#pragma warning restore CA1416
            return;
        }

        if (!_ffmpegProcess.HasExited)
        {
            var percentage = (int)Math.Round((double)_processedFrames / JobItems[_jobItemIndex].TotalFrames * 100.0, MidpointRounding.AwayFromZero);
            var durationMs = (DateTime.UtcNow.Ticks - _startTicks) / 10_000;
            var msPerFrame = (float)durationMs / _processedFrames;
            var estimatedTotalMs = msPerFrame * JobItems[_jobItemIndex].TotalFrames;
            var estimatedLeft = ProgressHelper.ToProgressTime(estimatedTotalMs - durationMs);

            if (JobItems.Count == 1)
            {
                ProgressText = $"Generating video... {percentage}%     {estimatedLeft}";
            }
            else
            {
                ProgressText = $"Generating video {_jobItemIndex + 1}/{JobItems.Count}... {percentage}%     {estimatedLeft}";
            }

            return;
        }

        _timerGenerate.Stop();
        ProgressText = string.Empty;

        var jobItem = JobItems[_jobItemIndex];

        if (!File.Exists(jobItem.OutputVideoFileName))
        {

            SeLogger.WhisperInfo("Output video file not found: " + jobItem.OutputVideoFileName + Environment.NewLine +
                                 "ffmpeg: " + _ffmpegProcess.StartInfo.FileName + Environment.NewLine +
                                 "Parameters: " + _ffmpegProcess.StartInfo.Arguments + Environment.NewLine +
                                 "OS: " + Environment.OSVersion + Environment.NewLine +
                                 "64-bit: " + Environment.Is64BitOperatingSystem + Environment.NewLine +
                                 "ffmpeg exit code: " + _ffmpegProcess.ExitCode + Environment.NewLine +
                                 "ffmpeg log: " + _log);

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Page!.DisplayAlert(
                    "Unable to generate video",
                    "Output video file not generated: " + jobItem.OutputVideoFileName + Environment.NewLine +
                    "Parameters: " + _ffmpegProcess.StartInfo.Arguments,
                    "OK");

                ButtonGenerate.IsEnabled = true;
                ButtonOk.IsEnabled = true;
                ButtonMode.IsEnabled = true;
                ProgressBar.IsVisible = false;
                ProgressValue = 0;
            });

            return;
        }

        JobItems[_jobItemIndex].Status = "Done";

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            ProgressValue = 0;

            if (_jobItemIndex < JobItems.Count - 1)
            {
                InitAndStartJobItem(_jobItemIndex + 1);
                return;
            }

            ButtonGenerate.IsEnabled = true;
            ButtonOk.IsEnabled = true;
            ButtonMode.IsEnabled = true;
            ProgressBar.IsVisible = false;

            if (JobItems.Count == 1)
            {
                UiUtil.OpenFolderFromFileName(jobItem.OutputVideoFileName);
            }
            else
            {
                await Page!.DisplayAlert(
                    "Generating done",
                    "Number of files generated: " + JobItems.Count,
                    "OK");
            }
        });
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query["Subtitle"] is Subtitle subtitle)
        {
            _subtitle = new Subtitle(subtitle, false);
            IsSubtitleLoaded = _subtitle.Paragraphs.Count > 0;
        }

        if (query["VideoFileName"] is string videoFileName)
        {
            LabelVideoFileName.IsVisible = true;
            LabelVideoFileSize.IsVisible = true;
            VideoFileName = videoFileName;
        }
        else
        {
            LabelVideoFileName.IsVisible = false;
            LabelVideoFileSize.IsVisible = false;
        }

        if (query["SubtitleFormat"] is SubtitleFormat subtitleFormat)
        {
            _subtitleFormat = subtitleFormat;
        }

        Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LoadSettings();

                SetUseSourceResolution();

                BatchView.IsVisible = false;
                bool batchMode;
                if (string.IsNullOrWhiteSpace(VideoFileName))
                {
                    batchMode = true;
                    ButtonMode.IsVisible = false;
                }
                else
                {
                    batchMode = false;
                    _mediaInfo = FfmpegMediaInfo2.Parse(VideoFileName);
                    VideoWidth = _mediaInfo.Dimension.Width;
                    VideoHeight = _mediaInfo.Dimension.Height;
                    var bytes = new FileInfo(VideoFileName).Length;
                    VideoFileSize = Utilities.FormatBytesToDisplayFileSize(bytes);

                    if (_subtitleFormat is { Name: AdvancedSubStationAlpha.NameOfFormat })
                    {
                        FontAssaView.IsVisible = true;
                        FontPropertiesView.IsVisible = false;
                    }
                }

                if (batchMode != _isBatchMode)
                {
                    ModeSwitch();
                }

                _loading = false;
                UpdateNonAssaPreview();
            });
            return false;
        });
    }

    private void SetUseSourceResolution()
    {
        if (_useSourceResolution)
        {
            EntryWidth.IsVisible = false;
            EntryHeight.IsVisible = false;
            LabelX.Text = "Use source resolution";
        }
        else
        {
            EntryWidth.IsVisible = true;
            EntryHeight.IsVisible = true;
            LabelX.Text = "x";
        }
    }

    private void LoadSettings()
    {
        var settings = Se.Settings.Video.BurnIn;
        SelectedFontFactor = settings.FontFactor;
        FontIsBold = settings.FontBold;
        SelectedFontOutline = settings.OutlineWidth;
        SelectedFontShadowWidth = settings.ShadowWidth;
        SelectedFontName = settings.FontName;
        FontTextColor = Color.FromArgb(settings.NonAssaTextColor);
        FontOutlineColor = Color.FromArgb(settings.NonAssaOutlineColor);
        FontBoxColor = Color.FromArgb(settings.NonAssaBoxColor);
        FontShadowColor = Color.FromArgb(settings.NonAssaShadowColor);
        FontFixRtl = settings.NonAssaFixRtlUnicode;
        SelectedFontAlignment = FontAlignments.First(p => p.Code == settings.NonAssaAlignment);
        OutputSourceFolder = settings.OutputFolder;
        UseOutputFolderVisible = settings.UseOutputFolder;
        UseSourceFolderVisible = !settings.UseOutputFolder;
        _useSourceResolution = settings.UseSourceResolution;
        SelectedFrameRate = Se.Settings.Video.Transparent.FrameRate;
    }

    private void SaveSettings()
    {
        var settings = Se.Settings.Video.BurnIn;
        settings.FontFactor = SelectedFontFactor;
        settings.FontBold = FontIsBold;
        settings.OutlineWidth = SelectedFontOutline;
        settings.ShadowWidth = SelectedFontShadowWidth;
        settings.FontName = SelectedFontName;
        settings.NonAssaTextColor = FontTextColor.ToArgbHex();
        settings.NonAssaOutlineColor = FontOutlineColor.ToArgbHex();
        settings.NonAssaBoxColor = FontBoxColor.ToArgbHex();
        settings.NonAssaShadowColor = FontShadowColor.ToArgbHex();
        settings.NonAssaFixRtlUnicode = FontFixRtl;
        settings.NonAssaAlignment = SelectedFontAlignment.Code;
        settings.OutputFolder = OutputSourceFolder;
        settings.UseOutputFolder = UseOutputFolderVisible;
        settings.UseSourceResolution = _useSourceResolution;

        Se.Settings.Video.Transparent.FrameRate = SelectedFrameRate;

        Se.SaveSettings();
    }

    [RelayCommand]
    private async Task Generate()
    {
        if (IsCutActive && CutFrom >= CutTo)
        {
            await Page!.DisplayAlert(
                "Cut settings error",
                "Cut end time must be after cut start time",
                "OK");

            return;
        }

        if (!_isBatchMode)
        {
            JobItems = GetCurrentVideoAsJobItems();
        }

        if (JobItems.Count == 0)
        {
            return;
        }

        // check that all jobs have subtitles
        foreach (var jobItem in JobItems)
        {
            if (string.IsNullOrWhiteSpace(jobItem.SubtitleFileName))
            {
                await Page!.DisplayAlert(
                    "Missing subtitle",
                    "Please add a subtitle to all batch items",
                    "OK");

                return;
            }
        }

        _doAbort = false;
        _log.Clear();
        _processedFrames = 0;
        ButtonGenerate.IsEnabled = false;
        ButtonOk.IsEnabled = false;
        ButtonMode.IsEnabled = false;
        ProgressValue = 0;
        ProgressBar.IsVisible = true;
        SaveSettings();

        InitAndStartJobItem(0);
    }

    private void InitAndStartJobItem(int index)
    {
        _startTicks = DateTime.UtcNow.Ticks;
        _jobItemIndex = index;
        var jobItem = JobItems[index];
        jobItem.OutputVideoFileName = MakeOutputFileName(jobItem.SubtitleFileName);
        if (!string.IsNullOrEmpty(jobItem.InputVideoFileName))
        {
            var mediaInfo = FfmpegMediaInfo2.Parse(jobItem.InputVideoFileName);
            jobItem.TotalSeconds = mediaInfo.Duration.TotalSeconds;
            jobItem.Width = mediaInfo.Dimension.Width;
            jobItem.Height = mediaInfo.Dimension.Height;
        }
        jobItem.UseTargetFileSize = UseTargetFileSize;
        jobItem.TargetFileSize = UseTargetFileSize ? TargetFileSize : 0;
        jobItem.AssaSubtitleFileName = MakeAssa(jobItem.SubtitleFileName);
        jobItem.Status = "Generating...";
        jobItem.TotalFrames = GetTotalFrames(jobItem.AssaSubtitleFileName);

        var result = RunEncoding(jobItem);
        if (result)
        {
            _timerGenerate.Start();
        }
    }

    private long GetTotalFrames(string subtitleFileName)
    {
        var subtitle = Subtitle.Parse(subtitleFileName);
        if (subtitle.Paragraphs.Count == 0)
        {
            return 0;
        }

        var totalSeconds = subtitle.Paragraphs.Max(p => p.EndTime.TotalSeconds);
        var frames = (long)Math.Round(SelectedFrameRate * totalSeconds, MidpointRounding.AwayFromZero);
        return frames;
    }

    private bool RunEncoding(BurnInJobItem jobItem)
    {
        _ffmpegProcess = GetFfmpegProcess(jobItem);
#pragma warning disable CA1416 // Validate platform compatibility
        _ffmpegProcess.Start();
#pragma warning restore CA1416 // Validate platform compatibility
        _ffmpegProcess.BeginOutputReadLine();
        _ffmpegProcess.BeginErrorReadLine();

        return true;
    }

    private Process GetFfmpegProcess(BurnInJobItem jobItem)
    {
        var subtitle = Subtitle.Parse(jobItem.AssaSubtitleFileName);
        var totalMs = subtitle.Paragraphs.Max(p => p.EndTime.TotalMilliseconds);
        var ts = TimeSpan.FromMilliseconds(totalMs + 2000);
        var timeCode = string.Format($"{ts.Hours:00}\\\\:{ts.Minutes:00}\\\\:{ts.Seconds:00}");

        return VideoPreviewGenerator.GenerateTransparentVideoFile(
            jobItem.AssaSubtitleFileName,
            jobItem.OutputVideoFileName,
            jobItem.Width,
            jobItem.Height,
            SelectedFrameRate.ToString(CultureInfo.InvariantCulture),
            timeCode,
            OutputHandler);
    }

    private void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
    {
        if (string.IsNullOrWhiteSpace(outLine.Data))
        {
            return;
        }

        _log?.AppendLine(outLine.Data);

        var match = FrameFinderRegex.Match(outLine.Data);
        if (!match.Success)
        {
            return;
        }

        var arr = match.Value.Split('=');
        if (arr.Length != 2)
        {
            return;
        }

        if (long.TryParse(arr[1].Trim(), out var f))
        {
            _processedFrames = f;
            ProgressValue = (double)_processedFrames / JobItems[_jobItemIndex].TotalFrames;
        }
    }

    private Subtitle GetSubtitleBasedOnCut(Subtitle inputSubtitle)
    {
        if (!IsCutActive)
        {
            return inputSubtitle;
        }

        var subtitle = new Subtitle();
        foreach (var p in inputSubtitle.Paragraphs)
        {
            if (p.StartTime.TotalMilliseconds >= CutFrom.TotalMilliseconds && p.EndTime.TotalMilliseconds <= CutTo.TotalMilliseconds)
            {
                subtitle.Paragraphs.Add(new Paragraph(p));
            }
        }

        subtitle.AddTimeToAllParagraphs(TimeSpan.FromMilliseconds(-CutFrom.TotalMilliseconds));

        return subtitle;
    }

    private ObservableCollection<BurnInJobItem> GetCurrentVideoAsJobItems()
    {
        var subtitle = new Subtitle(_subtitle);

        var srt = new SubRip();
        var subtitleFileName = Path.Combine(Path.GetTempFileName() + srt.Extension);
        if (_subtitleFormat is { Name: AdvancedSubStationAlpha.NameOfFormat })
        {
            var assa = new AdvancedSubStationAlpha();
            subtitleFileName = Path.Combine(Path.GetTempFileName() + assa.Extension);
            File.WriteAllText(subtitleFileName, assa.ToText(subtitle, string.Empty));
        }
        else
        {
            File.WriteAllText(subtitleFileName, srt.ToText(subtitle, string.Empty));
        }

        _mediaInfo = FfmpegMediaInfo2.Parse(VideoFileName);
        VideoWidth = _mediaInfo.Dimension.Width;
        VideoHeight = _mediaInfo.Dimension.Height;

        var jobItem = new BurnInJobItem(VideoFileName, _mediaInfo.Dimension.Width, _mediaInfo.Dimension.Height)
        {
            InputVideoFileName = VideoFileName,
            OutputVideoFileName = MakeOutputFileName(VideoFileName),
            UseTargetFileSize = UseTargetFileSize,
            TargetFileSize = TargetFileSize,
        };
        jobItem.AddSubtitleFileName(subtitleFileName);

        return new ObservableCollection<BurnInJobItem>(new[] { jobItem });
    }

    private string MakeAssa(string subtitleFileName)
    {
        if (string.IsNullOrWhiteSpace(subtitleFileName) || !File.Exists(subtitleFileName))
        {
            JobItems[_jobItemIndex].Status = "Skipped";
            return string.Empty;
        }

        var isAssa = subtitleFileName.EndsWith(".ass", StringComparison.OrdinalIgnoreCase);

        var subtitle = Subtitle.Parse(subtitleFileName);

        subtitle = GetSubtitleBasedOnCut(subtitle);

        if (!isAssa)
        {
            SetStyleForNonAssa(subtitle);
        }

        var assa = new AdvancedSubStationAlpha();
        var assaFileName = Path.Combine(Path.GetTempFileName() + assa.Extension);
        File.WriteAllText(assaFileName, assa.ToText(subtitle, string.Empty));
        return assaFileName;
    }

    private void SetStyleForNonAssa(Subtitle sub)
    {
        sub.Header = AdvancedSubStationAlpha.DefaultHeader;
        var style = AdvancedSubStationAlpha.GetSsaStyle("Default", sub.Header);
        style.FontSize = CalculateFontSize(JobItems[_jobItemIndex].Width, JobItems[_jobItemIndex].Height, SelectedFontFactor);
        style.Bold = FontIsBold;
        style.FontName = SelectedFontName;
        style.Background = System.Drawing.Color.FromArgb(255, (int)(FontShadowColor.Red * 255.0), (int)(FontShadowColor.Green * 255.0), (int)(FontShadowColor.Blue * 255.0));
        style.Primary = System.Drawing.Color.FromArgb(255, (int)(FontTextColor.Red * 255.0), (int)(FontTextColor.Green * 255.0), (int)(FontTextColor.Blue * 255.0));
        style.Outline = System.Drawing.Color.FromArgb(255, (int)(FontOutlineColor.Red * 255.0), (int)(FontOutlineColor.Green * 255.0), (int)(FontOutlineColor.Blue * 255.0));
        style.OutlineWidth = SelectedFontOutline;
        style.ShadowWidth = SelectedFontShadowWidth;
        style.Alignment = SelectedFontAlignment.Code;
        style.MarginLeft = FontMarginHorizontal;
        style.MarginRight = FontMarginHorizontal;
        style.MarginVertical = FontMarginVertical;

        if (SelectedFontBoxType.BoxType == FontBoxType.None)
        {
            style.BorderStyle = "0"; // bo box
        }
        else if (SelectedFontBoxType.BoxType == FontBoxType.BoxPerLine)
        {
            style.BorderStyle = "3"; // box - per line
        }
        else
        {
            style.BorderStyle = "4"; // box - multi line
        }

        sub.Header = AdvancedSubStationAlpha.GetHeaderAndStylesFromAdvancedSubStationAlpha(sub.Header, new List<SsaStyle> { style });
        sub.Header = AdvancedSubStationAlpha.AddTagToHeader("PlayResX", "PlayResX: " + ((int)VideoWidth).ToString(CultureInfo.InvariantCulture), "[Script Info]", sub.Header);
        sub.Header = AdvancedSubStationAlpha.AddTagToHeader("PlayResY", "PlayResY: " + ((int)VideoHeight).ToString(CultureInfo.InvariantCulture), "[Script Info]", sub.Header);
    }

    private static string MakeOutputFileName(string videoFileName)
    {
        if (string.IsNullOrEmpty(videoFileName))
        {
            return string.Empty;
        }

        var nameNoExt = Path.GetFileNameWithoutExtension(videoFileName);
        var ext = Path.GetExtension(videoFileName).ToLowerInvariant();
        if (ext != ".mp4" && ext != ".mkv")
        {
            ext = ".mkv";
        };

        var suffix = Se.Settings.Video.BurnIn.BurnInSuffix;
        var fileName = Path.Combine(Path.GetDirectoryName(videoFileName)!, nameNoExt + suffix + ext);
        if (Se.Settings.Video.BurnIn.UseOutputFolder &&
            !string.IsNullOrEmpty(Se.Settings.Video.BurnIn.OutputFolder) &&
            Directory.Exists(Se.Settings.Video.BurnIn.OutputFolder))
        {
            fileName = Path.Combine(Se.Settings.Video.BurnIn.OutputFolder, nameNoExt + suffix + ext);
        }

        if (File.Exists(fileName))
        {
            fileName = fileName.Remove(fileName.Length - ext.Length) + "_" + Guid.NewGuid() + ext;
        }

        return fileName;
    }

    [RelayCommand]
    private void PickFromTime()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var result = await _popupService
                .ShowPopupAsync<PickSubtitleLinePopupModel>(onPresenting: viewModel => viewModel.Initialize(_subtitle, "Select cut from line"), CancellationToken.None);
            if (result is Paragraph paragraph)
            {
                CutFrom = paragraph.StartTime.TimeSpan;
            }
        });
    }

    [RelayCommand]
    private void PickToTime()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var result = await _popupService
                .ShowPopupAsync<PickSubtitleLinePopupModel>(onPresenting: viewModel => viewModel.Initialize(_subtitle, "Select cut to line"), CancellationToken.None);
            if (result is Paragraph paragraph)
            {
                CutTo = paragraph.EndTime.TimeSpan;
            }
        });
    }


    [RelayCommand]
    private void PickFromVideoPosition()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var videoFileName = VideoFileName;
            if (string.IsNullOrEmpty(videoFileName))
            {
                videoFileName = await _fileHelper.PickAndShowVideoFile("Pick video file");
                if (string.IsNullOrWhiteSpace(videoFileName))
                {
                    return;
                }
            }

            var result = await _popupService
                .ShowPopupAsync<PickVideoPositionPopupModel>(onPresenting: viewModel => viewModel.Initialize(videoFileName, "Select cut from video position"), CancellationToken.None);
            if (result is TimeSpan timeSpan)
            {
                CutFrom = timeSpan;
            }
        });
    }

    [RelayCommand]
    private void PickToVideoPosition()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var videoFileName = VideoFileName;
            if (string.IsNullOrEmpty(videoFileName))
            {
                videoFileName = await _fileHelper.PickAndShowVideoFile("Pick video file");
                if (string.IsNullOrWhiteSpace(videoFileName))
                {
                    return;
                }
            }

            var result = await _popupService
                .ShowPopupAsync<PickVideoPositionPopupModel>(onPresenting: viewModel => viewModel.Initialize(videoFileName, "Select cut to video position"), CancellationToken.None);
            if (result is TimeSpan timeSpan)
            {
                CutTo = timeSpan;
            }
        });
    }


    [RelayCommand]
    private void ModeSwitch()
    {
        _isBatchMode = !_isBatchMode;
        ButtonModeText = _isBatchMode ? "Single mode" : "Batch mode";

        BatchView.IsVisible = _isBatchMode;

        if (_subtitleFormat is { Name: AdvancedSubStationAlpha.NameOfFormat })
        {
            FontAssaView.IsVisible = !_isBatchMode;
            FontPropertiesView.IsVisible = _isBatchMode;
        }

        UpdateNonAssaPreview();
    }

    [RelayCommand]
    private async Task PickResolution()
    {
        var resolutionItem = await _popupService.ShowPopupAsync<ResolutionPopupModel>(CancellationToken.None);
        if (resolutionItem is not ResolutionItem item)
        {
            return;
        }

        if (item.ItemType == ResolutionItemType.PickResolution)
        {
            var videoFileName = await _fileHelper.PickAndShowVideoFile("Open video file");
            if (string.IsNullOrWhiteSpace(videoFileName))
            {
                return;
            }

            var mediaInfo = FfmpegMediaInfo2.Parse(videoFileName);
            VideoWidth = mediaInfo.Dimension.Width;
            VideoHeight = mediaInfo.Dimension.Height;
            _useSourceResolution = false;
        }
        else if (item.ItemType == ResolutionItemType.UseSource)
        {
            _useSourceResolution = true;
        }
        else if (item.ItemType == ResolutionItemType.Resolution)
        {
            _useSourceResolution = false;
            VideoWidth = item.Width;
            VideoHeight = item.Height;
        }

        SetUseSourceResolution();
        SaveSettings();
    }

    private void UpdateNonAssaPreview()
    {
        if (_loading)
        {
            return;
        }

        var text = "This is a test";

        if (_subtitleFormat is { Name: AdvancedSubStationAlpha.NameOfFormat } && !_isBatchMode)
        {
            ImagePreview.Source = new SKBitmap(1, 1).ToImageSource();
            return;
        }


        var fontSize = (float)CalculateFontSize(VideoWidth, VideoHeight, SelectedFontFactor);
        SKBitmap bitmap;

        if (SelectedFontBoxType.BoxType == FontBoxType.BoxPerLine)
        {
            bitmap = TextToImageGenerator.GenerateImage(
                text,
                SelectedFontName,
                fontSize,
                FontIsBold,
                FontTextColor.ToSKColor(),
                FontShadowColor.ToSKColor(),
                FontOutlineColor.ToSKColor(),
                FontOutlineColor.ToSKColor(),
                0,
                (float)SelectedFontShadowWidth);

            if (SelectedFontShadowWidth > 0)
            {
                bitmap = TextToImageGenerator.AddShadowToBitmap(bitmap, (int)Math.Round(SelectedFontShadowWidth, MidpointRounding.AwayFromZero), FontShadowColor.ToSKColor());
            }
        }
        else if (SelectedFontBoxType.BoxType == FontBoxType.OneBox)
        {
            bitmap = TextToImageGenerator.GenerateImage(
                text,
                SelectedFontName,
                fontSize,
                FontIsBold,
                FontTextColor.ToSKColor(),
                FontOutlineColor.ToSKColor(),
                SKColors.Red,
                FontShadowColor.ToSKColor(),
                (float)SelectedFontOutline,
                0);
        }
        else // FontBoxType.None
        {
            bitmap = TextToImageGenerator.GenerateImage(
                text,
                SelectedFontName,
                fontSize,
                FontIsBold,
                FontTextColor.ToSKColor(),
                FontOutlineColor.ToSKColor(),
                FontShadowColor.ToSKColor(),
                SKColors.Transparent,
                (float)SelectedFontOutline,
                (float)SelectedFontShadowWidth);
        }

        ImagePreview.Source = bitmap.ToImageSource();
    }

    [RelayCommand]
    private async Task Ok()
    {
        SaveSettings();
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    public async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }

    public void FontTextColorTapped(object? sender, TappedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var result = await _popupService.ShowPopupAsync<ColorPickerPopupModel>(
                onPresenting: vm => vm.SetCurrentColor(FontTextColor),
                CancellationToken.None);

            if (result is Color color)
            {
                FontTextColor = color;
                UpdateNonAssaPreview();
            }
        });
    }

    public void FontOutlineColorTapped(object? sender, TappedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var result = await _popupService.ShowPopupAsync<ColorPickerPopupModel>(
                onPresenting: vm => vm.SetCurrentColor(FontOutlineColor),
                CancellationToken.None);

            if (result is Color color)
            {
                FontOutlineColor = color;
                UpdateNonAssaPreview();
            }
        });
    }

    public void FontShadowColorTapped(object? sender, TappedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var result = await _popupService.ShowPopupAsync<ColorPickerPopupModel>(
                onPresenting: vm => vm.SetCurrentColor(FontShadowColor),
                CancellationToken.None);

            if (result is Color color)
            {
                FontShadowColor = color;
                UpdateNonAssaPreview();
            }
        });
    }

    public static int CalculateFontSize(int videoWidth, int videoHeight, double factor, int minSize = 8, int maxSize = 2000)
    {
        factor = Math.Clamp(factor, 0, 1);

        // Calculate the diagonal resolution
        var diagonalResolution = Math.Sqrt(videoWidth * videoWidth + videoHeight * videoHeight);

        // Calculate base size (when factor is 0.5)
        var baseSize = diagonalResolution * 0.019; // around 2% of diagonal as base size

        // Apply logarithmic scaling
        var scaleFactor = Math.Pow(maxSize / baseSize, 2 * (factor - 0.5));
        var fontSize = (int)Math.Round(baseSize * scaleFactor);

        // Clamp the font size between minSize and maxSize
        return Math.Clamp(fontSize, minSize, maxSize);
    }

    public void FontFactorChanged(object? sender, EventArgs e)
    {
        UpdateFontSizeLabel();
        UpdateNonAssaPreview();
    }

    private void UpdateFontSizeLabel()
    {
        var fontSize = CalculateFontSize(VideoWidth, VideoHeight, SelectedFontFactor).ToString(CultureInfo.InvariantCulture);
        FontSizeText = $"Font size {fontSize} for {VideoWidth}x{VideoHeight}";
    }

    public void VideoWidthChanged(object? sender, TextChangedEventArgs e)
    {
        UpdateFontSizeLabel();
        UpdateNonAssaPreview();
    }

    public void VideoHeightChanged(object? sender, TextChangedEventArgs e)
    {
        UpdateFontSizeLabel();
        UpdateNonAssaPreview();
    }

    public void FontBoldToggled(object? sender, ToggledEventArgs e)
    {
        UpdateNonAssaPreview();
    }

    internal void FontBoxTypeChanged(object? sender, EventArgs e)
    {
        if (SelectedFontBoxType.BoxType == FontBoxType.None)
        {
            FontOutlineText = "Outline";
            FontShadowText = "Shadow";
        }

        if (SelectedFontBoxType.BoxType == FontBoxType.OneBox)
        {
            FontOutlineText = "Outline";
            FontShadowText = "Box";
        }

        if (SelectedFontBoxType.BoxType == FontBoxType.BoxPerLine)
        {
            FontOutlineText = "Box";
            FontShadowText = "Shadow";
        }

        UpdateNonAssaPreview();
    }

    public void FontShadowWidthChanged(object? sender, TextChangedEventArgs e)
    {
        UpdateNonAssaPreview();
    }

    public void FontOutlineWidthChanged(object? sender, TextChangedEventArgs e)
    {
        UpdateNonAssaPreview();
    }

    public void FontNameChanged(object? sender, EventArgs e)
    {
        UpdateNonAssaPreview();
    }

    [RelayCommand]
    private async Task BatchAdd()
    {
        var fileNames = await _fileHelper.PickAndShowSubtitleFiles("Pick subtitle files");
        if (fileNames.Length == 0)
        {
            return;
        }

        foreach (var fileName in fileNames)
        {
            var videoFileName = TryGetVideoFileName(fileName);
            var width = 0;
            var height = 0;
            long totalFrames = 0;
            double totalSeconds = 0;
            var resolution = string.Empty;
            if (!string.IsNullOrEmpty(videoFileName))
            {
                var mediaInfo = FfmpegMediaInfo2.Parse(videoFileName);
                width = mediaInfo.Dimension.Width;
                height = mediaInfo.Dimension.Height;
                totalFrames = mediaInfo.GetTotalFrames();
                totalSeconds = mediaInfo.Duration.TotalSeconds;
                resolution = mediaInfo.Dimension.ToString();
            }

            var fileInfo = new FileInfo(fileName);
            var jobItem = new BurnInJobItem(videoFileName, width, height)
            {
                OutputVideoFileName = MakeOutputFileName(videoFileName),
                TotalFrames = totalFrames,
                TotalSeconds = totalSeconds,
                Width = width,
                Height = height,
                Size = Utilities.FormatBytesToDisplayFileSize(fileInfo.Length),
                Resolution = resolution,
            };
            jobItem.AddSubtitleFileName(fileName);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                JobItems.Add(jobItem);
            });
        }
    }

    private static string TryGetVideoFileName(string fileName)
    {
        var srt = Path.ChangeExtension(fileName, ".mkv");
        if (File.Exists(srt))
        {
            return srt;
        }

        var assa = Path.ChangeExtension(fileName, ".mp4");
        if (File.Exists(srt))
        {
            return assa;
        }

        var dir = Path.GetDirectoryName(fileName);
        if (string.IsNullOrEmpty(dir))
        {
            return string.Empty;
        }

        var searchPath = Path.GetFileNameWithoutExtension(fileName);
        var files = Directory.GetFiles(dir, searchPath + "*");
        foreach (var ext in Utilities.VideoFileExtensions)
        {
            foreach (var file in files)
            {
                if (file.EndsWith(ext, StringComparison.InvariantCultureIgnoreCase))
                {
                    return file;
                }
            }
        }

        return string.Empty;
    }

    [RelayCommand]
    private void BatchRemove()
    {
        if (SelectedJobItem != null)
        {
            JobItems.Remove(SelectedJobItem);
        }
    }

    [RelayCommand]
    private void BatchClear()
    {
        JobItems.Clear();
    }

    [RelayCommand]
    private async Task BatchPickVideoFile()
    {
        if (SelectedJobItem == null)
        {
            return;
        }

        var fileName = await _fileHelper.PickAndShowVideoFile("Pick video file");
        if (string.IsNullOrEmpty(fileName))
        {
            var mediaInfo = FfmpegMediaInfo2.Parse(fileName);
            SelectedJobItem.AddInputVideoFileName(fileName);
            SelectedJobItem.TotalFrames = mediaInfo.GetTotalFrames();
            SelectedJobItem.TotalSeconds = mediaInfo.Duration.TotalSeconds;
            SelectedJobItem.Width = mediaInfo.Dimension.Width;
            SelectedJobItem.Height = mediaInfo.Dimension.Height;
        }
    }

    [RelayCommand]
    private async Task BatchOutputProperties()
    {
        var input = new OutputProperties
        {
            UseOutputFolder = Se.Settings.Video.BurnIn.UseOutputFolder,
            OutputFolder = Se.Settings.Video.BurnIn.OutputFolder,
            Suffix = Se.Settings.Video.Transparent.OutputSuffix,
        };

        var result = await _popupService.ShowPopupAsync<OutputPropertiesPopupModel>(onPresenting: viewModel => viewModel.Initialize(input), CancellationToken.None);

        if (result is OutputProperties outputResult)
        {
            Se.Settings.Video.BurnIn.UseOutputFolder = outputResult.UseOutputFolder;
            Se.Settings.Video.BurnIn.OutputFolder = outputResult.OutputFolder;
            Se.Settings.Video.Transparent.OutputSuffix = outputResult.Suffix;
            Se.SaveSettings();

            UseOutputFolderVisible = outputResult.UseOutputFolder;
            UseSourceFolderVisible = !Se.Settings.Video.BurnIn.UseOutputFolder;
            OutputSourceFolder = outputResult.OutputFolder;
        }
    }

    public void OutputFolderLinkMouseEntered(object? sender, PointerEventArgs e)
    {
        LabelOutputFolder.TextColor = (Color)Application.Current!.Resources[ThemeNames.LinkColor];
    }

    public void OutputFolderLinkMouseExited(object? sender, PointerEventArgs e)
    {
        LabelOutputFolder.TextColor = (Color)Application.Current!.Resources[ThemeNames.TextColor];
    }

    public void OutputFolderLinkMouseClicked(object? sender, TappedEventArgs e)
    {
        UiUtil.OpenFolder(Se.Settings.Video.BurnIn.OutputFolder);
    }
}
