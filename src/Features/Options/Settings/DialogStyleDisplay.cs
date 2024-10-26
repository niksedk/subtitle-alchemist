using Nikse.SubtitleEdit.Core.Enums;
using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Features.Options.Settings;
public class DialogStyleDisplay
{
    public string Name { get; set; }
    public DialogType Style { get; set; }

    public DialogStyleDisplay(string name, DialogType style)
    {
        Name = name;
        Style = style;
    }

    public override string ToString()
    {
        return Name;
    }

    public static List<DialogStyleDisplay> GetDialogStyles()
    {
        var l = Se.Language.Settings;
        return new List<DialogStyleDisplay>
        {
            new(l.DialogStyleDashBothLinesWithSpace, DialogType.DashBothLinesWithSpace),
            new(l.DialogStyleDashBothLinesWithoutSpace, DialogType.DashBothLinesWithoutSpace),
            new(l.DialogStyleDashSecondLineWithSpace, DialogType.DashSecondLineWithSpace),
            new(l.DialogStyleDashSecondLineWithoutSpace, DialogType.DashSecondLineWithoutSpace),
        };
    }
}
