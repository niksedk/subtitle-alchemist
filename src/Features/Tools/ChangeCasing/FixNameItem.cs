using CommunityToolkit.Mvvm.ComponentModel;

namespace SubtitleAlchemist.Features.Tools.ChangeCasing;

public partial class FixNameItem : ObservableObject
{
    public string Name { get; set; }

    [ObservableProperty]
    public partial bool IsChecked { get; set; }

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
