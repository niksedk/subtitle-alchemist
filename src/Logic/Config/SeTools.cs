namespace SubtitleAlchemist.Logic.Config;

public class SeTools
{
    public SeAudioToText AudioToText { get; set; } = new();
    public SeFixCommonErrors FixCommonErrors { get; set; } = new();
    public SeAdjustDisplayDurations AdjustDurations { get; set; } = new();
    public SeBatchConvert BatchConvert { get; set; } = new();
    public string AutoTranslateLastName { get; set; } = string.Empty;
}