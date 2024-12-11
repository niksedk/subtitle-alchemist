using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.ContainerFormats.Matroska;
using Nikse.SubtitleEdit.Core.SubtitleFormats;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.BluRaySup;
using SubtitleAlchemist.Logic.Media;

namespace SubtitleAlchemist.Features.Shared.PickMatroskaTrack;

public partial class PickMatroskaTrackPopupModel : ObservableObject
{
    public PickMatroskaTrackPopup? Popup { get; set; }
    public CollectionView TrackList { get; set; }

    [ObservableProperty] private string _fileNameInfo;
    [ObservableProperty] private ObservableCollection<MatroskaTrackItem> _trackItems;
    [ObservableProperty] private MatroskaTrackItem? _selectedTrackItem;
    [ObservableProperty] private string _trackInfo;
    [ObservableProperty] private ObservableCollection<ImageSource> _selectedTrackImages;

    private MatroskaFile? _matroskaFile;
    private string _fileName;

    public PickMatroskaTrackPopupModel()
    {
        TrackList = new();
        _fileNameInfo = string.Empty;
        _trackItems = new ObservableCollection<MatroskaTrackItem>();
        _selectedTrackItem = null;
        _trackInfo = string.Empty;
        _selectedTrackImages = new ObservableCollection<ImageSource>();
        _fileName = string.Empty;   
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

    [RelayCommand]
    private async Task Export()
    {
        if (SelectedTrackItem == null)
        {
            return;
        }

        var trackItem = SelectedTrackItem.MatroskaTrackInfo;
        var subtitles = _matroskaFile?.GetSubtitle(trackItem.TrackNumber, null);

        if (trackItem.CodecId == MatroskaTrackType.BluRay && subtitles != null && _matroskaFile != null)
        {
            SaveBluRaySup(subtitles);
            return;
        }

        if (trackItem.CodecId == MatroskaTrackType.SubRip && subtitles != null)
        {
            await SaveTextContent(SelectedTrackItem, subtitles, new SubRip());
        }
        else if (trackItem.CodecId == MatroskaTrackType.SubStationAlpha && subtitles != null)
        {
            await SaveTextContent(SelectedTrackItem, subtitles, new SubStationAlpha());
        }
        else if (trackItem.CodecId == MatroskaTrackType.AdvancedSubStationAlpha && subtitles != null)
        {
            await SaveTextContent(SelectedTrackItem, subtitles, new AdvancedSubStationAlpha());
        }
    }

    private void SaveBluRaySup(List<MatroskaSubtitle> subtitles)
    {
    }

    private async Task SaveTextContent(MatroskaTrackItem trackItem, List<MatroskaSubtitle> subtitles, SubtitleFormat format)
    {
        var sub = new Subtitle();
        Utilities.LoadMatroskaTextSubtitle(trackItem.MatroskaTrackInfo, _matroskaFile, subtitles, sub);

        var fileHelper = new FileHelper();
        var subtitleFileName = await fileHelper.SaveSubtitleFileAs(
            "Save subtitle file as",
            _fileName,
            format,
            sub);

        if (string.IsNullOrEmpty(subtitleFileName))
        {
            return;
        }

        await File.WriteAllTextAsync(subtitleFileName, format.ToText(sub, string.Empty));
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
                _fileName = fileName;
            });

            return false;
        });
    }

    public void OnTrackSelected(object? sender, SelectionChangedEventArgs e)
    {
        SelectedTrackImages.Clear();
        if (e.CurrentSelection.FirstOrDefault() is MatroskaTrackItem trackItem)
        {
            SelectedTrackItem = trackItem;
            TrackInfo = "#" + trackItem.MatroskaTrackInfo.TrackNumber + Environment.NewLine +
                        "Language: " + trackItem.MatroskaTrackInfo.Language + Environment.NewLine +
                        "Type: " + trackItem.MatroskaTrackInfo.CodecId + Environment.NewLine +
                        "Default: " + trackItem.MatroskaTrackInfo.IsDefault + Environment.NewLine +
                        "Forced: " + trackItem.MatroskaTrackInfo.IsForced + Environment.NewLine;

            var subtitles = _matroskaFile?.GetSubtitle(trackItem.MatroskaTrackInfo.TrackNumber, null);
            if (trackItem.MatroskaTrackInfo.CodecId == MatroskaTrackType.SubRip && subtitles != null)
            {
                AddTextContent(trackItem, subtitles, new SubRip());
            }
            else if (trackItem.MatroskaTrackInfo.CodecId == MatroskaTrackType.SubStationAlpha && subtitles != null)
            {
                AddTextContent(trackItem, subtitles, new SubStationAlpha());
            }
            else if (trackItem.MatroskaTrackInfo.CodecId == MatroskaTrackType.AdvancedSubStationAlpha && subtitles != null)
            {
                AddTextContent(trackItem, subtitles, new AdvancedSubStationAlpha());
            }
            else if (trackItem.MatroskaTrackInfo.CodecId == MatroskaTrackType.BluRay && subtitles != null && _matroskaFile != null)
            {
                var pcsData = BluRaySupParser.ParseBluRaySupFromMatroska(trackItem.MatroskaTrackInfo, _matroskaFile);
                TrackInfo += $"#Subtitles: {pcsData.Count}" + Environment.NewLine;
                for (var i = 0; i < 20 && i < pcsData.Count; i++)
                {
                    var item = pcsData[i];
                    var imageSource = item.GetBitmap().ToImageSource();
                    SelectedTrackImages.Add(imageSource);
                }
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