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
    public string FileSaveOriginal { get; set; }
    public string FileSaveOriginalAs { get; set; }
    public string FileOpenOriginalSubtitle { get; set; }
    public string FileCloseOriginalSubtitle { get; set; }
    public string FileTranslatedSubtitle { get; set; }
    public string FileCompare { get; set; }
    public string FileImportPlainText { get; set; }
    public string FileImportBluRaySupForOcr { get; set; }
    public string FileImportBluRaySupForEdit { get; set; }
    public string FileImportTimeCodes { get; set; }
    public string FileExportEbuStl { get; set; }
    public string FileExportPac { get; set; }
    public string FileExportEdlClipName { get; set; }
    public string FileExportPlainText { get; set; }
    public string FileExportCustomTextFormat1 { get; set; }
    public string FileExportCustomTextFormat2 { get; set; }
    public string FileExportCustomTextFormat3 { get; set; }
    public string FileExit { get; set; }
    public string EditFind { get; set; }
    public string EditFindNext { get; set; }
    public string EditReplace { get; set; }
    public string EditMultipleReplace { get; set; }
    public string EditModifySelection { get; set; }
    public string EditGoToSubtitleNumber { get; set; }

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
        FileSaveOriginal = "Save original";
        FileSaveOriginalAs = "Save original as";
        FileOpenOriginalSubtitle = "Open original subtitle";
        FileCloseOriginalSubtitle = "Close original subtitle";
        FileTranslatedSubtitle = "Translated subtitle";
        FileCompare = "Compare";
        FileImportPlainText = "Import plain text";
        FileImportBluRaySupForOcr = "Import Blu-ray SUP for OCR";
        FileImportBluRaySupForEdit = "Import Blu-ray SUP for edit";
        FileImportTimeCodes = "Import time codes";
        FileExportEbuStl = "Export EBU STL";
        FileExportPac = "Export PAC";
        FileExportEdlClipName = "Export EDL clip name";
        FileExportPlainText = "Export plain text";
        FileExportCustomTextFormat1 = "Export custom text format 1";
        FileExportCustomTextFormat2 = "Export custom text format 2";
        FileExportCustomTextFormat3 = "Export custom text format 3";
        FileExit = "Exit";

        EditFind = "Find";
        EditFindNext = "Find next";
        EditReplace = "Replace";
        EditMultipleReplace = "Multiple replace";
        EditModifySelection = "Modify selection";
        EditGoToSubtitleNumber = "Go to subtitle number";

    }
}