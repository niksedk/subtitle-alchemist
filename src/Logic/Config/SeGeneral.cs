﻿using Nikse.SubtitleEdit.Core.Settings;

namespace SubtitleAlchemist.Logic.Config;

public class SeGeneral
{
    public int LayoutNumber { get; set; } = 0;
    public string FfmpegPath { get; set; } = string.Empty;
    public string Theme { get; set; } = "Dark";
    public bool UseTimeFormatHhMmSsFf { get; set; } = false;
}