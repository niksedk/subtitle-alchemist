using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Controls.AudioVisualizerControl;
using SubtitleAlchemist.Features.Main;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Core.Primitives;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Features.Sync.AdjustAllTimes;

public partial class AdjustAllTimesPageModel : ObservableObject, IQueryAttributable
{
    public AdjustAllTimesPage? Page { get; set; }
    public CollectionView SubtitleList { get; internal set; } = new();
    public AudioVisualizer AudioVisualizer { get; internal set; } = new();
    public MediaElement VideoPlayer { get; set; } = new();

    [ObservableProperty]
    private bool _allLines;

    [ObservableProperty]
    private bool _selectedLinesOnly;

    [ObservableProperty]
    private bool _selectedAndSubsequentLines;

    [ObservableProperty]
    private TimeSpan _adjustTime;

    [ObservableProperty]
    private string _totalAdjustmentInfo = string.Empty;

    [ObservableProperty]
    private ObservableCollection<DisplayParagraph> _paragraphs = new();

    private long _totalAdjustmentMs;
    private readonly System.Timers.Timer _timer;
    private bool _stopping;
    private bool _firstPlay;
    private string _videoFileName;

    private readonly MediaElementState[] _allowUpdatePositionStates = new[]
{
        MediaElementState.Playing,
        MediaElementState.Paused,
        MediaElementState.Stopped,
    };

    public AdjustAllTimesPageModel()
    {
        _videoFileName = string.Empty;
        _timer = new System.Timers.Timer();
    }

    private void AudioVisualizer_OnVideoPositionChanged(object sender, AudioVisualizer.PositionEventArgs e)
    {
        var timeSpan = TimeSpan.FromSeconds(e.PositionInSeconds);
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (VideoPlayer != null && _allowUpdatePositionStates.Contains(VideoPlayer.CurrentState))
            {
                VideoPlayer.SeekTo(timeSpan);
                AudioVisualizer.InvalidateSurface();
            }
        });
    }

    private void AudioVisualizerOnPlayToggle(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_videoFileName) || VideoPlayer == null)
        {
            return;
        }

        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (VideoPlayer.CurrentState == MediaElementState.Playing)
            {
                VideoPlayer.Pause();
            }
            else
            {
                VideoPlayer.Play();
            }
        });
    }

    private void AudioVisualizerOnDoubleTapped(object sender, AudioVisualizer.PositionEventArgs e)
    {
        var timeSpan = TimeSpan.FromSeconds(e.PositionInSeconds);
        var ms = e.PositionInSeconds * 1000.0;
        var paragraph = Paragraphs.FirstOrDefault(p => p.Start.TotalMilliseconds <= ms && p.End.TotalMilliseconds >= ms);

        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (VideoPlayer != null && _allowUpdatePositionStates.Contains(VideoPlayer.CurrentState))
            {
                VideoPlayer.SeekTo(timeSpan);
                AudioVisualizer.InvalidateSurface();
            }

            //SelectParagraph(paragraph);
        });
    }

    private void _audioVisualizerOnSingleClick(object sender, ParagraphEventArgs e)
    {
        var timeSpan = TimeSpan.FromSeconds(e.Seconds);
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (VideoPlayer != null && _allowUpdatePositionStates.Contains(VideoPlayer.CurrentState))
            {
                VideoPlayer.SeekTo(timeSpan);
                AudioVisualizer.InvalidateSurface();
            }
        });
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (Page != null &&
            query["Subtitle"] is Subtitle subtitle &&
            query["VideoFileName"] is string videoFileName &&
            query["WavePeaks"] is WavePeakData wavePeakData)
        {
            Page.Initialize(subtitle, videoFileName, wavePeakData, this);
            _videoFileName = videoFileName;
            SetTimer();
            _timer.Start();

            AudioVisualizer.AllowMove = false;
            AudioVisualizer.AllowNewSelection = false;
            AudioVisualizer.OnVideoPositionChanged += AudioVisualizer_OnVideoPositionChanged;
            AudioVisualizer.OnSingleClick += _audioVisualizerOnSingleClick;
            AudioVisualizer.OnDoubleTapped += AudioVisualizerOnDoubleTapped;
            AudioVisualizer.OnPlayToggle += AudioVisualizerOnPlayToggle;

            AllLines = true;
            AdjustTime = TimeSpan.FromSeconds(Se.Settings.Synchronization.AdjustAllTimes.Seconds);
        }
    }

    public void SetTimer()
    {
        _timer.Elapsed += (_, _) =>
        {
            _timer.Stop();
            if (AudioVisualizer is { WavePeaks: { }, IsVisible: true }
                && VideoPlayer != null
                && _allowUpdatePositionStates.Contains(VideoPlayer.CurrentState))
            {
                var subtitle = new Subtitle();
                var selectedIndices = new List<int>();
                var orderedList = Paragraphs.OrderBy(p => p.Start.TotalMilliseconds).ToList();
                var firstSelectedIndex = -1;
                for (var i = 0; i < orderedList.Count; i++)
                {
                    var dp = orderedList[i];
                    var p = new Paragraph(dp.P, false);
                    p.Text = dp.Text;
                    p.StartTime.TotalMilliseconds = dp.Start.TotalMilliseconds;
                    p.EndTime.TotalMilliseconds = dp.End.TotalMilliseconds;
                    subtitle.Paragraphs.Add(p);

                    if (dp.IsSelected)
                    {
                        selectedIndices.Add(i);

                        if (firstSelectedIndex < 0)
                        {
                            firstSelectedIndex = i;
                        }
                    }
                }

                if (_stopping)
                {
                    return;
                }

                var mediaPlayerSeconds = VideoPlayer.Position.TotalSeconds;
                if (VideoPlayer.CurrentState == MediaElementState.Playing)
                {
                    // Hack to speed up waveform movement
                    if (_firstPlay && VideoPlayer.CurrentState == MediaElementState.Playing)
                    {
                        VideoPlayer.SetTimerInterval(25);
                        _firstPlay = false;
                    }

                    // var diff = DateTime.UtcNow.Ticks - _mediaPlayerStartAt;
                    // var seconds = TimeSpan.FromTicks(diff).TotalSeconds;
                    var startPos = mediaPlayerSeconds - 0.01;
                    if (startPos < 0)
                    {
                        startPos = 0;
                    }

                    if (mediaPlayerSeconds > AudioVisualizer.EndPositionSeconds || mediaPlayerSeconds < AudioVisualizer.StartPositionSeconds)
                    {
                        AudioVisualizer.SetPosition(startPos, subtitle, mediaPlayerSeconds, 0, selectedIndices.ToArray());
                    }
                    else
                    {
                        AudioVisualizer.SetPosition(AudioVisualizer.StartPositionSeconds, subtitle, mediaPlayerSeconds, firstSelectedIndex, selectedIndices.ToArray());
                    }
                }
                else
                {
                    if (mediaPlayerSeconds > AudioVisualizer.EndPositionSeconds || mediaPlayerSeconds < AudioVisualizer.StartPositionSeconds)
                    {
                        AudioVisualizer.SetPosition(mediaPlayerSeconds, subtitle, mediaPlayerSeconds, firstSelectedIndex, selectedIndices.ToArray());
                    }
                    else
                    {
                        AudioVisualizer.SetPosition(AudioVisualizer.StartPositionSeconds, subtitle, mediaPlayerSeconds, firstSelectedIndex, selectedIndices.ToArray());
                    }
                }
            }

            if (_stopping)
            {
                return;
            }

            try
            {
                AudioVisualizer.InvalidateSurface();
            }
            catch
            {
                // ignore
            }

            _timer.Start();
        };
    }

    [RelayCommand]
    private void ShowEarlier()
    {
        _totalAdjustmentMs -= (long)Math.Round(AdjustTime.TotalMilliseconds, MidpointRounding.AwayFromZero);
        ShowTotalAdjustment();

        SubtitleList.BatchBegin();
        foreach (var dp in Paragraphs)
        {
            dp.Start = TimeSpan.FromMilliseconds(dp.Start.TotalMilliseconds - AdjustTime.TotalMilliseconds);
            dp.End = TimeSpan.FromMilliseconds(dp.End.TotalMilliseconds - AdjustTime.TotalMilliseconds);
        }
        SubtitleList.BatchCommit();
    }

    [RelayCommand]
    private void ShowLater()
    {
        _totalAdjustmentMs += (long)Math.Round(AdjustTime.TotalMilliseconds, MidpointRounding.AwayFromZero);
        ShowTotalAdjustment();

        SubtitleList.BatchBegin();
        foreach (var dp in Paragraphs)
        {
            dp.Start = TimeSpan.FromMilliseconds(dp.Start.TotalMilliseconds + AdjustTime.TotalMilliseconds);
            dp.End = TimeSpan.FromMilliseconds(dp.End.TotalMilliseconds + AdjustTime.TotalMilliseconds);
        }
        SubtitleList.BatchCommit();
    }

    private void ShowTotalAdjustment()
    {
        var timeCode = new TimeCode(_totalAdjustmentMs);
        TotalAdjustmentInfo = $"Total adjustment: {timeCode.ToShortDisplayString()}";
    }

    [RelayCommand]
    public async Task Ok()
    {
        await Shell.Current.GoToAsync("..", new Dictionary<string, object>
        {
            { "Page", nameof(AdjustAllTimesPage) },
            { "Paragraphs", Paragraphs.ToList() },
            { "TotalAdjustmentMs", _totalAdjustmentMs },
        });
    }

    [RelayCommand]
    public async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }

    public void SubtitlesViewSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
    }

    internal void OnDisappearing()
    {
        _timer.Stop();
        _stopping = true;
        Se.SaveSettings();
        VideoPlayer.Handler?.DisconnectHandler();
        VideoPlayer.Dispose();
    }
}
