﻿namespace SubtitleAlchemist.Logic.Config.Language;

public class LanguageBurnIn
{
    public string Title { get; set; }
    public string InfoAssaOff { get; set; }
    public string InfoAssaOn { get; set; }
    public string XGeneratedWithBurnedInSubsInX { get; set; }
    public string TimeRemainingMinutes { get; set; }
    public string TimeRemainingOneMinute { get; set; }
    public string TimeRemainingSeconds { get; set; }
    public string TimeRemainingAFewSeconds { get; set; }
    public string TimeRemainingMinutesAndSeconds { get; set; }
    public string TimeRemainingOneMinuteAndSeconds { get; set; }
    public string TargetFileName { get; set; }
    public string TargetFileSize { get; set; }
    public string FileSizeMb { get; set; }
    public string PassX { get; set; }
    public string Encoding { get; set; }
    public string BitRate { get; set; }
    public string TotalBitRateX { get; set; }
    public string SampleRate { get; set; }
    public string Audio { get; set; }
    public string Stereo { get; set; }
    public string Preset { get; set; }
    public string PixelFormat { get; set; }
    public string Crf { get; set; }
    public string TuneFor { get; set; }
    public string AlignRight { get; set; }
    public string GetStartPosition { get; set; }
    public string GetEndPosition { get; set; }
    public string UseSource { get; set; }
    public string UseSourceResolution { get; set; }
    public string OutputSettings { get; set; }

    public LanguageBurnIn()
    {
        Title = "Generate video with burned-in subtitle";
        InfoAssaOff = "Note: Advanced SubStation Alpha styling supported.";
        InfoAssaOn = "Note: Advanced SubStation Alpha styling will be used :)";
        XGeneratedWithBurnedInSubsInX = "\"{0}\" generated with burned-in subtitle in {1}.";
        TimeRemainingMinutes = "Time remaining: {0} minutes";
        TimeRemainingOneMinute = "Time remaining: One minute";
        TimeRemainingSeconds = "Time remaining: {0} seconds";
        TimeRemainingAFewSeconds = "Time remaining: A few seconds";
        TimeRemainingMinutesAndSeconds = "Time remaining: {0} minutes and {1} seconds";
        TimeRemainingOneMinuteAndSeconds = "Time remaining: One minute and {0} seconds";
        TargetFileName = "Target file name: {0}";
        TargetFileSize = "Target file size (requires 2 pass encoding)";
        FileSizeMb = "File size in MB";
        PassX = "Pass {0}";
        Encoding = "Encoding";
        BitRate = "Bit rate";
        TotalBitRateX = "Total bit rate: {0}";
        SampleRate = "Sample rate";
        Audio = "Audio";
        Stereo = "Stereo";
        Preset = "Preset";
        PixelFormat = "Pixel format";
        Crf = "CRF";
        TuneFor = "Tune for";
        AlignRight = "Align right";
        GetStartPosition = "Get start position";
        GetEndPosition = "Get end position";
        UseSource = "Use source";
        UseSourceResolution = "Use source resolution";
        OutputSettings = "Output file/folder...";
    }
}