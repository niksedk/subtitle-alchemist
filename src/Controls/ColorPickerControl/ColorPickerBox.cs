using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace SubtitleAlchemist.Controls.ColorPickerControl;

public class ColorPickerBox : SKCanvasView
{
    public event EventHandler<ColorPickedEventArgs>? ColorPicked;
    public event EventHandler<ColorPickedEventArgs>? ColorHover;

    public ColorPickerBox()
    {
        EnableTouchEvents = true;
        Touch += OnCanvasViewTouch;
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        base.OnPaintSurface(e);

        var canvas = e.Surface.Canvas;
        canvas.Clear();

        var width = e.Info.Width;
        var height = e.Info.Height;
        var size = Math.Min(width, height);

        DrawColorPickerSquare(canvas, size);
    }

    private static void DrawColorPickerSquare(SKCanvas canvas, int size)
    {
        var bitmap = new SKBitmap(size, size);
        using var surface = new SKCanvas(bitmap);
        for (var x = 0; x < size; x++)
        {
            for (var y = 0; y < size; y++)
            {
                var color = GetColorFromPosition(x, y, size);
                var paint = new SKPaint { Color = color };
                surface.DrawPoint(x, y, paint);
            }
        }

        canvas.DrawBitmap(bitmap, 0, 0);
    }

    private static SKColor GetColorFromPosition(int x, int y, int size)
    {
        var hue = (x / (float)size) * 360f;
        var saturation = 1f - (y / (float)size);
        return SKColor.FromHsv(hue, saturation * 100, 100);
    }

    private void OnCanvasViewTouch(object? sender, SKTouchEventArgs e)
    {
        if (e.ActionType == SKTouchAction.Pressed)
        {
            HandleTouchEvent(e, ColorPicked);
        }
        else if (e.ActionType == SKTouchAction.Moved)
        {
            HandleTouchEvent(e, ColorHover);
        }
    }

    private void HandleTouchEvent(SKTouchEventArgs e, EventHandler<ColorPickedEventArgs>? ev)
    {
        var size = Math.Min((int)CanvasSize.Width, (int)CanvasSize.Height);
        if (e.Location.X >= 0 && e.Location.X < size && e.Location.Y >= 0 && e.Location.Y < size)
        {
            var skColor = GetColorFromPosition((int)e.Location.X, (int)e.Location.Y, size);
            ev?.Invoke(this, new ColorPickedEventArgs(skColor, skColor.ToMauiColor()));
        }
        e.Handled = true;
    }
}
