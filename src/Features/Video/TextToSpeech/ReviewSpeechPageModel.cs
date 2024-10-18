using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubtitleAlchemist.Controls.AudioVisualizerControl;
using SubtitleAlchemist.Features.Main;
using SubtitleAlchemist.Features.Video.TextToSpeech.Engines;
using SubtitleAlchemist.Features.Video.TextToSpeech.Voices;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Services;

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
    private ObservableCollection<DisplayParagraph> _paragraphs;

    [ObservableProperty]
    private bool _autoContinue;

    public ReviewSpeechPage? Page { get; set; }
    public CollectionView CollectionView { get; set; }
    public AudioVisualizer AudioVisualizer { get; set; }


    private ITtsEngine _engine;
    private Voice _voice;
    private CancellationTokenSource _cancellationTokenSource = new();
    private readonly IPopupService _popupService;
    private TtsStepResult[] _stepResults;

    public ReviewSpeechPageModel(IPopupService popupService)
    {
        AudioVisualizer = new AudioVisualizer();
        _lines = new ObservableCollection<ReviewRow>();
        _paragraphs = new ObservableCollection<DisplayParagraph>();
        _popupService = popupService;
        _voice = new Voice(new object());
        _engine = new AllTalk(new TtsDownloadService(new HttpClient()));
        CollectionView = new CollectionView();
        _stepResults = Array.Empty<TtsStepResult>();
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
                });
            }

            SelectedLine = Lines.FirstOrDefault();
        }

        if (query["Engines"] is ITtsEngine[] engines)
        {
            Engines = new ObservableCollection<ITtsEngine>(engines);
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
    }

    [RelayCommand]
    public async Task Regenerate()
    {
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

        if (result is string s && !string.IsNullOrEmpty(s))
        {
            SelectedLine.Text = s;
        }
    }

    [RelayCommand]
    public async Task Play()
    {
    }

    [RelayCommand]
    public async Task Stop()
    {
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