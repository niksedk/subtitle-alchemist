using CommunityToolkit.Mvvm.ComponentModel;

namespace SubtitleAlchemist.Features.Files.ExportImage;

public partial class BorderStyleItem : ObservableObject
{
    public string Name { get; set; }
    public BorderStyleType BorderStyle { get; set; }

    public BorderStyleItem(string name, BorderStyleType borderStyle)
    {
        Name = name;
        BorderStyle = borderStyle;
    }

    override public string ToString()
    {
        return Name;
    }

    public static List<BorderStyleItem> GetBorderStyles()
    {
        var result = new List<BorderStyleItem>();
        result.Add(new BorderStyleItem("None", BorderStyleType.None));
        result.Add(new BorderStyleItem("One box", BorderStyleType.OneBox));
        result.Add(new BorderStyleItem("Box per line", BorderStyleType.BoxPerLine));

        for (var i = 1; i <= 20; i++)
        {
            result.Add(new BorderStyleItem(i.ToString(), BorderStyleType.Numbered));
        }

        return result;
    }
}