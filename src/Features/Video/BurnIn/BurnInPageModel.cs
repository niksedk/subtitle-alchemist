using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Controls.ColorPickerControl;

namespace SubtitleAlchemist.Features.Video.BurnIn;

public partial class BurnInPageModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private ObservableCollection<int> _fontSizes;

    [ObservableProperty]
    private int _selectedFontSize;

    [ObservableProperty]
    private bool _fontIsBold;

    [ObservableProperty]
    private ObservableCollection<decimal> _fontOutlines;

    [ObservableProperty]
    private decimal _selectedFontOutline;

    [ObservableProperty]
    private ObservableCollection<string> _fontFamilies;

    [ObservableProperty]
    private string _selectedFontFamily;

    [ObservableProperty]
    private ObservableCollection<string> _fontBoxTypes;

    [ObservableProperty]
    private string _selectedFontBoxType;

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

    public BurnInPage? Page { get; set; }
    public MediaElement VideoPlayer { get; set; }

    private Subtitle _subtitle = new();
    private readonly IPopupService _popupService;
    private bool _loading = true;

    public BurnInPageModel(IPopupService popupService)
    {
        _popupService = popupService;
        _selectedFontSize = 39;
        _selectedFontOutline = 2.0m;

        _fontTextColor = Colors.WhiteSmoke;

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
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query["Subtitle"] is Subtitle subtitle)
        {
            _subtitle = new Subtitle(subtitle, false);
        }


        //   Page?.Initialize(_subtitle, this);

        Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Load settings


                // set batch or single file mode


                _loading = false;
                VideoEncodingChanged(null, EventArgs.Empty);
            });
            return false;
        });
    }

    [RelayCommand]
    private async Task Ok()
    {

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
}
