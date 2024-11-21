using Microsoft.Maui.Controls;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using System.Collections.ObjectModel;
using SkiaSharp.Views.Maui.Controls;

namespace SubtitleAlchemist.Controls.DrawingCanvasControl;

public class DrawingCanvasView : SKCanvasView
{
    // Collections to store paths
    private readonly ObservableCollection<SKPath> _paths;
    private SKPath _currentPath;
    private bool _isDrawing = false;

    // Paint configuration - can be customized
    private SKPaint _drawingPaint = new SKPaint
    {
        Style = SKPaintStyle.Stroke,
        Color = SKColors.Blue,
        StrokeWidth = 3,
        IsAntialias = true
    };

    public DrawingCanvasView()
    {
        // Constructor setup
        Touch += OnCanvasTouch;

        PaintSurface += OnPaintSurface;

        _paths = new ObservableCollection<SKPath>();
        _currentPath = new SKPath();
    }

    private void OnCanvasTouch(object sender, SKTouchEventArgs e)
    {
        switch (e.ActionType)
        {
            case SKTouchAction.Pressed:
                // Start a new path
                _currentPath = new SKPath();
                _currentPath.MoveTo(e.Location);
                _isDrawing = true;
                break;

            case SKTouchAction.Moved:
                if (_isDrawing)
                {
                    // Continue drawing the line
                    _currentPath.LineTo(e.Location);
                    InvalidateSurface();
                }
                break;

            case SKTouchAction.Released:
                if (_isDrawing)
                {
                    // Finish the current path
                    _paths.Add(_currentPath);
                    _isDrawing = false;
                    InvalidateSurface();
                }
                break;
        }

        e.Handled = true;
    }

    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.White);


        // Draw all previously drawn paths
        foreach (var path in _paths)
        {
            canvas.DrawPath(path, _drawingPaint);
        }

        // Draw the current path if drawing
        if (_isDrawing && _currentPath != null)
        {
            canvas.DrawPath(_currentPath, _drawingPaint);
        }
    }

    // Optional: Method to clear all drawings
    public void Clear()
    {
        _paths.Clear();
        _currentPath = null;
        _isDrawing = false;
        InvalidateSurface();
    }

    // Optional: Expose paint customization
    public void SetPaintColor(SKColor color)
    {
        _drawingPaint.Color = color;
    }

    public void SetStrokeWidth(float width)
    {
        _drawingPaint.StrokeWidth = width;
    }
}

// Example usage in a page
//public partial class MainPage : ContentPage
//{
//    public MainPage()
//    {
//        InitializeComponent();

//        var drawingCanvas = new DrawingCanvasView
//        {
//            VerticalOptions = LayoutOptions.FillAndExpand,
//            HorizontalOptions = LayoutOptions.FillAndExpand
//        };

//        // Optional customizations
//        drawingCanvas.SetPaintColor(SKColors.Red);
//        drawingCanvas.SetStrokeWidth(5);

//        Content = new VerticalStackLayout
//        {
//            Children =
//            {
//                drawingCanvas,
//                new Button
//                {
//                    Text = "Clear Drawing",
//                    Command = new Command(() => drawingCanvas.Clear())
//                }
//            }
//        };
//    }
//}