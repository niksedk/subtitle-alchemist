using SkiaSharp;
using System.Collections.ObjectModel;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace SubtitleAlchemist.Controls.RadialControl;

public class RadialView : ContentView
{
    private SKCanvasView canvasView;
    private ObservableCollection<RadialElement> elements;
    private float currentAngle = 0f;
    private float rotationOffset = 0f;
    private const float AnimationDuration = 0.5f; // seconds
    private const float RotationAnimationDuration = 0.3f; // seconds

    public RadialView()
    {
        elements = new ObservableCollection<RadialElement>();
        canvasView = new SKCanvasView();
        canvasView.PaintSurface += OnPaintSurface;
        Content = canvasView;

        // Start the animation loop
        Device.StartTimer(TimeSpan.FromMilliseconds(16), () =>
        {
            canvasView.InvalidateSurface();
            return true;
        });
    }

    public void AddElement(string imageUrl)
    {
        elements.Add(new RadialElement { ImageUrl = imageUrl });
        UpdateLayout();
    }

    public void RotateLeft()
    {
        if (elements.Count > 0)
        {
            rotationOffset += 360f / elements.Count;
            AnimateRotation();
        }
    }

    public void RotateRight()
    {
        if (elements.Count > 0)
        {
            rotationOffset -= 360f / elements.Count;
            AnimateRotation();
        }
    }

    private void AnimateRotation()
    {
        Device.StartTimer(TimeSpan.FromMilliseconds(16), () =>
        {
            rotationOffset = AnimateTowards(rotationOffset, 0, RotationAnimationDuration);
            canvasView.InvalidateSurface();
            return Math.Abs(rotationOffset) > 0.1f;
        });
    }

    private void UpdateLayout()
    {
        float targetAngle = 360f / elements.Count;
        currentAngle = AnimateTowards(currentAngle, targetAngle, AnimationDuration);
    }

    private float AnimateTowards(float current, float target, float duration)
    {
        float diff = target - current;
        return current + (diff * (1f / 60f) / duration);
    }

    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        SKCanvas canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.Transparent);

        float centerX = e.Info.Width / 2f;
        float centerY = e.Info.Height / 2f;
        float radius = Math.Min(centerX, centerY) * 0.8f;

        for (int i = 0; i < elements.Count; i++)
        {
            float angle = (i * currentAngle + rotationOffset) * (float)Math.PI / 180;
            float x = centerX + radius * (float)Math.Cos(angle);
            float y = centerY + radius * (float)Math.Sin(angle);

            using (SKPaint paint = new SKPaint())
            {
                paint.Color = SKColors.Blue;
                canvas.DrawCircle(x, y, 30, paint);
            }

            // Load and draw the image
            if (!string.IsNullOrEmpty(elements[i].ImageUrl))
            {
                using (SKBitmap bitmap = SKBitmap.Decode(elements[i].ImageUrl))
                using (SKPaint paint = new SKPaint())
                {
                    paint.FilterQuality = SKFilterQuality.High;
                    SKRect destRect = new SKRect(x - 25, y - 25, x + 25, y + 25);
                    canvas.DrawBitmap(bitmap, destRect, paint);
                }
            }
        }
    }
}

public class RadialElement
{
    public string ImageUrl { get; set; }
}