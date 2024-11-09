namespace SubtitleAlchemist.Features.Tools.BatchConvert;

public class BatchConvertFunction
{
    public BatchConvertFunctionType Type { get; set; }
    public string Name { get; set; }
    public bool IsSelected { get; set; }
    public View View { get; set; }

    public override string ToString()
    {
        return Name;
    }

    public BatchConvertFunction(BatchConvertFunctionType type, string name, bool isSelected, View view)
    {
        Type = type;
        Name = name;
        IsSelected = isSelected;
        View = view;
    }
}