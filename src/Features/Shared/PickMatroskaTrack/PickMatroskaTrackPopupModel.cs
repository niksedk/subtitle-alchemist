using System.Collections.ObjectModel;
using System.Text;
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
    public Label LabelStatusText { get; set; }


    [ObservableProperty]
    public partial string FileNameInfo { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<MatroskaTrackItem> TrackItems { get; set; }

    [ObservableProperty]
    public partial MatroskaTrackItem? SelectedTrackItem { get; set; }

    [ObservableProperty]
    public partial string TrackInfo { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<ImageSource> SelectedTrackImages { get; set; }

    [ObservableProperty]
    public partial string StatusText { get; set; }

    private MatroskaFile? _matroskaFile;
    private string _fileName;
    private bool _closing;

    public PickMatroskaTrackPopupModel()
    {
        TrackList = new();
        FileNameInfo = string.Empty;
        TrackItems = new ObservableCollection<MatroskaTrackItem>();
        SelectedTrackItem = null;
        TrackInfo = string.Empty;
        SelectedTrackImages = new ObservableCollection<ImageSource>();
        _fileName = string.Empty;
        StatusText = string.Empty;
        LabelStatusText = new Label();
    }

    [RelayCommand]
    private void Ok()
    {
        _closing = true;
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Popup?.Close(SelectedTrackItem?.MatroskaTrackInfo);
        });
    }

    [RelayCommand]
    private void Cancel()
    {
        _closing = true;
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
            await SaveBluRaySup(subtitles, trackItem);
            return;
        }

        if (trackItem.CodecId == MatroskaTrackType.SubRip && subtitles != null)
        {
            await SaveTextSubtitle(SelectedTrackItem, subtitles, new SubRip());
        }
        else if (trackItem.CodecId == MatroskaTrackType.SubStationAlpha && subtitles != null)
        {
            await SaveTextSubtitle(SelectedTrackItem, subtitles, new SubStationAlpha());
        }
        else if (trackItem.CodecId == MatroskaTrackType.AdvancedSubStationAlpha && subtitles != null)
        {
            await SaveTextSubtitle(SelectedTrackItem, subtitles, new AdvancedSubStationAlpha());
        }
    }

    private async Task SaveBluRaySup(List<MatroskaSubtitle> sub, MatroskaTrackInfo matroskaTrackInfo)
    {
        var subtitles = new List<BluRaySupParser.PcsData>();
        var log = new StringBuilder();
        var clusterStream = new MemoryStream();
        var lastPalettes = new Dictionary<int, List<PaletteInfo>>();
        var lastBitmapObjects = new Dictionary<int, List<BluRaySupParser.OdsData>>();
        foreach (var p in sub)
        {
            var buffer = p.GetData(matroskaTrackInfo);

            if (buffer != null && buffer.Length > 2)
            {
                clusterStream.Write(buffer, 0, buffer.Length);
                if (ContainsBluRayStartSegment(buffer))
                {
                    if (subtitles.Count > 0 && subtitles[subtitles.Count - 1].StartTime == subtitles[subtitles.Count - 1].EndTime)
                    {
                        subtitles[subtitles.Count - 1].EndTime = (long)((p.Start - 1) * 90.0);
                    }

                    clusterStream.Position = 0;
                    var list = BluRaySupParser.ParseBluRaySup(clusterStream, log, true, lastPalettes, lastBitmapObjects);
                    foreach (var sup in list)
                    {
                        sup.StartTime = (long)((p.Start - 1) * 90.0);
                        sup.EndTime = (long)((p.End - 1) * 90.0);
                        subtitles.Add(sup);

                        // fix overlapping
                        if (subtitles.Count > 1 && sub[subtitles.Count - 2].End > sub[subtitles.Count - 1].Start)
                        {
                            subtitles[subtitles.Count - 2].EndTime = subtitles[subtitles.Count - 1].StartTime - 1;
                        }
                    }

                    clusterStream = new MemoryStream();
                }
            }
            else if (subtitles.Count > 0)
            {
                var lastSub = subtitles[subtitles.Count - 1];
                if (lastSub.StartTime == lastSub.EndTime)
                {
                    lastSub.EndTime = (long)((p.Start - 1) * 90.0);
                    if (lastSub.EndTime - lastSub.StartTime > 1000000)
                    {
                        lastSub.EndTime = lastSub.StartTime;
                    }
                }
            }
        }

        using var ms = new MemoryStream();
        for (var index = 0; index < subtitles.Count; index++)
        {
            var p = subtitles[index];
            var brSub = new BluRaySupPicture
            {
                StartTime = p.StartTime,
                EndTime = p.EndTime,
                Width = 1920,
                Height = 1080,
                IsForced = p.IsForced,
                CompositionNumber = index * 2,
            };

            var bitmap = p.GetBitmap();
            var position = p.GetPosition();
            var buffer = BluRaySupPicture.CreateSupFrame(brSub, bitmap, 25, 0, 0, BluRayContentAlignment.BottomCenter, new BluRayPoint(position.Left, position.Top) );
            ms.Write(buffer, 0, buffer.Length);
        }

        var fileHelper = new FileHelper();
        var subtitleFileName =
            await fileHelper.SaveStreamAs(ms, "Save BluRay sup (PGS)", _fileName, ".sup", CancellationToken.None);
        if (!string.IsNullOrEmpty(subtitleFileName))
        {
            ShowStatus($"Saved subtitle file {subtitleFileName}");
        }
    }

    private static bool ContainsBluRayStartSegment(byte[] buffer)
    {
        const int epochStart = 0x80;
        var position = 0;
        while (position + 3 <= buffer.Length)
        {
            var segmentType = buffer[position];
            if (segmentType == epochStart)
            {
                return true;
            }

            var length = BigEndianInt16(buffer, position + 1) + 3;
            position += length;
        }

        return false;
    }

    public static int BigEndianInt16(byte[] buffer, int index)
    {
        if (buffer.Length < 2)
        {
            return 0;
        }

        return buffer[index + 1] | (buffer[index] << 8);
    }


    private async Task SaveTextSubtitle(MatroskaTrackItem trackItem, List<MatroskaSubtitle> subtitles, SubtitleFormat format)
    {
        var sub = new Subtitle();
        Utilities.LoadMatroskaTextSubtitle(trackItem.MatroskaTrackInfo, _matroskaFile, subtitles, sub);

        var fileHelper = new FileHelper();
        var subtitleFileName = await fileHelper.SaveSubtitleFileAs(
            "Save subtitle file as",
            _fileName,
            format,
            sub);

        if (!string.IsNullOrEmpty(subtitleFileName))
        {
            ShowStatus($"Saved subtitle file {subtitleFileName}");
        }
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

    private void ShowStatus(string statusText)
    {
        LabelStatusText.Opacity = 0;
        StatusText = statusText;
        LabelStatusText.FadeTo(1, 200);

        Popup?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(6_000), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (_closing)
                {
                    return;
                }

                if (StatusText == statusText)
                {
                    LabelStatusText.FadeTo(0, 200);
                }
            });
            return false;
        });
    }

}