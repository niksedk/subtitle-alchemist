using SubtitleAlchemist.Features.Tools.AdjustDuration;

namespace SubtitleAlchemist.Logic.Config;

public class SeAdjustDisplayDurations
{
    public decimal AdjustDurationSeconds { get; set; } = 0.1m;
    public int AdjustDurationPercent { get; set; } = 120;
    public string AdjustDurationLast { get; set; } = AdjustDurationModel.ModeSeconds;
    public bool AdjustDurationExtendOnly { get; set; } = true;
    public bool AdjustDurationExtendEnforceDurationLimits { get; set; } = true;
    public bool AdjustDurationExtendCheckShotChanges { get; set; } = true;
}