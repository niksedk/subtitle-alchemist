using System.Text;
using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Features.Options.Settings;
public class ShortcutDisplay
{
    public string Name { get; set; }
    public ShortcutType Type { get; set; }
    public ShortcutArea Area { get; set; }

    public ShortcutDisplay(ShortcutArea area, string name, ShortcutType keys)
    {
        Area = area;
        Name = name;
        Type = keys;
    }

    public ShortcutDisplay(ShortcutArea area, string name, ShortcutAction action)
    {
        Area = area;
        Name = name;

        var keys = Se.Settings.Shortcuts.FirstOrDefault(p => p.ActionName == action);
        if (keys != null)
        {
            Type = new ShortcutType(action, keys.Keys);
        }
        else
        {
            Type = new ShortcutType(action, new List<string>());
        }

    }

    public override string ToString()
    {
        return Name;
    }

    public static List<ShortcutDisplay> GetShortcuts()
    {
        var l = Se.Language.Settings.Shortcuts;
        return new List<ShortcutDisplay>
        {
            new (ShortcutArea.General, l.GeneralMergeSelectedLines, ShortcutAction.GeneralMergeSelectedLines),
            new (ShortcutArea.General, l.GeneralMergeWithPrevious, ShortcutAction.GeneralMergeWithPrevious),
            new (ShortcutArea.General, l.GeneralMergeWithNext, ShortcutAction.GeneralMergeWithNext),
            new (ShortcutArea.General, l.GeneralMergeWithPreviousAndUnbreak, ShortcutAction.GeneralMergeWithPreviousAndUnbreak),
            new (ShortcutArea.General, l.GeneralMergeWithNextAndUnbreak, ShortcutAction.GeneralMergeWithNextAndUnbreak),
            new (ShortcutArea.General, l.GeneralMergeWithPreviousAndAutoBreak, ShortcutAction.GeneralMergeWithPreviousAndAutoBreak),
            new (ShortcutArea.General, l.GeneralMergeWithNextAndAutoBreak, ShortcutAction.GeneralMergeWithNextAndAutoBreak),
            new (ShortcutArea.General, l.GeneralMergeSelectedLinesAndAutoBreak, ShortcutAction.GeneralMergeSelectedLinesAndAutoBreak),
            new (ShortcutArea.General, l.GeneralMergeSelectedLinesAndUnbreak, ShortcutAction.GeneralMergeSelectedLinesAndUnbreak),
            new (ShortcutArea.General, l.GeneralMergeSelectedLinesAndUnbreakCjk, ShortcutAction.GeneralMergeSelectedLinesAndUnbreakCjk),
            new (ShortcutArea.General, l.GeneralMergeSelectedLinesOnlyFirstText, ShortcutAction.GeneralMergeSelectedLinesOnlyFirstText),
            new (ShortcutArea.General, l.GeneralMergeSelectedLinesBilingual, ShortcutAction.GeneralMergeSelectedLinesBilingual),
            new (ShortcutArea.General, l.GeneralMergeWithPreviousBilingual, ShortcutAction.GeneralMergeWithPreviousBilingual),
            new (ShortcutArea.General, l.GeneralMergeWithNextBilingual, ShortcutAction.GeneralMergeWithNextBilingual),
            new (ShortcutArea.General, l.GeneralMergeOriginalAndTranslation, ShortcutAction.GeneralMergeOriginalAndTranslation),
            new (ShortcutArea.General, l.GeneralToggleTranslationMode, ShortcutAction.GeneralToggleTranslationMode),
            new (ShortcutArea.General, l.GeneralSwitchOriginalAndTranslation, ShortcutAction.GeneralSwitchOriginalAndTranslation),
            new (ShortcutArea.General, l.GeneralSwitchOriginalAndTranslationTextBoxes, ShortcutAction.GeneralSwitchOriginalAndTranslationTextBoxes),
            new (ShortcutArea.General, l.GeneralChooseLayout, ShortcutAction.GeneralChooseLayout),
            new (ShortcutArea.General, string.Format(l.GeneralLayoutChooseX, 1), ShortcutAction.GeneralLayoutChoose1),
            new (ShortcutArea.General, string.Format(l.GeneralLayoutChooseX, 2), ShortcutAction.GeneralLayoutChoose2),
            new (ShortcutArea.General, string.Format(l.GeneralLayoutChooseX, 3), ShortcutAction.GeneralLayoutChoose3),
            new (ShortcutArea.General, string.Format(l.GeneralLayoutChooseX, 4), ShortcutAction.GeneralLayoutChoose4),
            new (ShortcutArea.General, string.Format(l.GeneralLayoutChooseX, 5), ShortcutAction.GeneralLayoutChoose5),
            new (ShortcutArea.General, string.Format(l.GeneralLayoutChooseX, 6), ShortcutAction.GeneralLayoutChoose6),
            new (ShortcutArea.General, string.Format(l.GeneralLayoutChooseX, 7), ShortcutAction.GeneralLayoutChoose7),
            new (ShortcutArea.General, string.Format(l.GeneralLayoutChooseX, 8), ShortcutAction.GeneralLayoutChoose8),
            new (ShortcutArea.General, string.Format(l.GeneralLayoutChooseX, 9), ShortcutAction.GeneralLayoutChoose9),
            new (ShortcutArea.General, string.Format(l.GeneralLayoutChooseX, 10), ShortcutAction.GeneralLayoutChoose10),
            new (ShortcutArea.General, string.Format(l.GeneralLayoutChooseX, 11), ShortcutAction.GeneralLayoutChoose11),
            new (ShortcutArea.General, string.Format(l.GeneralLayoutChooseX, 12), ShortcutAction.GeneralLayoutChoose12),
            new (ShortcutArea.General, l.GeneralPlayFirstSelected, ShortcutAction.GeneralPlayFirstSelected),
            new (ShortcutArea.General, l.GeneralGoToFirstSelectedLine, ShortcutAction.GeneralGoToFirstSelectedLine),
            new (ShortcutArea.General, l.GeneralGoToNextEmptyLine, ShortcutAction.GeneralGoToNextEmptyLine),
            new (ShortcutArea.General, l.GeneralGoToNextSubtitle, ShortcutAction.GeneralGoToNextSubtitle),
            new (ShortcutArea.General, l.GeneralGoToNextSubtitlePlayTranslate, ShortcutAction.GeneralGoToNextSubtitlePlayTranslate),
            new (ShortcutArea.General, l.GeneralGoToNextSubtitleCursorAtEnd, ShortcutAction.GeneralGoToNextSubtitleCursorAtEnd),
            new (ShortcutArea.General, l.GeneralGoToPrevSubtitle, ShortcutAction.GeneralGoToPrevSubtitle),
            new (ShortcutArea.General, l.GeneralGoToPrevSubtitlePlayTranslate, ShortcutAction.GeneralGoToPrevSubtitlePlayTranslate),
            new (ShortcutArea.General, l.GeneralGoToStartOfCurrentSubtitle, ShortcutAction.GeneralGoToStartOfCurrentSubtitle),
            new (ShortcutArea.General, l.GeneralGoToEndOfCurrentSubtitle, ShortcutAction.GeneralGoToEndOfCurrentSubtitle),
            new (ShortcutArea.General, l.GeneralGoToPreviousSubtitleAndFocusVideo, ShortcutAction.GeneralGoToPreviousSubtitleAndFocusVideo),
            new (ShortcutArea.General, l.GeneralGoToNextSubtitleAndFocusVideo, ShortcutAction.GeneralGoToNextSubtitleAndFocusVideo),
            new (ShortcutArea.General, l.GeneralGoToPrevSubtitleAndPlay, ShortcutAction.GeneralGoToPrevSubtitleAndPlay),
            new (ShortcutArea.General, l.GeneralGoToNextSubtitleAndPlay, ShortcutAction.GeneralGoToNextSubtitleAndPlay),
            new (ShortcutArea.General, l.GeneralGoToPreviousSubtitleAndFocusWaveform, ShortcutAction.GeneralGoToPreviousSubtitleAndFocusWaveform),
            new (ShortcutArea.General, l.GeneralGoToNextSubtitleAndFocusWaveform, ShortcutAction.GeneralGoToNextSubtitleAndFocusWaveform),
            new (ShortcutArea.General, l.GeneralGoToLineNumber, ShortcutAction.GeneralGoToLineNumber),
            new (ShortcutArea.General, l.GeneralToggleBookmarks, ShortcutAction.GeneralToggleBookmarks),
            new (ShortcutArea.General, l.GeneralFocusTextBox, ShortcutAction.GeneralFocusTextBox),
            new (ShortcutArea.General, l.GeneralToggleBookmarksWithText, ShortcutAction.GeneralToggleBookmarksWithText),
            new (ShortcutArea.General, l.GeneralEditBookmarks, ShortcutAction.GeneralEditBookmarks),

            new (ShortcutArea.File, l.FileNew, ShortcutAction.FileNew),
            new (ShortcutArea.File, l.FileOpen, ShortcutAction.FileOpen),
            new (ShortcutArea.File, l.FileOpenKeepVideo, ShortcutAction.FileOpenKeepVideo),
            new (ShortcutArea.File, l.FileSave, ShortcutAction.FileSave),
            new (ShortcutArea.File, l.FileSaveAs, ShortcutAction.FileSaveAs),
            new (ShortcutArea.File, l.FileSaveAll, ShortcutAction.FileSaveAll),
            new (ShortcutArea.File, l.FileSaveOriginal, ShortcutAction.FileSaveOriginal),
            new (ShortcutArea.File, l.FileSaveOriginalAs, ShortcutAction.FileSaveOriginalAs),
            new (ShortcutArea.File, l.FileOpenOriginalSubtitle, ShortcutAction.FileOpenOriginalSubtitle),
            new (ShortcutArea.File, l.FileCloseOriginalSubtitle, ShortcutAction.FileCloseOriginalSubtitle),
            new (ShortcutArea.File, l.FileTranslatedSubtitle, ShortcutAction.FileTranslatedSubtitle),
            new (ShortcutArea.File, l.FileCompare, ShortcutAction.FileCompare),
            new (ShortcutArea.File, l.FileImportPlainText, ShortcutAction.FileImportPlainText),
            new (ShortcutArea.File, l.FileImportBluRaySupForOcr, ShortcutAction.FileImportBluRaySupForOcr),
            new (ShortcutArea.File, l.FileImportBluRaySupForEdit, ShortcutAction.FileImportBluRaySupForEdit),
            new (ShortcutArea.File, l.FileImportTimeCodes, ShortcutAction.FileImportTimeCodes),
            new (ShortcutArea.File, l.FileExportEbuStl, ShortcutAction.FileExportEbuStl),
            new (ShortcutArea.File, l.FileExportPac, ShortcutAction.FileExportPac),
            new (ShortcutArea.File, l.FileExportEdlClipName, ShortcutAction.FileExportEdlClipName),
            new (ShortcutArea.File, l.FileExportPlainText, ShortcutAction.FileExportPlainText),
            new (ShortcutArea.File, l.FileExportCustomTextFormat1, ShortcutAction.FileExportCustomTextFormat1),
            new (ShortcutArea.File, l.FileExportCustomTextFormat2, ShortcutAction.FileExportCustomTextFormat2),
            new (ShortcutArea.File, l.FileExportCustomTextFormat3, ShortcutAction.FileExportCustomTextFormat3),
            new (ShortcutArea.File, l.FileExit, ShortcutAction.FileExit),

            new (ShortcutArea.Edit, l.EditFind, ShortcutAction.EditFind),
            new (ShortcutArea.Edit, l.EditFindNext, ShortcutAction.EditFindNext),
            new (ShortcutArea.Edit, l.EditReplace, ShortcutAction.EditReplace),
            new (ShortcutArea.Edit, l.EditMultipleReplace, ShortcutAction.EditMultipleReplace),
            new (ShortcutArea.Edit, l.EditModifySelection, ShortcutAction.EditModifySelection),
        };
    }
}

public class ShortcutType
{
    public ShortcutAction ActionName { get; set; }
    public List<string> Keys { get; set; }

    public ShortcutType(ShortcutAction actionName, List<string> keys)
    {
        ActionName = actionName;
        Keys = keys;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var key in Keys)
        {
            var s = key;
            if (s.StartsWith("Vc"))
            {
                s = s.Remove(0, 2);
            }
            sb.Append(s);
            sb.Append("+");
        }

        return sb.ToString().TrimEnd('+');
    }
}