using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using SkiaSharp;
using SubtitleAlchemist.Controls.PickerControl;
using SubtitleAlchemist.Features.Main;
using SubtitleAlchemist.Logic.BluRaySup;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Logic.Media;
using SubtitleAlchemist.Logic.Ocr;
using SubtitleAlchemist.Services;
using System.Collections.ObjectModel;
using System.Text;

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
    private bool _isOkAndCancelActive;

    [ObservableProperty]
    private bool _isInspectVisible;

    [ObservableProperty]
    private bool _isInspectActive;

    [ObservableProperty]
    private ObservableCollection<string> _nOcrDatabases;

    [ObservableProperty]
    private string? _selectedNOcrDatabase;

    [ObservableProperty]
    private ObservableCollection<int> _nOcrMaxWrongPixelsList;

    [ObservableProperty]
    private int _selectedNOcrMaxWrongPixels;

    [ObservableProperty]
    private ObservableCollection<int> _nOcrPixelsAreSpaceList;

    [ObservableProperty]
    private int _selectedNOcrPixelsAreSpace;

    [ObservableProperty]
    private string _progressText;

    [ObservableProperty]
    private double _progressValue;

    [ObservableProperty]
    private bool _isProgressVisible;

    [ObservableProperty]
    private bool _nOcrDrawUnknownText;

    [ObservableProperty]
    private string _ollamaModel;

    [ObservableProperty]
    private ObservableCollection<string> _ollamaLanguages;

    [ObservableProperty]
    private string _ollamaLanguage;

    [ObservableProperty]
    private bool _isNOcrVisible;

    [ObservableProperty]
    private bool _isOllamaOcrVisible;

    [ObservableProperty]
    private bool _isTesseractVisible;

    [ObservableProperty]
    private ObservableCollection<TesseractDictionary> _tesseractDictionaryItems;

    [ObservableProperty]
    private TesseractDictionary? _selectedTesseractDictionaryItem;

    public OcrPage? Page { get; set; }
    public CollectionView ListView { get; set; }

    private bool _isRunningOcr;
    private IOcrSubtitle _ocrSubtitle;
    private readonly INOcrCaseFixer _nOcrCaseFixer;
    private string _language;
    private string _fileName;
    private bool _loading;
    private CancellationTokenSource _cancellationTokenSource;
    private NOcrDb? _nOcrDb;
    private bool _toolsItalicOn;
    private IPopupService _popupService;
    private ITesseractDownloadService _tesseractDownloadService;

    public OcrPageModel(INOcrCaseFixer nOcrCaseFixer, IPopupService popupService, ITesseractDownloadService tesseractDownloadService)
    {
        _nOcrCaseFixer = nOcrCaseFixer;
        _popupService = popupService;
        _tesseractDownloadService = tesseractDownloadService;
        _ocrEngines = new ObservableCollection<OcrEngineItem>(OcrEngineItem.GetOcrEngines());
        _selectedOcrEngine = _ocrEngines.FirstOrDefault();
        _ocrSubtitle = new BluRayPcsDataList(new List<BluRaySupParser.PcsData>());
        _ocrSubtitleItems = new ObservableCollection<OcrSubtitleItem>();
        _language = "eng";
        _currentBitmapInfo = string.Empty;
        _currentText = string.Empty;
        _startFromNumbers = new ObservableCollection<int>(Enumerable.Range(1, 2));
        _nOcrMaxWrongPixelsList = new ObservableCollection<int>(Enumerable.Range(1, 500));
        _nOcrPixelsAreSpaceList = new ObservableCollection<int>(Enumerable.Range(1, 50));
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
        _fileName = string.Empty;
        _nOcrDrawUnknownText = true;
        _isOkAndCancelActive = true;
        _selectedNOcrMaxWrongPixels = 25;
        _selectedNOcrPixelsAreSpace = 12;
        _tesseractDictionaryItems = new ObservableCollection<TesseractDictionary>();
        _ollamaLanguages = new ObservableCollection<string>(Iso639Dash2LanguageCode.List
            .Select(p => p.EnglishName)
            .OrderBy(p => p));
        _ollamaLanguage = "English";
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        var page = query["Page"].ToString();
        if (page == nameof(MainPage) && OcrSubtitleItems.Count > 0)
        {
            return;
        }

        if (page is nameof(NOcrCharacterInspectPage) or nameof(NOcrDbEditPage))
        {
            return;
        }

        if (query.ContainsKey("Abort") && query["Abort"] is true)
        {
            _cancellationTokenSource.Cancel();
            return;
        }

        var runOcr = false;

        if (page == nameof(MainPage))
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await CheckAndUnpackOcrFiles();
            });

            if (query.ContainsKey("FileName") && query["FileName"] is string fileName)
            {
                _fileName = fileName;
            }

            if (query["Subtitle"] is List<BluRaySupParser.PcsData> bluRaySup && OcrSubtitleItems.Count == 0)
            {
                _ocrSubtitle = new BluRayPcsDataList(bluRaySup); 
                OcrSubtitleItems = new ObservableCollection<OcrSubtitleItem>(_ocrSubtitle.MakeOcrSubtitleItems());
                StartFromNumbers = new ObservableCollection<int>(Enumerable.Range(1, _ocrSubtitle.Count));
                SelectedStartFromNumber = 1;

                foreach (var s in NOcrDb.GetDatabases().OrderBy(p => p))
                {
                    NOcrDatabases.Add(s);
                }
                SelectedNOcrDatabase = NOcrDb.GetDatabases().FirstOrDefault();

                TesseractDictionaryItems = new ObservableCollection<TesseractDictionary>(LoadActiveTesseractDictionaries());
                SelectedTesseractDictionaryItem = TesseractDictionaryItems.FirstOrDefault();

                // load all images in the background
                Task.Run(() =>
                {
                    Parallel.ForEach(OcrSubtitleItems, item =>
                    {
                        if (_cancellationTokenSource.Token.IsCancellationRequested)
                        {
                            return;
                        }

                        item.GetBitmap();
                    });
                });
            }
        }

        var runOnce = false;
        if (query.ContainsKey("RunOnce") && query["RunOnce"] is bool onlyRunOnce)
        {
            runOnce = onlyRunOnce;
        }

        if (query.ContainsKey("NOcrChar") && query["NOcrChar"] is NOcrChar nOcrChar)
        {
            if (!runOnce)
            {
                _nOcrDb?.Add(nOcrChar);
                _nOcrDb?.Save();
            }
            runOcr = true;
        }

        if (query.ContainsKey("OcrSubtitleItems") && query["OcrSubtitleItems"] is List<OcrSubtitleItem> ocrSubtitleItems)
        {
            OcrSubtitleItems = new ObservableCollection<OcrSubtitleItem>(ocrSubtitleItems);
        }

        int? startFromNumber = null;
        if (query.ContainsKey("StartFromNumber") && query["StartFromNumber"] is int startFrom)
        {
            SelectedStartFromNumber = startFrom;
            startFromNumber = startFrom;
        }

        if (query.ContainsKey("ItalicOn") && query["ItalicOn"] is bool toolsItalicOn)
        {
            _toolsItalicOn = toolsItalicOn;
        }

        Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                _loading = false;
                IsRunActive = true;
                LoadSettings();

                SelectedOcrSubtitleItem = OcrSubtitleItems.FirstOrDefault();

                if (runOcr)
                {
                    _isRunningOcr = false;
                    await RunOcr(startFromNumber);
                }
            });
            return false;
        });
    }

    private static List<TesseractDictionary> LoadActiveTesseractDictionaries()
    {
        var folder = Se.TesseractModelFolder;
        if (!Directory.Exists(folder))
        {
            return new List<TesseractDictionary>();
        }

        var allDictionaries = TesseractDictionary.List();
        var items = new List<TesseractDictionary>();
        foreach (var file in Directory.GetFiles(folder, "*.traineddata"))
        {
            var name = Path.GetFileNameWithoutExtension(file);
            if (name == "osd")
            {
                continue;
            }

            var dictionary = allDictionaries.FirstOrDefault(p => p.Code == name);
            if (dictionary != null)
            {
                items.Add(dictionary);
            }
            else
            {
                items.Add(new TesseractDictionary { Code = name, Name = name, Url = string.Empty });
            }
        }

        return items;
    }

    private void LoadSettings()
    {
        var ocr = Se.Settings.Ocr;
        if (!string.IsNullOrEmpty(ocr.Engine) && OcrEngines.Any(p => p.Name == ocr.Engine))
        {
            SelectedOcrEngine = OcrEngines.First(p => p.Name == ocr.Engine);
        }

        if (!string.IsNullOrEmpty(ocr.NOcrDatabase) && NOcrDatabases.Contains(ocr.NOcrDatabase))
        {
            SelectedNOcrDatabase = ocr.NOcrDatabase;
        }

        SelectedNOcrMaxWrongPixels = ocr.NOcrMaxWrongPixels;
        NOcrDrawUnknownText = ocr.NOcrDrawUnknownText;
        SelectedNOcrPixelsAreSpace = ocr.NOcrPixelsAreSpace;
        OllamaModel = ocr.OllamaModel;
        OllamaLanguage = ocr.OllamaLanguage;
    }

    private void SaveSettings()
    {
        var ocr = Se.Settings.Ocr;
        ocr.Engine = SelectedOcrEngine?.Name ?? "nOCR";
        ocr.NOcrDatabase = SelectedNOcrDatabase ?? "Latin";
        ocr.NOcrMaxWrongPixels = SelectedNOcrMaxWrongPixels;
        ocr.NOcrDrawUnknownText = NOcrDrawUnknownText;
        ocr.NOcrPixelsAreSpace = SelectedNOcrPixelsAreSpace;
        ocr.OllamaModel = OllamaModel;
        ocr.OllamaLanguage = OllamaLanguage;
        Se.SaveSettings();
    }

    private static async Task CheckAndUnpackOcrFiles()
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
    private async Task RunOcr(int? startFrom)
    {
        if (_isRunningOcr)
        {
            return;
        }

        if (!(SelectedOcrEngine is { } ocrEngine))
        {
            return;
        }

        if (ocrEngine.EngineType == OcrEngineType.Tesseract)
        {
            var tesseractOk = await CheckAndDownloadTesseract();
            if (!tesseractOk)
            {
                return;
            }

            if (SelectedTesseractDictionaryItem == null)
            {
                var tesseractModelOk = await TesseractModelDownload();
                if (!tesseractModelOk)
                {
                    return;
                }
            }
        }

        SaveSettings();
        _cancellationTokenSource = new CancellationTokenSource();
        _isRunningOcr = true;
        var startFromIndex = (startFrom ?? SelectedStartFromNumber) - 1;
        IsProgressVisible = true;
        ProgressText = "Running OCR...";
        ProgressValue = 0d;

        IsPauseActive = true;
        IsRunActive = false;
        IsOkAndCancelActive = false;
        IsInspectActive = false;

        if (ocrEngine.EngineType == OcrEngineType.nOcr)
        {
            RunNOcr(startFromIndex);
        }
        else if (ocrEngine.EngineType == OcrEngineType.Tesseract)
        {
            RunTesseractOcr(startFromIndex);
        }
        else if (ocrEngine.EngineType == OcrEngineType.Ollama)
        {
            RunOllamaOcr(startFromIndex);
        }
    }

    private void RunTesseractOcr(int startFromIndex)
    {
        var tesseractOcr = new TesseractOcr();
        var language = SelectedTesseractDictionaryItem?.Code ?? "eng";

        _ = Task.Run(async () =>
        {
            for (var i = startFromIndex; i < OcrSubtitleItems.Count; i++)
            {
                if (_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    return;
                }

                ProgressValue = i / (double)OcrSubtitleItems.Count;
                ProgressText = $"Running OCR... {i + 1}/{OcrSubtitleItems.Count}";
                SelectedStartFromNumber = i + 1;

                var item = OcrSubtitleItems[i];
                var bitmap = item.GetBitmap();

                var scrollToIndex = i;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ListView.ScrollTo(scrollToIndex, -1, ScrollToPosition.MakeVisible, false);
                    SelectedOcrSubtitleItem = item;
                    ListView.Focus();
                    ListView.SelectedItem = item;
                    ListView.UpdateSelectedItems(new List<object> { item });
                });


                var text = await tesseractOcr.Ocr(bitmap, language, _cancellationTokenSource.Token);
                item.Text = text;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (SelectedOcrSubtitleItem == item)
                    {
                        CurrentText = text;
                    }
                });
            }

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                SelectedStartFromNumber = OcrSubtitleItems.Count;
                await Pause();
            });
        });

    }

    private void RunOllamaOcr(int startFromIndex)
    {
        var ollamaOcr = new OllamaOcr();

        _ = Task.Run(async () =>
        {
            for (var i = startFromIndex; i < OcrSubtitleItems.Count; i++)
            {
                if (_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    return;
                }

                ProgressValue = i / (double)OcrSubtitleItems.Count;
                ProgressText = $"Running OCR... {i + 1}/{OcrSubtitleItems.Count}";
                SelectedStartFromNumber = i + 1;

                var item = OcrSubtitleItems[i];
                var bitmap = item.GetBitmap();

                var scrollToIndex = i;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ListView.ScrollTo(scrollToIndex, -1, ScrollToPosition.MakeVisible, false);
                    SelectedOcrSubtitleItem = item;
                    ListView.Focus();
                    ListView.SelectedItem = item;
                    ListView.UpdateSelectedItems(new List<object> { item });
                });


                var text = await ollamaOcr.Ocr(bitmap, OllamaModel, OllamaLanguage, _cancellationTokenSource.Token);
                item.Text = text;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (SelectedOcrSubtitleItem == item)
                    {
                        CurrentText = text;
                    }
                });
            }

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                SelectedStartFromNumber = OcrSubtitleItems.Count;
                await Pause();
            });
        });
    }

    private void RunNOcr(int startFromIndex)
    {
        if (!InitNOcrDb())
        {
            return;
        }

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
                SelectedStartFromNumber = i + 1;

                var item = OcrSubtitleItems[i];
                var bitmap = item.GetBitmap();
                var nBmp = new NikseBitmap2(bitmap);
                nBmp.MakeTwoColor(200);
                nBmp.CropTop(0, new SKColor(0, 0, 0, 0));
                var list = NikseBitmapImageSplitter2.SplitBitmapToLettersNew(nBmp, SelectedNOcrPixelsAreSpace, false, true, 20, true);
                var sb = new StringBuilder();
                SelectedOcrSubtitleItem = item;

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
                        var match = _nOcrDb!.GetMatch(splitterItem.NikseBitmap, splitterItem.Top, true, SelectedNOcrMaxWrongPixels);

                        if (NOcrDrawUnknownText && match == null)
                        {
                            MainThread.BeginInvokeOnMainThread(async () =>
                            {
                                await Pause();
                                await Shell.Current.GoToAsync(nameof(NOcrCharacterAddPage), new Dictionary<string, object>
                                    {
                                    { "Page", nameof(OcrPage) },
                                    { "Bitmap", bitmap },
                                    { "Letters", list },
                                    { "Item", splitterItem },
                                    { "OcrSubtitleItems", OcrSubtitleItems.ToList() },
                                    { "StartFromNumber", SelectedStartFromNumber },
                                    { "ItalicOn", _toolsItalicOn },
                                    });
                            });
                            return;
                        }

                        sb.Append(match != null ? _nOcrCaseFixer.FixUppercaseLowercaseIssues(splitterItem, match) : "*");
                    }
                }

                item.Text = sb.ToString().Trim();

                var scrollToIndex = i;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ListView.ScrollTo(scrollToIndex, -1, ScrollToPosition.MakeVisible, false);
                });
            }

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                SelectedStartFromNumber = OcrSubtitleItems.Count;
                await Pause();
            });
        });
    }

    private bool InitNOcrDb()
    {
        var fileName = GetNOcrLanguageFileName();
        if (_nOcrDb != null && _nOcrDb.FileName == fileName)
        {
            return true;
        }

        var nOcrDbFileName = GetNOcrLanguageFileName();
        if (nOcrDbFileName == null)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(nOcrDbFileName) && (_nOcrDb == null || _nOcrDb.FileName != nOcrDbFileName))
        {
            _nOcrDb = new NOcrDb(nOcrDbFileName);
        }

        return true;
    }

    [RelayCommand]
    private async Task NOcrAction()
    {
        await _cancellationTokenSource.CancelAsync();

        var result = await _popupService.ShowPopupAsync<NOcrDbActionPopupModel>(onPresenting: viewModel => viewModel.Initialize(SelectedNOcrDatabase, NOcrDatabases), CancellationToken.None);

        if (result is string action)
        {
            if (action == "delete")
            {
                var nOcrDbFileName = GetNOcrLanguageFileName();
                if (nOcrDbFileName != null)
                {
                    File.Delete(nOcrDbFileName);
                    NOcrDatabases.Remove(SelectedNOcrDatabase);
                    SelectedNOcrDatabase = NOcrDatabases.FirstOrDefault();
                    if (NOcrDatabases.Count == 0)
                    {
                        _nOcrDb = new NOcrDb(Path.Combine(Se.OcrFolder, "New.nocr"));
                        _nOcrDb.Save();
                        NOcrDatabases.Add("New");
                        SelectedNOcrDatabase = "New";
                    }
                }
            }
            else if (action == "edit")
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    if (!InitNOcrDb())
                    {
                        return;
                    }

                    await Pause();
                    await Shell.Current.GoToAsync(nameof(NOcrDbEditPage), new Dictionary<string, object>
                    {
                        { "Page", nameof(OcrPage) },
                        { "nOcrDb", _nOcrDb! },
                    });
                });
            }
            else if (action.StartsWith("new:"))
            {
                var newName = action.Substring(4).Trim();
                var nOcrDbFileName = Path.Combine(Se.OcrFolder, $"{newName}.nocr");
                if (!File.Exists(nOcrDbFileName))
                {
                    _nOcrDb = new NOcrDb(nOcrDbFileName);
                    _nOcrDb.Save();
                    NOcrDatabases.Add(newName);
                    SelectedNOcrDatabase = newName;
                }
            }
        }
    }

    [RelayCommand]
    private async Task Pause()
    {
        await _cancellationTokenSource.CancelAsync();

        _isRunningOcr = false;
        IsPauseActive = false;
        IsRunActive = true;
        IsProgressVisible = false;
        IsOkAndCancelActive = true;
        IsInspectActive = true;
    }

    [RelayCommand]
    private async Task TesseractDictionaryDownload()
    {
        await TesseractModelDownload();
    }

    private async Task<bool> TesseractModelDownload()
    {
        var result = await _popupService.ShowPopupAsync<TesseractDictionaryDownloadPopupModel>(
            onPresenting: vm => { vm.PickLanguage(TesseractDictionaryItems.ToList()); }, CancellationToken.None);

        if (result is TesseractDictionary model)
        {
            TesseractDictionaryItems = new ObservableCollection<TesseractDictionary>(LoadActiveTesseractDictionaries());
            SelectedTesseractDictionaryItem = TesseractDictionaryItems.FirstOrDefault(p => p.Code == model.Code);
            Se.SaveSettings();
            return true;
        }

        return false;
    }

    [RelayCommand]
    private async Task OllamaModelPick()
    {
        var result = await _popupService.ShowPopupAsync<PickerPopupModel>(
            onPresenting: vm =>
            {
                vm.SetItems(Se.Settings.Ocr.OllamaModels, OllamaModel);
                vm.SetSelectedItem(OllamaModel);
            }, CancellationToken.None);

        if (result is string model)
        {
            OllamaModel = model;
            Se.Settings.Ocr.OllamaModel = model;
            Se.SaveSettings();
        }
    }

    [RelayCommand]
    private void Inspect()
    {
        var item = SelectedOcrSubtitleItem;
        if (item == null)
        {
            return;
        }

        if (!InitNOcrDb())
        {
            return;
        }

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Pause();

            var item = SelectedOcrSubtitleItem;
            if (item == null)
            {
                return;
            }

            var bitmap = item.GetBitmap();
            var nBmp = new NikseBitmap2(bitmap);
            nBmp.MakeTwoColor(200);
            nBmp.CropTop(0, new SKColor(0, 0, 0, 0));
            var list = NikseBitmapImageSplitter2.SplitBitmapToLettersNew(nBmp, SelectedNOcrPixelsAreSpace, false, true, 20, true);
            var letters = new List<NOcrCharacterInspectPageModel.LetterItem>();
            var matches = new List<NOcrChar>();
            foreach (var letter in list)
            {
                var match = new NOcrChar(letter.SpecialCharacter ?? string.Empty);

                if (letter.NikseBitmap != null)
                {
                    match = _nOcrDb!.GetMatch(letter.NikseBitmap, letter.Top, true, SelectedNOcrMaxWrongPixels);
                    if (match == null)
                    {
                        match = new NOcrChar(letter.SpecialCharacter ?? "*");
                    }
                    matches.Add(match);
                }

                letters.Add(new NOcrCharacterInspectPageModel.LetterItem(letter, letter.NikseBitmap?.GetBitmap().ToImageSource(), match.Text, match));
            }

            await Shell.Current.GoToAsync(nameof(NOcrCharacterInspectPage), new Dictionary<string, object>
            {
                { "Page", nameof(OcrPage) },
                { "Letters", letters },
                { "Matches", matches },
                { "OcrSubtitleItem", item },
                { "nOcrDb", _nOcrDb ?? new NOcrDb(Path.Combine(Se.OcrFolder, "new.nocr")) },
            });
        });
    }

    [RelayCommand]
    private async Task Ok()
    {
        var subtitle = new Subtitle();
        foreach (var item in OcrSubtitleItems)
        {
            var start = item.StartTime;
            var end = item.EndTime;
            var text = item.Text;
            subtitle.Paragraphs.Add(new Paragraph(text, start.TotalMilliseconds, end.TotalMilliseconds));
        }

        await Shell.Current.GoToAsync("..", new Dictionary<string, object>
        {
            { "Page", nameof(OcrPage) },
            { "Subtitle", subtitle },
            { "FileName", _fileName },
            { "Status", "" },
        });
    }

    [RelayCommand]
    public async Task Cancel()
    {
        await Pause();
        await Shell.Current.GoToAsync("..", new Dictionary<string, object>
        {
            { "Page", nameof(OcrPage) },
        });
    }

    public void OnCollectionViewSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (SelectedOcrSubtitleItem == null)
        {
            IsInspectActive = false;
            return;
        }

        var bitmap = SelectedOcrSubtitleItem.GetBitmap();
        CurrentImageSource = bitmap.ToImageSource();
        CurrentBitmapInfo = $"Image {SelectedOcrSubtitleItem.Number} of {_ocrSubtitle.Count}: {bitmap.Width}x{bitmap.Height}";
        SelectedStartFromNumber = SelectedOcrSubtitleItem.Number;
        CurrentText = SelectedOcrSubtitleItem.Text;

        if (!_isRunningOcr)
        {
            IsInspectActive = true;
        }
    }

    public void OnOcrEngineChanged(object? sender, EventArgs e)
    {
        if (sender is Picker { SelectedItem: OcrEngineItem engine })
        {
            IsNOcrVisible = engine.EngineType == OcrEngineType.nOcr;
            IsTesseractVisible = engine.EngineType == OcrEngineType.Tesseract;
            IsInspectVisible = engine.EngineType == OcrEngineType.nOcr;
            IsOllamaOcrVisible = engine.EngineType == OcrEngineType.Ollama;

            if (engine.EngineType == OcrEngineType.nOcr)
            {
                InitNOcrDb();
            }
        }
    }

    private async Task<bool> CheckAndDownloadTesseract()
    {
        if (!Directory.Exists(Se.TesseractFolder)) //TODO: check executable file name
        {
            var answer = await Page!.DisplayAlert(
                "Download Tesseract OCR?",
                $"{Environment.NewLine}\"Tesseract\" requires downloading Tesseract OCR.{Environment.NewLine}{Environment.NewLine}Download and use Tesseract OCR?",
                "Yes",
                "No");

            if (!answer)
            {
                return false;
            }

            var result = await _popupService.ShowPopupAsync<TesseractDownloadPopupModel>(onPresenting: viewModel => viewModel.StartDownload(), CancellationToken.None);
            if (result is string tesseractFolder)
            {
                return true;
            }
        }

        return true;
    }
}
