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
     float shadowWidth,
     float cornerRadius = 1.0f// Parameter for rounded corners
 )
    {
        outlineWidth *= 1.7f; // factor to match ASSA

        using var typeface = SKTypeface.FromFamilyName(fontName, isBold ? SKFontStyleWeight.Bold : SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
        using var paint = new SKPaint
        {
            IsAntialias = true
        };

        using var font = new SKFont(typeface, fontSize);
        font.MeasureText(text, out var textBounds, paint);

        var padding = 20;
        var width = (int)(textBounds.Width + padding * 2 + outlineWidth * 2 + shadowWidth);
        var height = (int)(textBounds.Height + padding * 2 + outlineWidth * 2 + shadowWidth);

        var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(backgroundColor);

        var x = padding + outlineWidth;
        var y = height - padding - outlineWidth;

        // Create a path for the text
        using var textPath = font.GetTextPath(text, new SKPoint(x, y));

        // Draw shadow (includes both text and outline with rounded corners)
        if (shadowWidth > 0)
        {
            DrawTextWithRoundedOutline(x + shadowWidth, y + shadowWidth, shadowColor, shadowColor, textPath, x, y, cornerRadius, outlineWidth, paint, canvas);
        }

        // Draw main text with rounded outline
        DrawTextWithRoundedOutline(x, y, textColor, outlineColor, textPath, x, y, cornerRadius, outlineWidth, paint, canvas);

        return bitmap;
    }

    private static void DrawTextWithRoundedOutline(float xPos, float yPos, SKColor fillColor, SKColor strokeColor, SKPath textPath, float x, float y, float cornerRadius, float outlineWidth, SKPaint paint, SKCanvas canvas)
    {
        // Translate the path to the current position
        using var translatedPath = new SKPath();
        textPath.Transform(SKMatrix.CreateTranslation(xPos - x, yPos - y), translatedPath);

        // Set the path effect for rounded corners
        using var pathEffect = SKPathEffect.CreateCorner(cornerRadius);

        // Draw the outline
        if (outlineWidth > 0)
        {
            paint.Color = strokeColor;
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = outlineWidth;
            paint.PathEffect = pathEffect; // Apply rounded corners to the paint
            canvas.DrawPath(translatedPath, paint);
        }

        // Draw the fill
        paint.Color = fillColor;
        paint.Style = SKPaintStyle.Fill;
        paint.PathEffect = null; // Remove path effect for the fill
        canvas.DrawPath(translatedPath, paint);
    }


    public static SKBitmap AddShadowToBitmap(SKBitmap originalBitmap, int shadowWidth, SKColor shadowColor)
    {
        var offset = 2;

        // Calculate new dimensions
        var newWidth = originalBitmap.Width + shadowWidth + offset;
        var newHeight = originalBitmap.Height + shadowWidth + offset;

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
            canvas.DrawRect(0 + offset, originalBitmap.Height, newWidth, shadowWidth, paint);

            // Draw right shadow
            canvas.DrawRect(originalBitmap.Width, 0 + offset, shadowWidth + offset, originalBitmap.Height, paint);
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
