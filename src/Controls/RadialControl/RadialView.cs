using SkiaSharp;
using System.Collections.ObjectModel;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace SubtitleAlchemist.Controls.RadialControl;

public class RadialView : ContentView
{
    private readonly SKCanvasView _canvasView;
    private readonly ObservableCollection<RadialElement> _elements;
    private float _rotationAngle = 0f;
    private float _targetRotationAngle = 0f;
    private const float AnimationDuration = 0.3f; // seconds

    public RadialView()
    {
        _elements = new ObservableCollection<RadialElement>();
        _canvasView = new SKCanvasView();
        _canvasView.PaintSurface += OnPaintSurface;
        Content = _canvasView;
    }

    public void AddElement(string imageUrl)
    {
        _elements.Add(new RadialElement { ImageUrl = imageUrl });
        _canvasView.InvalidateSurface();
    }

    public void RotateLeft()
    {
        if (_elements.Count > 0)
        {
            _targetRotationAngle += 360f / _elements.Count;
            AnimateRotation();
        }
    }

    public void RotateRight()
    {
        if (_elements.Count > 0)
        {
            _targetRotationAngle -= 360f / _elements.Count;
            AnimateRotation();
        }
    }

    private void AnimateRotation()
    {
        Device.StartTimer(TimeSpan.FromMilliseconds(16), () =>
        {
            _rotationAngle = AnimateTowards(_rotationAngle, _targetRotationAngle, AnimationDuration);
            _canvasView.InvalidateSurface();
            return Math.Abs(_rotationAngle - _targetRotationAngle) > 0.1f;
        });
    }

    private static float AnimateTowards(float current, float target, float duration)
    {
        var diff = target - current;
        return current + (diff * (1f / 60f) / duration);
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.Transparent);

        var centerX = e.Info.Width / 2f;
        var centerY = e.Info.Height / 2f;
        var radius = Math.Min(centerX, centerY) * 0.8f;

        for (var i = 0; i < _elements.Count; i++)
        {
            var angle = (i * 360f / _elements.Count - _rotationAngle) * (float)Math.PI / 180;
            var x = centerX + radius * (float)Math.Cos(angle);
            var y = centerY + radius * (float)Math.Sin(angle);

            using (var paint = new SKPaint())
            {
                paint.Color = SKColors.Blue;
                canvas.DrawCircle(x, y, 30, paint);
            }

            // Load and draw the image
            if (!string.IsNullOrEmpty(_elements[i].ImageUrl))
            {
                using var bitmap = SKBitmap.Decode(_elements[i].ImageUrl);
                using var paint = new SKPaint();
                paint.FilterQuality = SKFilterQuality.High;
                var destRect = new SKRect(x - 25, y - 25, x + 25, y + 25);
                canvas.DrawBitmap(bitmap, destRect, paint);
            }
        }
    }
}