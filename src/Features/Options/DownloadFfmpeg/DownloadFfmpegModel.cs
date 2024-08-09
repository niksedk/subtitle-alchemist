using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Services;

namespace SubtitleAlchemist.Features.Options.DownloadFfmpeg
{
    public partial class DownloadFfmpegModel : ObservableObject
    {
        public DownloadFfmpegPopup? Popup { get; set; }

        private readonly IFfmpegDownloadService _ffmpegDownloadService;

        [ObservableProperty]
        private string _progress;

        private Task? _downloadTask;

        private readonly System.Timers.Timer _timer;


        public DownloadFfmpegModel(IFfmpegDownloadService ffmpegDownloadService)
        {
            _ffmpegDownloadService = ffmpegDownloadService;

            Progress = "Starting...";

            _timer = new System.Timers.Timer();
            _timer.Interval = 1000;
            _timer.Elapsed += (sender, args) =>
            {
                if (_downloadTask is { IsCompleted: true })
                {
                    Close();
                }
            };
            _timer.Start();
        }

        private void Close()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Popup?.Close();
            });
        }

        public void StartDownload(CancellationToken cancellationToken)
        {
            var downloadProgress = new Progress<float>(number =>
            {
                var percentage = (int)Math.Round(number * 100.0, MidpointRounding.AwayFromZero);
                var pctString = percentage.ToString(CultureInfo.InvariantCulture);;
                Progress = $"Downloading... {pctString}%";
            });

            var folder = Path.Combine(Configuration.DataDirectory, "ffmpeg");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            
            var fileName = Path.Combine(Configuration.DataDirectory, "ffmpeg", "ffmpeg.exe.$$$");
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            _downloadTask = _ffmpegDownloadService.DownloadFfmpeg(fileName, downloadProgress, cancellationToken);
        }
    }
}
