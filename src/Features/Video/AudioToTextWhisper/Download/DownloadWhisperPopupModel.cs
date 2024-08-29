using System.Collections.ObjectModel;
using System.Globalization;
using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.AudioToText;
using SubtitleAlchemist.Features.Video.AudioToTextWhisper.Engines;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Services;
using Timer = System.Timers.Timer;

namespace SubtitleAlchemist.Features.Video.AudioToTextWhisper.Download
{
    public partial class DownloadWhisperPopupModel : ObservableObject
    {
        public DownloadWhisperPopup? Popup { get; set; }

        private readonly IWhisperCppDownloadService _whisperCppDownloadService;

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

        readonly IWhisperEngine _engine = new WhisperEngineCpp();

        public DownloadWhisperPopupModel(IWhisperCppDownloadService whisperCppDownloadService)
        {
            _whisperCppDownloadService = whisperCppDownloadService;

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

                var folder = _engine.GetAndCreateWhisperFolder();
                Unpack(folder);

                if (Popup != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Popup.Close(folder);
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

        private void Unpack(string folder)
        {
            _downloadStream.Position = 0;
            ZipUnpacker.UnpackZipStream(_downloadStream, folder);
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

            _downloadTask = _whisperCppDownloadService.DownloadWhisperCpp(_downloadStream, downloadProgress, _cancellationTokenSource.Token);
        }
    }
}
