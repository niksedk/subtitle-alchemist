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
            new (ShortcutArea.Edit, l.EditGoToSubtitleNumber, ShortcutAction.EditGoToSubtitleNumber),
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