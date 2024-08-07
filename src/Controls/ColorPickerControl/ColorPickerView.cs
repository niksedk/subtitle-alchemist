using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace SubtitleAlchemist.Controls.ColorPickerControl;

public class ColorPickerView : SKCanvasView
{
    private SKColor _selectedColor = SKColors.White;

    public ColorPickerView()
    {
        EnableTouchEvents = true;
        Touch += OnCanvasViewTouch;
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        base.OnPaintSurface(e);

        var canvas = e.Surface.Canvas;
        canvas.Clear();

        int width = e.Info.Width;
        int height = e.Info.Height;
        int size = Math.Min(width, height);

        DrawColorPickerSquare(canvas, size);
    }

    private void DrawColorPickerSquare(SKCanvas canvas, int size)
    {
        var bitmap = new SKBitmap(size, size);
        using (var surface = new SKCanvas(bitmap))
        {
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    var color = GetColorFromPosition(x, y, size);
                    var paint = new SKPaint { Color = color };
                    surface.DrawPoint(x, y, paint);
                }
            }
        }

        canvas.DrawBitmap(bitmap, 0, 0);
    }

    private SKColor GetColorFromPosition(int x, int y, int size)
    {
        float hue = (x / (float)size) * 360f;
        float saturation = 1f - (y / (float)size);
        return SKColor.FromHsv(hue, saturation * 100, 100);
    }

    private void OnCanvasViewTouch(object? sender, SKTouchEventArgs e)
    {
        if (e.ActionType == SKTouchAction.Pressed || e.ActionType == SKTouchAction.Moved)
        {
            int size = Math.Min((int)CanvasSize.Width, (int)CanvasSize.Height);
            if (e.Location.X >= 0 && e.Location.X < size && e.Location.Y >= 0 && e.Location.Y < size)
            {
                _selectedColor = GetColorFromPosition((int)e.Location.X, (int)e.Location.Y, size);
                // Handle the selected color (e.g., update UI, notify user, etc.)
            }
            e.Handled = true;
        }
    }
}
