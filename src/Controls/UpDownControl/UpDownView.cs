using System.Timers;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace SubtitleAlchemist.Controls.UpDownControl;

public class UpDownView : SKCanvasView
{
    public event EventHandler<ValueChangedEventArgs>? ValueChanged;

    public double StepValue { get; set; } = 100.0;
    public double StepValueFast { get; set; } = 1_000.0;
    public new bool IsFocused => _focused;

    public float MinValue { set; get; } = float.MinValue;
    public float MaxValue { set; get; } = float.MaxValue;

    private const int ButtonsWidth = 13;

    private bool _focused;
    private bool _focusedUpArrow;
    private bool _focusedDownArrow;

    private readonly System.Timers.Timer _timer;
    private long _timerCount;

    private readonly SKPaint _arrowUpPaint = new() { Color = SKColors.Black };
    private readonly SKPaint _arrowDownPaint = new() { Color = SKColors.Black };
    private readonly SKPaint _arrowUpPaintOver = new() { Color = SKColors.CadetBlue };
    private readonly SKPaint _arrowDownPaintOver = new() { Color = SKColors.CadetBlue };

    /// <summary>
    ///     Initializes a new instance of the <see cref="UpDownView"/> class.
    /// </summary>
    public UpDownView()
    {
        WidthRequest = 25;
        HeightRequest = 25;

        var pointerGestureRecognizer = new PointerGestureRecognizer();
        pointerGestureRecognizer.PointerMoved += PointerMoved;
        pointerGestureRecognizer.PointerPressed += PointerPressed;
        pointerGestureRecognizer.PointerReleased += PointerReleased;
        pointerGestureRecognizer.PointerEntered += PointerEntered;
        pointerGestureRecognizer.PointerExited += PointerExited;
        GestureRecognizers.Add(pointerGestureRecognizer);

        _timer = new System.Timers.Timer(500);
        _timer.Elapsed += TimerElapsed;
    }

    private void TimerElapsed(object? sender, ElapsedEventArgs e)
    {
        _timerCount++;
        if (_timerCount > 20) // increase speed after 20 ticks
        {
            _timer.Interval = 1; // faster speed
            ButtonsPressed(StepValueFast);
        }
        else // start repeating after first tick
        {
            _timer.Interval = 75; // start repeat speed
            ButtonsPressed(StepValue);
        }
    }

    private void PointerMoved(object? sender, PointerEventArgs e)
    {
        var point = e.GetPosition(this);
        if (point == null)
        {
            return;
        }

        var y = (int)Math.Round(point.Value.Y, MidpointRounding.AwayFromZero);

        if (_focused)
        {
            if (y < Height / 2)
            {
                if (!_focusedUpArrow)
                {
                    _focusedUpArrow = true;
                    _focusedDownArrow = false;
                    InvalidateSurface();
                }
            }
            else
            {
                if (!_focusedDownArrow)
                {
                    _focusedUpArrow = false;
                    _focusedDownArrow = true;
                    InvalidateSurface();
                }
            }
        }
        else
        {
            _focusedUpArrow = false;
            _focusedDownArrow = false;
        }
    }

    private void PointerPressed(object? sender, PointerEventArgs e)
    {
        ButtonsPressed(StepValue);
        _timer.Start();
    }

    private void ButtonsPressed(double stepValue)
    {
        if (_focusedUpArrow)
        {
            Value += (float)stepValue;
            ValueChanged?.Invoke(this, new ValueChangedEventArgs(Value - stepValue, Value));
        }
        else if (_focusedDownArrow)
        {
            Value -= (float)stepValue;
            ValueChanged?.Invoke(this, new ValueChangedEventArgs(Value + stepValue, Value));
        }
    }

    private void PointerReleased(object? sender, PointerEventArgs e)
    {
        ResetTimer();
    }

    private void PointerEntered(object? sender, PointerEventArgs e)
    {
        _focused = true;
    }

    private void PointerExited(object? sender, PointerEventArgs e)
    {
        ResetTimer();

        _focused = false;
        _focusedUpArrow = false;
        _focusedDownArrow = false;
        InvalidateSurface();
    }

    private void ResetTimer()
    {
        _timer.Stop();
        _timer.Interval = 500;
        _timerCount = 0;
    }

    /// <summary>
    ///     Identifies the Value bindable property.
    /// </summary>
    public static readonly BindableProperty ValueProperty
        = BindableProperty.Create(
            nameof(Value),
            typeof(float),
            typeof(UpDownView),
            0.0f);

    /// <summary>
    /// Gets or sets the value displayed by the gauge.
    /// </summary>
    public float Value
    {
        get => (float)GetValue(ValueProperty);
        set => SetValue(ValueProperty, Math.Clamp(value, MinValue, MaxValue));
    }

    /// <summary>
    /// Identifies the TextColor bindable property.
    /// </summary>
    public static readonly BindableProperty TextColorProperty
        = BindableProperty.Create(
            nameof(TextColor),
            typeof(Color),
            typeof(UpDownView),
            Colors.Black);

    /// <summary>
    /// Gets or sets the color of the up/down arrows.
    /// </summary>
    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set
        {
            SetValue(TextColorProperty, value);
            _arrowUpPaint.Color = value.ToSKColor();
            _arrowDownPaint.Color = value.ToSKColor();
        }
    }

    private const float OriginalWidth = 25; // Set this to your original design width
    private const float OriginalHeight = 25; // Set this to your original design height

    /// <summary>
    ///     Called when the surface needs to be painted.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        base.OnPaintSurface(e);

        var canvas = e.Surface.Canvas;
        canvas.Clear();


        // Calculate scale factors
        float scaleX = e.Info.Width / OriginalWidth;
        float scaleY = e.Info.Height / OriginalHeight;
        float scale = Math.Min(scaleX, scaleY);

        // Calculate translation to center the content
        float translateX = (e.Info.Width - (OriginalWidth * scale)) / 2.0f;
        float translateY = (e.Info.Height - (OriginalHeight * scale)) / 2.0f;

        // Apply scaling and translation
        canvas.Save();
        canvas.Translate(translateX, translateY);
        canvas.Scale(scale);

        DrawUpDownArrows(canvas, OriginalWidth, OriginalHeight);
    }

    /// <summary>
    ///     Draws the up and down arrows.
    /// </summary>
    /// <param name="canvas">The canvas to draw on.</param>
    /// <param name="width">Width of UpDown area.</param>
    /// <param name="height">Height of UpDown area.</param>
    private void DrawUpDownArrows(SKCanvas canvas, float width, float height)
    {
        var left = width - ButtonsWidth;
        var h = (int)height / 2 - 4;
        const int top = 2;
        var paintUp = _focusedUpArrow ? _arrowUpPaintOver : _arrowUpPaint;
        ArrowDrawer.DrawArrowUp(canvas, paintUp, left, top, h);

        var downTop = h + 5;
        var paintDown = _focusedDownArrow ? _arrowDownPaintOver : _arrowDownPaint;
        ArrowDrawer.DrawArrowDown(canvas, paintDown, left, downTop, h);
    }
}
