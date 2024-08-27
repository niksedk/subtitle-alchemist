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

    [ObservableProperty] private ObservableCollection<IWhisperModel> _models;

    [ObservableProperty]
    private string _error;

    private Task? _downloadTask;
    private readonly Timer _timer = new();

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public DownloadWhisperModelPopupModel()
    {
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
}
