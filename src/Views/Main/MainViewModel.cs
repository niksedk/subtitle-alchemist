using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.SubtitleFormats;
using SubtitleAlchemist.Controls;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Media;
using SubtitleAlchemist.Views.Help.About;
using SubtitleAlchemist.Views.LayoutPicker;
using SubtitleAlchemist.Views.Options.Settings;
using System.Collections;
using System.Text;
using SubtitleAlchemist.Views.Options.DownloadFfmpeg;
using SubtitleAlchemist.Views.Tools.AdjustDuration;
using SubtitleAlchemist.Views.Translate;
using SubtitleAlchemist.Views.Video.AudioToTextWhisper;

namespace SubtitleAlchemist.Views.Main
{
    public partial class MainViewModel : ObservableObject
    {
        public int SelectedLayout { get; set; }

        public static Window? Window { get; set; }

        public MainPage MainPage { get; set; }
        public MediaElement? VideoPlayer { get; set; }
        public AudioVisualizer AudioVisualizer { get; set; }
        public CollectionView SubtitleList { get; set; }
        public Grid ListViewAndEditBox { get; set; }
        public static IList SubtitleFormatNames => SubtitleFormat.AllSubtitleFormats.Select(p => p.Name).ToList();

        public Picker? SubtitleFormatPicker { get; internal set; }
        public Picker? EncodingPicker { get; internal set; }
        public static IList EncodingNames => EncodingHelper.GetEncodings().Select(p => p.DisplayName).ToList();

        [ObservableProperty]
        private string _selectedLineInfo;

        [ObservableProperty]
        private string _statusText;

        [ObservableProperty]
        private List<DisplayParagraph> _paragraphs;

        [ObservableProperty]
        private string _currentText;

        private DisplayParagraph? _currentParagraph;


        private Subtitle UpdatedSubtitle
        {
            get
            {
                var subtitle = new Subtitle(_subtitle);
                subtitle.Paragraphs.Clear();
                subtitle.Paragraphs.AddRange(Paragraphs.Select(p => p.P));
                return subtitle;
            }
        }

        private Subtitle _subtitle;

        private string _subtitleFileName;
        private string _videoFileName;
        private readonly System.Timers.Timer _timer;
        private bool _updating;


        private readonly IPopupService _popupService;

        public MainViewModel(IPopupService popupService)
        {
            _popupService = popupService;
            VideoPlayer = new MediaElement { BackgroundColor = Colors.Orange, ZIndex = -10000 };
            SubtitleList = new CollectionView();
            _timer = new System.Timers.Timer(19);
            SetTimer();

            if (AudioVisualizer != null)
            {
                AudioVisualizer.OnVideoPositionChanged += AudioVisualizer_OnVideoPositionChanged;
            }

            _statusText = string.Empty;
            _selectedLineInfo = string.Empty;
            _videoFileName = string.Empty;
            _subtitleFileName = string.Empty;
            _subtitle = new Subtitle();
            _paragraphs = new List<DisplayParagraph>();
            _currentText = string.Empty;
            AudioVisualizer = new AudioVisualizer();
            ListViewAndEditBox = new Grid();
        }

        private void SetTimer()
        {
            _timer.Elapsed += (_, _) =>
            {
                _timer.Stop();
                if (AudioVisualizer is { WavePeaks: { }, IsVisible: true }
                    && VideoPlayer != null
                    && _allowUpdatePositionStates.Contains(VideoPlayer.CurrentState))
                {
                    var mediaPlayerSeconds = VideoPlayer.Position.TotalSeconds;
                    if (VideoPlayer.CurrentState == MediaElementState.Playing)
                    {
                        // var diff = DateTime.UtcNow.Ticks - _mediaPlayerStartAt;
                        // var seconds = TimeSpan.FromTicks(diff).TotalSeconds;
                        var startPos = mediaPlayerSeconds - 0.01;
                        if (startPos < 0)
                        {
                            startPos = 0;
                        }

                        if (mediaPlayerSeconds > AudioVisualizer.EndPositionSeconds || mediaPlayerSeconds < AudioVisualizer.StartPositionSeconds)
                        {
                            AudioVisualizer?.SetPosition(startPos, _subtitle, mediaPlayerSeconds, 0, new[] { 0 });
                        }
                        else
                        {
                            AudioVisualizer?.SetPosition(AudioVisualizer.StartPositionSeconds, _subtitle, mediaPlayerSeconds, 0, new[] { 0 });
                        }
                    }
                    else
                    {
                        if (mediaPlayerSeconds > AudioVisualizer.EndPositionSeconds || mediaPlayerSeconds < AudioVisualizer.StartPositionSeconds)
                        {
                            AudioVisualizer?.SetPosition(mediaPlayerSeconds, _subtitle, mediaPlayerSeconds, 0, new[] { 0 });
                        }
                        else
                        {
                            AudioVisualizer?.SetPosition(AudioVisualizer.StartPositionSeconds, _subtitle, mediaPlayerSeconds, 0, new[] { 0 });
                        }
                    }

                    try
                    {
                        AudioVisualizer?.InvalidateSurface();
                    }
                    catch
                    {
                        // ignore
                        return;
                    }
                }

                _timer.Start();
            };

            _timer.Enabled = true;
        }

        private readonly MediaElementState[] _allowUpdatePositionStates = new[]
        {
            MediaElementState.Playing,
            MediaElementState.Paused,
            MediaElementState.Stopped,
        };

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

        public void CleanUp()
        {
            _timer.Stop();
            _timer.Dispose();
            AudioVisualizer.OnVideoPositionChanged -= AudioVisualizer_OnVideoPositionChanged;
            SharpHookHandler.Dispose();
        }

        public void Loaded(MainPage mainPage)
        {
            mainPage.Window.MinimumHeight = 400;
            mainPage.Window.MinimumWidth = 800;
        }

        [RelayCommand]
        public async Task ShowLayoutPicker()
        {
            var result = await _popupService.ShowPopupAsync<LayoutPickerModel>(onPresenting: viewModel => viewModel.SelectedLayout = SelectedLayout, CancellationToken.None);

            if (result is LayoutPickerPopupResult popupResult)
            {
                SelectedLayout = popupResult.SelectedLayout;
                ShowStatus($"Selected layout: {SelectedLayout + 1}");
                MainPage.MakeLayout(SelectedLayout);
            }
        }

        [RelayCommand]
        public async Task ShowAbout()
        {
            await _popupService.ShowPopupAsync<AboutModel>(CancellationToken.None);
        }

        [RelayCommand]
        public async Task ShowOptionsSettings()
        {
            await Shell.Current.GoToAsync(nameof(SettingsPage));
        }

        [RelayCommand]
        public async Task SubtitleOpen()
        {
            var fileHelper = new FileHelper();
            var subtitleFileName = await fileHelper.PickAndShowSubtitleFile("Open subtitle file");
            if (string.IsNullOrEmpty(subtitleFileName))
            {
                return;
            }

            _subtitle = Subtitle.Parse(subtitleFileName);
            Paragraphs = _subtitle.Paragraphs.Select(p => new DisplayParagraph(p)).ToList();
            _subtitleFileName = subtitleFileName;
            if (Window != null)
            {
                Window.Title = $"{subtitleFileName} - Subtitle Alchemist";
            }

            if (FindVideoFileName.TryFindVideoFileName(subtitleFileName, out var videoFileName))
            {
                VideoOpenFile(videoFileName);
            }

            _timer.Start();
        }

        [RelayCommand]
        public void SubtitleNew()
        {
            _timer.Stop();
            _subtitleFileName = string.Empty;
            _videoFileName = string.Empty;
            _subtitle = new Subtitle();
            Paragraphs = new List<DisplayParagraph>();
            _currentParagraph = null;
            CurrentText = string.Empty;
            if (VideoPlayer != null)
            {
                VideoPlayer.Source = null;
            }

            AudioVisualizer.WavePeaks = null;

            if (Window != null)
            {
                Window.Title = "Untitled - Subtitle Alchemist";
            }
        }

        [RelayCommand]
        private async Task SubtitleSaveAs()
        {
            if (Paragraphs.Count == 0)
            {
                ShowStatus("Nothing to save");
                return;
            }

            var fileHelper = new FileHelper();
            var subtitleFileName = await fileHelper.SaveSubtitleFileAs(
                "Save subtitle file as",
                _videoFileName,
                CurrentSubtitleFormat,
                _subtitle);

            if (!string.IsNullOrEmpty(subtitleFileName))
            {
                var text = UpdatedSubtitle.ToText(CurrentSubtitleFormat);
                await File.WriteAllTextAsync(subtitleFileName, text, CurrentEncoding); //TODO: BOM or not...

                _subtitleFileName = subtitleFileName;
                if (Window != null)
                {
                    Window.Title = $"{subtitleFileName} - Subtitle Alchemist";
                }
            }
        }

        [RelayCommand]
        private async Task SubtitleSave()
        {
            if (Paragraphs.Count == 0)
            {
                ShowStatus("Nothing to save");
                return;
            }

            if (string.IsNullOrEmpty(_subtitleFileName))
            {
                await SubtitleSaveAs();
                return;
            }

            var text = UpdatedSubtitle.ToText(CurrentSubtitleFormat);
            await File.WriteAllTextAsync(_subtitleFileName, text);
            ShowStatus("Saved: " + _subtitleFileName);
        }

        private void ShowStatus(string statusText)
        {
            StatusText = statusText;
        }

        [RelayCommand]
        private async Task VideoOpen()
        {
            var fileHelper = new FileHelper();
            var videoFileName = await fileHelper.PickAndShowVideoFile("Open video file");
            if (string.IsNullOrEmpty(videoFileName) || !File.Exists(videoFileName))
            {
                return;
            }

            VideoOpenFile(videoFileName);
        }

        private void VideoOpenFile(string videoFileName)
        {
            if (VideoPlayer == null)
            {
                return;
            }

            VideoPlayer.Source = MediaSource.FromFile(videoFileName);

            var peakWaveFileName = WavePeakGenerator.GetPeakWaveFileName(videoFileName);
            if (!File.Exists(peakWaveFileName))
            {
                var tempWaveFileName = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.wav");
                var process = WaveFileExtractor.GetCommandLineProcess(videoFileName, -1, tempWaveFileName, Configuration.Settings.General.VlcWaveTranscodeSettings, out _);
                process.Start();
                while (!process.HasExited)
                {

                }

                if (File.Exists(tempWaveFileName))
                {
                    using var waveFile = new WavePeakGenerator(tempWaveFileName);
                    waveFile.GeneratePeaks(0, peakWaveFileName);
                }

                var wavePeaks = WavePeakData.FromDisk(peakWaveFileName);
                AudioVisualizer.WavePeaks = wavePeaks;
                AudioVisualizer.InvalidateSurface();
            }
            else
            {
                var wavePeaks = WavePeakData.FromDisk(peakWaveFileName);
                AudioVisualizer.WavePeaks = wavePeaks;
            }

            _videoFileName = videoFileName;
        }

        [RelayCommand]
        private void VideoClose()
        {
            if (VideoPlayer != null)
            {
                VideoPlayer.Source = null;
            }

            AudioVisualizer.WavePeaks = null;
            _videoFileName = string.Empty;
        }

        public SubtitleFormat CurrentSubtitleFormat
        {
            get
            {
                return SubtitleFormat.AllSubtitleFormats
                    .First(p => p.Name == (SubtitleFormatPicker != null
                    ? SubtitleFormatPicker.SelectedItem.ToString()
                    : Configuration.Settings.General.DefaultSubtitleFormat));
            }
        }

        public Encoding CurrentEncoding
        {
            get
            {
                return EncodingHelper.GetEncodings()
                    .First(p => p.DisplayName == (EncodingPicker != null
                    ? EncodingPicker.SelectedItem.ToString()
                    : Configuration.Settings.General.DefaultEncoding)).Encoding;
            }
        }

        public TextEncoding CurrentTextEncoding
        {
            get
            {
                return EncodingHelper.GetEncodings()
                    .First(p => p.DisplayName == (EncodingPicker != null
                    ? EncodingPicker.SelectedItem.ToString()
                    : Configuration.Settings.General.DefaultEncoding));
            }
        }

        public void OnCollectionViewSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            _updating = true;
            var previous = e.PreviousSelection;
            foreach (var item in previous)
            {
                if (item is DisplayParagraph paragraph)
                {
                    paragraph.IsSelected = false;
                    paragraph.BackgroundColor = Colors.Black;
                }
            }

            var current = e.CurrentSelection;
            var paragraphs = new List<DisplayParagraph>();
            var first = true;
            foreach (var item in current)
            {
                if (item is DisplayParagraph paragraph)
                {
                    if (first)
                    {
                        CurrentText = paragraph.Text;
                        _currentParagraph = paragraph;
                        first = false;
                    }

                    paragraph.IsSelected = true;
                    paragraph.BackgroundColor = Colors.DarkGreen;
                    paragraphs.Add(paragraph);
                }
            }

            _updating = false;
            ShowStatus("Selecting " + current.Count + " paragraphs: " + string.Join(',', paragraphs.Select(p => p.Number)));
            //            SelectedLineInfo = $"Selected: {paragraph.Number} {paragraph.Start} {paragraph.End} {paragraph.Duration}";
        }

        [RelayCommand]
        private async Task VideoAudioToTextWhisper()
        {
            var ffmpegOk = await RequireFfmpegOk();
            if (!ffmpegOk)
            {
                return;
            }

            await Shell.Current.GoToAsync(nameof(AudioToTextWhisperPage));
        }

        [RelayCommand]
        private async Task AutoTranslateShow()
        {
            await Shell.Current.GoToAsync(nameof(TranslatePage));
        }

        [RelayCommand]
        private async Task AdjustDurationsShow()
        {
            await Shell.Current.GoToAsync(nameof(AdjustDurationPage));
        }

        private async Task<bool> RequireFfmpegOk()
        {
            if (Configuration.IsRunningOnWindows && (string.IsNullOrWhiteSpace(Configuration.Settings.General.FFmpegLocation) || !File.Exists(Configuration.Settings.General.FFmpegLocation)))
            {
                var answer = await MainPage.DisplayAlert(
                    "Download ffmpeg?",
                    $"{Environment.NewLine}\"Audio to text\" requires ffmpeg.{Environment.NewLine}{Environment.NewLine}Download and use ffmpeg?",
                    "Yes",
                    "No");

                if (!answer)
                {
                    return false;
                }

                var result = await _popupService.ShowPopupAsync<DownloadFfmpegModel>(CancellationToken.None);
            }

            return true;
        }

        public void CurrentTextChanged(object? sender, TextChangedEventArgs e)
        {
            if (_updating)
            {
                return;
            }

            if (_currentParagraph != null && e.NewTextValue != _currentParagraph.Text)
            {
                _currentParagraph.Text = e.NewTextValue;
                _currentParagraph.P.Text = e.NewTextValue;
            }
        }
    }
}

