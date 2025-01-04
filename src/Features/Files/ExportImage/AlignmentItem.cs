namespace SubtitleAlchemist.Features.Files.ExportImage;

public partial class AlignmentItem
{
    public string Name { get; set; }
    public AlignmentType Alignment { get; set; }

    public AlignmentItem(string name, AlignmentType alignment)
    {
        Name = name;
        Alignment = alignment;
    }

    override public string ToString()
    {
        return Name;
    }

    public static IEnumerable<AlignmentItem> GetAlignments()
    {
        yield return new AlignmentItem("Top Left", AlignmentType.TopLeft);
        yield return new AlignmentItem("Top Center", AlignmentType.TopCenter);
        yield return new AlignmentItem("Top Right", AlignmentType.TopRight);
        yield return new AlignmentItem("Middle Left", AlignmentType.MiddleLeft);
        yield return new AlignmentItem("Middle Center", AlignmentType.MiddleCenter);
        yield return new AlignmentItem("Middle Right", AlignmentType.MiddleRight);
        yield return new AlignmentItem("Bottom Left", AlignmentType.BottomLeft);
        yield return new AlignmentItem("Bottom Center", AlignmentType.BottomCenter);
        yield return new AlignmentItem("Bottom Right", AlignmentType.BottomRight);
    }
}