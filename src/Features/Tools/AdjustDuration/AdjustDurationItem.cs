namespace SubtitleAlchemist.Features.Tools.AdjustDuration;
public class AdjustDurationItem
{
    public AdjustDurationType Type { get; set; }
    public string Name { get; set; }

    public AdjustDurationItem(AdjustDurationType type, string name)
    {
        Type = type;
        Name = name;
    }

    public override string ToString()
    {
        return Name;
    }
}