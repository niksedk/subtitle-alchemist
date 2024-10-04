using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Controls.ColorPickerControl;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Constants;
using SubtitleAlchemist.Logic.Media;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using Nikse.SubtitleEdit.Core.SubtitleFormats;
using SubtitleAlchemist.Logic.Config;
using Timer = System.Timers.Timer;

namespace SubtitleAlchemist.Features.Video.BurnIn;

public partial class BurnInPageModel : ObservableObject, IQueryAttributable
{
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
    private string _fontShadowText;

    [ObservableProperty]
    private ObservableCollection<string> _fontFamilies;

    [ObservableProperty]
    private string _selectedFontFamily;

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
    private bool _fontFixRtl;

    [ObservableProperty]
    private bool _fontAlignRight;


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
    private bool _cutIsActive;

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

    public BurnInPage? Page { get; set; }
    public MediaElement VideoPlayer { get; set; }
    public Label LabelHelp { get; set; }
    public Button ButtonGenerate { get; set; }
    public Button ButtonOk { get; set; }
    public Button ButtonMode { get; internal set; }

    private Subtitle _subtitle = new();
    private readonly IPopupService _popupService;
    private bool _loading = true;
    private string _videoFileName;
    private StringBuilder _log;
    private static readonly Regex FrameFinderRegex = new Regex(@"[Ff]rame=\s*\d+", RegexOptions.Compiled);
    private long _startTicks;
    private long _processedFrames;
    private Process? _onePassProcess;
    private readonly Timer _timerOnePass = new();
    private bool _doAbort;
    private bool _isBatchMode;
    private List<BurnInJobItem> _jobItems = new();
    private int _jobItemIndex = -1;
    private FfmpegMediaInfo2? _mediaInfo;

    public BurnInPageModel(IPopupService popupService)
    {
        _popupService = popupService;

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


        _fontBoxTypes = new ObservableCollection<FontBoxItem>
        {
            new FontBoxItem(FontBoxType.None, "None"),
            new FontBoxItem(FontBoxType.OneBox, "One box"),
            new FontBoxItem(FontBoxType.BoxPerLine, "Box per line"),
        };
        _selectedFontBoxType = _fontBoxTypes[0];

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

        _buttonModeText = "Batch mode";

        _log = new StringBuilder();

        _timerOnePass.Elapsed += TimerOnePassElapsed;
        _timerOnePass.Interval = 100;
    }

    private void TimerOnePassElapsed(object? sender, ElapsedEventArgs e)
    {
        if (_onePassProcess == null)
        {
            return;
        }

        if (_doAbort)
        {
            _timerOnePass.Stop();
#pragma warning disable CA1416
            _onePassProcess.Kill(true);
#pragma warning restore CA1416
            return;
        }

        if (!_onePassProcess.HasExited)
        {
            var durationMs = (DateTime.UtcNow.Ticks - _startTicks) / 10_000;
            // ElapsedText = $"Time elapsed: {new TimeCode(durationMs).ToShortDisplayString()}";

            return;
        }

        _timerOnePass.Stop();


        var jobItem = _jobItems[_jobItemIndex];

        if (!File.Exists(jobItem.OutputVideoFileName))
        {

            SeLogger.WhisperInfo("Output video file not found: " + jobItem.OutputVideoFileName + Environment.NewLine +
                                 "ffmpeg: " + _onePassProcess.StartInfo.FileName + Environment.NewLine +
                                 "Parameters: " + _onePassProcess.StartInfo.Arguments + Environment.NewLine +
                                 "OS: " + Environment.OSVersion + Environment.NewLine +
                                 "64-bit: " + Environment.Is64BitOperatingSystem + Environment.NewLine +
                                 "ffmpeg exit code: " + _onePassProcess.ExitCode + Environment.NewLine +
                                 "ffmpeg log: " + _log);

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                var answer = await Page!.DisplayAlert(
                    "Unable to generate video",
                    "Output video file not generated: " + jobItem.OutputVideoFileName + Environment.NewLine +
                    "Parameters: " + _onePassProcess.StartInfo.Arguments,
                    "OK", "Cancel");

                ButtonGenerate.IsEnabled = true;
                ButtonOk.IsEnabled = true;
            });

            return;
        }

        MainThread.BeginInvokeOnMainThread(() =>
        {
            ButtonGenerate.IsEnabled = true;
            ButtonOk.IsEnabled = true;
            UiUtil.OpenFolderFromFileName(jobItem.OutputVideoFileName);
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
            _videoFileName = videoFileName;
        }


        //   Page?.Initialize(_subtitle, this);

        Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LoadSettings();

                if (string.IsNullOrWhiteSpace(_videoFileName))
                {
                    _isBatchMode = true;
                }
                else
                {
                    _mediaInfo = FfmpegMediaInfo2.Parse(_videoFileName);
                    VideoWidth = _mediaInfo.Dimension.Width;
                    VideoHeight = _mediaInfo.Dimension.Height;
                    _isBatchMode = false;
                }

                _loading = false;
                VideoEncodingChanged(null, EventArgs.Empty);
            });
            return false;
        });
    }

    private void LoadSettings()
    {
        var settings = Se.Settings.Video.BurnIn;
        SelectedFontFactor = settings.FontFactor;
        FontIsBold = settings.FontBold;
        SelectedFontOutline = settings.Outline;
        SelectedFontFamily = settings.FontName;
        FontTextColor = settings.NonAssaTextColor;
        FontOutlineColor = settings.NonAssaOutlineColor;
        FontBoxColor = settings.NonAssaBoxColor;
        FontShadowColor = settings.NonAssaShadowColor;
        FontFixRtl = settings.NonAssaFixRtlUnicode;
        FontAlignRight = settings.NonAssaAlignRight;
    }

    private void SaveSettings()
    {
        var settings = Se.Settings.Video.BurnIn;
        settings.FontFactor = SelectedFontFactor;
        settings.FontBold = FontIsBold;
        settings.Outline = SelectedFontOutline;
        settings.FontName = SelectedFontFamily;
        settings.NonAssaTextColor = FontTextColor;
        settings.NonAssaOutlineColor = FontOutlineColor;
        settings.NonAssaBoxColor = FontBoxColor;
        settings.NonAssaShadowColor = FontShadowColor;
        settings.NonAssaFixRtlUnicode = FontFixRtl;
        settings.NonAssaAlignRight = FontAlignRight;

        Se.SaveSettings();
    }

    [RelayCommand]
    private async Task Generate()
    {
        if (CutIsActive && CutFrom >= CutTo)
        {
            var answer = await Page!.DisplayAlert(
                "Cut settings error",
                "Cut end time must be after cut start time",
                "OK", "Cancel");

            return;
        }

        _doAbort = false;
        _log.Clear();
        _processedFrames = 0;
        ButtonGenerate.IsEnabled = false;
        ButtonOk.IsEnabled = false;
        SaveSettings();
        _jobItems = GetJobItems();
        if (_jobItems.Count == 0)
        {
            return;
        }

        _startTicks = DateTime.UtcNow.Ticks;

        for (var index = 0; index < _jobItems.Count; index++)
        {
            _jobItemIndex = index;
            var jobItem = _jobItems[index];
            var mediaInfo = FfmpegMediaInfo2.Parse(jobItem.InputVideoFileName);
            jobItem.TotalFrames = mediaInfo.GetTotalFrames();
            jobItem.Width = mediaInfo.Dimension.Width;
            jobItem.Height = mediaInfo.Dimension.Height;
            jobItem.UseTargetFileSize = UseTargetFileSize;
            jobItem.TargetFileSize = UseTargetFileSize ? TargetFileSize : 0;
            jobItem.AssaSubtitleFileName = MakeAssa(jobItem.SubtitleFileName);


            bool result;
            if (jobItem.UseTargetFileSize)
            {
                result = RunTwoPassEncoding(jobItem);
                if (result)
                {
                    //_timerFirstPass.Start();
                }
            }
            else
            {
                result = RunOnePassEncoding(jobItem);
                if (result)
                {
                    _timerOnePass.Start();
                }
            }
        }
    }

    private bool RunTwoPassEncoding(BurnInJobItem jobItem)
    {
        var process = GetFfmpegProcess(jobItem);
        return true;
    }

    private bool RunOnePassEncoding(BurnInJobItem jobItem)
    {
        _onePassProcess = GetFfmpegProcess(jobItem);
#pragma warning disable CA1416 // Validate platform compatibility
        _onePassProcess.Start();
#pragma warning restore CA1416 // Validate platform compatibility
        _onePassProcess.BeginOutputReadLine();
        _onePassProcess.BeginErrorReadLine();
        return true;
    }

    private Process GetFfmpegProcess(BurnInJobItem jobItem, int? passNumber = null, string? twoPassBitRate = null, bool preview = false)
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
        if (CutIsActive && !preview)
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
            SelectedVideoPreset,
            SelectedVideoPixelFormat.Codec,
            SelectedVideoCrf,
            SelectedAudioEncoding,
            AudioIsStereo,
            SelectedAudioSampleRate.Replace("Hz", string.Empty).Trim(),
            SelectedVideoTuneFor,
            SelectedAudioBitRate,
            pass,
            twoPassBitRate,
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
        }
    }

    private List<BurnInJobItem> GetJobItems()
    {

        var subtitle = new Subtitle(_subtitle);

        var srt = new SubRip();
        var srtFileName = Path.Combine(Path.GetTempFileName() + srt.Extension);
        File.WriteAllText(srtFileName, srt.ToText(subtitle, string.Empty));

        return new List<BurnInJobItem>()
        {
            new()
            {
                InputVideoFileName = _videoFileName,
                OutputVideoFileName = MakeOutputFileName(_videoFileName),
                SubtitleFileName = srtFileName,
            },
        };
    }

    private string MakeAssa(string subtitleFileName)
    {
        var isAssa = subtitleFileName.EndsWith(".ass");

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
        style.FontSize = CalculateFontSize(_jobItems[_jobItemIndex].Width, _jobItems[_jobItemIndex].Height, SelectedFontFactor);
        style.Bold = FontIsBold;
        style.FontName = SelectedFontFamily;
        style.Background = System.Drawing.Color.FromArgb(255, (int)(FontOutlineColor.Red * 255.0), (int)(FontOutlineColor.Green * 255.0), (int)(FontOutlineColor.Blue * 255.0));
        style.Primary = System.Drawing.Color.FromArgb(255, (int)(FontTextColor.Red * 255.0), (int)(FontTextColor.Green * 255.0), (int)(FontTextColor.Blue * 255.0));
        style.OutlineWidth = SelectedFontOutline;
        style.ShadowWidth = style.OutlineWidth * 0.5m;

        if (FontAlignRight)
        {
            style.Alignment = "3";
        }

        if (SelectedFontBoxType.BoxType == FontBoxType.BoxPerLine)
        {
            style.BorderStyle = "4"; // box - multi line
            style.ShadowWidth = 0;
        }
        else
        {
            style.Outline = System.Drawing.Color.FromArgb(255, (int)(FontOutlineColor.Red * 255.0), (int)(FontOutlineColor.Green * 255.0), (int)(FontOutlineColor.Blue * 255.0));
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

        var fileName = Path.Combine(Path.GetDirectoryName(videoFileName)!, nameNoExt + "_burned-in" + ext);

        if (File.Exists(fileName))
        {
            fileName = Path.Combine(Path.GetDirectoryName(videoFileName)!, nameNoExt + "_burned-in_" + Guid.NewGuid() + ext);
        }

        return fileName;
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

    public void HelpTapped(object? sender, TappedEventArgs e)
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

    public void LabelHelpMouseEntered(object? sender, PointerEventArgs e)
    {
        LabelHelp.TextColor = (Color)Application.Current!.Resources[ThemeNames.LinkColor];
    }

    public void LabelHelpMouseExited(object? sender, PointerEventArgs e)
    {
        LabelHelp.TextColor = (Color)Application.Current!.Resources[ThemeNames.TextColor];
    }

    public static int CalculateFontSize(int videoWidth, int videoHeight, double factor, int minSize = 8, int maxSize = 2000)
    {
        if (factor is < 0 or > 5)
        {
            throw new ArgumentException("Factor must be between 0 and 1", nameof(factor));
        }

        // Calculate the diagonal resolution
        var diagonalResolution = Math.Sqrt(videoWidth * videoWidth + videoHeight * videoHeight);

        // Calculate base size (when factor is 0.5)
        var baseSize = diagonalResolution * 0.019; // 2% of diagonal as base size

        // Apply logarithmic scaling
        var scaleFactor = Math.Pow(maxSize / baseSize, 2 * (factor - 0.5));
        var fontSize = (int)Math.Round(baseSize * scaleFactor);

        // Clamp the font size between minSize and maxSize
        return Math.Clamp(fontSize, minSize, maxSize);
    }

    public void FontFactorChanged(object? sender, EventArgs e)
    {
        UpdateFontSizeLabel();
    }

    private void UpdateFontSizeLabel()
    {
        var fontSize = CalculateFontSize(VideoWidth, VideoHeight, SelectedFontFactor).ToString(CultureInfo.InvariantCulture);
        FontSizeText = $"Font size {fontSize} for {VideoWidth}x{VideoHeight}";
    }

    public void VideoWidthChanged(object? sender, TextChangedEventArgs e)
    {
        UpdateFontSizeLabel();
    }

    public void VideoHeightChanged(object? sender, TextChangedEventArgs e)
    {
        UpdateFontSizeLabel();
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
    }
}
