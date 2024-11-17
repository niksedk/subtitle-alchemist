using CommunityToolkit.Mvvm.ComponentModel;

namespace SubtitleAlchemist.Features.Tools.ChangeCasing;

public partial class FixNameItem : ObservableObject
{
    public string Name { get; set; }

    [ObservableProperty] private bool _isChecked;

    public FixNameItem(string name, bool isChecked)
    {
        Name = name;
        _isChecked = isChecked;
    }

    public override string ToString()
    {
        return Name;
    }
}
