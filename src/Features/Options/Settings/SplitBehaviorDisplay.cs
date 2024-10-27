using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Features.Options.Settings;
public class SplitBehaviorDisplay
{
    public string Name { get; set; }
    public SplitBehavior SplitBehavior { get; set; }

    public SplitBehaviorDisplay(string name, SplitBehavior splitBehavior)
    {
        Name = name;
        SplitBehavior = splitBehavior;
    }

    public override string ToString()
    {
        return Name;
    }

    public static List<SplitBehaviorDisplay> GetSplitBehaviors()
    {
        var l = Se.Language.Settings;
        return new List<SplitBehaviorDisplay>
        {
            new(l.SplitBehaviorPrevious, SplitBehavior.SplitBehaviorPrevious),
            new(l.SplitBehaviorHalf, SplitBehavior.SplitBehaviorHalf),
            new(l.SplitBehaviorNext, SplitBehavior.SplitBehaviorNext),
        };
    }
}

public enum SplitBehavior
{
    SplitBehaviorPrevious,
    SplitBehaviorHalf,
    SplitBehaviorNext
}