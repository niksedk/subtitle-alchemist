using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Features.Sync.ChangeFrameRate;

public class ChangeFrameRateResult
{
    public double FromFrameRate { get; set; }
    public double ToFrameRate { get; set; }
    public Subtitle Subtitle { get; set; } = new Subtitle();


    public ChangeFrameRateResult(Subtitle subtitle, double selectedFromFrameRate, double selectedToFrameRate)
    {
        Subtitle = subtitle;
        FromFrameRate = selectedFromFrameRate;
        ToFrameRate = selectedToFrameRate;
    }
}
