namespace SubtitleAlchemist.Features.Tools.ChangeCasing;

public class FixNameItem
{
    public string Name { get; set; }
    public bool IsChecked { get; set; }

    public FixNameItem(string name, bool isChecked)
    {
        Name = name;
        IsChecked = isChecked;
    }

    public override string ToString()
    {
        return Name;
    }
}
