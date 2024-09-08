﻿using SubtitleAlchemist.Features.Video.AudioToTextWhisper.Engines;

namespace SubtitleAlchemist.Logic.Config;

public class SeTools
{
    public SeAudioToText AudioToText { get; set; } = new();
    public SeFixCommonErrors FixCommonErrors { get; set; } = new();
    public string AutoTranslateLastName { get; set; } = string.Empty;
}

public class SeAudioToText
{
    public bool PostProcessing { get; set; } = true;

    public string WhisperChoice { get; set; } = WhisperEngineCpp.StaticName;

    public bool WhisperIgnoreVersion { get; set; } = false;

    public bool WhisperDeleteTempFiles { get; set; } = true;

    public string? WhisperModel { get; set; } = string.Empty;

    public string WhisperLanguageCode { get; set; } = string.Empty;

    public string WhisperLocation { get; set; } = string.Empty;

    public string WhisperCtranslate2Location { get; set; } = string.Empty;

    public string WhisperPurfviewFasterWhisperLocation { get; set; } = string.Empty;

    public string WhisperPurfviewFasterWhisperDefaultCmd { get; set; } = string.Empty;

    public string WhisperXLocation { get; set; } = string.Empty;

    public string WhisperStableTsLocation { get; set; } = string.Empty;

    public string WhisperCppModelLocation { get; set; } = string.Empty;

    public string WhisperCustomCommandLineArguments { get; set; } = string.Empty;
    public bool WhisperCustomCommandLineArgumentsPurfviewBlank { get; set; }

    public string WhisperExtraSettingsHistory { get; set; } = string.Empty;

    public bool WhisperAutoAdjustTimings { get; set; } = true;

    public bool WhisperUseLineMaxChars { get; set; } = true;

    public bool WhisperPostProcessingAddPeriods { get; set; } = false;

    public bool WhisperPostProcessingMergeLines { get; set; } = true;

    public bool WhisperPostProcessingSplitLines { get; set; } = true;

    public bool WhisperPostProcessingFixCasing { get; set; } = false;

    public bool WhisperPostProcessingFixShortDuration { get; set; } = true;
}