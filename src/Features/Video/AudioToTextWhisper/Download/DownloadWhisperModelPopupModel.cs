using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.AudioToText;
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

    [ObservableProperty] private ObservableCollection<IWhisperModel> _models = new();

    [ObservableProperty]
    private IWhisperModel? _selectedModel;

    [ObservableProperty]
    private string _error;

    private Task? _downloadTask;
    
    private readonly Timer _timer = new();

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public DownloadWhisperModelPopupModel()
    {
        _progress = string.Empty;
        ProgressValue = 0;
    }

    private void Close()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Popup?.Close();
        });
    }

    [RelayCommand]
    public void Download()
    {
    }

    [RelayCommand]
    public void Cancel()
    {
        _cancellationTokenSource.Cancel();
        Close();
    }

    public void SetModels(ObservableCollection<IWhisperModel> models)
    {
        foreach (var model in models)
        {
            Models.Add(model);
        }

        if (models.Count > 0)
        {
            SelectedModel = models[0];
        }
    }
}
