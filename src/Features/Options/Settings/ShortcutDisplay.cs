using System.Text;
using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Features.Options.Settings;
public class ShortcutDisplay
{
    public string Name { get; set; }
    public ShortcutType Keys { get; set; }
    public ShortcutArea Area { get; set; }

    public ShortcutDisplay(ShortcutArea area, string name, ShortcutType keys)
    {
        Area = area;
        Name = name;
        Keys = keys;
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
            new (ShortcutArea.General, l.GeneralMergeSelectedLines, new ShortcutType(ShortcutAction.GeneralMergeSelectedLines, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.General, l.GeneralMergeWithPrevious, new ShortcutType(ShortcutAction.GeneralMergeWithPrevious, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.General, l.GeneralMergeWithNext, new ShortcutType(ShortcutAction.GeneralMergeWithNext, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.General, l.GeneralMergeWithPreviousAndUnbreak, new ShortcutType(ShortcutAction.GeneralMergeWithPreviousAndUnbreak, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.General, l.GeneralMergeWithNextAndUnbreak, new ShortcutType(ShortcutAction.GeneralMergeWithNextAndUnbreak, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.General, l.GeneralMergeWithPreviousAndAutoBreak, new ShortcutType(ShortcutAction.GeneralMergeWithPreviousAndAutoBreak, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.General, l.GeneralMergeWithNextAndAutoBreak, new ShortcutType(ShortcutAction.GeneralMergeWithNextAndAutoBreak, () => { }, new List<string> { "VcControl" })),

            new (ShortcutArea.File, l.FileNew, new ShortcutType(ShortcutAction.FileNew, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.File, l.FileOpen, new ShortcutType(ShortcutAction.FileOpen, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.File, l.FileOpenKeepVideo, new ShortcutType(ShortcutAction.FileOpenKeepVideo, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.File, l.FileSave, new ShortcutType(ShortcutAction.FileSave, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.File, l.FileSaveAs, new ShortcutType(ShortcutAction.FileSaveAs, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.File, l.FileSaveAll, new ShortcutType(ShortcutAction.FileSaveAll, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.File, l.FileSaveOriginal, new ShortcutType(ShortcutAction.FileSaveOriginal, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.File, l.FileSaveOriginalAs, new ShortcutType(ShortcutAction.FileSaveOriginalAs, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.File, l.FileOpenOriginalSubtitle, new ShortcutType(ShortcutAction.FileOpenOriginalSubtitle, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.File, l.FileCloseOriginalSubtitle, new ShortcutType(ShortcutAction.FileCloseOriginalSubtitle, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.File, l.FileTranslatedSubtitle, new ShortcutType(ShortcutAction.FileTranslatedSubtitle, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.File, l.FileCompare, new ShortcutType(ShortcutAction.FileCompare, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.File, l.FileImportPlainText, new ShortcutType(ShortcutAction.FileImportPlainText, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.File, l.FileImportBluRaySupForOcr, new ShortcutType(ShortcutAction.FileImportBluRaySupForOcr, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.File, l.FileImportBluRaySupForEdit, new ShortcutType(ShortcutAction.FileImportBluRaySupForEdit, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.File, l.FileImportTimeCodes, new ShortcutType(ShortcutAction.FileImportTimeCodes, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.File, l.FileExportEbuStl, new ShortcutType(ShortcutAction.FileExportEbuStl, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.File, l.FileExportPac, new ShortcutType(ShortcutAction.FileExportPac, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.File, l.FileExportEdlClipName, new ShortcutType(ShortcutAction.FileExportEdlClipName, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.File, l.FileExportPlainText, new ShortcutType(ShortcutAction.FileExportPlainText, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.File, l.FileExportCustomTextFormat1, new ShortcutType(ShortcutAction.FileExportCustomTextFormat1, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.File, l.FileExportCustomTextFormat2, new ShortcutType(ShortcutAction.FileExportCustomTextFormat2, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.File, l.FileExportCustomTextFormat3, new ShortcutType(ShortcutAction.FileExportCustomTextFormat3, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.File, l.FileExit, new ShortcutType(ShortcutAction.FileExit, () => { }, new List<string> { "VcControl" })),

            new (ShortcutArea.Edit, l.EditFind, new ShortcutType(ShortcutAction.EditFind, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.Edit, l.EditFindNext, new ShortcutType(ShortcutAction.EditFindNext, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.Edit, l.EditReplace, new ShortcutType(ShortcutAction.EditReplace, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.Edit, l.EditMultipleReplace, new ShortcutType(ShortcutAction.EditMultipleReplace, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.Edit, l.EditModifySelection, new ShortcutType(ShortcutAction.EditModifySelection, () => { }, new List<string> { "VcControl" })),
            new (ShortcutArea.Edit, l.EditGoToSubtitleNumber, new ShortcutType(ShortcutAction.EditGoToSubtitleNumber, () => { }, new List<string> { "VcControl" })),



        };
    }
}

public class ShortcutType
{
    public ShortcutAction ActionName { get; set; }
    public Action Action { get; set; }
    public List<string> Keys { get; set; }

    public ShortcutType(ShortcutAction actionName, Action action, List<string> keys)
    {
        ActionName = actionName;
        Action = action;
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