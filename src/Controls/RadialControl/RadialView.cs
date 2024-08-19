using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using System.Collections.ObjectModel;

namespace SubtitleAlchemist.Controls.RadialControl;

public partial class RadialView : ContentView
{
    private readonly SKCanvasView _canvasView;
    private readonly ObservableCollection<RadialElement> _elements;
    private float _rotationAngle = 0f;
    private float _targetRotationAngle = 0f;
    private const float AnimationDuration = 0.3f; // seconds
    private readonly Random _random = new();
    public RadialElement? CenterImage { get; set; }

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
            var random = _random.Next(0, 10);
            _targetRotationAngle += (360f / _elements.Count) + random;
            AnimateRotation();
        }
    }

    public void RotateRight()
    {
        if (_elements.Count > 0)
        {
            var random = _random.Next(0, 10);
            _targetRotationAngle -= 360f / _elements.Count + random;
            AnimateRotation();
        }
    }

    private void AnimateRotation()
    {
        Dispatcher.StartTimer(TimeSpan.FromMilliseconds(16), () =>
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

    public static SKBitmap Rotate(SKBitmap bitmap, double angle)
    {
        var radians = Math.PI * angle / 180;
        var sine = (float)Math.Abs(Math.Sin(radians));
        var cosine = (float)Math.Abs(Math.Cos(radians));
        var originalWidth = bitmap.Width;
        var originalHeight = bitmap.Height;
        var rotatedWidth = (int)Math.Round((cosine * originalWidth + sine * originalHeight), MidpointRounding.AwayFromZero);
        var rotatedHeight = (int)Math.Round((cosine * originalHeight + sine * originalWidth), MidpointRounding.AwayFromZero);

        var rotatedBitmap = new SKBitmap(rotatedWidth, rotatedHeight);

        using var surface = new SKCanvas(rotatedBitmap);
        surface.Clear();
        surface.Translate(rotatedWidth / 2.0f, rotatedHeight / 2.0f);
        surface.RotateDegrees((float)angle);
        surface.Translate(-originalWidth / 2.0f, -originalHeight / 2.0f);
        surface.DrawBitmap(bitmap, new SKPoint());
        return rotatedBitmap;
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.Transparent);

        var centerX = e.Info.Width / 2f;
        var centerY = e.Info.Height / 2f;
        var radius = Math.Min(centerX, centerY) * 0.8f;

        using (var paint = new SKPaint())
        {
            paint.IsAntialias = true;
            paint.Color = SKColor.Parse("#ff555555");
            paint.FilterQuality = SKFilterQuality.High;
            canvas.DrawCircle(centerX, centerY, Math.Min(centerX, centerY), paint);
        }

        using (var paint = new SKPaint())
        {
            paint.IsAntialias = true;
            paint.Color = SKColor.Parse("#ff222222");
            paint.FilterQuality = SKFilterQuality.High;
            canvas.DrawCircle(centerX, centerY, Math.Min(centerX, centerY) / 3.0f, paint);
        }

        if (CenterImage != null && !string.IsNullOrEmpty(CenterImage.ImageUrl))
        {
            using var bitmap = SKBitmap.Decode(CenterImage.ImageUrl); //TODO: cache image
            if (bitmap != null)
            {
                var angle =  _rotationAngle * -1;
                using var rotated = Rotate(bitmap, angle);
                using var paint = new SKPaint();
                paint.FilterQuality = SKFilterQuality.High;
                paint.IsAntialias = true;
                var left = centerX - rotated.Width / 2.0f;
                var top = centerY - rotated.Height / 2.0f;
                var destRect = new SKRect(left, top, left + rotated.Width, top + rotated.Height);
                canvas.DrawBitmap(rotated, destRect, paint);
            }
        }

        for (var i = 0; i < _elements.Count; i++)
        {
            var angle = (i * 360f / _elements.Count - _rotationAngle) * (float)Math.PI / 180;
            var x = centerX + radius * (float)Math.Cos(angle);
            var y = centerY + radius * (float)Math.Sin(angle);

            // draw circle around each image
            //using (var paint = new SKPaint())
            //{
            //    paint.Color = SKColors.DarkOrange;
            //    canvas.DrawCircle(x, y, 30, paint);
            //}

            // Load and draw the image
            if (!string.IsNullOrEmpty(_elements[i].ImageUrl)) //TODO: cache images
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