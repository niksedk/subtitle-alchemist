using System.Collections.ObjectModel;
using System.Globalization;
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
using SubtitleAlchemist.Logic.Config;

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
    private ObservableCollection<TtsLanguage> _languages;

    [ObservableProperty]
    private TtsLanguage? _selectedLanguage;

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
    private CancellationTokenSource _cancellationTokenSource = new();
    private readonly IPopupService _popupService;
    private TtsStepResult[] _stepResults;
    private bool _skipAutoContinue;
    private WavePeakData _wavePeakData;
    private string _waveFolder;

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
                });
                return false;
            });
        }

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

        if (query["WaveFolder"] is string waveFolder && !string.IsNullOrEmpty(waveFolder))
        {
            _waveFolder = waveFolder;
        }

        if (query.ContainsKey("WavePeaks") && query["WavePeaks"] is WavePeakData wavePeakData)
        {
            _wavePeakData = wavePeakData;
            AudioVisualizer.WavePeaks = wavePeakData;
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

        var isEngineInstalled = await engine.IsInstalled();
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

        var speakResult = await engine.Speak(line.Text, _waveFolder, voice, SelectedLanguage);
        line.StepResult.CurrentFileName = speakResult.FileName;

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
    public async Task Done()
    {
        await Shell.Current.GoToAsync(nameof(TextToSpeechPage), new Dictionary<string, object>
        {
            { "Page", nameof(ReviewSpeechPage) },
            { "StepResult", _stepResults },
        });
    }

    private void SaveSettings(Type engineType)
    {
        Se.SaveSettings();
    }

    private void LoadSettings(Type engineType)
    {
        Se.SaveSettings();
    }

    public void CollectionViewSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
    }
}