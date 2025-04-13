using CommunityToolkit.Mvvm.ComponentModel;
using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Features.Tools.FixCommonErrors;

public partial class FixDisplayItem : ObservableObject
{
    [ObservableProperty]
    public partial string Action { get; set; }

    [ObservableProperty]
    public partial string Before { get; set; }

    [ObservableProperty]
    public partial string After { get; set; }

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    [ObservableProperty]
    public partial int Number { get; set; }
    public Paragraph Paragraph { get; set; }

    public FixDisplayItem(Paragraph p, int number, string action, string before, string after, bool isChecked)
    {
        Paragraph = p;
        Number = number;
        Action = action;
        Before = before;
        After = after;
        IsSelected = isChecked;
    }
}
