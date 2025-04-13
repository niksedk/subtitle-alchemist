using CommunityToolkit.Mvvm.ComponentModel;
using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Features.Main;

public partial class DisplayParagraph : ObservableObject
{
    [ObservableProperty]
    public partial TimeSpan Start { get; set; }

    [ObservableProperty]
    public partial TimeSpan End { get; set; }

    [ObservableProperty]
    public partial TimeSpan Duration { get; set; }

    [ObservableProperty]
    public partial string Text { get; set; }

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    [ObservableProperty]
    public partial Color BackgroundColor { get; set; }

    [ObservableProperty]
    public partial int Number { get; set; }
    public Paragraph P { get; set; }

    public DisplayParagraph(Paragraph paragraph)
    {
        BackgroundColor = Colors.Transparent;
        P = paragraph;
        Start = paragraph.StartTime.TimeSpan;
        End = paragraph.EndTime.TimeSpan;
        Duration = paragraph.Duration.TimeSpan;
        Text = paragraph.Text;
        IsSelected = false;
        Number = paragraph.Number;
    }

    public DisplayParagraph(DisplayParagraph paragraph)
    {
        BackgroundColor = Colors.Transparent;
        P = paragraph.P;
        Start = paragraph.Start;
        End = paragraph.End;
        Duration = paragraph.Duration;
        Text = paragraph.Text;
        IsSelected = paragraph.IsSelected;
        Number = paragraph.Number;
    }
}