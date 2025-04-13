using System.Collections.ObjectModel;
using System.Globalization;
using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Services;

namespace SubtitleAlchemist.Features.Shared.Ocr;

public partial class TesseractDictionaryDownloadPopupModel : ObservableObject
{
    public TesseractDictionaryDownloadPopup? Popup { get; set; }

    private readonly ITesseractDownloadService _tesseractDownloadService;

    [ObservableProperty]
    public partial ObservableCollection<TesseractDictionary> TesseractDictionaryItems { get; set; }

    [ObservableProperty]
    public partial TesseractDictionary? SelectedTesseractDictionaryItem { get; set; }

    [ObservableProperty]
    public partial float ProgressValue { get; set; }

    [ObservableProperty]
    public partial string Progress { get; set; }

    [ObservableProperty]
    public partial bool IsProgressVisible { get; set; }

    [ObservableProperty]
    public partial bool IsPickerAndDownloadButtonEnabled { get; set; }

    [ObservableProperty]
    public partial string Error { get; set; }

    private Task? _downloadTask;
    private readonly System.Timers.Timer _timer = new();

    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly MemoryStream _downloadStream;

    public TesseractDictionaryDownloadPopupModel(ITesseractDownloadService tesseractDownloadService)
    {
        _tesseractDownloadService = tesseractDownloadService;

        _cancellationTokenSource = new CancellationTokenSource();

        _downloadStream = new MemoryStream();
        TesseractDictionaryItems = new ObservableCollection<TesseractDictionary>(TesseractDictionary.List().OrderBy(p => p.ToString()));
        SelectedTesseractDictionaryItem = TesseractDictionaryItems.FirstOrDefault(p => p.Code == "eng");
        Progress = "Starting...";
        Error = string.Empty;
        IsPickerAndDownloadButtonEnabled = true;

        _timer.Interval = 500;
        _timer.Elapsed += OnTimerOnElapsed;
        _timer.Start();
    }

    private void OnTimerOnElapsed(object? sender, ElapsedEventArgs args)
    {
        if (_downloadTask is { IsCompleted: true })
        {
            _timer.Stop();

            if (_downloadStream.Length == 0)
            {
                Progress = "Download failed";
                Error = "No data received";
                return;
            }

            UnpackTesseract(GetTesseractModelFolder());

            if (Popup != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Popup.Close(SelectedTesseractDictionaryItem);
                });
            }
        }
        else if (_downloadTask is { IsFaulted: true })
        {
            _timer.Stop();
            var ex = _downloadTask.Exception?.InnerException ?? _downloadTask.Exception;
            if (ex is OperationCanceledException)
            {
                Progress = "Download canceled";
                Close();
            }
            else
            {
                Progress = "Download failed";
                Error = ex?.Message ?? "Unknown error";
            }
        }
    }

    private void UnpackTesseract(string folder)
    {
        if (!(SelectedTesseractDictionaryItem is { } model))
        {
            return;
        }

        _downloadStream.Position = 0;
        var fileName = Path.Combine(folder, model.Code + ".traineddata");
        File.WriteAllBytes(fileName, _downloadStream.ToArray());
        _downloadStream.Dispose();
    }

    public static string GetTesseractModelFolder()
    {
        return Se.TesseractModelFolder;
    }

    private void Close()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Popup?.Close();
        });
    }

    [RelayCommand]
    public void Cancel()
    {
        _cancellationTokenSource.Cancel();
        Close();
    }

    [RelayCommand]
    public void Download()
    {
        IsPickerAndDownloadButtonEnabled = false;
        IsProgressVisible = true;
        StartDownload();
    }

    public void StartDownload()
    {
        if (!(SelectedTesseractDictionaryItem is { } model))
        {
            return;
        }

        var downloadProgress = new Progress<float>(number =>
        {
            var percentage = (int)Math.Round(number * 100.0, MidpointRounding.AwayFromZero);
            var pctString = percentage.ToString(CultureInfo.InvariantCulture);
            ProgressValue = number;
            Progress = $"Downloading... {pctString}%";
        });

        var folder = GetTesseractModelFolder();
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        _downloadTask = _tesseractDownloadService.DownloadTesseractModel(model.Url, _downloadStream, downloadProgress, _cancellationTokenSource.Token);
    }

    public void PickLanguage(List<TesseractDictionary> downloadedModels)
    {
        if (downloadedModels.All(p => p.Code != "eng"))
        {
            SelectedTesseractDictionaryItem = TesseractDictionaryItems.FirstOrDefault(p => p.Code == "eng");
            return;
        }

        var culture = CultureInfo.CurrentCulture;
        var lang = culture.Name.Split('-').LastOrDefault();
        if (string.IsNullOrEmpty(lang))
        {
            return;
        }

        var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
        var uiCulture = cultures.FirstOrDefault(p => p.Name.EndsWith(lang));
        if (uiCulture == null)
        {
            return;
        }

        var model = TesseractDictionaryItems.FirstOrDefault(p =>
            p.Code.Contains(uiCulture.ThreeLetterISOLanguageName, StringComparison.InvariantCultureIgnoreCase));

        if (model != null)
        {
            SelectedTesseractDictionaryItem = model;
        }
    }
}
