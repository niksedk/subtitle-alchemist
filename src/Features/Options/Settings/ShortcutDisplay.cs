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