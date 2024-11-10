﻿using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.SubtitleFormats;

namespace SubtitleAlchemist.Features.Tools.BatchConvert;

public class BatchConvertConfig
{
    public string OutputFolder { get; set; }
    public bool SaveInSourceFolder { get; set; }
    public bool Overwrite { get; set; }
    public string TargetFormatName { get; set; }
    public string TargetEncoding { get; set; }
    
    public RemoveFormattingSettings RemoveFormatting { get; set; }
    public OffsetTimeCodesSettings OffsetTimeCodes { get; set; }
    public AdjustDurationSettings AdjustDuration { get; set; }
    public ChangeFrameRateSettings ChangeFrameRate { get; set; }
    public RemoveLineBreaksSettings RemoveLineBreaks { get; set; }
    public DeleteLinesSettings DeleteLines { get; set; }
    public AdjustDisplayDurationSettings AdjustDisplayDuration { get; set; }

    public BatchConvertConfig()
    {
        OutputFolder = string.Empty;
        SaveInSourceFolder = true;
        TargetFormatName = SubRip.NameOfFormat;
        TargetEncoding = TextEncoding.Utf8WithBom;
        RemoveFormatting = new RemoveFormattingSettings();
        OffsetTimeCodes = new OffsetTimeCodesSettings();
        AdjustDuration = new AdjustDurationSettings();
        RemoveLineBreaks = new RemoveLineBreaksSettings();
        ChangeFrameRate = new ChangeFrameRateSettings();
        DeleteLines = new DeleteLinesSettings();
        AdjustDisplayDuration = new AdjustDisplayDurationSettings();
    }

    public class RemoveFormattingSettings
    {
        public bool IsActive { get; set; }
        public bool RemoveAll { get; set; }
        public bool RemoveItalic { get; set; }
        public bool RemoveBold { get; set; }
        public bool RemoveUnderline { get; set; }
        public bool RemoveColor { get; set; }
        public bool RemoveFontName { get; set; }
        public bool RemoveAlignment { get; set; }
    }

    public class OffsetTimeCodesSettings
    {
        public bool IsActive { get; set; }
        public bool Forward { get; set; }
        public long Milliseconds { get; set; }
    }

    public class AdjustDurationSettings
    {
        public bool IsActive { get; set; }
        public string AdjustmentType { get; set; }
        public int Percentage { get; set; }
        public int FixedMilliseconds { get; set; }
        public double MaxCharsSecond { get; set; }

        public AdjustDurationSettings()
        {
            AdjustmentType = string.Empty;
        }
    }

    public class ChangeFrameRateSettings
    {
        public bool IsActive { get; set; }
        public double FromFrameRate { get; set; }
        public double ToFrameRate { get; set; }
    }

    public class RemoveLineBreaksSettings
    {
        public bool IsActive { get; set; }
    }

    public class DeleteLinesSettings
    {
        public bool IsActive { get; set; }
        public int DeleteXFirst { get; set; }
        public int DeleteXLast { get; set; }
        public string DeleteContains { get; set; }

        public DeleteLinesSettings()
        {
            DeleteContains = string.Empty;
        }
    }

    public class AdjustDisplayDurationSettings
    {
        public bool IsActive { get; set; }
        public string AdjustmentType { get; set; }
        public int Percentage { get; set; }
        public int FixedMilliseconds { get; set; }
        public double MaxCharsSecond { get; set; }

        public AdjustDisplayDurationSettings()
        {
            AdjustmentType = string.Empty;
        }
    }
}

