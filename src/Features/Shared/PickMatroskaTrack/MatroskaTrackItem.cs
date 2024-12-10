using Nikse.SubtitleEdit.Core.ContainerFormats.Matroska;

namespace SubtitleAlchemist.Features.Shared.PickMatroskaTrack;
public class MatroskaTrackItem
{
    public MatroskaTrackInfo MatroskaTrackInfo { get; set; }
    public string DisplayName { get; set; }

    public MatroskaTrackItem(string displayName, MatroskaTrackInfo matroskaTrackInfo)
    {
        DisplayName = displayName;
        MatroskaTrackInfo = matroskaTrackInfo;
    }

    public override string ToString()
    {
        return DisplayName;
    }
}
