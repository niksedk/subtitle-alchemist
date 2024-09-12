using CommunityToolkit.Mvvm.ComponentModel;
using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Features.Tools.FixCommonErrors;

public partial class FixDisplayItem : ObservableObject
{
    [ObservableProperty]
    private string _action;

    [ObservableProperty]
    private string _before;

    [ObservableProperty]
    private string _after;

    [ObservableProperty] 
    private bool _isSelected;

    [ObservableProperty]
    private int _number;

    public Paragraph Paragraph { get; set; }

    public FixDisplayItem(Paragraph p, int number, string action, string before, string after, bool isChecked)
    {
        Paragraph = p;
        _number = number;
        _action = action;
        _before = before;
        _after = after;
        _isSelected = isChecked;
    }
}
