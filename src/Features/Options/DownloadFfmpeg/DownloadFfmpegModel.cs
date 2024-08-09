using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Services;
using System.Globalization;
using System.Timers;

namespace SubtitleAlchemist.Features.Options.DownloadFfmpeg
{
    public partial class DownloadFfmpegModel : ObservableObject
    {
        public DownloadFfmpegPopup? Popup { get; set; }

        private readonly IFfmpegDownloadService _ffmpegDownloadService;

        [ObservableProperty]
        private float _progressValue;

        [ObservableProperty]
        private string _progress;

        private Task? _downloadTask;

        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly MemoryStream _ms;


        public DownloadFfmpegModel(IFfmpegDownloadService ffmpegDownloadService)
        {
            _ffmpegDownloadService = ffmpegDownloadService;

            _cancellationTokenSource = new CancellationTokenSource();

            _ms = new MemoryStream();

            Progress = "Starting...";

            var timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += OnTimerOnElapsed;
            timer.Start();
        }

        private void OnTimerOnElapsed(object? sender, ElapsedEventArgs args)
        {
            if (_downloadTask is { IsCompleted: true })
            {
                var oldFileName = GetFfmpegTempFileName();
                var newFileName = GetFfmpegFileName();
                if (File.Exists(oldFileName))
                {
                    if (File.Exists(newFileName))
                    {
                        File.Delete(newFileName);
                    }

                    File.Move(oldFileName, newFileName, true);
                }

                Close();
            }
        }

        private static string GetFfmpegTempFileName()
        {
            return $"{GetFfmpegFileName()}.$$$";
        }

        private static string GetFfmpegFolder()
        {
            return Path.Combine(Configuration.DataDirectory, "ffmpeg");
        }

        private static string GetFfmpegFileName()
        {
            return Path.Combine(GetFfmpegFolder() , "ffmpeg.exe");
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
            
            var fileName = GetFfmpegTempFileName();
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            //_downloadTask = _ffmpegDownloadService.DownloadFfmpeg(fileName, downloadProgress, _cancellationTokenSource.Token);
            _downloadTask = _ffmpegDownloadService.DownloadFfmpeg(_ms, downloadProgress, _cancellationTokenSource.Token);
        }
    }
}
