using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Common;
using SharpCompress.Readers;
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

        public IWhisperEngine Engine { get; set; } = new WhisperEngineCpp();
        public Label LabelTitle { get; set; } = new();

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

        private readonly IWhisperDownloadService _whisperCppDownloadService;
        private readonly IZipUnpacker _zipUnpacker;

        public DownloadWhisperPopupModel(IWhisperDownloadService whisperCppDownloadService, IZipUnpacker zipUnpacker)
        {
            _whisperCppDownloadService = whisperCppDownloadService;
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
            _timer.Stop();
            if (_downloadTask is { IsCompleted: true })
            {
                if (Engine.Name == WhisperEnginePurfviewFasterWhisperXxl.StaticName)
                {
                    var dir = Engine.GetAndCreateWhisperFolder();
                    var tempFileName = Path.Combine(dir, Engine.Name + ".7z");

                    Progress = "Unpacking 7-zip archive...";
                    Extract7Zip(tempFileName, dir);

                    try
                    {
                        File.Delete(tempFileName);
                    }
                    catch 
                    {
                        // ignore
                    }

                    if (_cancellationTokenSource.IsCancellationRequested)
                    {
                        Cancel();
                        return;
                    }

                    if (Popup != null)
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            Popup.Close(dir);
                        });
                    }

                }
                else
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

        private void Extract7Zip(string tempFileName, string dir)
        {
            using Stream stream = File.OpenRead(tempFileName);
            using var archive = SevenZipArchive.Open(stream);
            double totalSize = archive.TotalUncompressSize;
            double unpackedSize = 0;

            var reader = archive.ExtractAllEntries();
            while (reader.MoveToNextEntry())
            {
                if (_cancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }

                var skipFolderLevel = "Faster-Whisper-XXL";
                if (!string.IsNullOrEmpty(reader.Entry.Key))
                {
                    var entryFullName = reader.Entry.Key;
                    if (!string.IsNullOrEmpty(skipFolderLevel) && entryFullName.StartsWith(skipFolderLevel))
                    {
                        entryFullName = entryFullName[skipFolderLevel.Length..];
                    }

                    entryFullName = entryFullName.Replace('/', Path.DirectorySeparatorChar);
                    entryFullName = entryFullName.TrimStart(Path.DirectorySeparatorChar);

                    var fullFileName = Path.Combine(dir, entryFullName);

                    if (reader.Entry.IsDirectory)
                    {
                        if (!Directory.Exists(fullFileName))
                        {
                            Directory.CreateDirectory(fullFileName);
                        }

                        continue;
                    }

                    var fullPath = Path.GetDirectoryName(fullFileName);
                    if (fullPath == null)
                    {
                        continue;
                    }

                    var displayName = entryFullName;
                    if (displayName.Length > 30)
                    {
                        displayName = "..." + displayName.Remove(0, displayName.Length - 26).Trim();
                    }

                    Progress = $"Unpacking: {displayName}";
                    ProgressValue = (float)(unpackedSize / totalSize);
                    reader.WriteEntryToDirectory(fullPath,
                        new ExtractionOptions() { ExtractFullPath = false, Overwrite = true });
                    unpackedSize += reader.Entry.Size;
                }
            }

            ProgressValue = 1.0f;
        }

        private void Unpack(string folder, string skipFolderLevel)
        {
            _downloadStream.Position = 0;
            _zipUnpacker.UnpackZipStream(_downloadStream, folder, skipFolderLevel, false, new List<string>());
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
            else if (Engine is WhisperEnginePurfviewFasterWhisperXxl)
            {
                var dir = Engine.GetAndCreateWhisperFolder();
                var tempFileName = Path.Combine(dir, Engine.Name + ".7z");
                _downloadTask = _whisperCppDownloadService.DownloadWhisperPurfviewFasterWhisperXxl(tempFileName, downloadProgress, _cancellationTokenSource.Token);
            }
        }
    }
}
