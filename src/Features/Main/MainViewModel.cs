using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.SubtitleFormats;
using SharpHook;
using SubtitleAlchemist.Controls.AudioVisualizerControl;
using SubtitleAlchemist.Features.Help.About;
using SubtitleAlchemist.Features.LayoutPicker;
using SubtitleAlchemist.Features.Options.DownloadFfmpeg;
using SubtitleAlchemist.Features.Options.Settings;
using SubtitleAlchemist.Features.Tools.AdjustDuration;
using SubtitleAlchemist.Features.Translate;
using SubtitleAlchemist.Features.Video.AudioToTextWhisper;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Media;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using SubtitleAlchemist.Controls.SubTimeControl;
using System;

namespace SubtitleAlchemist.Features.Main
{
    public partial class MainViewModel : ObservableObject
    {
        public int SelectedLayout { get; set; }

        public List<MenuFlyoutItem> SubtitleListViewContextMenuItems { get; set; }
        public MenuFlyout SubtitleListViewContextMenu { get; set; }

        public static Window? Window { get; set; }

        public MainPage MainPage { get; set; }
        public MediaElement? VideoPlayer { get; set; }

        public AudioVisualizer AudioVisualizer => _audioVisualizer;
        private readonly AudioVisualizer _audioVisualizer;

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
        private ObservableCollection<DisplayParagraph> _paragraphs;

        [ObservableProperty]
        private string _currentText;

        [ObservableProperty]
        private string _currentStart;

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
        private bool _stopping;


        private readonly IPopupService _popupService;

        public MainViewModel(IPopupService popupService)
        {
            _popupService = popupService;
            VideoPlayer = new MediaElement { BackgroundColor = Colors.Orange, ZIndex = -10000 };
            SubtitleList = new CollectionView();
            _timer = new System.Timers.Timer(19);
            _statusText = string.Empty;
            _selectedLineInfo = string.Empty;
            _videoFileName = string.Empty;
            _subtitleFileName = string.Empty;
            _subtitle = new Subtitle();
            _paragraphs = new ObservableCollection<DisplayParagraph>();
            _currentText = string.Empty;
            _currentStart = "00.00.00,000";
            _audioVisualizer = new AudioVisualizer();
            ListViewAndEditBox = new Grid();

            _audioVisualizer.OnVideoPositionChanged += AudioVisualizer_OnVideoPositionChanged;
            _audioVisualizer.OnSingleClick += _audioVisualizer_OnSingleClick;
            _audioVisualizer.OnDoubleTapped += AudioVisualizer_OnDoubleTapped;
            _audioVisualizer.OnStatus += (sender, args) =>
            {
                ShowStatus(args.Paragraph.Text);
            };

            SetTimer();
        }

        private void SetTimer()
        {
            if (_stopping)
            {
                return;
            }

            _timer.Elapsed += (_, _) =>
            {
                _timer.Stop();
                if (_audioVisualizer is { WavePeaks: { }, IsVisible: true }
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

                        if (mediaPlayerSeconds > _audioVisualizer.EndPositionSeconds || mediaPlayerSeconds < _audioVisualizer.StartPositionSeconds)
                        {
                            _audioVisualizer.SetPosition(startPos, _subtitle, mediaPlayerSeconds, 0, GetSelectedIndexes());
                        }
                        else
                        {
                            _audioVisualizer.SetPosition(_audioVisualizer.StartPositionSeconds, _subtitle, mediaPlayerSeconds, GetFirstSelectedIndex(), GetSelectedIndexes());
                        }
                    }
                    else
                    {
                        if (mediaPlayerSeconds > _audioVisualizer.EndPositionSeconds || mediaPlayerSeconds < _audioVisualizer.StartPositionSeconds)
                        {
                            _audioVisualizer.SetPosition(mediaPlayerSeconds, _subtitle, mediaPlayerSeconds, GetFirstSelectedIndex(), GetSelectedIndexes());
                        }
                        else
                        {
                            _audioVisualizer.SetPosition(_audioVisualizer.StartPositionSeconds, _subtitle, mediaPlayerSeconds, GetFirstSelectedIndex(), GetSelectedIndexes());
                        }
                    }

                    try
                    {
                        _audioVisualizer.InvalidateSurface();
                    }
                    catch (Exception ex)
                    {
                        ShowStatus(ex.Message);
                    }
                }

                _timer.Start();
            };

            _timer.Enabled = true;
        }

        private int GetFirstSelectedIndex()
        {
            for (var index = 0; index < Paragraphs.Count; index++)
            {
                var displayParagraph = Paragraphs[index];
                if (displayParagraph.IsSelected)
                {
                    return index;
                }
            }

            return -1;
        }

        private int[] GetSelectedIndexes()
        {
            var list = new List<int>();
            for (var index = 0; index < Paragraphs.Count; index++)
            {
                var displayParagraph = Paragraphs[index];
                if (displayParagraph.IsSelected)
                {
                    list.Add(index);
                }
            }

            return list.ToArray();
        }

        private readonly MediaElementState[] _allowUpdatePositionStates = new[]
        {
            MediaElementState.Playing,
            MediaElementState.Paused,
            MediaElementState.Stopped,
        };

        private void _audioVisualizer_OnSingleClick(object sender, ParagraphEventArgs e)
        {
            var timeSpan = TimeSpan.FromSeconds(e.Seconds);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (VideoPlayer != null && _allowUpdatePositionStates.Contains(VideoPlayer.CurrentState))
                {
                    VideoPlayer.SeekTo(timeSpan);
                    _audioVisualizer.InvalidateSurface();
                }
            });
        }

        private void AudioVisualizer_OnVideoPositionChanged(object sender, AudioVisualizer.PositionEventArgs e)
        {
            var timeSpan = TimeSpan.FromSeconds(e.PositionInSeconds);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (VideoPlayer != null && _allowUpdatePositionStates.Contains(VideoPlayer.CurrentState))
                {
                    VideoPlayer.SeekTo(timeSpan);
                    _audioVisualizer.InvalidateSurface();
                }
            });
        }

        private void SelectParagraph(int index)
        {
            if (index < 0 || index >= Paragraphs.Count)
            {
                return;
            }

            var paragraph = Paragraphs[index];
            SelectParagraph(paragraph);
        }

        private void SelectParagraph(DisplayParagraph? paragraph)
        {
            if (paragraph == null)
            {
                return;
            }

            foreach (var item in Paragraphs)
            {
                item.IsSelected = false;
                item.BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"];
            }

            paragraph.IsSelected = true;
            paragraph.BackgroundColor = Colors.DarkGreen;
            _currentParagraph = paragraph;
            CurrentText = paragraph.Text;
            CurrentStart = paragraph.Start;
        }

        private void AudioVisualizer_OnDoubleTapped(object sender, AudioVisualizer.PositionEventArgs e)
        {
            var timeSpan = TimeSpan.FromSeconds(e.PositionInSeconds);
            var ms = e.PositionInSeconds * 1000.0;
            var paragraph = Paragraphs.FirstOrDefault(p => p.P.StartTime.TotalMilliseconds <= ms && p.P.EndTime.TotalMilliseconds >= ms);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (VideoPlayer != null && _allowUpdatePositionStates.Contains(VideoPlayer.CurrentState))
                {
                    VideoPlayer.SeekTo(timeSpan);
                    _audioVisualizer.InvalidateSurface();
                }

                SelectParagraph(paragraph);
            });
        }

        public void CleanUp()
        {
            _stopping = true;
            _timer.Stop();
            _audioVisualizer.OnVideoPositionChanged -= AudioVisualizer_OnVideoPositionChanged;
            //SharpHookHandler.Dispose();
            //_timer.Dispose();

            Configuration.Settings.Save();
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
            Paragraphs = _subtitle.Paragraphs.Select(p => new DisplayParagraph(p)).ToObservableCollection();
            _subtitleFileName = subtitleFileName;
            if (Window != null)
            {
                Window.Title = $"{subtitleFileName} - Subtitle Alchemist";
            }

            if (FindVideoFileName.TryFindVideoFileName(subtitleFileName, out var videoFileName))
            {
                VideoOpenFile(videoFileName);
            }

            if (!_stopping)
            {
                _timer.Start();
            }
        }

        [RelayCommand]
        public void SubtitleNew()
        {
            _timer.Stop();
            _subtitleFileName = string.Empty;
            _videoFileName = string.Empty;
            _subtitle = new Subtitle();
            Paragraphs = new ObservableCollection<DisplayParagraph>();
            _currentParagraph = null;
            CurrentText = string.Empty;
            CurrentStart = "00.00.00,000";
            if (VideoPlayer != null)
            {
                VideoPlayer.Source = null;
            }

            _audioVisualizer.WavePeaks = null;

            if (Window != null)
            {
                Window.Title = "Untitled - Subtitle Alchemist";
            }
        }

        [RelayCommand]
        private async Task SubtitleSaveAs()
        {
            if (!Paragraphs.Any())
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
            if (!Paragraphs.Any())
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

            _timer.Stop();
            _audioVisualizer.WavePeaks = null;
            VideoPlayer.Source = MediaSource.FromFile(videoFileName);

            var peakWaveFileName = WavePeakGenerator.GetPeakWaveFileName(videoFileName);
            if (!File.Exists(peakWaveFileName))
            {
                if (FfmpegHelper.IsFfmpegInstalled())
                {
                    var tempWaveFileName = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.wav");
                    var process = WaveFileExtractor.GetCommandLineProcess(videoFileName, -1, tempWaveFileName, Configuration.Settings.General.VlcWaveTranscodeSettings, out _);
                    ShowStatus("Extracting wave info...");
                    Task.Run(async () =>
                    {
                        await ExtractWaveformAndSpectrogram(process, tempWaveFileName, peakWaveFileName);
                    });
                }
            }
            else
            {
                var wavePeaks = WavePeakData.FromDisk(peakWaveFileName);
                _audioVisualizer.WavePeaks = wavePeaks;
            }

            _videoFileName = videoFileName;

            if (!_stopping)
            {
                _timer.Start();
            }
        }

        private async Task ExtractWaveformAndSpectrogram(Process process, string tempWaveFileName, string peakWaveFileName)
        {
            process.Start();
            var token = new CancellationTokenSource().Token;
            while (!process.HasExited)
            {
                await Task.Delay(100, token);
            }

            if (process.ExitCode != 0)
            {
                ShowStatus("Failed to extract wave info.");
                return;
            }

            if (File.Exists(tempWaveFileName))
            {
                using var waveFile = new WavePeakGenerator(tempWaveFileName);
                waveFile.GeneratePeaks(0, peakWaveFileName);

                var wavePeaks = WavePeakData.FromDisk(peakWaveFileName);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _audioVisualizer.WavePeaks = wavePeaks;

                    if (!_stopping)
                    {
                        _timer.Start();
                        _audioVisualizer.InvalidateSurface();
                        ShowStatus("Wave info loaded.");
                    }
                });
            }
        }

        [RelayCommand]
        private void VideoClose()
        {
            if (VideoPlayer != null)
            {
                VideoPlayer.Source = null;
            }

            _audioVisualizer.WavePeaks = null;
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
            foreach (var item in Paragraphs)
            {
                item.IsSelected = false;
                item.BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"];
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
                        CurrentStart = paragraph.Start;
                        _currentParagraph = paragraph;
                        first = false;
                    }

                    paragraph.IsSelected = true;
                    paragraph.BackgroundColor = Colors.DarkGreen;
                    paragraphs.Add(paragraph);
                }
            }

            _updating = false;
            //ShowStatus("Selecting " + current.Count + " paragraphs: " + string.Join(',', paragraphs.Select(p => p.Number)));

            const int contextMenuDeleteOneLine = 0;
            const int contextMenuInsertBefore = 1;
            const int contextMenuInsertAfter = 2;
            const int contextMenuItalic = 4;
            if (current.Count == 1)
            {
                SubtitleListViewContextMenuItems[contextMenuDeleteOneLine].Text = "Delete one line";
            }
            else if (current.Count > 1)
            {
                SubtitleListViewContextMenuItems[contextMenuDeleteOneLine].Text = "Delete selected lines";
            }
            SubtitleListViewContextMenuItems[contextMenuDeleteOneLine].IsEnabled = current.Count == 1;
            SubtitleListViewContextMenuItems[contextMenuInsertBefore].IsEnabled = current.Count == 1;
            SubtitleListViewContextMenuItems[contextMenuInsertAfter].IsEnabled = current.Count == 1;
            SubtitleListViewContextMenuItems[contextMenuItalic].IsEnabled = current.Count == 1;
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
            if (FfmpegHelper.IsFfmpegInstalled())
            {
                return true;
            }

            if (Configuration.IsRunningOnMac && File.Exists("/usr/local/bin/ffmpeg"))
            {
                Configuration.Settings.General.FFmpegLocation = "/usr/local/bin/ffmpeg";
                return true;
            }

            if (Configuration.IsRunningOnWindows)
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

            return false;
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

        public void CurrentStartChanged(object? sender, ValueChangedEventArgs e)
        {
            if (_updating)
            {
                return;
            }

            var startText = SubTimeUpDown.ToDisplayText(TimeSpan.FromMilliseconds(e.NewValue), false);
            if (_currentParagraph != null && startText != _currentParagraph.Start)
            {
                _currentParagraph.P.StartTime = new TimeCode(e.NewValue);
                CurrentStart = startText;
            }
        }


        [RelayCommand]
        private async Task DeleteSelectedLines()
        {
            var selectedParagraphs = Paragraphs.Where(p => p.IsSelected).ToList();
            var text = selectedParagraphs.Count > 1
                ? $"Delete {selectedParagraphs.Count} lines?"
                : $"Delete line {selectedParagraphs.First().Number}?";
            var title = selectedParagraphs.Count > 1
                ? "Delete selected lines"
                : "Delete one line";

            var answer = await MainPage.DisplayAlert(
                title,
                $"{Environment.NewLine}{text}",
                "Yes",
                "No");
            if (!answer)
            {
                return;
            }

            SubtitleList.BatchBegin();
            var firstIdx = -1;
            foreach (var displayParagraph in selectedParagraphs)
            {
                if (firstIdx < 0)
                {
                    firstIdx = Paragraphs.IndexOf(displayParagraph);
                }

                Paragraphs.Remove(displayParagraph);
            }

            if (firstIdx > Paragraphs.Count)
            {
                firstIdx = Paragraphs.Count - 1;
            }

            if (firstIdx >= 0)
            {
                Paragraphs[firstIdx].IsSelected = true;
                Paragraphs[firstIdx].BackgroundColor = Colors.DarkGreen;

                _currentParagraph = Paragraphs[firstIdx];
            }

            Renumber();

            SubtitleList.BatchCommit();

            if (firstIdx >= 0)
            {
                SubtitleList.ScrollTo(firstIdx, 1, ScrollToPosition.Center, false);
                SubtitleList.ScrollTo(firstIdx, 1, ScrollToPosition.Center, false);
                SubtitleList.ScrollTo(firstIdx, 1, ScrollToPosition.Center, false);
                SubtitleList.ScrollTo(firstIdx, 1, ScrollToPosition.Center, false);

                SubtitleList.SelectedItem = Paragraphs[firstIdx];
            }
        }

        private void Renumber()
        {
            for (var index = 0; index < Paragraphs.Count; index++)
            {
                var displayParagraph = Paragraphs[index];
                displayParagraph.Number = index + 1;
            }
        }

        [RelayCommand]
        private void InsertBefore()
        {
            if (_currentParagraph == null)
            {
                return;
            }

            var p = Paragraphs.FirstOrDefault(p => p.P == _currentParagraph.P);
            if (p == null)
            {
                return;
            }

            var idx = Paragraphs.IndexOf(p);

            SubtitleList.BatchBegin();
            var newParagraph = new DisplayParagraph(new Paragraph());
            Paragraphs.Insert(idx, newParagraph);
            SelectParagraph(newParagraph);

            Renumber();

            _currentParagraph = newParagraph;

            SubtitleList.BatchCommit();

            SubtitleList.ScrollTo(idx, 1, ScrollToPosition.Center);
            SubtitleList.ScrollTo(idx, 1, ScrollToPosition.Center);
            SubtitleList.ScrollTo(idx, 1, ScrollToPosition.Center);
            SubtitleList.ScrollTo(idx, 1, ScrollToPosition.Center);

            SubtitleList.SelectedItem = _currentParagraph;
        }

        [RelayCommand]
        private void InsertAfter()
        {
            if (_currentParagraph == null)
            {
                return;
            }

            var p = Paragraphs.FirstOrDefault(p => p.P == _currentParagraph.P);
            if (p == null)
            {
                return;
            }

            var idx = Paragraphs.IndexOf(p);

            SubtitleList.BatchBegin();
            var newParagraph = new DisplayParagraph(new Paragraph());
            Paragraphs.Insert(idx + 1, newParagraph);
            SelectParagraph(newParagraph);
            Renumber();

            SubtitleList.BatchCommit();

            _currentParagraph = newParagraph;

            SubtitleList.ScrollTo(idx + 1, 1, ScrollToPosition.Center);
            SubtitleList.ScrollTo(idx + 1, 1, ScrollToPosition.Center);
            SubtitleList.ScrollTo(idx + 1, 1, ScrollToPosition.Center);
            SubtitleList.ScrollTo(idx + 1, 1, ScrollToPosition.Center);

            SubtitleList.SelectedItem = _currentParagraph;
        }

        [RelayCommand]
        private void Italic()
        {
            var tag = "i";
            var isAssa = false;

            ToggleTag(tag, isAssa);
        }

        private void ToggleTag(string tag, bool isAssa)
        {
            var first = true;
            var toggleOn = true;

            SubtitleList.BatchBegin();

            foreach (var displayParagraph in Paragraphs)
            {
                if (displayParagraph.IsSelected)
                {
                    if (first)
                    {
                        _currentParagraph = displayParagraph;
                        toggleOn = !HtmlUtil.IsTagOn(displayParagraph.Text, tag, true, isAssa);
                        first = false;
                    }

                    if (toggleOn)
                    {
                        displayParagraph.Text = HtmlUtil.TagOn(displayParagraph.Text, tag, true, isAssa);
                    }
                    else
                    {
                        displayParagraph.Text = HtmlUtil.TagOff(displayParagraph.Text, tag, true, isAssa);
                    }

                    if (_currentParagraph == displayParagraph)
                    {
                        CurrentText = displayParagraph.Text;
                    }
                }
            }

            SubtitleList.BatchCommit();
        }

        public void KeyPressed(object? sender, KeyboardHookEventArgs e)
        {
            ShowStatus(e.Data.ToString());
        }

        public void ListViewDoubleTapped(object? sender, TappedEventArgs e)
        {
            var point = e.GetPosition(sender as Element);
            if (point.HasValue)
            {
                if (e.Parameter is DisplayParagraph paragraph)
                {
                    // ShowStatus("Double tab at " + (int)point.Value.X + "," + (int)point.Value.Y + "  " + paragraph.Number + ": " + paragraph.Text.Replace(Environment.NewLine, "<br />"));
                    // TODO: make customizable
                    if (VideoPlayer is { IsLoaded: true })
                    {
                        VideoPlayer.SeekTo(TimeSpan.FromSeconds(paragraph.P.StartTime.TotalSeconds));
                    }
                }

            }
        }
    }
}

