using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.Json;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Controls.AudioVisualizerControl;
using SubtitleAlchemist.Features.Main;
using SubtitleAlchemist.Features.Video.TextToSpeech.DownloadTts;
using SubtitleAlchemist.Features.Video.TextToSpeech.Engines;
using SubtitleAlchemist.Features.Video.TextToSpeech.Voices;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;
using CommunityToolkit.Maui.Storage;

namespace SubtitleAlchemist.Features.Video.TextToSpeech;

public partial class ReviewSpeechPageModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private ObservableCollection<ITtsEngine> _engines;

    [ObservableProperty]
    private ITtsEngine? _selectedEngine;

    [ObservableProperty]
    private ObservableCollection<Voice> _voices;

    [ObservableProperty]
    private Voice? _selectedVoice;

    [ObservableProperty]
    private ObservableCollection<ReviewRow> _lines;

    [ObservableProperty]
    private ReviewRow? _selectedLine;

    [ObservableProperty]
    private bool _hasLanguageParameter;

    [ObservableProperty]
    private ObservableCollection<TtsLanguage> _languages;

    [ObservableProperty]
    private TtsLanguage? _selectedLanguage;

    [ObservableProperty]
    private bool _hasRegion;

    [ObservableProperty]
    private ObservableCollection<string> _regions;

    [ObservableProperty]
    private string? _selectedRegion;

    [ObservableProperty]
    private bool _hasModel;

    [ObservableProperty]
    private ObservableCollection<string> _models;

    [ObservableProperty]
    private string? _selectedModel;

    [ObservableProperty]
    private ObservableCollection<DisplayParagraph> _paragraphs;

    [ObservableProperty]
    private bool _autoContinue;

    [ObservableProperty]
    private bool _isRegenerateEnabled;

    public ReviewSpeechPage? Page { get; set; }
    public CollectionView CollectionView { get; set; }
    public AudioVisualizer AudioVisualizer { get; set; }
    public MediaElement Player { get; set; }


    private ITtsEngine? _engine;
    private Voice _voice;
    private CancellationTokenSource _cancellationTokenSource;
    private CancellationToken _cancellationToken;
    private readonly IPopupService _popupService;
    private TtsStepResult[] _stepResults;
    private bool _skipAutoContinue;
    private WavePeakData _wavePeakData;
    private readonly System.Timers.Timer _audioVisualizerTimer;
    private bool _allowUpdatePositionStates = true;
    private string _waveFolder;
    private string _videoFileName;
    private double _positionInSeconds;

    public ReviewSpeechPageModel(IPopupService popupService)
    {
        _lines = new ObservableCollection<ReviewRow>();
        _paragraphs = new ObservableCollection<DisplayParagraph>();
        _popupService = popupService;
        _voices = new ObservableCollection<Voice>();
        _voice = new Voice(new object());
        _engines = new ObservableCollection<ITtsEngine>();
        _languages = new ObservableCollection<TtsLanguage>();
        CollectionView = new CollectionView();
        _stepResults = Array.Empty<TtsStepResult>();
        _isRegenerateEnabled = true;
        AudioVisualizer = new AudioVisualizer();
        Player = new MediaElement();
        Player.MediaEnded += PlayEnded;
        _skipAutoContinue = false;
        _wavePeakData = new WavePeakData(1, new List<WavePeak>());
        _waveFolder = Path.GetTempPath();
        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;
        _regions = new ObservableCollection<string>();
        _models = new ObservableCollection<string>();
        _audioVisualizerTimer = new System.Timers.Timer(200);
    }

    private void PlayEnded(object? sender, EventArgs e)
    {
        var line = SelectedLine;
        if (line == null || !AutoContinue || _skipAutoContinue)
        {
            return;
        }

        var index = Lines.IndexOf(line);
        if (index < Lines.Count - 1)
        {
            SelectedLine = Lines[index + 1];
            Play(SelectedLine.StepResult.CurrentFileName);
        }
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query["Engines"] is ITtsEngine[] engines)
        {
            Engines = new ObservableCollection<ITtsEngine>(engines);
        }

        if (query["Engine"] is ITtsEngine engine)
        {
            _engine = engine;
            SelectedEngine = engine;
        }

        if (query["Voices"] is Voice[] voices)
        {
            Voices = new ObservableCollection<Voice>(voices);
        }

        if (query["Voice"] is Voice voice)
        {
            _voice = voice;
            SelectedVoice = voice;
        }

        if (query["VideoFileName"] is string videoFileName && !string.IsNullOrEmpty(videoFileName))
        {
            _videoFileName = videoFileName;
        }

        if (query["WaveFolder"] is string waveFolder && !string.IsNullOrEmpty(waveFolder))
        {
            _waveFolder = waveFolder;
        }

        if (query.ContainsKey("WavePeaks") && query["WavePeaks"] is WavePeakData wavePeakData)
        {
            _wavePeakData = wavePeakData;
            AudioVisualizer.WavePeaks = wavePeakData;
            AudioVisualizer.AllowMove = true;
            AudioVisualizer.AllowNewSelection = false;
            AudioVisualizer.OnTimeChanged += OnAudioVisualizerOnOnTimeChanged;
        }

        if (query["StepResult"] is TtsStepResult[] stepResult)
        {
            _stepResults = stepResult;
            Paragraphs = new ObservableCollection<DisplayParagraph>(stepResult.Select(p => new DisplayParagraph(p.Paragraph)).ToList()); ;
            Lines.Clear();
            foreach (var p in stepResult)
            {
                Lines.Add(new ReviewRow
                {
                    Include = true,
                    Number = p.Paragraph.Number,
                    Text = p.Text,
                    Voice = p.Voice == null ? string.Empty : p.Voice.ToString(),
                    Speed = Math.Round(p.SpeedFactor, 2).ToString(CultureInfo.CurrentCulture),
                    Cps = Math.Round(p.Paragraph.GetCharactersPerSecond(), 2).ToString(CultureInfo.CurrentCulture),
                    StepResult = p
                });
            }

            Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    SelectedLine = Lines.FirstOrDefault();
                    CollectionView.SelectedItem = SelectedLine;
                    CollectionView.Focus();
                    CollectionView.SelectedItem = SelectedLine;

                    if (SelectedLine != null)
                    {
                        _positionInSeconds = SelectedLine.StepResult.Paragraph.StartTime.TotalSeconds;
                    }
                    SetTimer();
                    _audioVisualizerTimer.Start();
                });
                return false;
            });
        }
    }

    private void OnAudioVisualizerOnOnTimeChanged(object sender, ParagraphEventArgs e)
    {
        var line = SelectedLine;
        if (line == null)
        {
            return;
        }

        var dp = line.StepResult.Paragraph;
        if (e.MouseDownParagraphType == MouseDownParagraphType.Start)
        {
            dp.StartTime.TotalMilliseconds = e.Paragraph.StartTime.TotalMilliseconds;
        }
        else if (e.MouseDownParagraphType == MouseDownParagraphType.End)
        {
            dp.EndTime.TotalMilliseconds = e.Paragraph.EndTime.TotalMilliseconds;
        }
        else if (e.MouseDownParagraphType == MouseDownParagraphType.Whole)
        {
            dp.StartTime.TotalMilliseconds = e.Paragraph.StartTime.TotalMilliseconds;
            dp.EndTime.TotalMilliseconds = e.Paragraph.EndTime.TotalMilliseconds;
        }
    }

    [RelayCommand]
    public async Task Regenerate()
    {
        var engine = SelectedEngine ?? _engine;
        var voice = SelectedVoice;
        var line = SelectedLine;
        if (engine == null || voice == null || line == null)
        {
            return;
        }

        var isEngineInstalled = await engine.IsInstalled(SelectedRegion);
        if (!isEngineInstalled)
        {
            return;
        }

        IsRegenerateEnabled = false;
        if (!engine.IsVoiceInstalled(voice) && voice.EngineVoice is PiperVoice piperVoice)
        {
            var modelFileName = Path.Combine(Piper.GetSetPiperFolder(), piperVoice.ModelShort);
            if (!File.Exists(modelFileName))
            {
                var r1 = await _popupService.ShowPopupAsync<DownloadTtsPopupModel>(onPresenting: viewModel => viewModel.StartDownloadPiperVoice(piperVoice), CancellationToken.None);
            }

            var configFileName = Path.Combine(Piper.GetSetPiperFolder(), piperVoice.ConfigShort);
            if (!File.Exists(configFileName))
            {
                var r1 = await _popupService.ShowPopupAsync<DownloadTtsPopupModel>(onPresenting: viewModel => viewModel.StartDownloadPiperVoice(piperVoice), CancellationToken.None);
            }
        }

        var speakResult = await engine.Speak(line.Text, _waveFolder, voice, SelectedLanguage, SelectedRegion, SelectedModel, _cancellationToken);
        line.StepResult.CurrentFileName = speakResult.FileName;
        line.StepResult.Voice = voice;
        //TODO: speed!

        _skipAutoContinue = true;
        Play(speakResult.FileName);

        IsRegenerateEnabled = true;
    }

    private void Play(string audioFileName)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Player.Stop();
            Player.Source = null;
            Player.Source = MediaSource.FromFile(audioFileName);
            Player.Play();
        });
    }

    [RelayCommand]
    public async Task EditText()
    {
        if (SelectedLine == null)
        {
            return;
        }

        var result = await _popupService
        .ShowPopupAsync<EditTextPopupModel>(
            onPresenting: viewModel => viewModel.Initialize(SelectedLine.Text),
            CancellationToken.None);

        if (result is EditTextPopupResult popupResult && !string.IsNullOrEmpty(popupResult.Text))
        {
            SelectedLine.Text = popupResult.Text;
            if (popupResult.Regenerate)
            {
                await Regenerate();
            }
        }
    }

    [RelayCommand]
    public void Play()
    {
        var line = SelectedLine;
        if (line == null)
        {
            return;
        }

        _skipAutoContinue = false;
        Play(line.StepResult.CurrentFileName);
    }

    [RelayCommand]
    public void Stop()
    {
        Player.Stop();
        Player.Source = null;
    }

    [RelayCommand]
    public async Task Export()
    {
        // Choose folder
        var result = await FolderPicker.Default.PickAsync(_cancellationToken);
        if (!result.IsSuccessful)
        {
            return;
        }

        var folder = result.Folder.Path;

        // Copy files
        var index = 0;
        var exportFormat = new TtsImportExport { VideoFileName = _videoFileName };
        foreach (var line in Lines)
        {
            index++;
            var sourceFileName = line.StepResult.CurrentFileName;
            var targetFileName = Path.Combine(folder, index.ToString().PadLeft(4,'0') + Path.GetExtension(sourceFileName));
            File.Copy(sourceFileName, targetFileName, true);

            exportFormat.Items.Add(new TtsImportExportItem
            {
                AudioFileName = targetFileName,
                StartMs = (long)Math.Round(line.StepResult.Paragraph.StartTime.TotalMilliseconds, MidpointRounding.AwayFromZero),
                EndMs = (long)Math.Round(line.StepResult.Paragraph.EndTime.TotalMilliseconds, MidpointRounding.AwayFromZero),
                VoiceName = line.StepResult.Voice?.Name ?? string.Empty,
                EngineName = SelectedEngine != null ? SelectedEngine.ToString() : string.Empty,
                SpeedFactor = line.StepResult.SpeedFactor,
                Text = line.Text,
                Include = line.Include,
            });
        }

        // Export json
        var jsonFileName = Path.Combine(folder, "SubtitleEditTts.json");
        var json = JsonSerializer.Serialize(exportFormat, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(jsonFileName, json, _cancellationToken);

        // Open folder
        UiUtil.OpenFolder(folder);
    }

    [RelayCommand]
    public async Task Done()
    {
        OnDisappearing();
        await Shell.Current.GoToAsync("..", new Dictionary<string, object>
        {
            { "Page", nameof(ReviewSpeechPage) },
            { "StepResult", _stepResults },
        });
    }

    public void CollectionViewSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var line = SelectedLine;
        if (line == null)
        {
            return;
        }

        _positionInSeconds = line.StepResult.Paragraph.StartTime.TotalSeconds;
    }

    public void PickerEngineSelectedIndexChanged(object? sender, EventArgs e)
    {
        var engine = SelectedEngine;
        if (engine == null)
        {
            return;
        }

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var voices = await engine.GetVoices();
            Voices.Clear();
            foreach (var vo in voices)
            {
                Voices.Add(vo);
            }

            var lastVoice = Voices.FirstOrDefault(v => v.Name == Se.Settings.Video.TextToSpeech.Voice);
            if (lastVoice == null)
            {
                lastVoice = Voices.FirstOrDefault(p => p.Name.StartsWith("en", StringComparison.OrdinalIgnoreCase) ||
                                                       p.Name.Contains("English", StringComparison.OrdinalIgnoreCase));
            }
            SelectedVoice = lastVoice ?? Voices.First();

            HasLanguageParameter = engine.HasLanguageParameter;
            HasRegion = engine.HasRegion;
            HasModel = engine.HasModel;

            if (HasLanguageParameter)
            {
                var languages = await engine.GetLanguages(SelectedVoice);
                Languages.Clear();
                foreach (var language in languages)
                {
                    Languages.Add(language);
                }

                SelectedLanguage = Languages.FirstOrDefault();
            }

            if (HasRegion)
            {
                var regions = await engine.GetRegions();
                Regions.Clear();
                foreach (var region in regions)
                {
                    Regions.Add(region);
                }

                SelectedRegion = Regions.FirstOrDefault();
            }

            if (HasModel)
            {
                var models = await engine.GetModels();
                Models.Clear();
                foreach (var model in models)
                {
                    Models.Add(model);
                }

                SelectedModel = Models.FirstOrDefault();
            }
        });
    }

    public void SetTimer()
    {
        _audioVisualizerTimer.Elapsed += (_, _) =>
        {
            _audioVisualizerTimer.Stop();
            if (AudioVisualizer is { WavePeaks: { }, IsVisible: true })
            {
                var subtitle = new Subtitle();
                var selectedIndices = new List<int>();
                var orderedList = _stepResults.Select(p => p.Paragraph).OrderBy(p => p.StartTime.TotalMilliseconds).ToList();
                var firstSelectedIndex = -1;
                for (var i = 0; i < orderedList.Count; i++)
                {
                    var dp = orderedList[i];
                    var p = new Paragraph(dp, false);
                    p.Text = dp.Text;
                    p.StartTime.TotalMilliseconds = dp.StartTime.TotalMilliseconds;
                    p.EndTime.TotalMilliseconds = dp.EndTime.TotalMilliseconds;
                    subtitle.Paragraphs.Add(p);

                    if (dp.Id == SelectedLine?.StepResult.Paragraph.Id)
                    {
                        selectedIndices.Add(i);

                        if (firstSelectedIndex < 0)
                        {
                            firstSelectedIndex = i;
                        }
                    }
                }

                if (_cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (_positionInSeconds > AudioVisualizer.EndPositionSeconds ||
                    _positionInSeconds < AudioVisualizer.StartPositionSeconds)
                {
                    AudioVisualizer.SetPosition(
                        _positionInSeconds,
                        subtitle,
                        _positionInSeconds,
                        firstSelectedIndex,
                        selectedIndices.ToArray());
                }
                else
                {
                    AudioVisualizer.SetPosition(
                        AudioVisualizer.StartPositionSeconds,
                        subtitle,
                        _positionInSeconds,
                        firstSelectedIndex,
                        selectedIndices.ToArray());
                }
            }

            if (_cancellationToken.IsCancellationRequested)
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

            _audioVisualizerTimer.Start();
        };
    }

    public void OnDisappearing()
    {
        _audioVisualizerTimer.Stop();
        _cancellationTokenSource.Cancel();
    }
}