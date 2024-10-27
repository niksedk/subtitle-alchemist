using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Features.Options.Settings;
public class AutoBackupDeleteDisplay
{
    public string Name { get; set; }
    public int Months { get; set; }

    public AutoBackupDeleteDisplay(string name, int months)
    {
        Name = name;
        Months = months;
    }

    public override string ToString()
    {
        return Name;
    }

    public static List<AutoBackupDeleteDisplay> GetAutoBackupDeleteOptions()
    {
        var l = Se.Language.Settings;
        var list = new List<AutoBackupDeleteDisplay>();
        for (var i= 1; i < 12; i++)
        {
            list.Add(new(string.Format(l.AutoBackupDeleteAfterXMonths, i), i));
        }

        return list;
    }
}
