using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.SubtitleFormats;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SubtitleAlchemist.Controls.ColorPickerControl;
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
using SubtitleAlchemist.Features.Shared.PickVideoPosition;
using Timer = System.Timers.Timer;

namespace SubtitleAlchemist.Features.Video.BurnIn;

public partial class BurnInPageModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private string _videoFileName;

    [ObservableProperty]
    private string _videoFileSize;

    [ObservableProperty]
    private ObservableCollection<string> _fontNames;

    [ObservableProperty]
    private string _selectedFontName;

    [ObservableProperty]
    private ObservableCollection<double> _fontFactors;

    [ObservableProperty]
    private double _selectedFontFactor;

    [ObservableProperty]
    private string _fontSizeText;

    [ObservableProperty]
    private bool _fontIsBold;

    [ObservableProperty]
    private ObservableCollection<decimal> _fontOutlines;

    [ObservableProperty]
    private decimal _selectedFontOutline;

    [ObservableProperty]
    private string _fontOutlineText;

    [ObservableProperty]
    private ObservableCollection<decimal> _fontShadowWidths;

    [ObservableProperty]
    private decimal _selectedFontShadowWidth;

    [ObservableProperty]
    private string _fontShadowText;

    [ObservableProperty]
    private ObservableCollection<FontBoxItem> _fontBoxTypes;

    [ObservableProperty]
    private FontBoxItem _selectedFontBoxType;

    [ObservableProperty]
    private Color _fontTextColor;

    [ObservableProperty]
    private Color _fontBoxColor;

    [ObservableProperty]
    private Color _fontOutlineColor;

    [ObservableProperty]
    private Color _fontShadowColor;

    [ObservableProperty]
    private int _fontMarginHorizontal;

    [ObservableProperty]
    private int _fontMarginVertical;

    [ObservableProperty]
    private bool _fontFixRtl;

    [ObservableProperty]
    private ObservableCollection<AlignmentItem> _fontAlignments;

    [ObservableProperty]
    private AlignmentItem _selectedFontAlignment;

    [ObservableProperty]
    private string _fontAssaInfo;

    [ObservableProperty]
    private int _videoWidth;

    [ObservableProperty]
    private int _videoHeight;

    [ObservableProperty]
    private ObservableCollection<VideoEncodingItem> _videoEncodings;

    [ObservableProperty]
    private VideoEncodingItem _selectedVideoEncoding;

    [ObservableProperty]
    private ObservableCollection<PixelFormatItem> _videoPixelFormats;

    [ObservableProperty]
    private PixelFormatItem? _selectedVideoPixelFormat;

    [ObservableProperty]
    private ObservableCollection<string> _videoPresets;

    [ObservableProperty]
    private string? _selectedVideoPreset;

    [ObservableProperty]
    private string _videoPresetText;

    [ObservableProperty]
    private ObservableCollection<string> _videoCrf;

    [ObservableProperty]
    private string? _selectedVideoCrf;

    [ObservableProperty]
    private string _videoCrfText;

    [ObservableProperty]
    private string _videoCrfHint;

    [ObservableProperty]
    private ObservableCollection<string> _videoTuneFor;

    [ObservableProperty]
    private string? _selectedVideoTuneFor;

    [ObservableProperty]
    private ObservableCollection<string> _videoExtension;

    [ObservableProperty]
    private int _selectedVideoExtension;


    [ObservableProperty]
    private ObservableCollection<string> _audioEncodings;

    [ObservableProperty]
    private string _selectedAudioEncoding;

    [ObservableProperty]
    private bool _audioIsStereo;

    [ObservableProperty]
    private ObservableCollection<string> _audioSampleRates;

    [ObservableProperty]
    private string _selectedAudioSampleRate;

    [ObservableProperty]
    private ObservableCollection<string> _audioBitRates;

    [ObservableProperty]
    private string _selectedAudioBitRate;

    [ObservableProperty]
    private string _outputSourceFolder;

    [ObservableProperty]
    private bool _useOutputFolderVisible;

    [ObservableProperty]
    private bool _useSourceFolderVisible;

    [ObservableProperty]
    private bool _isCutActive;

    [ObservableProperty]
    private TimeSpan _cutFrom;

    [ObservableProperty]
    private TimeSpan _cutTo;


    [ObservableProperty]
    private bool _useTargetFileSize;

    [ObservableProperty]
    private int _targetFileSize;

    [ObservableProperty]
    private string _buttonModeText;

    [ObservableProperty]
    private string _progressText;

    [ObservableProperty]
    private double _progressValue;

    [ObservableProperty]
    private ObservableCollection<BurnInJobItem> _jobItems;

    [ObservableProperty]
    private BurnInJobItem? _selectedJobItem;

    public BurnInPage? Page { get; set; }
    public MediaElement VideoPlayer { get; set; }
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
    public Border VideoSizeView { get; set; }
    public Border VideoInfoView { get; set; }

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

    public BurnInPageModel(IPopupService popupService, IFileHelper fileHelper)
    {
        _popupService = popupService;
        _fileHelper = fileHelper;

        _fontBoxColor = Colors.Wheat;
        _fontOutlineColor = Colors.Black;
        _fontShadowColor = Colors.Black;
        _videoCrfText = string.Empty;
        _videoCrf = new();
        _videoTuneFor = new();
        _videoExtension = new();
        _outputSourceFolder = string.Empty;
        _progressText = string.Empty;
        VideoPlayer = new();
        ButtonGenerate = new();
        ButtonOk = new();
        ButtonMode = new();
        ButtonResolution = new();
        ProgressValue = 0;
        ProgressBar = new();
        ImagePreview = new();
        BatchView = new();
        LabelOutputFolder = new();
        _videoFileName = string.Empty;
        _videoFileSize = string.Empty;
        EntryWidth = new();
        EntryHeight = new();
        LabelX = new();
        LabelVideoFileName = new();
        LabelVideoFileSize = new();
        FontAssaView = new();
        FontPropertiesView = new();

        _fontNames = new ObservableCollection<string>(FontHelper.GetSystemFonts());
        _selectedFontName = _fontNames.First();

        // font factors between 0-1
        _fontFactors = new ObservableCollection<double>(
            Enumerable.Range(200, 1000)
            .Select(i => Math.Round(i * 0.0005, 3))
            .ToList().Distinct());
        _selectedFontFactor = 0.4;
        _fontSizeText = string.Empty;

        _fontTextColor = Colors.WhiteSmoke;

        _fontOutlineText = "Outline";
        _fontOutlines = new ObservableCollection<decimal>(Enumerable.Range(0, 50).Select(p => (decimal)p));
        _selectedFontOutline = 2.0m;

        _fontShadowText = "Shadow";
        _fontShadowWidths = new ObservableCollection<decimal>(Enumerable.Range(0, 50).Select(p => (decimal)p));
        _selectedFontShadowWidth = 2.0m;

        _fontBoxTypes = new ObservableCollection<FontBoxItem>
        {
            new(FontBoxType.None, "None"),
            new(FontBoxType.OneBox, "One box"),
            new(FontBoxType.BoxPerLine, "Box per line"),
        };
        _selectedFontBoxType = _fontBoxTypes[0];

        _fontMarginHorizontal = 10;
        _fontMarginVertical = 10;

        _fontAlignments = new ObservableCollection<AlignmentItem>(AlignmentItem.Alignments);
        _selectedFontAlignment = AlignmentItem.Alignments[7];

        _fontAssaInfo = string.Empty;

        _videoWidth = 1920;
        _videoHeight = 1080;

        _videoEncodings = new ObservableCollection<VideoEncodingItem>(VideoEncodingItem.VideoEncodings);
        _selectedVideoEncoding = _videoEncodings[0];

        _videoPixelFormats = new ObservableCollection<PixelFormatItem>(PixelFormatItem.PixelFormats);
        _selectedVideoPixelFormat = _videoPixelFormats[0];

        _videoPresetText = "Preset";

        _videoCrfText = "CRF";
        _videoCrfHint = string.Empty;

        _videoPresets = new ObservableCollection<string>();


        _audioEncodings = new ObservableCollection<string>
        {
            "copy",
            "aac",
            "ac3",
            "mp3",
            "opus",
            "vorbis",
        };
        _selectedAudioEncoding = "copy";

        _audioSampleRates = new ObservableCollection<string>
        {
            "44100 Hz",
            "48000 Hz",
            "88200 Hz",
            "96000 Hz",
            "192000 Hz",
        };
        _selectedAudioSampleRate = _audioSampleRates[1];

        _audioBitRates = new ObservableCollection<string>
        {
            "64k",
            "96k",
            "128k",
            "160k",
            "192k",
            "256k",
            "320k",
        };
        _selectedAudioBitRate = _audioBitRates[2];


        VideoInfoView = new Border();
        VideoSizeView = new Border();

        _jobItems = new ObservableCollection<BurnInJobItem>();

        _buttonModeText = "Batch mode";

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
        _ffmpegProcess = GetFfmpegProcess(jobItem, 2);
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
            percentage = Math.Clamp(percentage, 0, 100);

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
                VideoSizeView.IsVisible = true;
                VideoInfoView.IsVisible = true;

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
                VideoEncodingChanged(null, EventArgs.Empty);
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
        var mediaInfo = FfmpegMediaInfo2.Parse(jobItem.InputVideoFileName);
        jobItem.TotalFrames = mediaInfo.GetTotalFrames();
        jobItem.TotalSeconds = mediaInfo.Duration.TotalSeconds;
        jobItem.Width = mediaInfo.Dimension.Width;
        jobItem.Height = mediaInfo.Dimension.Height;
        jobItem.UseTargetFileSize = UseTargetFileSize;
        jobItem.TargetFileSize = UseTargetFileSize ? TargetFileSize : 0;
        jobItem.AssaSubtitleFileName = MakeAssa(jobItem.SubtitleFileName);
        jobItem.Status = "Generating...";

        bool result;
        if (jobItem.UseTargetFileSize)
        {
            result = RunTwoPassEncoding(jobItem);
            if (result)
            {
                _timerAnalyze.Start();
            }
        }
        else
        {
            result = RunOnePassEncoding(jobItem);
            if (result)
            {
                _timerGenerate.Start();
            }
        }
    }

    private bool RunTwoPassEncoding(BurnInJobItem jobItem)
    {
        var bitRate = GetVideoBitRate(jobItem);
        jobItem.VideoBitRate = bitRate.ToString(CultureInfo.InvariantCulture) + "k";

        if (bitRate < 10)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Page!.DisplayAlert(
                    "Unable to generate video",
                    $"Bit rate too low: {bitRate}k",
                    "OK");
            });
            return false;
        }

        _ffmpegProcess = GetFfmpegProcess(jobItem, 1);

#pragma warning disable CA1416 // Validate platform compatibility
        _ffmpegProcess.Start();
#pragma warning restore CA1416 // Validate platform compatibility
        _ffmpegProcess.BeginOutputReadLine();
        _ffmpegProcess.BeginErrorReadLine();
        _startTicks = DateTime.UtcNow.Ticks;

        return true;
    }

    private int GetVideoBitRate(BurnInJobItem item)
    {
        var audioMb = 0;
        if (SelectedAudioEncoding == "copy")
        {
            audioMb = GetAudioFileSizeInMb(item);
        }

        // (MiB * 8192 [converts MiB to kBit]) / video seconds = kBit/s total bitrate
        var bitRate = (int)Math.Round(((double)TargetFileSize - audioMb) * 8192.0 / item.TotalSeconds);
        if (SelectedAudioEncoding != "copy" && !string.IsNullOrWhiteSpace(SelectedAudioBitRate))
        {
            var audioBitRate = int.Parse(SelectedAudioBitRate.RemoveChar('k').TrimEnd());
            bitRate -= audioBitRate;
        }

        return bitRate;
    }

    private int GetAudioFileSizeInMb(BurnInJobItem item)
    {
        var ffmpegLocation = Configuration.Settings.General.FFmpegLocation;
        if (!Configuration.IsRunningOnWindows && (string.IsNullOrEmpty(ffmpegLocation) || !File.Exists(ffmpegLocation)))
        {
            ffmpegLocation = "ffmpeg";
        }

        var tempFileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".aac");
        var process = new Process
        {
            StartInfo =
            {
                FileName = ffmpegLocation,
                Arguments = $"-i \"{item.InputVideoFileName}\" -vn -acodec copy \"{tempFileName}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };

        _ = process.Start();
        process.WaitForExit();
        try
        {
            var length = (int)Math.Round(new FileInfo(tempFileName).Length / 1024.0 / 1024);
            try
            {
                File.Delete(tempFileName);
            }
            catch
            {
                // ignore
            }

            return length;
        }
        catch
        {
            return 0;
        }
    }

    private bool RunOnePassEncoding(BurnInJobItem jobItem)
    {
        _ffmpegProcess = GetFfmpegProcess(jobItem);
#pragma warning disable CA1416 // Validate platform compatibility
        _ffmpegProcess.Start();
#pragma warning restore CA1416 // Validate platform compatibility
        _ffmpegProcess.BeginOutputReadLine();
        _ffmpegProcess.BeginErrorReadLine();

        return true;
    }

    private Process GetFfmpegProcess(BurnInJobItem jobItem, int? passNumber = null, bool preview = false)
    {
        var audioCutTracks = string.Empty;
        //if (listViewAudioTracks.Visible)
        //{
        //    for (var index = 0; index < listViewAudioTracks.Items.Count; index++)
        //    {
        //        var listViewItem = listViewAudioTracks.Items[index];
        //        if (!listViewItem.Checked)
        //        {
        //            audioCutTracks += $"-map 0:a:{index} ";
        //        }
        //    }
        //}

        var pass = string.Empty;
        if (passNumber.HasValue)
        {
            pass = passNumber.Value.ToString(CultureInfo.InvariantCulture);
        }

        var cutStart = string.Empty;
        var cutEnd = string.Empty;
        if (IsCutActive && !preview)
        {
            var start = CutFrom;
            cutStart = $"-ss {start.Hours:00}:{start.Minutes:00}:{start.Seconds:00}";

            var end = CutTo;
            var duration = end - start;
            cutEnd = $"-t {duration.Hours:00}:{duration.Minutes:00}:{duration.Seconds:00}";
        }

        return VideoPreviewGenerator.GenerateHardcodedVideoFile(
            jobItem.InputVideoFileName,
            jobItem.AssaSubtitleFileName,
            jobItem.OutputVideoFileName,
            jobItem.Width,
            jobItem.Height,
            SelectedVideoEncoding.Codec,
            SelectedVideoPreset ?? string.Empty,
            SelectedVideoPixelFormat?.Codec ?? string.Empty,
            SelectedVideoCrf ?? string.Empty,
            SelectedAudioEncoding,
            AudioIsStereo,
            SelectedAudioSampleRate.Replace("Hz", string.Empty).Trim(),
            SelectedVideoTuneFor ?? string.Empty,
            SelectedAudioBitRate,
            pass,
            jobItem.VideoBitRate,
            OutputHandler,
            cutStart,
            cutEnd,
            audioCutTracks);
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
        VideoSizeView.IsVisible = !_isBatchMode;
        VideoInfoView.IsVisible = !_isBatchMode;

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

    public void VideoEncodingChanged(object? sender, EventArgs e)
    {
        if (_loading)
        {
            return;
        }

        FillPreset(SelectedVideoEncoding.Codec);
        FillTuneIn(SelectedVideoEncoding.Codec);
        FillCrf(SelectedVideoEncoding.Codec);
    }

    private void FillPreset(string videoCodec)
    {
        VideoPresetText = "Preset";
        SelectedVideoPreset = null;

        var items = new List<string>
        {
           "ultrafast",
           "superfast",
           "veryfast",
           "faster",
           "fast",
           "medium",
           "slow",
           "slower",
           "veryslow",
        };

        var defaultItem = "medium";

        if (videoCodec == "h264_nvenc")
        {
            items = new List<string>
            {
                "default",
                "slow",
                "medium",
                "fast",
                "hp",
                "hq",
                "bd",
                "ll",
                "llhq",
                "llhp",
                "lossless",
                "losslesshp",
                "p1",
                "p2",
                "p3",
                "p4",
                "p5",
                "p6",
                "p7",
            };
        }
        else if (videoCodec == "hevc_nvenc")
        {
            items = new List<string>
            {
                "default",
                "slow",
                "medium",
                "fast",
                "hp",
                "hq",
                "bd",
                "ll",
                "llhq",
                "llhp",
                "lossless",
                "losslesshp",
                "p1",
                "p2",
                "p3",
                "p4",
                "p5",
                "p6",
                "p7",
            };
        }
        else if (videoCodec == "h264_amf")
        {
            items = new List<string> { string.Empty };
        }
        else if (videoCodec == "hevc_amf")
        {
            items = new List<string> { string.Empty };
        }
        else if (videoCodec == "libvpx-vp9")
        {
            items = new List<string> { string.Empty };
        }
        else if (videoCodec == "prores_ks")
        {
            items = new List<string>
            {
                "proxy",
                "lt",
                "standard",
                "hq",
                "4444",
                "4444xq",
            };

            defaultItem = "standard";

            VideoPresetText = "Profile";
        }

        VideoPresets = new ObservableCollection<string>(items);
        if (VideoPresets.Contains(defaultItem))
        {
            SelectedVideoPreset = defaultItem;
        }
    }

    private void FillTuneIn(string videoCodec)
    {
        VideoCrfHint = string.Empty;
        SelectedVideoTuneFor = null;

        var items = new List<string>
        {
            " ",
            "film",
            "animation",
            "grain",
        };

        var defaultItem = string.Empty;

        if (videoCodec == "libx265")
        {
            items = new List<string>
            {
                " ",
                "psnr",
                "ssim",
                "grain",
                "zerolatency",
                "fastdecode",
            };
        }
        else if (videoCodec == "libx264")
        {
            items = new List<string>
            {
                " ",
                "film",
                "animation",
                "grain",
                "stillimage",
                "fastdecode",
                "zerolatency",
            };
        }
        else if (videoCodec == "h264_nvenc")
        {
            items = new List<string>
            {
                " ",
                "hq",
                "ll",
                "ull",
                "lossless",
            };
        }
        else if (videoCodec == "hevc_nvenc")
        {
            items = new List<string>
            {
                " ",
                "hq",
                "ll",
                "ull",
                "lossless",
            };
        }
        else if (videoCodec == "h264_amf")
        {
            items = new List<string>();
        }
        else if (videoCodec == "hevc_amf")
        {
            items = new List<string>();
        }
        else if (videoCodec == "libvpx-vp9")
        {
            items = new List<string>();
        }
        else if (videoCodec == "prores_ks")
        {
            items = new List<string>();
        }
        else if (videoCodec == "h264_qsv" || videoCodec == "hevc_qsv") // Intel
        {
            items = new List<string>();
        }

        VideoTuneFor = new ObservableCollection<string>(items);
        SelectedVideoTuneFor = items.Contains(defaultItem) ? defaultItem : null;
    }

    public void FillCrf(string videoCodec)
    {
        SelectedVideoCrf = null;
        VideoCrfText = "CRF";

        var items = new List<string> { " " };

        if (videoCodec == "libx265")
        {
            for (var i = 0; i < 51; i++)
            {
                items.Add(i.ToString(CultureInfo.InvariantCulture));
            }

            VideoCrf = new ObservableCollection<string>(items);
            SelectedVideoCrf = "28";
        }
        else if (videoCodec == "libvpx-vp9")
        {
            for (var i = 4; i <= 63; i++)
            {
                items.Add(i.ToString(CultureInfo.InvariantCulture));
            }

            VideoCrf = new ObservableCollection<string>(items);
            SelectedVideoCrf = "10";
        }
        else if (videoCodec == "h264_nvenc" ||
                 videoCodec == "hevc_nvenc")
        {
            for (var i = 0; i <= 51; i++)
            {
                items.Add(i.ToString(CultureInfo.InvariantCulture));
            }

            VideoCrfText = "CQ";
            VideoCrfHint = "0=best quality, 51=best speed";
            SelectedVideoCrf = null;
        }
        else if (videoCodec == "h264_amf" ||
                 videoCodec == "hevc_amf")
        {
            for (var i = 0; i <= 10; i++)
            {
                items.Add(i.ToString(CultureInfo.InvariantCulture));
            }

            VideoCrfText = "Quality";
            VideoCrfHint = "0=best quality, 10=best speed";
            SelectedVideoCrf = null;
        }
        else if (videoCodec == "prores_ks")
        {
            items = new List<string>();
            VideoCrf = new ObservableCollection<string>(items);
        }
        else
        {
            for (var i = 17; i <= 28; i++)
            {
                items.Add(i.ToString(CultureInfo.InvariantCulture));
            }

            VideoCrf = new ObservableCollection<string>(items);
            SelectedVideoCrf = "23";
        }
    }

    [RelayCommand]
    public void ShowHelp()
    {
        var codec = SelectedVideoEncoding.Codec;

        if (codec == "libx265")
        {
            UiUtil.OpenUrl("http://trac.ffmpeg.org/wiki/Encode/H.265");
        }
        else if (codec == "libvpx-vp9")
        {
            UiUtil.OpenUrl("http://trac.ffmpeg.org/wiki/Encode/VP9");
        }
        else if (codec is "h264_nvenc" or "hevc_nvenc")
        {
            UiUtil.OpenUrl("https://trac.ffmpeg.org/wiki/HWAccelIntro");
        }
        else if (codec == "prores_ks")
        {
            UiUtil.OpenUrl("https://ottverse.com/ffmpeg-convert-to-apple-prores-422-4444-hq");
        }
        else
        {
            UiUtil.OpenUrl("http://trac.ffmpeg.org/wiki/Encode/H.264");
        }
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
        var fileNames = await _fileHelper.PickAndShowVideoFiles("Pick video files");
        if (fileNames.Length == 0)
        {
            return;
        }

        foreach (var fileName in fileNames)
        {
            var mediaInfo = FfmpegMediaInfo2.Parse(fileName);
            var fileInfo = new FileInfo(fileName);
            var jobItem = new BurnInJobItem(fileName, mediaInfo.Dimension.Width, mediaInfo.Dimension.Height)
            {
                OutputVideoFileName = MakeOutputFileName(fileName),
                TotalFrames = mediaInfo.GetTotalFrames(),
                TotalSeconds = mediaInfo.Duration.TotalSeconds,
                Width = mediaInfo.Dimension.Width,
                Height = mediaInfo.Dimension.Height,
                Size = Utilities.FormatBytesToDisplayFileSize(fileInfo.Length),
                Resolution = mediaInfo.Dimension.ToString(),
            };
            jobItem.AddSubtitleFileName(TryGetSubtitleFileName(fileName));

            MainThread.BeginInvokeOnMainThread(() =>
            {
                JobItems.Add(jobItem);
            });
        }
    }

    private static string TryGetSubtitleFileName(string fileName)
    {
        var srt = Path.ChangeExtension(fileName, ".srt");
        if (File.Exists(srt))
        {
            return srt;
        }

        var assa = Path.ChangeExtension(fileName, ".ass");
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
        var subtitleExtensions = SubtitleFormat.AllSubtitleFormats.Select(p => p.Extension).Distinct();
        foreach (var ext in subtitleExtensions)
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
            var idx = JobItems.IndexOf(SelectedJobItem);
            JobItems.Remove(SelectedJobItem);
        }
    }

    [RelayCommand]
    private void BatchClear()
    {
        JobItems.Clear();
    }


    [RelayCommand]
    private async Task BatchPickSubtitleFile()
    {
        if (SelectedJobItem == null)
        {
            return;
        }

        var fileName = await _fileHelper.PickAndShowSubtitleFile("Open subtitle file");
        if (string.IsNullOrEmpty(fileName))
        {
            SelectedJobItem.SubtitleFileName = fileName;
        }
    }

    [RelayCommand]
    private async Task BatchOutputProperties()
    {
        var input = new OutputProperties
        {
            UseOutputFolder = Se.Settings.Video.BurnIn.UseOutputFolder,
            OutputFolder = Se.Settings.Video.BurnIn.OutputFolder,
            Suffix = Se.Settings.Video.BurnIn.BurnInSuffix,
        };

        var result = await _popupService.ShowPopupAsync<OutputPropertiesPopupModel>(onPresenting: viewModel => viewModel.Initialize(input), CancellationToken.None);

        if (result is OutputProperties outputResult)
        {
            Se.Settings.Video.BurnIn.UseOutputFolder = outputResult.UseOutputFolder;
            Se.Settings.Video.BurnIn.OutputFolder = outputResult.OutputFolder;
            Se.Settings.Video.BurnIn.BurnInSuffix = outputResult.Suffix;
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
