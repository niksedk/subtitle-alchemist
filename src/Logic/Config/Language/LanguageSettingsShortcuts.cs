namespace SubtitleAlchemist.Logic.Config.Language;

public class LanguageSettingsShortcuts
{
    public string GeneralMergeSelectedLines { get; set; }
    public string GeneralMergeWithPrevious { get; set; }
    public string GeneralMergeWithNext { get; set; }
    public string GeneralMergeWithPreviousAndUnbreak { get; set; }
    public string GeneralMergeWithNextAndUnbreak { get; set; }
    public string GeneralMergeWithPreviousAndAutoBreak { get; set; }
    public string GeneralMergeWithNextAndAutoBreak { get; set; }
    public string FileNew { get; set; }
    public string FileOpen { get; set; }
    public string FileOpenKeepVideo { get; set; }
    public string FileSave { get; set; }
    public string FileSaveAs { get; set; }
    public string FileSaveAll { get; set; }

    public LanguageSettingsShortcuts()
    {
        GeneralMergeSelectedLines = "Merge selected lines";
        GeneralMergeWithPrevious = "Merge with previous";
        GeneralMergeWithNext = "Merge with next";
        GeneralMergeWithPreviousAndUnbreak = "Merge with previous and unbreak";
        GeneralMergeWithNextAndUnbreak = "Merge with next and unbreak";
        GeneralMergeWithPreviousAndAutoBreak = "Merge with previous and auto-break";
        GeneralMergeWithNextAndAutoBreak = "Merge with next and auto-break";
        FileNew = "New";
        FileOpen = "Open";
        FileOpenKeepVideo = "Open (keep video)";
        FileSave = "Save";
        FileSaveAs = "Save as";
        FileSaveAll = "Save all";
    }
}