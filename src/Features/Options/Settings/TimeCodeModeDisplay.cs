using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Features.Options.Settings;
public class TimeCodeModeDisplay
{
    public string Name { get; set; }
    public TimeCodeMode Mode { get; set; }

    public TimeCodeModeDisplay(string name, TimeCodeMode mode)
    {
        Name = name;
        Mode = mode;
    }

    public override string ToString()
    {
        return Name;
    }

    public static List<TimeCodeModeDisplay> GetTimeCodeModes()
    {
        var l = Se.Language.Settings;
        return new List<TimeCodeModeDisplay>
        {
            new(l.TimeCodeModeHhMmSsMs, TimeCodeMode.HhMmSsMs),
            new(l.TimeCodeModeHhMmSsFf, TimeCodeMode.HhMmSsFf),
        };
    }
}