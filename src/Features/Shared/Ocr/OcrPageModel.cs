﻿using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.BluRaySup;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.VobSub.Ocr.Service;
using SkiaSharp;
using SubtitleAlchemist.Controls.PickerControl;
using SubtitleAlchemist.Features.Main;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Logic.Ocr;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace SubtitleAlchemist.Features.Shared.Ocr;

public partial class OcrPageModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    public partial ObservableCollection<OcrEngineItem> OcrEngines { get; set; }

    [ObservableProperty]
    public partial OcrEngineItem? SelectedOcrEngine { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<OcrSubtitleItem> OcrSubtitleItems { get; set; }

    [ObservableProperty]
    public partial OcrSubtitleItem? SelectedOcrSubtitleItem { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<int> StartFromNumbers { get; set; }

    [ObservableProperty]
    public partial int SelectedStartFromNumber { get; set; }

    [ObservableProperty]
    public partial ImageSource? CurrentImageSource { get; set; }

    [ObservableProperty]
    public partial string CurrentBitmapInfo { get; set; }

    [ObservableProperty]
    public partial string CurrentText { get; set; }

    [ObservableProperty]
    public partial bool IsRunActive { get; set; }

    [ObservableProperty]
    public partial bool IsPauseActive { get; set; }

    [ObservableProperty]
    public partial bool IsOkAndCancelActive { get; set; }

    [ObservableProperty]
    public partial bool IsInspectVisible { get; set; }

    [ObservableProperty]
    public partial bool IsInspectActive { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<string> NOcrDatabases { get; set; }

    [ObservableProperty]
    public partial string? SelectedNOcrDatabase { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<int> NOcrMaxWrongPixelsList { get; set; }

    [ObservableProperty]
    public partial int SelectedNOcrMaxWrongPixels { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<int> NOcrPixelsAreSpaceList { get; set; }

    [ObservableProperty]
    public partial int SelectedNOcrPixelsAreSpace { get; set; }

    [ObservableProperty]
    public partial string ProgressText { get; set; }

    [ObservableProperty]
    public partial double ProgressValue { get; set; }

    [ObservableProperty]
    public partial bool IsProgressVisible { get; set; }

    [ObservableProperty]
    public partial bool NOcrDrawUnknownText { get; set; }

    [ObservableProperty]
    public partial string OllamaModel { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<string> OllamaLanguages { get; set; }

    [ObservableProperty]
    public partial string OllamaLanguage { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<OcrLanguage> GoogleVisionLanguages { get; set; }

    [ObservableProperty]
    public partial OcrLanguage? SelectedGoogleVisionLanguage { get; set; }

    [ObservableProperty]
    public partial string GoogleVisionApiKey { get; set; }

    [ObservableProperty]
    public partial string GoogleVisionLanguage { get; set; }

    [ObservableProperty]
    public partial bool IsNOcrVisible { get; set; }

    [ObservableProperty]
    public partial bool IsPaddleOcrOcrVisible { get; set; }

    [ObservableProperty]
    public partial bool IsOllamaOcrVisible { get; set; }

    [ObservableProperty]
    public partial bool IsTesseractVisible { get; set; }

    [ObservableProperty]
    public partial bool IsGoogleVisionVisible { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<TesseractDictionary> TesseractDictionaryItems { get; set; }

    [ObservableProperty]
    public partial TesseractDictionary? SelectedTesseractDictionaryItem { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<OcrLanguage2> PaddleLanguageItems { get; set; }

    [ObservableProperty]
    public partial OcrLanguage2? SelectedPaddleLanguageItem { get; set; }

    [ObservableProperty]
    public partial bool PaddleUseGpu { get; set; }
    public OcrPage? Page { get; set; }
    public CollectionView ListView { get; set; }

    private bool _isRunningOcr;
    private IOcrSubtitle _ocrSubtitle;
    private readonly INOcrCaseFixer _nOcrCaseFixer;
    private string _fileName;
    private CancellationTokenSource _cancellationTokenSource;
    private NOcrDb? _nOcrDb;
    private bool _toolsItalicOn;
    private readonly IPopupService _popupService;
    private readonly List<SkipOnceChar> _runOnceChars;
    private readonly List<SkipOnceChar> _skipOnceChars;

    public OcrPageModel(INOcrCaseFixer nOcrCaseFixer, IPopupService popupService)
    {
        OllamaModel = string.Empty;
        _nOcrCaseFixer = nOcrCaseFixer;
        _popupService = popupService;
        OcrEngines = new ObservableCollection<OcrEngineItem>(OcrEngineItem.GetOcrEngines());
        SelectedOcrEngine = OcrEngines.FirstOrDefault();
        _ocrSubtitle = new BluRayPcsDataList(new List<BluRaySupParser.PcsData>());
        OcrSubtitleItems = new ObservableCollection<OcrSubtitleItem>();
        CurrentBitmapInfo = string.Empty;
        CurrentText = string.Empty;
        StartFromNumbers = new ObservableCollection<int>(Enumerable.Range(1, 2));
        NOcrMaxWrongPixelsList = new ObservableCollection<int>(Enumerable.Range(1, 500));
        NOcrPixelsAreSpaceList = new ObservableCollection<int>(Enumerable.Range(1, 50));
        SelectedStartFromNumber = 1;
        NOcrDatabases = new ObservableCollection<string>();
        ListView = new CollectionView();
        ProgressText = string.Empty;
        ProgressValue = 0d;
        IsProgressVisible = false;
        _cancellationTokenSource = new CancellationTokenSource();
        _isRunningOcr = false;
        _nOcrDb = null;
        _fileName = string.Empty;
        NOcrDrawUnknownText = true;
        IsOkAndCancelActive = true;
        SelectedNOcrMaxWrongPixels = 25;
        SelectedNOcrPixelsAreSpace = 12;
        TesseractDictionaryItems = new ObservableCollection<TesseractDictionary>();
        OllamaLanguages = new ObservableCollection<string>(Iso639Dash2LanguageCode.List
            .Select(p => p.EnglishName)
            .OrderBy(p => p));
        OllamaLanguage = "English";
        GoogleVisionLanguages = new ObservableCollection<OcrLanguage>();
        GoogleVisionApiKey = string.Empty;
        GoogleVisionLanguage = string.Empty;
        _runOnceChars = new List<SkipOnceChar>();
        _skipOnceChars = new List<SkipOnceChar>();
        PaddleLanguageItems = new ObservableCollection<OcrLanguage2>(PaddleOcr.GetLanguages());
        SelectedPaddleLanguageItem = PaddleLanguageItems.FirstOrDefault(p => p.Code == "en");
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

            MainThread.BeginInvokeOnMainThread(() =>
            {
                var scrollToIndex = SelectedStartFromNumber - 1;
                ListView.ScrollTo(scrollToIndex, -1, ScrollToPosition.Center, true);
                ListView.Focus();
            });

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
                GoogleVisionLanguages = new ObservableCollection<OcrLanguage>(GoogleVisionOcr.GetLanguages().OrderBy(p => p.ToString()));

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

        int? startFromNumber = null;
        if (query.ContainsKey("StartFromNumber") && query["StartFromNumber"] is int startFrom)
        {
            SelectedStartFromNumber = startFrom;
            startFromNumber = startFrom;
        }

        int? letterIndex = null;
        if (query.ContainsKey("LetterIndex") && query["LetterIndex"] is int letterIndexValue)
        {
            letterIndex = letterIndexValue;
        }

        NOcrChar? nOcrChar = null;
        if (query.ContainsKey("NOcrChar") && query["NOcrChar"] is NOcrChar nOcrCharValue)
        {
            nOcrChar = nOcrCharValue;
            runOcr = true;
        }

        var useOnce = false;
        if (query.ContainsKey("UseOnce") && query["UseOnce"] is bool useOnceValue &&
            letterIndex != null &&
            startFromNumber != null)
        {
            useOnce = useOnceValue;
            if (useOnce && nOcrChar != null)
            {
                _runOnceChars.Add(new SkipOnceChar(startFromNumber.Value - 1, letterIndex.Value, nOcrChar.Text));
            }
        }

        var skipOnce = false;
        if (query.ContainsKey("Skip") && query["Skip"] is bool doSkip &&
            letterIndex != null &&
            startFromNumber != null)
        {
            skipOnce = doSkip;
            if (skipOnce)
            {
                _skipOnceChars.Add(new SkipOnceChar(startFromNumber.Value - 1, letterIndex.Value));
            }
        }

        if (query.ContainsKey("OcrSubtitleItems") && query["OcrSubtitleItems"] is List<OcrSubtitleItem> ocrSubtitleItems)
        {
            OcrSubtitleItems = new ObservableCollection<OcrSubtitleItem>(ocrSubtitleItems);
        }

        if (query.ContainsKey("ItalicOn") && query["ItalicOn"] is bool toolsItalicOn)
        {
            _toolsItalicOn = toolsItalicOn;
        }

        Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                IsRunActive = true;
                LoadSettings();


                await Task.Delay(50);
                ListView.Focus();
                SelectedOcrSubtitleItem = OcrSubtitleItems.FirstOrDefault();
                OnPropertyChanged(nameof(SelectedOcrSubtitleItem));

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
        GoogleVisionApiKey = ocr.GoogleVisionApiKey;
        SelectedGoogleVisionLanguage = GoogleVisionLanguages.FirstOrDefault(p => p.Code == ocr.GoogleVisionLanguage);
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
        ocr.GoogleVisionApiKey = GoogleVisionApiKey;
        ocr.GoogleVisionLanguage = SelectedGoogleVisionLanguage?.Code ?? "en";
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
        else if (ocrEngine.EngineType == OcrEngineType.PaddleOcr)
        {
            if (Page != null && !Directory.Exists(Se.PaddleOcrFolder))
            {
                var answer = await Page.DisplayAlert(
                    "Download Paddle OCR models?",
                    $"{Environment.NewLine}\"Paddle OCR\" requires AI model files.{Environment.NewLine}{Environment.NewLine}Download and use Paddle OCR models?",
                    "Yes",
                    "No");

                if (!answer)
                {
                    await Pause();
                    return;
                }

                var result = await _popupService.ShowPopupAsync<DownloadPaddleOcrModelsPopupModel>(CancellationToken.None);
                if (result is not string)
                {
                    await Pause();
                    return;
                }
            }

            //  RunPaddleOcr(startFromIndex);
            RunPaddleOcrBatch(startFromIndex, 10);
        }
        else if (ocrEngine.EngineType == OcrEngineType.Ollama)
        {
            RunOllamaOcr(startFromIndex);
        }
        else if (ocrEngine.EngineType == OcrEngineType.Ollama)
        {
            RunGoogleVisionOcr(startFromIndex);
        }
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
                        var match = _nOcrDb!.GetMatch(nBmp, list, splitterItem, splitterItem.Top, true, SelectedNOcrMaxWrongPixels);

                        if (NOcrDrawUnknownText && match == null)
                        {
                            var letterIndex = list.IndexOf(splitterItem);

                            if (_skipOnceChars.Any(p => p.LetterIndex == letterIndex && p.LineIndex == i))
                            {
                                sb.Append("*");
                                continue;
                            }

                            var runOnceChar = _runOnceChars.FirstOrDefault(p => p.LetterIndex == letterIndex && p.LineIndex == i);
                            if (runOnceChar != null)
                            {
                                sb.Append(runOnceChar.Text);
                                continue;
                            }

                            MainThread.BeginInvokeOnMainThread(async () =>
                            {
                                await Pause();
                                await Shell.Current.GoToAsync(nameof(NOcrCharacterAddPage), new Dictionary<string, object>
                                    {
                                    { "Page", nameof(OcrPage) },
                                    { "Bitmap", nBmp.GetBitmap() },
                                    { "Letters", list },
                                    { "Item", splitterItem },
                                    { "OcrSubtitleItems", OcrSubtitleItems.ToList() },
                                    { "StartFromNumber", SelectedStartFromNumber },
                                    { "ItalicOn", _toolsItalicOn },
                                    { "nOcrDb", _nOcrDb },
                                    { "MaxWrongPixels", SelectedNOcrMaxWrongPixels },
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

                _runOnceChars.Clear();
                _skipOnceChars.Clear();
            }

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                SelectedStartFromNumber = OcrSubtitleItems.Count;
                await Pause();
            });
        });
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

    private void RunPaddleOcr(int startFromIndex)
    {
        var ocrEngine = new PaddleOcr();
        var language = SelectedPaddleLanguageItem?.Code ?? "en";

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


                var text = await ocrEngine.Ocr(bitmap, language, PaddleUseGpu, _cancellationTokenSource.Token);
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

    private Lock BatchLock = new Lock();

    private void RunPaddleOcrBatch(int startFromIndex, int batchSize)
    {
        var ocrEngine = new PaddleOcr();
        var language = SelectedPaddleLanguageItem?.Code ?? "en";
        var batchImages = new List<PaddleOcrBatchInput>(batchSize);
        var max = -1;

        var ocrProgress = new Progress<PaddleOcrBatchProgress>(p =>
        {
            if (_cancellationTokenSource.Token.IsCancellationRequested)
            {
                return;
            }   

            lock (BatchLock)
            {
                var number = p.Index;
                if (max < number)
                {
                    max = number;
                }
                else
                {
                    return;
                }

                var percentage = (int)Math.Round(number * 100.0, MidpointRounding.AwayFromZero);
                var pctString = percentage.ToString(CultureInfo.InvariantCulture);
                ProgressValue = number / (double)OcrSubtitleItems.Count;
                ProgressText = $"Running OCR... {number + 1}/{OcrSubtitleItems.Count}";

                var scrollToIndex = number;
                var item = p.Item;
                if (item == null)
                {
                    item = OcrSubtitleItems[p.Index];
                }

                item.Text = p.Text;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ListView.ScrollTo(scrollToIndex, -1, ScrollToPosition.MakeVisible, false);
                    SelectedOcrSubtitleItem = item;
                    ListView.Focus();
                    ListView.SelectedItem = item;
                    ListView.UpdateSelectedItems(new List<object> { item });
                });
            }
        });

        _ = Task.Run(async () =>
        {
            var i = startFromIndex;
            for (; i < OcrSubtitleItems.Count; i++)
            {
                if (_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    return;
                }

                if (batchImages.Count >= batchSize)
                {
                    await ocrEngine.OcrBatch(batchImages, language, PaddleUseGpu, ocrProgress, _cancellationTokenSource.Token);
                    batchImages.Clear();
                }

                var item = OcrSubtitleItems[i];
                var bitmap = item.GetBitmap();

                if (item == null)
                {
                }
                else
                {
                    var paddleOcrBatchInput = new PaddleOcrBatchInput()
                    {
                        Bitmap = bitmap,
                        Index = i,
                        Item = item,
                    };
                    batchImages.Add(paddleOcrBatchInput);
                }
            }

            if (batchImages.Count > 0)
            {
                await ocrEngine.OcrBatch(batchImages, language, PaddleUseGpu, ocrProgress, _cancellationTokenSource.Token);
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

    private void RunGoogleVisionOcr(int startFromIndex)
    {
        var googleVisionOcr = new GoogleVisionOcr();

        if (SelectedGoogleVisionLanguage is not { } language)
        {
            return;
        }

        if (GoogleVisionApiKey is not { } apiKey)
        {
            return;
        }

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


                var text = await googleVisionOcr.Ocr(bitmap, apiKey, language.Code, _cancellationTokenSource.Token);
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
                    if (SelectedNOcrDatabase != null)
                    {
                        NOcrDatabases.Remove(SelectedNOcrDatabase);
                    }

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
                    match = _nOcrDb!.GetMatch(nBmp, list, letter, letter.Top, true, SelectedNOcrMaxWrongPixels);
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
            IsPaddleOcrOcrVisible = engine.EngineType == OcrEngineType.PaddleOcr;
            IsOllamaOcrVisible = engine.EngineType == OcrEngineType.Ollama;
            IsGoogleVisionVisible = engine.EngineType == OcrEngineType.GoogleVision;

            if (engine.EngineType == OcrEngineType.nOcr)
            {
                InitNOcrDb();
            }
        }
    }

    private async Task<bool> CheckAndDownloadTesseract()
    {
        var tesseractExe = Path.Combine(Se.TesseractFolder, "tesseract.exe");

        if (!File.Exists(tesseractExe)) //TODO: check for mac/Linux executable on mac/Linux
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

            return false;
        }

        return true;
    }
}
