using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using SubtitleAlchemist.Logic.Ocr;

namespace SubtitleAlchemist.Controls.DrawingCanvasControl;

public class NOcrDrawingCanvasView : SKCanvasView
{
    public List<NOcrLine> HitPaths { get; set; }
    public List<NOcrLine> MissPaths{ get; set; }

    public float ZoomFactor
    {
        get => _zoomFactor;
        set
        {
            _zoomFactor = value;
            WidthRequest = BackgroundImage.Width * _zoomFactor;
            HeightRequest = BackgroundImage.Height * _zoomFactor;
            InvalidateSurface();
        }
    }

    private NOcrLine _currentPath;
    private bool _isDrawing = false;
    private int _mouseMoveStartX = -1;
    private int _mouseMoveStartY = -1;

    private readonly SKPaint _drawingPaint = new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 3,
        IsAntialias = true
    };

    private float _zoomFactor = 1.0f;

    public bool NewLinesAreHits { get; set; } = true;

    public SKColor CanvasColor { get; set; } = SKColors.DarkGray;
    public SKColor HitColor { get; set; } = SKColors.Green;
    public SKColor MissColor { get; set; } = SKColors.Red;
    public SKBitmap BackgroundImage { get; set; }

    public NOcrDrawingCanvasView()
    {
        PaintSurface += OnPaintSurface;

        HitPaths = new List<NOcrLine>();
        MissPaths = new List<NOcrLine>();
        _currentPath = new NOcrLine();
        BackgroundImage = new SKBitmap(1, 1);
        ZoomFactor = 1;

        var pointerGestureRecognizer = new PointerGestureRecognizer();
        pointerGestureRecognizer.PointerMoved += PointerMoved;
        pointerGestureRecognizer.PointerPressed += PointerPressed;
        pointerGestureRecognizer.PointerReleased += PointerReleased;
        GestureRecognizers.Add(pointerGestureRecognizer);
    }

    private void PointerReleased(object? sender, PointerEventArgs e)
    {
        _isDrawing = false;
        if (!_currentPath.IsEmpty) 
        {
            if (NewLinesAreHits)
            {
                HitPaths.Add(_currentPath);
            }
            else
            {
                MissPaths.Add(_currentPath);
            }
            _currentPath = new NOcrLine();
        }
    }

    private void PointerPressed(object? sender, PointerEventArgs e)
    {
        _isDrawing = true;
        var pos = e.GetPosition(this);
        if (pos.HasValue)
        {
            _mouseMoveStartX = (int)Math.Round(pos.Value.X / ZoomFactor, MidpointRounding.AwayFromZero);
            _mouseMoveStartY = (int)Math.Round(pos.Value.Y / ZoomFactor, MidpointRounding.AwayFromZero);
        }
    }

    private void PointerMoved(object? sender, PointerEventArgs e)
    {
        var pos = e.GetPosition(this);
        if (pos.HasValue)
        {
            var x = (int)Math.Round(pos.Value.X / ZoomFactor, MidpointRounding.AwayFromZero);
            var y = (int)Math.Round(pos.Value.Y / ZoomFactor, MidpointRounding.AwayFromZero);
            if (_isDrawing)
            {
                _currentPath = new NOcrLine(new OcrPoint(_mouseMoveStartX, _mouseMoveStartY), new OcrPoint(x, y));
                InvalidateSurface();
            }
        }
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(CanvasColor);
        canvas.DrawBitmap(BackgroundImage, new SKRect(0, 0, BackgroundImage.Width * ZoomFactor, BackgroundImage.Height * ZoomFactor));

        _drawingPaint.Color = MissColor;
        foreach (var path in MissPaths)
        {
            DrawNOcrLine(path, canvas);
        }

        _drawingPaint.Color = HitColor;
        foreach (var path in HitPaths)
        {
            DrawNOcrLine(path, canvas);
        }

        // Draw the current path if drawing
        _drawingPaint.Color = NewLinesAreHits ? HitColor : MissColor;
        if (_isDrawing && !_currentPath.IsEmpty)
        {
            DrawNOcrLine(_currentPath, canvas);
        }
    }

    private void DrawNOcrLine(NOcrLine path, SKCanvas canvas)
    {
        var skPath = new SKPath();
        skPath.MoveTo(path.Start.X * ZoomFactor, path.Start.Y * ZoomFactor);
        skPath.LineTo(path.End.X * ZoomFactor, path.End.Y * ZoomFactor);
        canvas.DrawPath(skPath, _drawingPaint);
    }

    public void SetStrokeWidth(float width)
    {
        _drawingPaint.StrokeWidth = width;
    }
}
