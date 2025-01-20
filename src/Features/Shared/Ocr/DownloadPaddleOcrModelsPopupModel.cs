using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Services;
using System.Globalization;
using System.Timers;
using SubtitleAlchemist.Logic.Config;
using Timer = System.Timers.Timer;

namespace SubtitleAlchemist.Features.Shared.Ocr;

public partial class DownloadPaddleOcrModelsPopupModel : ObservableObject
{
    public DownloadPaddleOcrModelsPopup? Popup { get; set; }

    private readonly IPaddleOcrDownloadService _paddleOcrDownloadService;

    [ObservableProperty]
    private float _progressValue;

    [ObservableProperty]
    private string _progress;

    [ObservableProperty]
    private string _error;

    private Task? _downloadTask;
    private readonly Timer _timer = new();

    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly MemoryStream _downloadStream;
    private readonly IZipUnpacker _zipUnpacker;

    public DownloadPaddleOcrModelsPopupModel(IPaddleOcrDownloadService paddleOcrDownloadService, IZipUnpacker zipUnpacker)
    {
        _paddleOcrDownloadService = paddleOcrDownloadService;
        _zipUnpacker = zipUnpacker;

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

            if (_cancellationTokenSource.Token.IsCancellationRequested)
            {
                return;
            }

            UnpackPaddleOcrModels(Se.PaddleOcrFolder);

            if (Popup != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Popup.Close(Se.PaddleOcrFolder);
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

        return;
    }

    private void UnpackPaddleOcrModels(string folder)
    {
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        _downloadStream.Position = 0;
        _zipUnpacker.UnpackZipStream(_downloadStream, folder);

        _downloadStream.Dispose();
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

        _downloadTask = _paddleOcrDownloadService.DownloadModels(_downloadStream, downloadProgress, _cancellationTokenSource.Token);
    }
}
