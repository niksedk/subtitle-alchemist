using System.Collections.ObjectModel;
using System.IO.Compression;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using SkiaSharp;
using SubtitleAlchemist.Logic.BluRaySup;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Logic.Media;
using SubtitleAlchemist.Logic.Ocr;

namespace SubtitleAlchemist.Features.Shared.Ocr;

public partial class OcrPageModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private ObservableCollection<OcrEngineItem> _ocrEngines;

    [ObservableProperty]
    private OcrEngineItem? _selectedOcrEngine;

    [ObservableProperty]
    private ObservableCollection<OcrSubtitleItem> _ocrSubtitleItems;

    [ObservableProperty]
    private OcrSubtitleItem? _selectedOcrSubtitleItem;

    [ObservableProperty]
    private ObservableCollection<int> _startFromNumbers;

    [ObservableProperty]
    private int _selectedStartFromNumber;

    [ObservableProperty]
    private ImageSource? _currentImageSource;

    [ObservableProperty]
    private string _currentBitmapInfo;

    [ObservableProperty]
    private string _currentText;

    [ObservableProperty]
    private bool _isRunActive;

    [ObservableProperty]
    private bool _isPauseActive;

    [ObservableProperty]
    private ObservableCollection<string> _nOcrDatabases;

    [ObservableProperty]
    private string? _selectedNOcrDatabase;

    [ObservableProperty]
    private string _progressText;

    [ObservableProperty]
    private double _progressValue;

    [ObservableProperty]
    private bool _isProgressVisible;

    public OcrPage? Page { get; set; }
    public CollectionView ListView { get; set; }

    private bool _isRunningOcr;
    private Subtitle _subtitle;
    private IOcrSubtitle _ocrSubtitle;
    private INOcrCaseFixer _nOcrCaseFixer;
    private string _language;
    private bool _loading;
    private CancellationTokenSource _cancellationTokenSource;
    private NOcrDb? _nOcrDb;

    public OcrPageModel(INOcrCaseFixer nOcrCaseFixer)
    {
        _nOcrCaseFixer = nOcrCaseFixer;
        _ocrEngines = new ObservableCollection<OcrEngineItem>(OcrEngineItem.GetOcrEngines());
        _selectedOcrEngine = _ocrEngines.FirstOrDefault();
        _subtitle = new Subtitle();
        _ocrSubtitle = new BluRayPcsDataList(new List<BluRaySupParser.PcsData>());
        _ocrSubtitleItems = new ObservableCollection<OcrSubtitleItem>();
        _language = "eng";
        _currentBitmapInfo = string.Empty;
        _currentText = string.Empty;
        _startFromNumbers = new ObservableCollection<int>(Enumerable.Range(1, 2));
        _selectedStartFromNumber = 1;
        _nOcrDatabases = new ObservableCollection<string>();
        ListView = new CollectionView();
        _progressText = string.Empty;
        _progressValue = 0d;
        _isProgressVisible = false;
        _loading = true;
        _cancellationTokenSource = new CancellationTokenSource();
        _isRunningOcr = false;
        _nOcrDb = null;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await CheckAndUnpackOcrFiles();
        });

        if (query["Subtitle"] is List<BluRaySupParser.PcsData> bluRaySup)
        {
            _ocrSubtitle = new BluRayPcsDataList(bluRaySup); ;
            OcrSubtitleItems = new ObservableCollection<OcrSubtitleItem>(_ocrSubtitle.MakeOcrSubtitleItems());
            StartFromNumbers = new ObservableCollection<int>(Enumerable.Range(1, _ocrSubtitle.Count));
            SelectedStartFromNumber = 1;

            foreach (var s in NOcrDb.GetDatabases().OrderBy(p => p))
            {
                NOcrDatabases.Add(s);
            }
            SelectedNOcrDatabase = NOcrDb.GetDatabases().FirstOrDefault();

            // load all images in the background
            Task.Run(() =>
            {
                foreach (var item in OcrSubtitleItems)
                {
                    item.GetBitmap();
                }
            });
        }

        Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                _loading = false;
            });
            return false;
        });
    }

    private async Task CheckAndUnpackOcrFiles()
    {
        if (!Directory.Exists(Se.OcrFolder))
        {
            Directory.CreateDirectory(Se.OcrFolder);
        }

        var nOcrLatin = Path.Combine(Se.OcrFolder, "Latin.nocr");
        if (!File.Exists(nOcrLatin))
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("Latin.nocr");
            using var fileStream = File.Create(nOcrLatin);
            stream.Seek(0, SeekOrigin.Begin);
            stream.CopyTo(fileStream);
        }
    }

    private string? GetNOcrLanguageFileName()
    {
        if (SelectedNOcrDatabase == null)
        {
            return null;
        }

        return Path.Combine(Se.OcrFolder, $"{SelectedNOcrDatabase}.nocr");
    }

    [RelayCommand]
    private async Task RunOcr()
    {
        if (_isRunningOcr)
        {
            return;
        }

        _cancellationTokenSource = new CancellationTokenSource();
        _isRunningOcr = true;
        var startFromIndex = SelectedStartFromNumber - 1;
        IsProgressVisible = true;
        ProgressText = "Running OCR...";
        ProgressValue = 0d;

        IsPauseActive = true;
        IsRunActive = false;


        var nOcrDbFileName = GetNOcrLanguageFileName();
        if (!string.IsNullOrEmpty(nOcrDbFileName))
        {
            _nOcrDb = new NOcrDb(nOcrDbFileName);
        }


        // Run OCR in background task
        _ = Task.Run(() =>
        {
            for (var i = startFromIndex; i < OcrSubtitleItems.Count; i++)
            {
                if (_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    return;
                }

                ProgressValue = i / (double)OcrSubtitleItems.Count;
                ProgressText = $"Running OCR... {i + 1}/{OcrSubtitleItems.Count}";

                var item = OcrSubtitleItems[i];
                var bitmap = item.GetBitmap();
                var nBmp = new NikseBitmap2(bitmap);
                nBmp.MakeTwoColor(200);
                nBmp.CropTop(0, new SKColor(0, 0, 0, 0));
                var list = NikseBitmapImageSplitter2.SplitBitmapToLettersNew(nBmp, 10, false, true, 20, true);
                var sb = new StringBuilder();
                foreach (var splitterItem in list)
                {
                    if (splitterItem.NikseBitmap == null)
                    {
                        if (splitterItem.SpecialCharacter != null)
                        {
                            sb.Append(splitterItem.SpecialCharacter);
                        }
                    }
                    else
                    {
                        var match = _nOcrDb.GetMatch(splitterItem.NikseBitmap, splitterItem.Top, true, 50);
                        sb.Append(match != null ? _nOcrCaseFixer.FixUppercaseLowercaseIssues(splitterItem, match) : "*");
                    }
                }

                item.Text = sb.ToString().Trim().Replace(Environment.NewLine, " ");

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ListView.ScrollTo(i, -1, ScrollToPosition.MakeVisible, false);
                });

                SelectedStartFromNumber = i + 1;
            }

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                SelectedStartFromNumber = OcrSubtitleItems.Count;
                await Pause();
            });
        });
    }


    [RelayCommand]
    private async Task Pause()
    {
        await _cancellationTokenSource.CancelAsync();

        _isRunningOcr = false;
        IsPauseActive = false;
        IsRunActive = true;
        IsProgressVisible = false;
    }

    [RelayCommand]
    private async Task Ok()
    {
        var subtitle = new Subtitle(_subtitle, false);

        await Shell.Current.GoToAsync("..", new Dictionary<string, object>
        {
            { "Page", nameof(OcrPage) },
            { "Subtitle", subtitle },
        });
    }

    [RelayCommand]
    public async Task Cancel()
    {
        await Shell.Current.GoToAsync("..", new Dictionary<string, object>
        {
            { "Page", nameof(OcrPage) },
        });
    }

    public void OnCollectionViewSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (SelectedOcrSubtitleItem != null)
        {
            var bitmap = SelectedOcrSubtitleItem.GetBitmap();
            CurrentImageSource = bitmap.ToImageSource();
            CurrentBitmapInfo = $"{SelectedOcrSubtitleItem.Number}/{_ocrSubtitle.Count}: {bitmap.Width}x{bitmap.Height}";
            SelectedStartFromNumber = SelectedOcrSubtitleItem.Number;
        }
    }
}
