using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Services;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Timers;
using SubtitleAlchemist.Logic.Config;
using Timer = System.Timers.Timer;

namespace SubtitleAlchemist.Features.Options.DownloadFfmpeg
{
    public partial class DownloadFfmpegPopupModel : ObservableObject
    {
        public DownloadFfmpegPopup? Popup { get; set; }

        private readonly IFfmpegDownloadService _ffmpegDownloadService;

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

        public DownloadFfmpegPopupModel(IFfmpegDownloadService ffmpegDownloadService, IZipUnpacker zipUnpacker)
        {
            _ffmpegDownloadService = ffmpegDownloadService;
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

                var ffmpegFileName = GetFfmpegFileName();

                if (File.Exists(ffmpegFileName))
                {
                    File.Delete(ffmpegFileName);
                }

                UnpackFfmpeg(ffmpegFileName);

                if (Popup != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Popup.Close(ffmpegFileName);
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

        private void UnpackFfmpeg(string newFileName)
        {
            var folder = Path.GetDirectoryName(newFileName);
            if (folder != null)
            {
                _downloadStream.Position = 0;
                _zipUnpacker.UnpackZipStream(_downloadStream, folder);
            }

            _downloadStream.Dispose();
        }

        public static string GetFfmpegFolder()
        {
            return Se.FfmpegFolder;
        }

        public static string GetFfmpegFileName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Path.Combine(GetFfmpegFolder(), "ffmpeg.exe");
            }

            return Path.Combine(GetFfmpegFolder(), "ffmpeg");
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

            var folder = GetFfmpegFolder();
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            _downloadTask = _ffmpegDownloadService.DownloadFfmpeg(_downloadStream, downloadProgress, _cancellationTokenSource.Token);
        }
    }
}
