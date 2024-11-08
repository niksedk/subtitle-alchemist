﻿using CommunityToolkit.Mvvm.ComponentModel;
using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Logic.Config;

public class SeGeneral
{
    public int LayoutNumber { get; set; } = 0;
    public string FfmpegPath { get; set; } = string.Empty;
    public bool UseTimeFormatHhMmSsFf { get; set; } = false;
    public double DefaultFrameRate { get; set; }
    public double CurrentFrameRate { get; set; }
    public string DefaultSubtitleFormat { get; set; }
    public string DefaultSaveAsFormat { get; set; }
    public string FavoriteSubtitleFormats { get; set; }
    public string DefaultEncoding { get; set; }
    public bool AutoConvertToUtf8 { get; set; }
    public bool AutoGuessAnsiEncoding { get; set; }
    public int SubtitleLineMaximumPixelWidth { get; set; }
    public int SubtitleLineMaximumLength { get; set; }
    public int MaxNumberOfLines { get; set; }
    public int MaxNumberOfLinesPlusAbort { get; set; }
    public int MergeLinesShorterThan { get; set; }
    public int SubtitleMinimumDisplayMilliseconds { get; set; }
    public int SubtitleMaximumDisplayMilliseconds { get; set; }
    public int MinimumMillisecondsBetweenLines { get; set; }
    public double SubtitleMaximumCharactersPerSeconds { get; set; }
    public double SubtitleOptimalCharactersPerSeconds { get; set; }
    public string CpsLineLengthStrategy { get; set; }
    public double SubtitleMaximumWordsPerMinute { get; set; }

    public bool ColorDurationTooShort { get; set; }
    public bool ColorDurationTooLong { get; set; }
    public bool ColorTextTooLong { get; set; }
    public bool ColorTextTooWide { get; set; }
    public bool ColorTextTooManyLines { get; set; }
    public bool ColorTimeCodeOverlap { get; set; }
    public bool ColorGapTooShort { get; set; }

    public SeGeneral()
    {
        LayoutNumber = 0;
        FfmpegPath = string.Empty;
        UseTimeFormatHhMmSsFf = false;
        DefaultFrameRate = 23.976;
        CurrentFrameRate = DefaultFrameRate;
        SubtitleLineMaximumPixelWidth = 576;
        DefaultSubtitleFormat = "SubRip";
        DefaultEncoding = TextEncoding.Utf8WithBom;
        AutoConvertToUtf8 = false;
        AutoGuessAnsiEncoding = true;
        SubtitleLineMaximumLength = 43;
        MaxNumberOfLines = 2;
        MaxNumberOfLinesPlusAbort = 1;
        MergeLinesShorterThan = 33;
        SubtitleMinimumDisplayMilliseconds = 1000;
        SubtitleMaximumDisplayMilliseconds = 8 * 1000;
        MinimumMillisecondsBetweenLines = 24;
        SubtitleMaximumCharactersPerSeconds = 25.0;
        SubtitleOptimalCharactersPerSeconds = 15.0;
        SubtitleMaximumWordsPerMinute = 400;
        DefaultSaveAsFormat = "SubRip";
        FavoriteSubtitleFormats = "SubRip";
        CpsLineLengthStrategy = "";//TODO: Add default value

    }
}