using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.ContainerFormats.Matroska;
using Nikse.SubtitleEdit.Core.SubtitleFormats;

namespace SubtitleAlchemist.Features.Shared.PickMatroskaTrack;

public partial class PickMatroskaTrackPopupModel : ObservableObject
{
    public PickMatroskaTrackPopup? Popup { get; set; }
    public CollectionView TrackList { get; set; }

    [ObservableProperty] private string _fileNameInfo;
    [ObservableProperty] private ObservableCollection<MatroskaTrackItem> _trackItems;
    [ObservableProperty] private MatroskaTrackItem? _selectedTrackItem;
    [ObservableProperty] private string _trackInfo;

    private MatroskaFile? _matroskaFile;

    public PickMatroskaTrackPopupModel()
    {
        TrackList = new();
        _fileNameInfo = string.Empty;
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

    public void Initialize(MatroskaFile matroskaFile, List<MatroskaTrackInfo> tracks, string fileName)
    {
        Popup?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                _matroskaFile = matroskaFile;

                foreach (var track in tracks)
                {
                    TrackItems.Add(new MatroskaTrackItem($"#{track.TrackNumber} {track.CodecId} {track.Name} {(track.IsDefault ? "Default" : string.Empty)} {(track.IsForced ? "Forced" : string.Empty)}", track));
                }

                SelectedTrackItem = TrackItems.FirstOrDefault();
                FileNameInfo = $"Input file: {fileName}";
            });

            return false;
        });
    }

    public void OnTrackSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is MatroskaTrackItem trackItem)
        {
            SelectedTrackItem = trackItem;
            TrackInfo = "#" + trackItem.MatroskaTrackInfo.TrackNumber + Environment.NewLine +
                        "Language: " + trackItem.MatroskaTrackInfo.Language + Environment.NewLine +
                        "Type: " + trackItem.MatroskaTrackInfo.CodecId + Environment.NewLine +
                        "Default: " + trackItem.MatroskaTrackInfo.IsDefault + Environment.NewLine +
                        "Forced: " + trackItem.MatroskaTrackInfo.IsForced + Environment.NewLine;

            var subtitles = _matroskaFile?.GetSubtitle(trackItem.MatroskaTrackInfo.TrackNumber, null);
            if (trackItem.MatroskaTrackInfo.CodecId == "S_TEXT/UTF8" && subtitles != null)
            {
                AddTextContent(trackItem, subtitles, new SubRip());
            }
            else if (trackItem.MatroskaTrackInfo.CodecId == "S_TEXT/SSA" && subtitles != null)
            {
                AddTextContent(trackItem, subtitles, new SubStationAlpha());
            }
            else if (trackItem.MatroskaTrackInfo.CodecId == "S_TEXT/ASS" && subtitles != null)
            {
                AddTextContent(trackItem, subtitles, new AdvancedSubStationAlpha());
            }
        }
    }

    private void AddTextContent(MatroskaTrackItem trackItem, List<MatroskaSubtitle> subtitles, SubtitleFormat format)
    {
        var sub = new Subtitle();
        Utilities.LoadMatroskaTextSubtitle(trackItem.MatroskaTrackInfo, _matroskaFile, subtitles, sub);
        var raw = format.ToText(sub, string.Empty);
        TrackInfo += $"#Subtitles: {sub.Paragraphs.Count}" + Environment.NewLine;
        TrackInfo += $"Format: {format.Name}" + Environment.NewLine;
        TrackInfo += Environment.NewLine + raw;
    }
}