using SkiaSharp;

namespace SubtitleAlchemist.Controls.RadialControl;

public class RadialElement
{
    public string ImageUrl { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool IsMouseOver { get; set; }
    public bool IsSelected { get; set; }
    public SKRect Bounds { get; set; } = new();
}