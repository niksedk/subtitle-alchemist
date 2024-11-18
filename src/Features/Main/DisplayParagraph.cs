using CommunityToolkit.Mvvm.ComponentModel;
using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Features.Main;

public partial class DisplayParagraph : ObservableObject
{
    [ObservableProperty]
    public TimeSpan _start;

    [ObservableProperty]
    public TimeSpan _end;

    [ObservableProperty]
    public TimeSpan _duration;

    [ObservableProperty]
    private string _text;

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private Color _backgroundColor;

    [ObservableProperty]
    private int _number;

    public Paragraph P { get; set; }

    public DisplayParagraph(Paragraph paragraph)
    {
        _backgroundColor = Colors.Transparent;
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
        _backgroundColor = Colors.Transparent;
        P = paragraph.P;
        Start = paragraph.Start;
        End = paragraph.End;
        Duration = paragraph.Duration;
        Text = paragraph.Text;
        IsSelected = paragraph.IsSelected;
        Number = paragraph.Number;
    }
}