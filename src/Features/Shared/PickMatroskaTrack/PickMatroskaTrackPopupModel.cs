using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.ContainerFormats.Matroska;

namespace SubtitleAlchemist.Features.Shared.PickMatroskaTrack;

public partial class PickMatroskaTrackPopupModel : ObservableObject
{
    public PickMatroskaTrackPopup? Popup { get; set; }
    public CollectionView TrackList { get; set; }

    [ObservableProperty] private string _title;
    [ObservableProperty] private ObservableCollection<MatroskaTrackItem> _trackItems;
    [ObservableProperty] private MatroskaTrackItem? _selectedTrackItem;
    [ObservableProperty] private string _trackInfo;

    public PickMatroskaTrackPopupModel()
    {
        TrackList = new();
        _title = string.Empty;
        _trackItems = new ObservableCollection<MatroskaTrackItem>();
        _selectedTrackItem = null;
        _trackInfo = string.Empty;
    }

    [RelayCommand]
    private void Ok()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Popup?.Close(SelectedTrackItem?.MatroskaTrackInfo);
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

    public void Initialize(List<MatroskaTrackInfo> tracks, string fileName)
    {
        var items = tracks.Select(t => new MatroskaTrackItem(t.Name, t));
        Popup?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                TrackItems = new ObservableCollection<MatroskaTrackItem>(items);
                SelectedTrackItem = TrackItems.FirstOrDefault();
                Title = fileName;
            });

            return false;
        });
    }

    public void OnTrackSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is MatroskaTrackItem trackItem)
        {
            SelectedTrackItem = trackItem;
            TrackInfo = trackItem.MatroskaTrackInfo.Name + Environment.NewLine +
                        trackItem.MatroskaTrackInfo.CodecId + Environment.NewLine +
                        trackItem.MatroskaTrackInfo.GetCodecPrivate() + Environment.NewLine;
        }
    }
}