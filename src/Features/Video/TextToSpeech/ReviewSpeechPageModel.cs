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
using SubtitleAlchemist.Logic.Media;

namespace SubtitleAlchemist.Features.Video.TextToSpeech;

public partial class ReviewSpeechPageModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private ObservableCollection<ITtsEngine> _engines;

    [ObservableProperty]
    private ITtsEngine? _selectedEngine;

    [ObservableProperty]
    private bool _isEngineSettingsVisible;

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
    private bool _hasStyleParameter;

    [ObservableProperty]
    private ObservableCollection<string> _styles;

    [ObservableProperty]
    private string? _selectedStyle;

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
    public Label LabelEngineSettings { get; set; }


    private ITtsEngine? _engine;
    private Voice _voice;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly CancellationToken _cancellationToken;
    private readonly IPopupService _popupService;
    private TtsStepResult[] _stepResults;
    private bool _skipAutoContinue;
    private WavePeakData _wavePeakData;
    private readonly System.Timers.Timer _audioVisualizerTimer;
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
        LabelEngineSettings = new();
        _skipAutoContinue = false;
        _wavePeakData = new WavePeakData(1, new List<WavePeak>());
        _waveFolder = Path.GetTempPath();
        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;
        _regions = new ObservableCollection<string>();
        _models = new ObservableCollection<string>();
        _audioVisualizerTimer = new System.Timers.Timer(40);
        _videoFileName = string.Empty;
        _styles = new ObservableCollection<string>();
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
        var line = Lines.FirstOrDefault(row => row.StepResult.Paragraph.Id == e.Paragraph.Id);
        if (line == null)
        {
            return;
        }

        var p = line.StepResult.Paragraph;
        if (e.MouseDownParagraphType == MouseDownParagraphType.Start)
        {
            p.StartTime.TotalMilliseconds = e.Paragraph.StartTime.TotalMilliseconds;
        }
        else if (e.MouseDownParagraphType == MouseDownParagraphType.End)
        {
            p.EndTime.TotalMilliseconds = e.Paragraph.EndTime.TotalMilliseconds;
        }
        else if (e.MouseDownParagraphType == MouseDownParagraphType.Whole)
        {
            p.StartTime.TotalMilliseconds = e.Paragraph.StartTime.TotalMilliseconds;
            p.EndTime.TotalMilliseconds = e.Paragraph.EndTime.TotalMilliseconds;
        }

        line.Cps = Math.Round(p.GetCharactersPerSecond(), 2).ToString(CultureInfo.CurrentCulture);
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

        var oldStyle = SelectedStyle;
        if (engine is Murf && !string.IsNullOrEmpty(SelectedStyle))
        {
            Se.Settings.Video.TextToSpeech.MurfStyle = SelectedStyle;
        }

        var speakResult = await engine.Speak(line.Text, _waveFolder, voice, SelectedLanguage, SelectedRegion, SelectedModel, _cancellationToken);
        line.StepResult.CurrentFileName = speakResult.FileName;
        line.StepResult.Voice = voice;

        var adjustSpeedStepResult = await TrimAndAdjustSpeed(line.StepResult);
        line.Speed = Math.Round(adjustSpeedStepResult.SpeedFactor, 2).ToString(CultureInfo.CurrentCulture);
        line.Cps = Math.Round(adjustSpeedStepResult.Paragraph.GetCharactersPerSecond(), 2).ToString(CultureInfo.CurrentCulture);
        line.StepResult = adjustSpeedStepResult;
        line.Voice = voice.ToString();

        _skipAutoContinue = true;
        Play(line.StepResult.CurrentFileName);

        IsRegenerateEnabled = true;

        if (engine is Murf && oldStyle != null)
        {
            Se.Settings.Video.TextToSpeech.MurfStyle = oldStyle;
        }
    }

    private async Task<TtsStepResult> TrimAndAdjustSpeed(TtsStepResult item)
    {
        var p = item.Paragraph;
        var index = _stepResults.ToList().IndexOf(item);
        var next = index + 1 < _stepResults.Length ? _stepResults[index + 1] : null;
        var outputFileNameTrim = Path.Combine(_waveFolder, Guid.NewGuid() + ".wav");
        var trimProcess = VideoPreviewGenerator.TrimSilenceStartAndEnd(item.CurrentFileName, outputFileNameTrim);
#pragma warning disable CA1416 // Validate platform compatibility
        _ = trimProcess.Start();
#pragma warning restore CA1416 // Validate platform compatibility
        await trimProcess.WaitForExitAsync(_cancellationToken);

        var addDuration = 0d;
        if (next != null && p.EndTime.TotalMilliseconds < next.Paragraph.StartTime.TotalMilliseconds)
        {
            var diff = next.Paragraph.StartTime.TotalMilliseconds - p.EndTime.TotalMilliseconds;
            addDuration = Math.Min(1000, diff);
            if (addDuration < 0)
            {
                addDuration = 0;
            }
        }

        var mediaInfo = FfmpegMediaInfo2.Parse(outputFileNameTrim);
        if (mediaInfo.Duration.TotalMilliseconds <= p.DurationTotalMilliseconds + addDuration)
        {
            return new TtsStepResult
            {
                Paragraph = p,
                Text = item.Text,
                CurrentFileName = outputFileNameTrim,
                SpeedFactor = 1.0f,
                Voice = item.Voice,
            };
        }

        var divisor = (decimal)(p.DurationTotalMilliseconds + addDuration);
        if (divisor <= 0)
        {
            return new TtsStepResult
            {
                Paragraph = p,
                Text = item.Text,
                CurrentFileName = item.CurrentFileName,
                SpeedFactor = 1.0f,
                Voice = item.Voice,
            };
        }

        var ext = ".wav";
        var factor = (decimal)mediaInfo.Duration.TotalMilliseconds / divisor;
        var outputFileName2 = Path.Combine(_waveFolder, $"{index}_{Guid.NewGuid()}{ext}");
        var overrideFileName = string.Empty;
        if (!string.IsNullOrEmpty(overrideFileName) && File.Exists(Path.Combine(_waveFolder, overrideFileName)))
        {
            outputFileName2 = Path.Combine(_waveFolder, $"{Path.GetFileNameWithoutExtension(overrideFileName)}_{Guid.NewGuid()}{ext}");
        }

        var mergeProcess = VideoPreviewGenerator.ChangeSpeed(outputFileNameTrim, outputFileName2, (float)factor);
#pragma warning disable CA1416 // Validate platform compatibility
        _ = mergeProcess.Start();
#pragma warning restore CA1416 // Validate platform compatibility
        await mergeProcess.WaitForExitAsync(_cancellationToken);

        return new TtsStepResult
        {
            Paragraph = p,
            Text = item.Text,
            CurrentFileName = outputFileName2,
            SpeedFactor = (float)factor,
            Voice = item.Voice,
        };
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
        if (Page == null)
        {
            return;
        }

        // Choose folder
        var result = await FolderPicker.Default.PickAsync(_cancellationToken);
        if (!result.IsSuccessful)
        {
            return;
        }

        var folder = result.Folder.Path;

        var jsonFileName = Path.Combine(folder, "SubtitleEditTts.json");

        // ask if overwrite if jsonFileName exists
        if (File.Exists(jsonFileName))
        {
            var answer = await Page.DisplayAlert(
                "Overwrite?",
                $"Do you want overwrite files in \"{folder}?",
                "Yes",
                "No");

            if (!answer)
            {
                return;
            }

            try
            {
                File.Delete(jsonFileName);
            }
            catch (Exception e)
            {
                await Page.DisplayAlert(
                    "Overwrite failed",
                    $"Could not overwrite the file \"{jsonFileName}" + Environment.NewLine + e.Message,
                    "OK");
                return;
            }
        }


        // Copy files
        var index = 0;
        var exportFormat = new TtsImportExport { VideoFileName = _videoFileName };
        foreach (var line in Lines)
        {
            index++;
            var sourceFileName = line.StepResult.CurrentFileName;
            var targetFileName = Path.Combine(folder, index.ToString().PadLeft(4, '0') + Path.GetExtension(sourceFileName));

            if (File.Exists(targetFileName))
            {
                try
                {
                    File.Delete(targetFileName);
                }
                catch (Exception e)
                {
                    await Page.DisplayAlert(
                        "Overwrite failed",
                        $"Could not overwrite the file \"{targetFileName}" + Environment.NewLine + e.Message,
                        "OK");
                    return;
                }
            }

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
            { "StepResult", Lines.Where(p=>p.Include).Select(p => p.StepResult).ToArray() },
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
            var voices = await engine.GetVoices(SelectedLanguage?.Code ?? string.Empty);
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

            if (engine is ElevenLabs)
            {
                IsEngineSettingsVisible = true;
                if (string.IsNullOrEmpty(Se.Settings.Video.TextToSpeech.ElevenLabsModel))
                {
                    Se.Settings.Video.TextToSpeech.ElevenLabsModel = (await engine.GetModels()).First();
                }

                SelectedModel = Se.Settings.Video.TextToSpeech.ElevenLabsModel;
            }

            HasStyleParameter = engine is Murf;
            if (engine is Murf && SelectedVoice?.EngineVoice is MurfVoice mv)
            {
                Styles = new ObservableCollection<string>(mv.AvailableStyles);
                SelectedStyle = Styles.FirstOrDefault(p => p == Se.Settings.Video.TextToSpeech.MurfStyle);
                if (SelectedStyle == null)
                {
                    SelectedStyle = Styles.FirstOrDefault();
                }
            }

            if (HasLanguageParameter)
            {
                var languages = await engine.GetLanguages(SelectedVoice, SelectedModel);
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

    [RelayCommand]
    public void ShowEngineSettings()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await _popupService.ShowPopupAsync<ElevenLabSettingsPopupModel>(CancellationToken.None);
        });
    }
}