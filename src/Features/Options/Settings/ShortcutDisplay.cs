using System.Text;
using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Features.Options.Settings;
public class ShortcutDisplay
{
    public string Name { get; set; }
    public ShortcutType Keys { get; set; }
    public string Area { get; set; }

    public ShortcutDisplay(string area, string name, ShortcutType keys)
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
        var l = Se.Language.Settings;
        return new List<ShortcutDisplay>
        {
            new ("General",l.DialogStyle, new ShortcutType(() => { }, new List<string> { "VcControl" })),
            new ("General",l.DialogStyle, new ShortcutType(() => { }, new List<string> { "VcUp" })),
            new ("General",l.DialogStyle, new ShortcutType(() => { }, new List<string> { "VcDown" })),
            new ("General",l.DialogStyle, new ShortcutType(() => { }, new List<string> { "VcHome" })),
            new ("General",l.DialogStyle, new ShortcutType(() => { }, new List<string> { "VcEnd" })),
            new ("General",l.DialogStyle, new ShortcutType(() => { }, new List<string> { "VcEnter" })),
        };
    }
}

public class ShortcutType
{
    public Action Action { get; set; }
    public List<string> Keys { get; set; }

    public ShortcutType(Action action, List<string> keys)
    {
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