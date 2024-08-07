﻿using CommunityToolkit.Mvvm.ComponentModel;
using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Features.Main;

public partial class DisplayParagraph : ObservableObject
{
    [ObservableProperty] 
    public string _start;

    public string End => P.EndTime.ToDisplayString();
    public string Duration => P.Duration.ToShortDisplayString();

    [ObservableProperty] 
    private string _text;

    [ObservableProperty] 
    private bool _isSelected;

    [ObservableProperty]
    private int _number;

    public Paragraph P { get; set; }

    [ObservableProperty]
    public Color _backgroundColor;

    public DisplayParagraph(Paragraph paragraph)
    {
        P = paragraph;
        Start = paragraph.StartTime.ToDisplayString();
        Text = paragraph.Text;
        BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"];
        IsSelected = false;
        Number = paragraph.Number;
    }
}