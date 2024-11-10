namespace SubtitleAlchemist.Logic.Config;

public class SeBatchConvert
{
    public string[] ActiveFunctions { get; set; } = Array.Empty<string>();
    public bool UseOutputFolder { get; set; }
    public string OutputFolder { get; set; }
    public bool Overwrite { get; set; }
    public string TargetFormat { get; set; }
    public string TargetEncoding { get; set; }

    public bool FormattingRemoveAll { get; set; }
    public bool FormattingRemoveItalic { get; set; }
    public bool FormattingRemoveUnderline { get; set; }
    public bool FormattingRemoveFontTags { get; set; }
    public bool FormattingRemoveColorTags { get; set; }
    public bool FormattingRemoveBold { get; set; }
    public bool FormattingRemoveAlignmentTags { get; set; }

    public double OffsetTimeCodesMilliseconds { get; set; }
    public bool OffsetTimeCodesForward { get; set; }

    public SeBatchConvert()
    {
        OutputFolder = string.Empty;
        UseOutputFolder = true;
        TargetFormat = string.Empty;
        TargetEncoding = string.Empty;
        OffsetTimeCodesForward = true;
    }
}