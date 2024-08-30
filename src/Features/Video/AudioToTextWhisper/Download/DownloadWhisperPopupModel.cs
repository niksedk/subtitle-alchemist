using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubtitleAlchemist.Features.Video.AudioToTextWhisper.Engines;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Services;
using System.Globalization;
using System.Timers;
using Timer = System.Timers.Timer;

namespace SubtitleAlchemist.Features.Video.AudioToTextWhisper.Download
{
    public partial class DownloadWhisperPopupModel : ObservableObject
    {
        public DownloadWhisperPopup? Popup { get; set; }

        private readonly IWhisperDownloadService _whisperCppDownloadService;

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

        public IWhisperEngine Engine { get; set; } = new WhisperEngineCpp();
        public Label LabelTitle { get; set; } = new();

        public DownloadWhisperPopupModel(IWhisperDownloadService whisperCppDownloadService)
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
            _timer.Stop();
            if (_downloadTask is { IsCompleted: true })
            {
                if (_downloadStream.Length == 0)
                {
                    Progress = "Download failed";
                    Error = "No data received";
                    return;
                }

                var folder = Engine.GetAndCreateWhisperFolder();
                Unpack(folder, Engine.UnpackSkipFolder);

                if (Popup != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Popup.Close(folder);
                    });
                }

                return;
            }

            if (_downloadTask is { IsFaulted: true })
            {
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

                return;
            }

            _timer.Start();
        }

        private void Unpack(string folder, string skipFolderLevel)
        {
            _downloadStream.Position = 0;
            ZipUnpacker.UnpackZipStream(_downloadStream, folder, skipFolderLevel);
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
            LabelTitle.Text = $"Downloading {Engine.Name}";

            var downloadProgress = new Progress<float>(number =>
            {
                var percentage = (int)Math.Round(number * 100.0, MidpointRounding.AwayFromZero);
                var pctString = percentage.ToString(CultureInfo.InvariantCulture);
                ProgressValue = number;
                Progress = $"Downloading... {pctString}%";
            });

            if (Engine is WhisperEngineCpp)
            {
                _downloadTask = _whisperCppDownloadService.DownloadWhisperCpp(_downloadStream, downloadProgress, _cancellationTokenSource.Token);
            }
            else if (Engine is WhisperEngineConstMe)
            {
                _downloadTask = _whisperCppDownloadService.DownloadWhisperConstMe(_downloadStream, downloadProgress, _cancellationTokenSource.Token);
            }
            else if (Engine is WhisperEnginePurfviewFasterWhisper)
            {
                _downloadTask = _whisperCppDownloadService.DownloadWhisperPurfviewFasterWhisper(_downloadStream, downloadProgress, _cancellationTokenSource.Token);
            }
        }
    }
}
