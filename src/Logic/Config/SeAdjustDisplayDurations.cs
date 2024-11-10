using SubtitleAlchemist.Features.Tools.AdjustDuration;

namespace SubtitleAlchemist.Logic.Config;

public class SeAdjustDisplayDurations
{
    public double AdjustDurationSeconds { get; set; } = 0.1;
    public decimal AdjustDurationFixed { get; set; } = 0.1m;
    public int AdjustDurationPercent { get; set; } = 120;
    public string AdjustDurationLast { get; set; } = AdjustDurationType.Seconds.ToString();
    public bool AdjustDurationExtendOnly { get; set; } = true;
    public bool AdjustDurationExtendEnforceDurationLimits { get; set; } = true;
    public bool AdjustDurationExtendCheckShotChanges { get; set; } = true;
    public decimal AdjustDurationMaximumCps { get; set; }
    public decimal AdjustDurationOptimalCps { get; set; }
}