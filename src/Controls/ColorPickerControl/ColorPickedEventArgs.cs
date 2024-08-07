using SkiaSharp;

namespace SubtitleAlchemist.Controls.ColorPickerControl;

public class ColorPickedEventArgs : EventArgs
{
    public ColorPickedEventArgs(SKColor skColor, Color color)
    {
        SkColor = skColor;
        Color = color;
    }

    public SKColor SkColor { get; private set; }

    public Color Color { get; private set; }
}