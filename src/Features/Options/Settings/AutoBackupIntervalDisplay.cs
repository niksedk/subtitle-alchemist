using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Features.Options.Settings;
public class AutoBackupIntervalDisplay
{
    public string Name { get; set; }
    public int Minutes { get; set; }

    public AutoBackupIntervalDisplay(string name, int minutes)
    {
        Name = name;
        Minutes = minutes;
    }

    public override string ToString()
    {
        return Name;
    }

    public static List<AutoBackupIntervalDisplay> GetAutoBackupIntervals()
    {
        var l = Se.Language.Settings;
        return new List<AutoBackupIntervalDisplay>
        {
            new(l.AutoBackupEveryMinute, 1),
            new(string.Format(l.AutoBackupEveryXthMinute, 5) , 5),
            new(string.Format(l.AutoBackupEveryXthMinute, 15) , 15),
        };
    }
}
