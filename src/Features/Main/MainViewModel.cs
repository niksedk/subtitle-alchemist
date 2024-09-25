﻿using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.SubtitleFormats;
using SharpHook;
using SubtitleAlchemist.Controls.AudioVisualizerControl;
using SubtitleAlchemist.Features.Edit.RedoUndoHistory;
using SubtitleAlchemist.Features.Files;
using SubtitleAlchemist.Features.Files.ExportBinary.Cavena890Export;
using SubtitleAlchemist.Features.Files.ExportBinary.EbuExport;
using SubtitleAlchemist.Features.Files.ExportBinary.PacExport;
using SubtitleAlchemist.Features.Help.About;
using SubtitleAlchemist.Features.LayoutPicker;
using SubtitleAlchemist.Features.Options.DownloadFfmpeg;
using SubtitleAlchemist.Features.Options.Settings;
using SubtitleAlchemist.Features.SpellCheck;
using SubtitleAlchemist.Features.Tools.AdjustDuration;
using SubtitleAlchemist.Features.Tools.FixCommonErrors;
using SubtitleAlchemist.Features.Translate;
using SubtitleAlchemist.Features.Video.AudioToTextWhisper;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Logic.Dictionaries;
using SubtitleAlchemist.Logic.Media;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Timers;
using SubtitleAlchemist.Features.Edit.Find;
using GoToLineNumberPopupModel = SubtitleAlchemist.Features.Edit.GoToLineNumber.GoToLineNumberPopupModel;
using Path = System.IO.Path;

namespace SubtitleAlchemist.Features.Main;

public partial class MainViewModel : ObservableObject, IQueryAttributable
{
    public int SelectedLayout { get; set; }

    public List<MenuFlyoutItem> SubtitleListViewContextMenuItems { get; set; } = new();
    public MenuFlyout SubtitleListViewContextMenu { get; set; } = new();

    public static Window? Window { get; set; }

    public MainPage? MainPage { get; set; }
    public MediaElement? VideoPlayer { get; set; }

    public AudioVisualizer AudioVisualizer => _audioVisualizer;
    private readonly AudioVisualizer _audioVisualizer;

    public CollectionView SubtitleList { get; set; }
    public Grid ListViewAndEditBox { get; set; }
    public static IList SubtitleFormatNames => SubtitleFormat.AllSubtitleFormats.Select(p => p.Name).ToList();
    public Editor TextBox { get; set; } = new();
    public Picker? SubtitleFormatPicker { get; internal set; }
    public Picker? EncodingPicker { get; internal set; }
    public static IList EncodingNames => EncodingHelper.GetEncodings().Select(p => p.DisplayName).ToList();
    public MenuFlyoutSubItem MenuFlyoutItemReopen { get; set; } = new();

    [ObservableProperty]
    private string _selectedLineInfo;

    [ObservableProperty]
    private string _statusText;

    [ObservableProperty]
    private ObservableCollection<DisplayParagraph> _paragraphs;

    [ObservableProperty]
    private string _currentText;

    [ObservableProperty]
    private TimeSpan _currentStart;

    [ObservableProperty]
    private TimeSpan _currentDuration;

    [ObservableProperty]
    private TimeSpan _currentEnd;

    private DisplayParagraph? _currentParagraph;

    private bool _loading = true;

    private Subtitle _subtitle;

    private string _subtitleFileName;
    private string _videoFileName;
    private readonly System.Timers.Timer _timer;
    private readonly System.Timers.Timer _timerAutoBackup;
    private bool _updating;
    private bool _stopping;
    private bool _firstPlay = true;
    private int _changeSubtitleHash = -1;
    private int _autoBackupSubtitleHash = -1;

    private readonly IPopupService _popupService;
    private readonly IAutoBackup _autoBackup;
    private readonly IUndoRedoManager _undoRedoManager;
    private readonly IFindService _findService;

    public MainViewModel(IPopupService popupService, IAutoBackup autoBackup, IUndoRedoManager undoRedoManager, IFindService findService)
    {
        _popupService = popupService;
        _autoBackup = autoBackup;
        _undoRedoManager = undoRedoManager;
        _findService = findService;
        VideoPlayer = new MediaElement { BackgroundColor = Colors.Orange, ZIndex = -10000 };
        SubtitleList = new CollectionView();
        _timer = new System.Timers.Timer(19);
        _timerAutoBackup = new System.Timers.Timer(60_0 * 5); //TODO: use settings
        _statusText = string.Empty;
        _selectedLineInfo = string.Empty;
        _videoFileName = string.Empty;
        _subtitleFileName = string.Empty;
        _subtitle = new Subtitle();
        _paragraphs = new ObservableCollection<DisplayParagraph>();
        _currentText = string.Empty;
        _currentStart = new TimeSpan();
        _currentEnd = new TimeSpan();
        _currentDuration = new TimeSpan();
        _audioVisualizer = new AudioVisualizer { Margin = 10 };
        ListViewAndEditBox = new Grid();

        _audioVisualizer.OnVideoPositionChanged += AudioVisualizer_OnVideoPositionChanged;
        _audioVisualizer.OnSingleClick += _audioVisualizerOnSingleClick;
        _audioVisualizer.OnDoubleTapped += AudioVisualizerOnDoubleTapped;
        _audioVisualizer.OnTimeChanged += OnAudioVisualizerOnOnTimeChanged;
        _audioVisualizer.OnNewSelectionInsert += AudioVisualizerOnNewSelectionInsert;
        _audioVisualizer.OnPlayToggle += AudioVisualizerOnPlayToggle;
        _audioVisualizer.SetContextMenu(MakeAudioVisualizerContextMenu());
        _audioVisualizer.OnStatus += (sender, args) =>
        {
            ShowStatus(args.Paragraph.Text);
        };

        SetTimer();
        _timerAutoBackup.Elapsed += AutoBackupTimerOnElapsed;
    }

    private Subtitle UpdatedSubtitle
    {
        get
        {
            var subtitle = new Subtitle(_subtitle);
            subtitle.Paragraphs.Clear();

            foreach (var displayParagraph in Paragraphs)
            {
                var p = new Paragraph(displayParagraph.P, false);
                p.Text = displayParagraph.Text;
                p.StartTime.TotalMilliseconds = displayParagraph.Start.TotalMilliseconds;
                p.EndTime.TotalMilliseconds = displayParagraph.End.TotalMilliseconds;
                subtitle.Paragraphs.Add(displayParagraph.P);
            }

            return subtitle;
        }
    }

    private readonly object _autoBackupLock = new();
    private void AutoBackupTimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        lock (_autoBackupLock)
        {
            var hash = GetFastSubtitleHash();
            if (_changeSubtitleHash == hash || Paragraphs.Count == 0 || hash == _autoBackupSubtitleHash || _loading)
            {
                return;
            }

            _autoBackupSubtitleHash = hash;
            _autoBackup.SaveAutoBackup(_subtitle, CurrentSubtitleFormat);
        }
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.ContainsKey("Page") || query["Page"] is not string page)
        {
            return;
        }

        if (page == nameof(RestoreAutoBackupPage))
        {
            if (query["SubtitleFileName"] is string subtitleFileName && !string.IsNullOrWhiteSpace(subtitleFileName))
            {
                MakeHistoryForUndo($"Before restore auto-backup");
                _subtitleFileName = subtitleFileName;
                SubtitleOpen(subtitleFileName, null);
            }
        }

        if (page == nameof(TranslatePage))
        {
            if (query["TranslatedRows"] is List<TranslateRow> lines)
            {
                MakeHistoryForUndo($"Before translate");
                for (var i = 0; i < lines.Count && i < Paragraphs.Count; i++)
                {
                    var displayParagraph = Paragraphs[i];
                    displayParagraph.Text = lines[i].TranslatedText;
                }
            }
        }

        if (page == nameof(AudioToTextWhisperPage))
        {
            MakeHistoryForUndo($"Before Whisper speech recognization");
            if (query["TranscribedSubtitle"] is Subtitle subtitle)
            {
                Paragraphs = new ObservableCollection<DisplayParagraph>(subtitle.Paragraphs.Select(p => new DisplayParagraph(p)));
                ShowStatus($"{Paragraphs.Count} lines generated from Whisper");
                SelectParagraph(0);
            }
        }

        if (page == nameof(FixCommonErrorsPage))
        {
            MakeHistoryForUndo($"Before Fix common errors");

            var totalFixes = 0;
            if (query["TotalFixes"] is int totalFixesNumber)
            {
                totalFixes = totalFixesNumber;
            }

            if (query["Subtitle"] is Subtitle subtitle)
            {
                Paragraphs = new ObservableCollection<DisplayParagraph>(subtitle.Paragraphs.Select(p => new DisplayParagraph(p)));
                ShowStatus($"{totalFixes} fixes applied");
                SelectParagraph(0);
            }
        }

        if (page == nameof(ExportEbuPage))
        {
            Ebu.EbuUiHelper = new EbuHelper();

            var useCurrentSubtitleFileName = false;
            if (query["UseSubtitleFileName"] is bool useSubtitleFileName)
            {
                useCurrentSubtitleFileName = useSubtitleFileName;
            }

            if (query["Subtitle"] is Subtitle subtitle)
            {
                var format = new Ebu();
                using var ms = new MemoryStream();
                _subtitle.Header = subtitle.Header;
                format.Save(_subtitleFileName, ms, UpdatedSubtitle, false);

                if (!useCurrentSubtitleFileName || string.IsNullOrEmpty(_subtitleFileName))
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        var fileHelper = new FileHelper();
                        var subtitleFileName = await fileHelper.SaveStreamAs(ms, $"Save {format.Name} file as", _videoFileName, format);
                        if (!string.IsNullOrEmpty(subtitleFileName))
                        {
                            ShowStatus($"File exported in format {format.Name} to {subtitleFileName}");
                        }
                    });

                    return;
                }

                File.WriteAllBytes(_subtitleFileName, ms.ToArray());
            }

            if (page == nameof(AdjustDurationPage))
            {
                MakeHistoryForUndo($"Before adjust durations");

                if (query["Subtitle"] is Subtitle adjustedSubtitle)
                {
                    Paragraphs = new ObservableCollection<DisplayParagraph>(adjustedSubtitle.Paragraphs.Select(p => new DisplayParagraph(p)));
                    SelectParagraph(0);
                }

                if (query["Status"] is string statusInfo)
                {
                    ShowStatus(statusInfo);
                }
            }
        }
    }

    private void MakeHistoryForUndo(string description)
    {
        if (Paragraphs.Count == 0 || Paragraphs.Count == 1 && string.IsNullOrWhiteSpace(CurrentText))
        { 
            return;
        }

        var hash = GetFastSubtitleHash();
        if (hash == _changeSubtitleHash && _undoRedoManager.CanUndo)
        {
            return; // no changes
        }

        var undoRedoObject = MakeUndoRedoObject(description);
        _undoRedoManager.Do(undoRedoObject);
        _changeSubtitleHash = GetFastSubtitleHash();
    }

    private UndoRedoItem? MakeUndoRedoObject(string description)
    {
        return new UndoRedoItem(
            description, 
            UpdatedSubtitle, 
            _subtitleFileName, 
            GetSelectedIndexes(), 
            TextBox.CursorPosition, 
            TextBox.SelectionLength);
    }

    [RelayCommand]
    public void Undo()
    {
        if (!_undoRedoManager.CanUndo)
        {
            return;
        }

        var undoRedoObject = _undoRedoManager.Undo()!;
        RestoreUndoRedoState(undoRedoObject);
        ShowStatus("Undo performed");
    }

    [RelayCommand]
    public void Redo()
    {
        if (!_undoRedoManager.CanRedo)
        {
            return;
        }

        var undoRedoObject = _undoRedoManager.Redo()!;
        RestoreUndoRedoState(undoRedoObject);
        ShowStatus("Redo performed");
    }

    [RelayCommand]
    public async Task HistoryShow()
    {
        await Shell.Current.GoToAsync(nameof(UndoRedoHistoryPage), new Dictionary<string, object>
        {
            { "Page", nameof(MainPage) },
            { "UndoRedoManager", _undoRedoManager },
        });
    }

    private void RestoreUndoRedoState(UndoRedoItem undoRedoObject)
    {
        Paragraphs = new ObservableCollection<DisplayParagraph>(undoRedoObject.Subtitle.Paragraphs.Select(p => new DisplayParagraph(p)));
        _subtitleFileName = undoRedoObject.SubtitleFileName;
        SetTitle(_subtitleFileName);
        SelectParagraph(undoRedoObject.SelectedLines.First());
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

    private void AudioVisualizerOnNewSelectionInsert(object sender, ParagraphEventArgs e)
    {
        var newDp = new DisplayParagraph(e.Paragraph);

        MainThread.BeginInvokeOnMainThread(() =>
        {
            foreach (var dp in Paragraphs)
            {
                if (dp.P.StartTime.TotalMilliseconds >= e.Paragraph.StartTime.TotalMilliseconds)
                {
                    Paragraphs.Insert(Paragraphs.IndexOf(dp), newDp);
                    SelectParagraph(newDp);
                    TextBox.Focus();
                    return;
                }
            }

            Paragraphs.Insert(0, newDp);
            SelectParagraph(newDp);
            TextBox.Focus();
        });
    }

    public MenuFlyout MakeAudioVisualizerContextMenu()
    {
        var imagePath = Path.Combine("Resources", "Images", "Menu");

        var menuFlyout = new MenuFlyout();
        var items = new List<MenuFlyoutItem>
        {
            new MenuFlyoutItem
            {
                Text = "Delete x lines?",
                Command = DeleteSelectedLinesCommand,
                IconImageSource = ImageSource.FromFile(Path.Combine(imagePath,"Delete.png")),
                IsEnabled = false,
            },
            new MenuFlyoutItem
            {
                Text = "Insert line before",
                Command = InsertBeforeCommand,
                IconImageSource = ImageSource.FromFile(Path.Combine(imagePath, "Add.png")),
                IsEnabled = false,
            },
            new MenuFlyoutItem
            {
                Text = "Insert line after",
                Command = InsertAfterCommand,
                IconImageSource = ImageSource.FromFile(Path.Combine(imagePath, "Add.png")),
                IsEnabled = false,
            },
            new MenuFlyoutSeparator(),
            new MenuFlyoutItem
            {
                Text = "Italic",
                Command = ItalicCommand, KeyboardAccelerators =
                {
                    new KeyboardAccelerator
                    {
                        Modifiers = KeyboardAcceleratorModifiers.Ctrl,
                        Key = "I",
                    }
                },
                IconImageSource = ImageSource.FromFile(Path.Combine(imagePath, "Italic.png")),
                IsEnabled = false,
            },
        };

        foreach (var item in items)
        {
            menuFlyout.Add(item);
        }

        return menuFlyout;
    }

    private void OnAudioVisualizerOnOnTimeChanged(object sender, ParagraphEventArgs e)
    {
        var dp = Paragraphs.FirstOrDefault(p => p.P.Id == e.Paragraph.Id);
        if (dp == null)
        {
            return;
        }

        if (e.MouseDownParagraphType == MouseDownParagraphType.Start)
        {
            dp.Start = TimeSpan.FromMilliseconds(e.Paragraph.StartTime.TotalMilliseconds);
            dp.Duration = dp.End - dp.Start;
            if (dp == _currentParagraph)
            {
                CurrentStart = dp.Start;
                CurrentDuration = dp.Duration;
            }
        }
        else if (e.MouseDownParagraphType == MouseDownParagraphType.End)
        {
            dp.End = TimeSpan.FromMilliseconds(e.Paragraph.EndTime.TotalMilliseconds);
            dp.Duration = dp.End - dp.Start;

            if (dp == _currentParagraph)
            {
                CurrentDuration = dp.Duration;
            }
        }
        else if (e.MouseDownParagraphType == MouseDownParagraphType.Whole)
        {
            dp.Start = TimeSpan.FromMilliseconds(e.Paragraph.StartTime.TotalMilliseconds);
            dp.End = TimeSpan.FromMilliseconds(e.Paragraph.EndTime.TotalMilliseconds);
            dp.Duration = dp.End - dp.Start;

            if (dp == _currentParagraph)
            {
                CurrentStart = dp.Start;
                CurrentDuration = dp.Duration;
            }
        }
    }

    public void StopTimer()
    {
        _stopping = true;
        _timer.Stop();

        // Delay 100 ms
        Task.Delay(100).Wait();
    }

    public void SetTimer()
    {
        _timer.Elapsed += (_, _) =>
        {
            _timer.Stop();
            if (_audioVisualizer is { WavePeaks: { }, IsVisible: true }
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

                    if (mediaPlayerSeconds > _audioVisualizer.EndPositionSeconds || mediaPlayerSeconds < _audioVisualizer.StartPositionSeconds)
                    {
                        _audioVisualizer.SetPosition(startPos, subtitle, mediaPlayerSeconds, 0, selectedIndices.ToArray());
                    }
                    else
                    {
                        _audioVisualizer.SetPosition(_audioVisualizer.StartPositionSeconds, subtitle, mediaPlayerSeconds, firstSelectedIndex, selectedIndices.ToArray());
                    }
                }
                else
                {
                    if (mediaPlayerSeconds > _audioVisualizer.EndPositionSeconds || mediaPlayerSeconds < _audioVisualizer.StartPositionSeconds)
                    {
                        _audioVisualizer.SetPosition(mediaPlayerSeconds, subtitle, mediaPlayerSeconds, firstSelectedIndex, selectedIndices.ToArray());
                    }
                    else
                    {
                        _audioVisualizer.SetPosition(_audioVisualizer.StartPositionSeconds, subtitle, mediaPlayerSeconds, firstSelectedIndex, selectedIndices.ToArray());
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

    private void _audioVisualizerOnSingleClick(object sender, ParagraphEventArgs e)
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
        }

        paragraph.IsSelected = true;
        _currentParagraph = paragraph;
        CurrentText = paragraph.Text;
        CurrentStart = paragraph.Start;
        CurrentEnd = paragraph.End;
        CurrentDuration = paragraph.End - paragraph.Start;

        SubtitleList.SelectedItem = paragraph;
        SubtitleList.ScrollTo(paragraph, -1, ScrollToPosition.MakeVisible, false);
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
                _audioVisualizer.InvalidateSurface();
            }

            SelectParagraph(paragraph);
        });
    }

    public void Stop()
    {
        _stopping = true;
        _timer.Stop();
        _audioVisualizer.OnVideoPositionChanged -= AudioVisualizer_OnVideoPositionChanged;
        SharpHookHandler.Clear();
        Se.Settings.File.AddToRecentFiles(_subtitleFileName, _videoFileName, GetFirstSelectedIndex(), CurrentTextEncoding.DisplayName);
        Se.SaveSettings();
        SharpHookHandler.Dispose();
    }

    public void Start()
    {
        _stopping = false;
        _timer.Start();
        _audioVisualizer.OnVideoPositionChanged += AudioVisualizer_OnVideoPositionChanged;
        SharpHookHandler.AddKeyPressed(KeyPressed);
    }

    public void Loaded(MainPage mainPage)
    {
        if (!_loading)
        {
            return;
        }

        mainPage.Window.MinimumHeight = 600;
        mainPage.Window.MinimumWidth = 900;

        if (Se.Settings.File.ShowRecentFiles)
        {
            var first = Se.Settings.File.RecentFiles.FirstOrDefault();
            if (first != null && File.Exists(first.SubtitleFileName))
            {
                ReopenSubtitle(first.SubtitleFileName);
            }
        }

        _loading = false;
    }

    [RelayCommand]
    public async Task ShowLayoutPicker()
    {
        var result = await _popupService.ShowPopupAsync<LayoutPickerModel>(onPresenting: viewModel => viewModel.SelectedLayout = SelectedLayout, CancellationToken.None);

        if (result is LayoutPickerPopupResult popupResult && MainPage != null)
        {
            SelectedLayout = popupResult.SelectedLayout;
            Se.Settings.General.LayoutNumber = SelectedLayout;
            ShowStatus($"Selected layout: {SelectedLayout + 1}");
            MainPage.MakeLayout(SelectedLayout);
            Se.SaveSettings();
        }
    }

    [RelayCommand]
    public void ExportCapMakerPlus()
    {
        //TODO: wait for libse 4.0.9
        //var format = new CapMakerPlus();
        //using var ms = new MemoryStream();
        //format.Save(_subtitleFileName, ms, UpdatedSubtitle, false);

        //var fileHelper = new FileHelper();
        //var subtitleFileName = await fileHelper.SaveStreamAs(ms, $"Save {CurrentSubtitleFormat.Name} file as", _videoFileName, format);
        //if (!string.IsNullOrEmpty(subtitleFileName))
        //{
        //    ShowStatus($"File exported in format {format.Name} to {subtitleFileName}");
        //}
    }

    [RelayCommand]
    public async Task ExportCavena890()
    {
        var result = await _popupService
            .ShowPopupAsync<ExportCavena890PopupModel>(onPresenting: viewModel
                => viewModel.SetValues(UpdatedSubtitle), CancellationToken.None);

        if (result is not (string and "OK"))
        {
            return;
        }

        var cavena = new Cavena890();
        using var ms = new MemoryStream();
        cavena.Save(_subtitleFileName, ms, UpdatedSubtitle, false);

        var fileHelper = new FileHelper();
        var subtitleFileName = await fileHelper.SaveStreamAs(ms, $"Save {CurrentSubtitleFormat.Name} file as", _videoFileName, cavena);
        if (!string.IsNullOrEmpty(subtitleFileName))
        {
            ShowStatus($"File exported in format {cavena.Name} to {subtitleFileName}");
        }
    }

    [RelayCommand]
    public async Task ExportPac()
    {
        var result = await _popupService.ShowPopupAsync<ExportPacPopupModel>(CancellationToken.None);
        if (result is not int codePage || codePage < 0)
        {
            return;
        }

        var pac = new Pac { CodePage = codePage };
        using var ms = new MemoryStream();
        pac.Save(_subtitleFileName, ms, UpdatedSubtitle);

        var fileHelper = new FileHelper();
        var subtitleFileName = await fileHelper.SaveStreamAs(ms, $"Save {CurrentSubtitleFormat.Name} file as", _videoFileName, pac);
        if (!string.IsNullOrEmpty(subtitleFileName))
        {
            ShowStatus($"File exported in format {pac.Name} to {subtitleFileName}");
        }
    }

    [RelayCommand]
    public async Task ExportPacUnicode()
    {
        var pacUnicode = new PacUnicode();
        using var ms = new MemoryStream();
        pacUnicode.Save(_subtitleFileName, ms, UpdatedSubtitle);

        var fileHelper = new FileHelper();
        var subtitleFileName = await fileHelper.SaveStreamAs(ms, $"Save {CurrentSubtitleFormat.Name} file as", _videoFileName, pacUnicode);
        if (!string.IsNullOrEmpty(subtitleFileName))
        {
            ShowStatus($"File exported in format {pacUnicode.Name} to {subtitleFileName}");
        }
    }

    [RelayCommand]
    public async Task ExportEbuStl()
    {
        await Shell.Current.GoToAsync(nameof(ExportEbuPage), new Dictionary<string, object>
        {
            { "Page", nameof(MainPage) },
            { "Subtitle", UpdatedSubtitle },
            { "UseSubtitleFileName", false },
        });
    }

    [RelayCommand]
    public async Task ShowAbout()
    {
        await _popupService.ShowPopupAsync<AboutPopupModel>(CancellationToken.None);
    }

    [RelayCommand]
    public async Task ShowGoToLineNumber()
    {
        var lineNumber = await _popupService
            .ShowPopupAsync<GoToLineNumberPopupModel>(onPresenting: viewModel => viewModel.Initialize(UpdatedSubtitle), CancellationToken.None);

        if (lineNumber is int lineNumberInt)
        {
            SelectParagraph(lineNumberInt - 1);
        }
    }

    [RelayCommand]
    public async Task ShowOptionsSettings()
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }

    [RelayCommand]
    public async Task ShowRestoreAutoBackup()
    {
        await Shell.Current.GoToAsync(nameof(RestoreAutoBackupPage));
    }

    [RelayCommand]
    public async Task SubtitleOpen()
    {
        Se.Settings.File.AddToRecentFiles(_subtitleFileName, _videoFileName, GetFirstSelectedIndex(), CurrentTextEncoding.DisplayName);

        var fileHelper = new FileHelper();
        var subtitleFileName = await fileHelper.PickAndShowSubtitleFile("Open subtitle file");
        if (string.IsNullOrEmpty(subtitleFileName))
        {
            return;
        }

        SubtitleOpen(subtitleFileName, null);
    }

    private void SubtitleOpen(string subtitleFileName, string? videoFileName)
    {
        if (string.IsNullOrEmpty(subtitleFileName))
        {
            SubtitleNew();
            return;
        }

        _timerAutoBackup.Stop();

        var subtitle = Subtitle.Parse(subtitleFileName);
        if (subtitle == null)
        {
            foreach (var f in SubtitleFormat.GetBinaryFormats(false))
            {
                if (f.IsMine(null, subtitleFileName))
                {
                    subtitle = new Subtitle();
                    f.LoadSubtitle(subtitle, null, subtitleFileName);
                    break; // format found, exit the loop
                }
            }

            if (subtitle == null)
            {
                var fileSize = (long)0;
                try
                {
                    var fi = new FileInfo(subtitleFileName);
                    fileSize = fi.Length;
                }
                catch
                {
                    // ignore
                }

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    var message = fileSize == 0 ? "File size is zero!" : "Unknown format?";
                    await MainPage!.DisplayAlert(
                        "Open subtitle failed",
                        $"Unable to read subtitle file \"{subtitleFileName}\". {message}",
                        "OK");
                });

                return;
            }
        }

        _subtitle = subtitle;
        if (SubtitleFormatPicker != null)
        {
            SubtitleFormatPicker.SelectedItem = subtitle.OriginalFormat.Name;
        }

        Paragraphs = _subtitle.Paragraphs.Select(p => new DisplayParagraph(p)).ToObservableCollection();
        _subtitleFileName = subtitleFileName;
        SetTitle(subtitleFileName);

        if (!string.IsNullOrEmpty(videoFileName) && File.Exists(videoFileName))
        {
            VideoOpenFile(videoFileName);
        }
        else if (FindVideoFileName.TryFindVideoFileName(subtitleFileName, out videoFileName))
        {
            VideoOpenFile(videoFileName);
        }

        AddToRecentFiles();

        if (!_stopping)
        {
            _changeSubtitleHash = GetFastSubtitleHash();
            _timer.Start();
            _timerAutoBackup.Start();
        }
    }

    private static void SetTitle(string subtitleFileName)
    {
        if (Window != null)
        {
            Window.Title = $"{subtitleFileName} - Subtitle Alchemist";
        }
    }

    private void AddToRecentFiles()
    {
        Se.Settings.File.AddToRecentFiles(_subtitleFileName, _videoFileName, GetFirstSelectedIndex(), CurrentTextEncoding.DisplayName);
        Se.SaveSettings();


        //TODO: cannot update - very annoying bug, see https://github.com/microsoft/microsoft-ui-xaml/issues/7797
        MenuFlyoutItemReopen.Clear();
        foreach (var recentFile in Se.Settings.File.RecentFiles)
        {
            MenuFlyoutItemReopen.Add(new MenuFlyoutItem() { Text = recentFile.SubtitleFileName });
        }

        // Hack to update menu
        InitMenuBar.CreateMenuBar(MainPage!, this);
    }

    public void ReopenSubtitle(string subtitleFileName)
    {
        var recentFile = Se.Settings.File.RecentFiles.FirstOrDefault(p => p.SubtitleFileName == subtitleFileName);
        if (recentFile != null)
        {
            SubtitleOpen(recentFile.SubtitleFileName, recentFile.VideoFileName);

            var idx = recentFile.SelectedLine >= 0 && recentFile.SelectedLine < Paragraphs.Count
                ? recentFile.SelectedLine
                : 0;

            SelectParagraph(idx);
            SubtitleList.ScrollTo(idx, -1, ScrollToPosition.MakeVisible, false);
            SubtitleList.ScrollTo(idx, -1, ScrollToPosition.MakeVisible, false);
            SubtitleList.ScrollTo(idx, -1, ScrollToPosition.MakeVisible, false);
            SubtitleList.ScrollTo(idx, -1, ScrollToPosition.MakeVisible, false);
        }
        else
        {
            SubtitleOpen(subtitleFileName, null);
        }
    }

    [RelayCommand]
    public void SubtitleNew()
    {
        MakeHistoryForUndo("Before \"New\"");

        Se.Settings.File.AddToRecentFiles(_subtitleFileName, _videoFileName, GetFirstSelectedIndex(), CurrentTextEncoding.DisplayName);

        _timerAutoBackup.Stop();
        _timer.Stop();
        _subtitleFileName = string.Empty;
        _videoFileName = string.Empty;
        _subtitle = new Subtitle();
        Paragraphs = new ObservableCollection<DisplayParagraph>();
        _currentParagraph = null;
        CurrentText = string.Empty;
        CurrentStart = new TimeSpan();
        CurrentEnd = new TimeSpan();
        CurrentDuration = new TimeSpan();
        if (VideoPlayer != null)
        {
            VideoPlayer.Source = null;
        }

        _audioVisualizer.WavePeaks = null;

        if (Window != null)
        {
            Window.Title = "Untitled - Subtitle Alchemist";
        }

        if (!_stopping)
        {
            _changeSubtitleHash = GetFastSubtitleHash();
            _timer.Start();
            _timerAutoBackup.Start();
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

        if (string.IsNullOrEmpty(subtitleFileName))
        {
            return;
        }

        _subtitleFileName = subtitleFileName;
        await SubtitleSave();
        SetTitle(_subtitleFileName);
    }

    [RelayCommand]
    private async Task SubtitleSave()
    {
        if (!Paragraphs.Any())
        {
            ShowStatus("Nothing to save");
            return;
        }

        var format = CurrentSubtitleFormat;

        if (string.IsNullOrEmpty(_subtitleFileName) ||
            !_subtitleFileName.EndsWith(format.Extension, StringComparison.OrdinalIgnoreCase))
        {
            await SubtitleSaveAs();
            return;
        }

        if (format.Name == Ebu.NameOfFormat)
        {
            Ebu.EbuUiHelper = new EbuHelper();
            var ebu = new Ebu();
            if (_subtitleFileName.EndsWith(ebu.Extension, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(_subtitle.Header) &&
                _subtitle.Header.Length > 20 && _subtitle.Header.Substring(3, 3) == "STL")
            {
                ebu.Save(_subtitleFileName, UpdatedSubtitle);
                ShowStatus("Saved: " + _subtitleFileName);
                return;
            }

            await Shell.Current.GoToAsync(nameof(ExportEbuPage), new Dictionary<string, object>
            {
                { "Page", nameof(MainPage) },
                { "Subtitle", UpdatedSubtitle },
                { "UseSubtitleFileName", true },
            });

            return;
        }

        _autoBackup.SaveAutoBackup(_subtitle, CurrentSubtitleFormat);
        _changeSubtitleHash = GetFastSubtitleHash();

        var text = UpdatedSubtitle.ToText(CurrentSubtitleFormat);
        await File.WriteAllTextAsync(_subtitleFileName, text);

        ShowStatus("Saved: " + _subtitleFileName);

        AddToRecentFiles();
    }

    private void ShowStatus(string statusText)
    {
        StatusText = statusText;

        MainPage?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(6_000), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (StatusText == statusText)
                {
                    StatusText = string.Empty;
                }
            });
            return false;
        });
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
            ShowStatus("Loading wave info from cache...");
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
#pragma warning disable CA1416 // Validate platform compatibility
        process.Start();
#pragma warning restore CA1416 // Validate platform compatibility

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
        }

        var current = e.CurrentSelection;
        var first = true;
        foreach (var item in current)
        {
            if (item is DisplayParagraph paragraph)
            {
                if (first)
                {
                    CurrentText = paragraph.Text;
                    CurrentStart = paragraph.Start;
                    CurrentEnd = paragraph.End;
                    CurrentDuration = paragraph.End - paragraph.Start;
                    _currentParagraph = paragraph;
                    first = false;
                }

                paragraph.IsSelected = true;
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
        if (await CheckIfSubtitleIsEmpty())
        {
            return;
        }

        var ffmpegOk = await RequireFfmpegOk();
        if (!ffmpegOk)
        {
            return;
        }

        await Shell.Current.GoToAsync(nameof(AudioToTextWhisperPage), new Dictionary<string, object>
        {
            { "Page", nameof(MainPage) },
            { "VideoFileName", _videoFileName },
        });
    }

    private async Task<bool> CheckIfSubtitleIsEmpty()
    {
        if (MainPage == null)
        {
            return false;
        }

        if (Paragraphs.Count == 0 || Paragraphs.Count == 1 && string.IsNullOrEmpty(Paragraphs[0].Text))
        {
            await MainPage.DisplayAlert(
                "No subtitle loaded",
                "You need to load/create a subtitle.",
                "OK");

            return true;
        }

        return false;
    }

    [RelayCommand]
    private async Task AutoTranslateShow()
    {
        if (await CheckIfSubtitleIsEmpty())
        {
            return;
        }

        await Shell.Current.GoToAsync(nameof(TranslatePage), new Dictionary<string, object>
        {
            { "Page", nameof(MainPage) },
            { nameof(Paragraphs), Paragraphs },
            { "Encoding", Encoding.UTF8 },
            { "Subtitle", UpdatedSubtitle },
        });
    }

    [RelayCommand]
    private async Task AdjustDurationsShow()
    {
        await Shell.Current.GoToAsync(nameof(AdjustDurationPage), new Dictionary<string, object>
        {
            { "Page", nameof(MainPage) },
            { "Subtitle", UpdatedSubtitle },
            { "SelectedIndexes", GetSelectedIndexes().ToList() },
            { "ShotChanges", new List<double>() },
        });
    }

    [RelayCommand]
    private async Task FixCommonErrorsShow()
    {
        await DictionaryLoader.UnpackIfNotFound();
        await Shell.Current.GoToAsync(nameof(FixCommonErrorsPage), new Dictionary<string, object>
        {
            { "Page", nameof(MainPage) },
            { "Subtitle", UpdatedSubtitle },
            { "Encoding", CurrentEncoding },
            { "Format", CurrentSubtitleFormat },
        });
    }

    [RelayCommand]
    private async Task SpellCheckShow()
    {
        await Shell.Current.GoToAsync(nameof(SpellCheckerPage), new Dictionary<string, object>
        {
            { "Page", nameof(MainPage) },
            { "Subtitle", UpdatedSubtitle },
        });
    }

    [RelayCommand]
    private async Task SpellCheckGetDictionariesShow()
    {
        var result = await _popupService.ShowPopupAsync<GetDictionaryPopupModel>(onPresenting: viewModel => viewModel.Initialize(), CancellationToken.None);

        if (result is SpellCheckDictionary dictionary && MainPage != null)
        {
            ShowStatus($"Dictionary downloaded: {dictionary.NativeName}");
        }
    }

    [RelayCommand]
    private async Task FindShow()
    {
        IFindService findService = new FindService(Paragraphs.Select(p => p.Text).ToList(), GetFirstSelectedIndex(), false, FindService.FindMode.Normal);
        var result = await _popupService.ShowPopupAsync<FindPopupModel>(onPresenting: viewModel => viewModel.Initialize(findService), CancellationToken.None);

        if (result is IFindService fs && !string.IsNullOrEmpty(fs.SearchText))
        {
            var idx = fs.Find(fs.SearchText);
            SelectParagraph(idx);
            ShowStatus($"'{fs.SearchText}' found in line '{idx + 1}'");
        }
    }

    private async Task<bool> RequireFfmpegOk()
    {
        if (MainPage == null)
        {
            return false;
        }

        if (FfmpegHelper.IsFfmpegInstalled())
        {
            return true;
        }

        if (File.Exists(DownloadFfmpegModel.GetFfmpegFileName()))
        {
            Se.Settings.General.FfmpegPath = DownloadFfmpegModel.GetFfmpegFileName();
            return true;
        }

        if (Configuration.IsRunningOnMac && File.Exists("/usr/local/bin/ffmpeg"))
        {
            Se.Settings.General.FfmpegPath = "/usr/local/bin/ffmpeg";
            return true;
        }

        if (Configuration.IsRunningOnWindows || Configuration.IsRunningOnMac)
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
            if (result is string ffmpegFileNameResult)
            {
                Se.Settings.General.FfmpegPath = ffmpegFileNameResult;
                ShowStatus($"ffmpeg downloaded and installed to {ffmpegFileNameResult}");
                return true;
            }
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
        if (_updating || _currentParagraph == null)
        {
            return;
        }

        _currentParagraph.P.StartTime = new TimeCode(e.NewValue);
        CurrentStart = TimeSpan.FromMilliseconds(e.NewValue);
        var idx = Paragraphs.IndexOf(_currentParagraph);
        if (idx >= 0)
        {
            Paragraphs[idx].Start = CurrentStart;
        }
    }

    public void CurrentDurationChanged(object? sender, ValueChangedEventArgs e)
    {
        if (_updating || _currentParagraph == null)
        {
            return;
        }

        var idx = Paragraphs.IndexOf(_currentParagraph);
        if (idx < 0)
        {
            return;
        }

        _currentParagraph.P.EndTime = new TimeCode(_currentParagraph.Start.TotalMilliseconds + e.NewValue);
        CurrentEnd = TimeSpan.FromMilliseconds(CurrentStart.TotalMilliseconds + e.NewValue);
        CurrentDuration = TimeSpan.FromMilliseconds(e.NewValue);
        Paragraphs[idx].Duration = CurrentDuration;
        Paragraphs[idx].End = CurrentEnd;
    }


    [RelayCommand]
    private async Task DeleteSelectedLines()
    {
        if (MainPage == null)
        {
            return;
        }

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

        MakeHistoryForUndo($"Before delete selected lines");

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

        if (firstIdx >= Paragraphs.Count)
        {
            firstIdx = Paragraphs.Count - 1;
        }

        if (firstIdx >= 0)
        {
            Paragraphs[firstIdx].IsSelected = true;

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

        MakeHistoryForUndo($"Before \"Insert before\"");

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

        MakeHistoryForUndo($"Before \"Insert after\"");

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
        MakeHistoryForUndo($"Before toggle tag \"{tag}\"");

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
                    VideoPlayer.SeekTo(TimeSpan.FromSeconds(paragraph.Start.TotalSeconds));
                }
            }
        }
    }

    private int GetFastSubtitleHash()
    {
        var pre = _subtitleFileName + CurrentTextEncoding.DisplayName;
        unchecked // Overflow is fine, just wrap
        {
            var hash = 17;
            hash = hash * 23 + pre.GetHashCode();

            if (_subtitle.Header != null)
            {
                hash = hash * 23 + _subtitle.Header.Trim().GetHashCode();
            }

            if (_subtitle.Footer != null)
            {
                hash = hash * 23 + _subtitle.Footer.Trim().GetHashCode();
            }

            var max = Paragraphs.Count;
            for (var i = 0; i < max; i++)
            {
                var p = Paragraphs[i];
                hash = hash * 23 + p.Number.GetHashCode();
                hash = hash * 23 + p.Start.TotalMilliseconds.GetHashCode();
                hash = hash * 23 + p.End.TotalMilliseconds.GetHashCode();

                foreach (var line in p.Text.SplitToLines())
                {
                    hash = hash * 23 + line.GetHashCode();
                }

                if (p.P.Style != null)
                {
                    hash = hash * 23 + p.P.Style.GetHashCode();
                }
                if (p.P.Extra != null)
                {
                    hash = hash * 23 + p.P.Extra.GetHashCode();
                }
                if (p.P.Actor != null)
                {
                    hash = hash * 23 + p.P.Actor.GetHashCode();
                }
                hash = hash * 23 + p.P.Layer.GetHashCode();
            }

            return hash;
        }
    }
}