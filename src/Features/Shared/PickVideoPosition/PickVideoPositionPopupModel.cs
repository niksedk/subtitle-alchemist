using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubtitleAlchemist.Features.Main;

namespace SubtitleAlchemist.Features.Shared.PickVideoPosition;

public partial class PickVideoPositionPopupModel : ObservableObject
{
    public PickVideoPositionPopup? Popup { get; set; }
    public CollectionView SubtitleList { get; set; }
    public MediaElement VideoPlayer { get; set; }

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private ObservableCollection<DisplayParagraph> _paragraphs;

    [ObservableProperty]
    private DisplayParagraph? _selectedParagraph;

    public PickVideoPositionPopupModel()
    {
        SubtitleList = new();
        _title = string.Empty;
        _paragraphs = new ObservableCollection<DisplayParagraph>();
        _selectedParagraph = null;
        VideoPlayer = new MediaElement();
    }

    [RelayCommand]
    private void Ok()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Popup?.Close(VideoPlayer.Position);
        });
    }

    [RelayCommand]
    private void Cancel()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Popup?.Close();
        });
    }

    public void Initialize(string videoFileName, string title)
    {
        Popup?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Title = title;
                VideoPlayer.Source = MediaSource.FromFile(videoFileName);
            });

            return false;
        });
    }
}