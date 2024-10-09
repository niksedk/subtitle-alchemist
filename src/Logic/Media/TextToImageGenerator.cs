using SkiaSharp;

namespace SubtitleAlchemist.Logic.Media;

public static class TextToImageGenerator
{
    public static SKBitmap GenerateImage(
        string text,
        string fontName,
        float fontSize,
        bool isBold,
        SKColor textColor,
        SKColor outlineColor,
        SKColor shadowColor,
        SKColor backgroundColor,
        float outlineWidth,
        float shadowWidth)
    {
        using var typeface = SKTypeface.FromFamilyName(fontName, isBold ? SKFontStyleWeight.Bold : SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
        using var paint = new SKPaint();
        paint.Typeface = typeface;
        paint.TextSize = fontSize;
        paint.IsAntialias = true;

        var textBounds = new SKRect();
        paint.MeasureText(text, ref textBounds);

        var padding = 20;
        var width = (int)(textBounds.Width + padding * 2 + outlineWidth * 2 + shadowWidth);
        var height = (int)(textBounds.Height + padding * 2 + outlineWidth * 2 + shadowWidth);

        var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(backgroundColor);

        var x = padding + outlineWidth;
        var y = height - padding - outlineWidth;

        // Function to draw text with outline
        Action<float, float, SKColor, SKColor> drawTextWithOutline = (xPos, yPos, fillColor, strokeColor) =>
        {
            // Draw outline
            if (outlineWidth > 0)
            {
                paint.Color = strokeColor;
                paint.Style = SKPaintStyle.Stroke;
                paint.StrokeWidth = outlineWidth;
                canvas.DrawText(text, xPos, yPos, paint);
            }

            // Draw fill
            paint.Color = fillColor;
            paint.Style = SKPaintStyle.Fill;
            canvas.DrawText(text, xPos, yPos, paint);
        };

        // Draw shadow (includes both text and outline)
        if (shadowWidth > 0)
        {
            drawTextWithOutline(x + shadowWidth, y + shadowWidth, shadowColor, shadowColor);
        }

        // Draw main text with outline
        drawTextWithOutline(x, y, textColor, outlineColor);

        return bitmap;
    }

    public static SKBitmap AddShadowToBitmap(SKBitmap originalBitmap, int shadowWidth, SKColor shadowColor)
    {
        // Calculate new dimensions
        var newWidth = originalBitmap.Width + shadowWidth;
        var newHeight = originalBitmap.Height + shadowWidth;

        // Create a new bitmap with increased size
        using var surface = SKSurface.Create(new SKImageInfo(newWidth, newHeight));
        var canvas = surface.Canvas;

        // Clear the canvas with transparent color
        canvas.Clear(SKColors.Transparent);

        // Draw the shadow
        using (var paint = new SKPaint
               {
                   Color = shadowColor,
                   Style = SKPaintStyle.Fill
               })
        {
            // Draw bottom shadow
            canvas.DrawRect(0, originalBitmap.Height, newWidth, shadowWidth, paint);

            // Draw right shadow
            canvas.DrawRect(originalBitmap.Width, 0, shadowWidth, originalBitmap.Height, paint);
        }

        // Draw the original bitmap
        canvas.DrawBitmap(originalBitmap, 0, 0);

        // Create a new bitmap from the surface
        using (var image = surface.Snapshot())
        {
            return SKBitmap.FromImage(image);
        }
    }
}
