namespace SubtitleAlchemist.Features.Tools.ChangeCasing;

public class FixNameHitItem
{
    public string Name { get; set; }
    public int LineIndex { get; set; }
    public string Before { get; set; }
    public string After { get; set; }

    public FixNameHitItem(string name, int lineIndex, string before, string after)
    {
        Name = name;
        LineIndex = lineIndex;
        Before = before;
        After = after;
    }

    public override string ToString()
    {
        return Name;
    }
}
