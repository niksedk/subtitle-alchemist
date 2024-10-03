using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Features.Video.BurnIn;
public class BurnInJobItem
{
    public string InputVideoFileName { get; set; }
    public string OutputVideoFileName { get; set; }
    public string SubtitleFileName { get; set; }
    public string AssaSubtitleFileName { get; set; }
    public bool UseTargetFileSize { get; set; }
    public long TargetFileSize { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public long TotalFrames { get; set; }

    public BurnInJobItem()
    {
        InputVideoFileName = string.Empty;
        OutputVideoFileName = string.Empty;
        SubtitleFileName = string.Empty;
        AssaSubtitleFileName = string.Empty;
    }
}
