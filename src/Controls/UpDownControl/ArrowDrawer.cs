using SkiaSharp;

namespace SubtitleAlchemist.Controls.UpDownControl;

public static class ArrowDrawer
{
    public static void DrawArrowUp(SKCanvas canvas, SKPaint paint, float left, float top, float height)
    {
        using var path = new SKPath();
        path.MoveTo(left + 5, top + 2);
        path.LineTo(left + 1, top + height);
        path.LineTo(left + 9, top + height);
        path.Close();

        canvas.DrawPath(path, paint);
    }

    public static void DrawArrowDown(SKCanvas canvas, SKPaint paint, float left, float top, float height)
    {
        using var path = new SKPath();
        path.MoveTo(left + 1, top);
        path.LineTo(left + 9, top);
        path.LineTo(left + 5, top + height - 2);
        path.Close();

        canvas.DrawPath(path, paint);
    }
}