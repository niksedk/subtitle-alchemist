using System.Globalization;
using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Services;

namespace SubtitleAlchemist.Features.Shared.Ocr;

public partial class TesseractDownloadPopupModel : ObservableObject
{
    public TesseractDownloadPopup? Popup { get; set; }

    private readonly ITesseractDownloadService _tesseractDownloadService;

    [ObservableProperty]
    public partial float ProgressValue { get; set; }

    [ObservableProperty]
    public partial string Progress { get; set; }

    [ObservableProperty]
    public partial string Error { get; set; }

    private Task? _downloadTask;
    private readonly System.Timers.Timer _timer = new();

    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly MemoryStream _downloadStream;
    private readonly IZipUnpacker _zipUnpacker;

    public TesseractDownloadPopupModel(IZipUnpacker zipUnpacker, ITesseractDownloadService tesseractDownloadService)
    {
        _zipUnpacker = zipUnpacker;
        _tesseractDownloadService = tesseractDownloadService;

        _cancellationTokenSource = new CancellationTokenSource();

        _downloadStream = new MemoryStream();
        Progress = "Starting...";
        Error = string.Empty;

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

            UnpackTesseract(GetTesseractFolder());

            if (Popup != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Popup.Close(GetTesseractFolder());
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
        _downloadStream.Position = 0;
        _zipUnpacker.UnpackZipStream(_downloadStream, folder);
        _downloadStream.Dispose();
    }

    public static string GetTesseractFolder()
    {
        return Se.TesseractFolder;
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

    public void StartDownload()
    {
        var downloadProgress = new Progress<float>(number =>
        {
            var percentage = (int)Math.Round(number * 100.0, MidpointRounding.AwayFromZero);
            var pctString = percentage.ToString(CultureInfo.InvariantCulture);
            ProgressValue = number;
            Progress = $"Downloading... {pctString}%";
        });

        var folder = GetTesseractFolder();
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        _downloadTask = _tesseractDownloadService.DownloadTesseract(_downloadStream, downloadProgress, _cancellationTokenSource.Token);
    }
}
