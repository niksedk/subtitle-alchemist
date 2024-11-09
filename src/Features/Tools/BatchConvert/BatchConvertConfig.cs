using Nikse.SubtitleEdit.Core.Common;
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

    public BatchConvertConfig()
    {
        OutputFolder = string.Empty;
        SaveInSourceFolder = true;
        TargetFormatName = SubRip.NameOfFormat;
        TargetEncoding = TextEncoding.Utf8WithBom;
        RemoveFormatting = new RemoveFormattingSettings();
        OffsetTimeCodes = new OffsetTimeCodesSettings();
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
}

