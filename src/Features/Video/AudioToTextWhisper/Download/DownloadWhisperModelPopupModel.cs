using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.AudioToText;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Features.Video.AudioToTextWhisper.Engines;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Services;
using Timer = System.Timers.Timer;

namespace SubtitleAlchemist.Features.Video.AudioToTextWhisper.Download;

public partial class DownloadWhisperModelPopupModel : ObservableObject
{

    [ObservableProperty]
    private float _progressValue;

    [ObservableProperty]
    private string _progress;

    public DownloadWhisperModelPopup? Popup { get; set; }

    public Picker ModelPicker { get; set; } = new();
    public ProgressBar ProgressBar { get; set; } = new();

    [ObservableProperty] private ObservableCollection<AudioToTextWhisperModel.WhisperModelDisplay> _models = new();

    [ObservableProperty]
    private AudioToTextWhisperModel.WhisperModelDisplay? _selectedModel;

    private IWhisperEngine _whisperEngine = new WhisperEngineCpp();

    [ObservableProperty]
    private string _error = string.Empty;

    private Task? _downloadTask;
    private readonly List<string> _downloadUrls = new();
    private int _downloadIndex;
    private string _downloadFileName = string.Empty;

    private readonly Timer _timer = new();

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private readonly IWhisperDownloadService _whisperCppDownloadService;
    private WhisperModel _downloadModel;

    private const string TemporaryFileExtension = ".$$$";

    public DownloadWhisperModelPopupModel(IWhisperDownloadService whisperCppDownloadService)
    {
        _whisperCppDownloadService = whisperCppDownloadService;
        _progress = string.Empty;
        ProgressValue = 0;
        _downloadModel = new WhisperModel();
    }

    private void Close(WhisperModel downloadModel)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Popup?.Close(downloadModel);
        });
    }

    [RelayCommand]
    public void Cancel()
    {
        _cancellationTokenSource.Cancel();
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Popup?.Close();
        });
    }

    public void SetModels(ObservableCollection<AudioToTextWhisperModel.WhisperModelDisplay> models, IWhisperEngine whisperEngine, AudioToTextWhisperModel.WhisperModelDisplay? whisperModel)
    {
        _whisperEngine = whisperEngine;

        foreach (var model in models)
        {
            Models.Add(model);
        }

        if (whisperModel != null)
        {
            SelectedModel = whisperModel;
        }
        else if (models.Count > 0)
        {
            SelectedModel = models[0];
        }
    }

    [RelayCommand]
    public void OpenModelsFolder()
    {
        var folderName = _whisperEngine.GetAndCreateWhisperModelFolder(null);
        UiUtil.OpenFolder(folderName);
    }

    [RelayCommand]
    public void StartDownload()
    {
        if (SelectedModel is not AudioToTextWhisperModel.WhisperModelDisplay model)
        {
            return;
        }

        //TODO: disable download buttons

        _downloadUrls.Clear();
        _downloadUrls.AddRange(model.Model.Urls);
        _downloadIndex = 0;
        _downloadModel = model.Model;
        _downloadFileName = GetDownloadFileName(model.Model, _downloadUrls[_downloadIndex]);
        _downloadTask = _whisperCppDownloadService.DownloadFile(_downloadUrls[_downloadIndex], _downloadFileName, MakeDownloadProgress(), _cancellationTokenSource.Token);
        _timer.Interval = 500;
        _timer.Elapsed += OnTimerOnElapsed;
        _timer.Start();

        ProgressBar.IsVisible = true;
    }

    private Progress<float> MakeDownloadProgress()
    {
        return new Progress<float>(number =>
        {
            var percentage = (int)Math.Round(number * 100.0, MidpointRounding.AwayFromZero);
            var pctString = percentage.ToString(CultureInfo.InvariantCulture);
            ProgressValue = number;
            Progress = $"Downloading... {pctString}%";
        });
    }

    private string GetDownloadFileName(WhisperModel whisperModel, string url)
    {
        var fileName = _whisperEngine.GetWhisperModelDownloadFileName(whisperModel, url);
        return fileName + TemporaryFileExtension;
    }

    private void OnTimerOnElapsed(object? sender, ElapsedEventArgs args)
    {
        _timer.Stop();

        if (_downloadTask is { IsCompleted: true })
        {
            CompleteDownload();

            _downloadIndex++;
            if (_downloadIndex < _downloadUrls.Count)
            {
                _downloadFileName = GetDownloadFileName(_downloadModel, _downloadUrls[_downloadIndex]); 
                _downloadTask = _whisperCppDownloadService.DownloadFile(_downloadUrls[_downloadIndex], _downloadFileName, MakeDownloadProgress(), _cancellationTokenSource.Token);
                ProgressValue = 0;
                _timer.Start();

                return;
            }

            _downloadTask = null;

            if (Popup != null)
            {
                Close(_downloadModel); 
            }

            return;
        }

        if (_downloadTask is { IsFaulted: true })
        {
            var ex = _downloadTask.Exception?.InnerException ?? _downloadTask.Exception;
            if (ex is OperationCanceledException)
            {
                Progress = "Download canceled";
                Cancel();
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

    private void CompleteDownload()
    {
        if (string.IsNullOrEmpty(_downloadFileName) || !File.Exists(_downloadFileName))
        {
            return;
        }

        var fileInfo = new FileInfo(_downloadFileName);
        if (fileInfo.Length < 50)
        {
            var text = FileUtil.ReadAllTextShared(_downloadFileName, Encoding.UTF8);
            if (text.StartsWith("Entry not found", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    File.Delete(_downloadFileName);
                }
                catch
                {
                    // ignore
                }

                return;
            }

            if (text.Contains("Invalid username or password."))
            {
                throw new Exception("Unable to download file - Invalid username or password! (Perhaps file has a new location)");
            }
        }

        var newFileName = _downloadFileName.Replace(TemporaryFileExtension, string.Empty);

        if (File.Exists(newFileName))
        {
            File.Delete(newFileName);
        }

        File.Move(_downloadFileName, newFileName);
        _downloadFileName = string.Empty;
    }
}
