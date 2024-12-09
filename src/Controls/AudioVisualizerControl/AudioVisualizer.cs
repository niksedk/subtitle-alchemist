using System.Runtime.CompilerServices;
using MauiCursor;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.Forms;
using SharpHook;
using SharpHook.Native;
using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;
using SubtitleAlchemist.Logic;
using SKPaintSurfaceEventArgs = SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs;

namespace SubtitleAlchemist.Controls.AudioVisualizerControl;

public class AudioVisualizer : SKCanvasView
{
    private readonly object _lock = new();

    // actual canvas instance to draw on
    private SKCanvas? _canvas;

    // holds information about the dimensions, etc.
    private SKImageInfo _info;

    public WavePeakData? WavePeaks { get; set; }
    public bool AllowOverlap { get; set; }

    public const double ZoomMinimum = 0.1;
    public const double ZoomMaximum = 2.5;
    private double _zoomFactor = 1.0; // 1.0=no zoom
    private long _lastMouseWheelScroll = -1;
    private const int MinimumSelectionMilliseconds = 100;
    public int ClosenessForBorderSelection { get; set; } = 15;
    public int ShotChangeSnapPixels = 8;
    private readonly Subtitle _subtitle = new();
    private readonly List<Paragraph> _displayableParagraphs = new();
    private readonly List<Paragraph> _allSelectedParagraphs = new();
    public Paragraph? SelectedParagraph { get; private set; }
    public Paragraph? NewSelectionParagraph { get; set; }
    public Paragraph? RightClickedParagraph { get; private set; }
    public bool AllowMove { get; set; } = true;
    public bool AllowNewSelection { get; set; } = true;

    public KeyboardModifierKeys ModifierKeys { get; set; } = new();
    public MouseStatus MouseStatus { get; set; } = new();

    // Mouse down helpers
    private bool _firstMove = true;
    private int _mouseMoveLastX = -1;
    private int _mouseMoveStartX = -1;
    private double _moveWholeStartDifferenceMilliseconds = -1;
    private int _mouseMoveEndX = -1;
    private bool _mouseDown;
    private bool _mouseOver;
    private Paragraph? _prevParagraph;
    private Paragraph? _nextParagraph;
    private Paragraph? _oldParagraph;
    private Paragraph? _mouseDownParagraph;
    private Paragraph[]? _mouseDownParagraphs;
    private MouseDownParagraphType _mouseDownParagraphType = MouseDownParagraphType.Start;
    private double _wholeParagraphMinMilliseconds;
    private double _wholeParagraphMaxMilliseconds = double.MaxValue;
    private double _gapAtStart = -1;

    // Mouse down 
    private long _buttonDownTimeTicks;
    private bool _noClear;
    public double RightClickedSeconds { get; private set; }

    private readonly List<double> _shotChanges = new();

    public bool ShowGridLines { get; set; } = true;

    private double _currentVideoPositionSeconds = -1;

    public delegate void ParagraphEventHandler(object sender, ParagraphEventArgs e);

    public event PositionEventHandler? OnVideoPositionChanged;
    public event PositionEventHandler? OnDoubleTapped;
    public event ParagraphEventHandler? OnPositionSelected;
    public event ParagraphEventHandler? OnTimeChanged;
    public event ParagraphEventHandler? OnStartTimeChanged;
    public event ParagraphEventHandler? OnTimeChangedAndOffsetRest;
    public event ParagraphEventHandler? OnNewSelectionRightClicked;
    public event ParagraphEventHandler? OnNewSelectionInsert;
    public event ParagraphEventHandler? OnParagraphRightClicked;
    public event ParagraphEventHandler? OnNonParagraphRightClicked;
    public event ParagraphEventHandler? OnSingleClick;
    public event ParagraphEventHandler? OnStatus;

    public event EventHandler? OnZoomedChanged;
    public event EventHandler? OnPlayToggle;

    public class PositionEventArgs : EventArgs
    {
        public double PositionInSeconds { get; set; }
    }

    public delegate void PositionEventHandler(object sender, PositionEventArgs e);


    public double ZoomFactor
    {
        get => _zoomFactor;
        set
        {
            if (value < ZoomMinimum)
            {
                value = ZoomMinimum;
            }

            if (value > ZoomMaximum)
            {
                value = ZoomMaximum;
            }

            value = Math.Round(value, 2); // round to prevent accumulated rounding errors
            if (Math.Abs(_zoomFactor - value) > 0.01)
            {
                _zoomFactor = value;
            }
        }
    }

    public const double VerticalZoomMinimum = 1.0;
    public const double VerticalZoomMaximum = 20.0;
    private double _verticalZoomFactor = 1.0; // 1.0=no zoom

    public double VerticalZoomFactor
    {
        get => _verticalZoomFactor;
        set
        {
            if (value < VerticalZoomMinimum)
            {
                value = VerticalZoomMinimum;
            }

            if (value > VerticalZoomMaximum)
            {
                value = VerticalZoomMaximum;
            }

            value = Math.Round(value, 2); // round to prevent accumulated rounding errors
            if (Math.Abs(_verticalZoomFactor - value) > 0.01)
            {
                _verticalZoomFactor = value;
            }
        }
    }

    private double _startPositionSeconds;

    public double StartPositionSeconds
    {
        get => _startPositionSeconds;
        set
        {
            if (WavePeaks != null)
            {
                var endPositionSeconds = value + (double)_info.Width / WavePeaks.SampleRate / _zoomFactor;
                if (endPositionSeconds > WavePeaks.LengthInSeconds)
                {
                    value -= endPositionSeconds - WavePeaks.LengthInSeconds;
                }
            }
            if (value < 0)
            {
                value = 0;
            }

            if (Math.Abs(_startPositionSeconds - value) > 0.01)
            {
                _startPositionSeconds = value;
            }
        }
    }

    private readonly SKPaint _paintBackground = new()
    {
        Color = SKColor.Parse("#2effffff"),
        StrokeWidth = 1,
        IsAntialias = true,
        Style = SKPaintStyle.Fill,
    };

    // left edge
    private readonly SKPaint _paintLeft = new()
    {
        Color = SKColor.Parse("#ff99ff99"),
        StrokeWidth = 1,
        IsAntialias = true,
        Style = SKPaintStyle.Stroke,
    };

    private readonly SKPaint _paintRight = new()
    {
        Color = SKColor.Parse("#ffff0000"),
        StrokeWidth = 1,
        IsAntialias = true,
        Style = SKPaintStyle.Stroke,
    };

    private readonly SKPaint _paintText = new()
    {
        Color = SKColors.WhiteSmoke,
        StrokeWidth = 1,
        IsAntialias = true,
        Style = SKPaintStyle.Stroke,
        TextSize = 14,
    };

    private readonly SKFont _fontText = new()
    {
        Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright),
        Size = 14,
    };

    private readonly SKTextAlign _textAlignText = SKTextAlign.Left;
    
    private readonly SKPaint _paintGridColor = new()
    {
        Color = SKColor.Parse("#ff222222"),
        StrokeWidth = 1,
        IsAntialias = true,
        Style = SKPaintStyle.Stroke,
    };

    public AudioVisualizer()
    {
        var tapGestureRecognizerSingle = new TapGestureRecognizer();
        tapGestureRecognizerSingle.NumberOfTapsRequired = 1;
        tapGestureRecognizerSingle.Tapped += TappedSingle;
        GestureRecognizers.Add(tapGestureRecognizerSingle);

        var tapGestureRecognizerDouble = new TapGestureRecognizer();
        tapGestureRecognizerDouble.NumberOfTapsRequired = 2;
        tapGestureRecognizerDouble.Tapped += TappedDouble;
        GestureRecognizers.Add(tapGestureRecognizerDouble);

        var pointerGestureRecognizer = new PointerGestureRecognizer();
        pointerGestureRecognizer.PointerMoved += PointerMoved;
        pointerGestureRecognizer.PointerPressed += PointerPressed;
        pointerGestureRecognizer.PointerReleased += PointerReleased;
        pointerGestureRecognizer.PointerEntered += PointerEntered;
        pointerGestureRecognizer.PointerExited += PointerExited;
        GestureRecognizers.Add(pointerGestureRecognizer);

        SharpHookHandler.AddMousePressed(OnMousePressed);
        SharpHookHandler.AddMouseReleased(OnMouseReleased);
        SharpHookHandler.AddKeyPressed(OnKeyPressed);

        //TODO: test mpv player with _canvas.Handle
    }

    private void OnKeyPressed(object? sender, KeyboardHookEventArgs e)
    {
        if (!_mouseOver)
        {
            return;
        }

        if (e.Data.KeyCode == KeyCode.VcNumPadAdd) //TODO: modifiers?
        {
            ZoomIn();
            e.SuppressEvent = true;
        }
        else if (e.Data.KeyCode == KeyCode.VcNumPadSubtract)
        {
            ZoomOut();
            e.SuppressEvent = true;
        }
        else if (e.Data.KeyCode is KeyCode.VcNumPad0 or KeyCode.Vc0)
        {
            ZoomFactor = 1.0;
            OnZoomedChanged?.Invoke(this, EventArgs.Empty);
            e.SuppressEvent = true;
        }
        else if (e.Data.KeyCode == KeyCode.VcLeft)
        {
            StartPositionSeconds -= 0.1;
            e.SuppressEvent = true;
        }
        else if (e.Data.KeyCode == KeyCode.VcRight)
        {
            StartPositionSeconds += 0.1;
            e.SuppressEvent = true;
        }
        else if (e.Data.KeyCode == KeyCode.VcEnter)
        {
            if (NewSelectionParagraph != null)
            {
                OnNewSelectionInsert?.Invoke(this, new ParagraphEventArgs(NewSelectionParagraph));
                NewSelectionParagraph = null;
            }
        }
        else if (e.Data.KeyCode == KeyCode.VcSpace)
        {
            OnPlayToggle?.Invoke(this, EventArgs.Empty);
        }
    }

    public void ZoomIn()
    {
        ZoomFactor += 0.1;
        OnZoomedChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ZoomOut()
    {
        ZoomFactor -= 0.1;
        OnZoomedChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetContextMenu(MenuFlyout menuFlyout)
    {
        FlyoutBase.SetContextFlyout(this, menuFlyout);
    }

    private void PointerExited(object? sender, PointerEventArgs e)
    {
        _mouseOver = false;
    }

    private void PointerEntered(object? sender, PointerEventArgs e)
    {
        _mouseOver = true;
        MouseStatus.MouseButton1 = false;
        MouseStatus.MouseButton2 = false;

        if (WavePeaks == null)
        {
            return;
        }

        if (_noClear)
        {
            _noClear = false;
        }
        else
        {
            SetCursor(CursorIcon.Arrow);
            _mouseDown = false;
            _mouseDownParagraph = null;
            _mouseMoveStartX = -1;
            _mouseMoveEndX = -1;
        }

        if (NewSelectionParagraph != null)
        {
            _mouseMoveStartX = SecondsToXPosition(NewSelectionParagraph.StartTime.TotalSeconds - _startPositionSeconds);
            _mouseMoveEndX = SecondsToXPosition(NewSelectionParagraph.EndTime.TotalSeconds - _startPositionSeconds);
        }
    }

    private void OnMousePressed(object? sender, MouseHookEventArgs e)
    {
        UpdateMouseState("Pressed", e);
    }

    private void OnMouseReleased(object? sender, MouseHookEventArgs e)
    {
        UpdateMouseState("Released", e);
    }

    private void UpdateMouseState(string action, MouseHookEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            MouseStatus.MouseButton1 = e.Data.Button == MouseButton.Button1;
            MouseStatus.MouseButton2 = e.Data.Button == MouseButton.Button2;
        });
    }

    private void PointerPressed(object? sender, PointerEventArgs e)
    {
        _mouseDown = true;

        //for debug
        if (MouseStatus.MouseButtonNone)
        {
            MouseStatus.MouseButton1 = true;
        }

        if (WavePeaks == null)
        {
            return;
        }

        var point = e.GetPosition(this);
        if (point == null)
        {
            return;
        }

        var x = (int)Math.Round(point.Value.X, MidpointRounding.AwayFromZero);

        Paragraph? oldMouseDownParagraph = null;
        _mouseDownParagraphType = MouseDownParagraphType.None;
        _gapAtStart = -1;
        _firstMove = true;
        if (MouseStatus.MouseButton1) // left
        {
            _buttonDownTimeTicks = DateTime.UtcNow.Ticks;

            SetCursor(CursorIcon.ResizeLeftRight); // VSplit

            var seconds = RelativeXPositionToSeconds(x);
            var milliseconds = (int)(seconds * TimeCode.BaseUnit);

            if (SetParagraphBorderHit(milliseconds, NewSelectionParagraph))
            {
                if (_mouseDownParagraph != null)
                {
                    oldMouseDownParagraph = new Paragraph(_mouseDownParagraph, false);
                }

                if (_mouseDownParagraphType == MouseDownParagraphType.Start)
                {
                    if (_mouseDownParagraph != null)
                    {
                        _mouseDownParagraph.StartTime.TotalMilliseconds = milliseconds;
                        OnTimeChanged?.Invoke(this, new ParagraphEventArgs(seconds, _mouseDownParagraph, _oldParagraph, _mouseDownParagraphType, AllowMovePrevOrNext));
                    }

                    if (NewSelectionParagraph != null)
                    {
                        NewSelectionParagraph.StartTime.TotalMilliseconds = milliseconds;
                        _mouseMoveStartX = x;
                        _mouseMoveEndX = SecondsToXPosition(NewSelectionParagraph.EndTime.TotalSeconds - _startPositionSeconds);
                    }
                }
                else
                {
                    if (_mouseDownParagraph != null)
                    {
                        _mouseDownParagraph.EndTime.TotalMilliseconds = milliseconds;
                        OnTimeChanged?.Invoke(this, new ParagraphEventArgs(seconds, _mouseDownParagraph, _oldParagraph, _mouseDownParagraphType, AllowMovePrevOrNext));
                    }

                    if (NewSelectionParagraph != null)
                    {
                        NewSelectionParagraph.EndTime.TotalMilliseconds = milliseconds;
                        _mouseMoveStartX = SecondsToXPosition(NewSelectionParagraph.StartTime.TotalSeconds - _startPositionSeconds);
                    }

                    _mouseMoveEndX = x;
                }
                SetMinMaxViaSeconds(seconds);
            }
            else if (SetParagraphBorderHit(milliseconds, SelectedParagraph) || SetParagraphBorderHit(milliseconds, _displayableParagraphs))
            {
                NewSelectionParagraph = null;
                if (_mouseDownParagraph != null)
                {
                    oldMouseDownParagraph = new Paragraph(_mouseDownParagraph, false);
                    var curIdx = _subtitle.Paragraphs.IndexOf(_mouseDownParagraph);
                    if (_mouseDownParagraphType == MouseDownParagraphType.Start && !ModifierKeys.Alt)
                    {
                        if (curIdx > 0)
                        {
                            var prev = _subtitle.Paragraphs[curIdx - 1];
                            if (prev.EndTime.TotalMilliseconds + Configuration.Settings.General.MinimumMillisecondsBetweenLines < milliseconds)
                            {
                                _mouseDownParagraph.StartTime.TotalMilliseconds = milliseconds;
                                OnTimeChanged?.Invoke(this, new ParagraphEventArgs(seconds, _mouseDownParagraph, _oldParagraph, _mouseDownParagraphType, AllowMovePrevOrNext));
                            }
                        }
                        else
                        {
                            _mouseDownParagraph.StartTime.TotalMilliseconds = milliseconds;
                            OnTimeChanged?.Invoke(this, new ParagraphEventArgs(seconds, _mouseDownParagraph, _oldParagraph, _mouseDownParagraphType, AllowMovePrevOrNext));
                        }
                    }
                }
                SetMinAndMax();
            }
            else
            {
                var p = GetParagraphAtMilliseconds(milliseconds);
                if (p != null)
                {
                    _oldParagraph = new Paragraph(p, false);
                    _mouseDownParagraph = p;
                    oldMouseDownParagraph = new Paragraph(_mouseDownParagraph, false);
                    _mouseDownParagraphType = MouseDownParagraphType.Whole;
                    _moveWholeStartDifferenceMilliseconds = (RelativeXPositionToSeconds(x) * TimeCode.BaseUnit) - p.StartTime.TotalMilliseconds;
                    SetCursor(CursorIcon.Hand); // Hand
                    SetMinAndMax();
                }
                else if (!AllowNewSelection)
                {
                    SetCursor(CursorIcon.Arrow); // Default;
                }
                if (p == null)
                {
                    SetMinMaxViaSeconds(seconds);
                }

                NewSelectionParagraph = null;
                _mouseMoveStartX = x;
                _mouseMoveEndX = x;
            }

            if (_mouseDownParagraphType == MouseDownParagraphType.Start)
            {
                if (_subtitle != null && _mouseDownParagraph != null)
                {
                    var curIdx = _subtitle.Paragraphs.IndexOf(_mouseDownParagraph);
                    if (curIdx > 0 && oldMouseDownParagraph != null)
                    {
                        _gapAtStart = oldMouseDownParagraph.StartTime.TotalMilliseconds - _subtitle.Paragraphs[curIdx - 1].EndTime.TotalMilliseconds;
                    }
                }
            }
            else if (_mouseDownParagraphType == MouseDownParagraphType.End)
            {
                if (_subtitle != null && _mouseDownParagraph != null)
                {
                    var curIdx = _subtitle.Paragraphs.IndexOf(_mouseDownParagraph);
                    if (curIdx >= 0 && curIdx < _subtitle.Paragraphs.Count - 1 && oldMouseDownParagraph != null)
                    {
                        _gapAtStart = _subtitle.Paragraphs[curIdx + 1].StartTime.TotalMilliseconds - oldMouseDownParagraph.EndTime.TotalMilliseconds;
                    }
                }
            }
            _mouseDown = true;
        }
        else
        {
            if (MouseStatus.MouseButton2) // Right
            {
                var seconds = RelativeXPositionToSeconds(x);
                var milliseconds = (int)(seconds * TimeCode.BaseUnit);

                if (OnNewSelectionRightClicked != null && NewSelectionParagraph != null)
                {
                    OnNewSelectionRightClicked.Invoke(this, new ParagraphEventArgs(NewSelectionParagraph));
                    RightClickedParagraph = null;
                    _noClear = true;
                }
                else
                {
                    var p = GetParagraphAtMilliseconds(milliseconds);
                    RightClickedParagraph = p;
                    RightClickedSeconds = seconds;
                    if (p != null)
                    {
                        if (OnParagraphRightClicked != null)
                        {
                            NewSelectionParagraph = null;
                            OnParagraphRightClicked.Invoke(this, new ParagraphEventArgs(seconds, p));
                        }
                    }
                    else
                    {
                        OnNonParagraphRightClicked?.Invoke(this, new ParagraphEventArgs(seconds, null));
                    }
                }
            }

            SetCursor(CursorIcon.Arrow); // Default
        }
    }

    private void SetMinMaxViaSeconds(double seconds)
    {
        _wholeParagraphMinMilliseconds = 0;
        _wholeParagraphMaxMilliseconds = double.MaxValue;
        if (_subtitle != null)
        {
            Paragraph? prev = null;
            Paragraph? next = null;

            List<Paragraph> paragraphs;
            lock (_lock)
            {
                paragraphs = _subtitle.Paragraphs.ToList();
            }

            for (var i = 0; i < paragraphs.Count; i++)
            {
                var p2 = paragraphs[i];
                if (p2.StartTime.TotalSeconds < seconds)
                {
                    prev = p2;
                }
                else if (p2.EndTime.TotalSeconds > seconds)
                {
                    next = p2;
                    break;
                }
            }

            if (prev != null)
            {
                _wholeParagraphMinMilliseconds = prev.EndTime.TotalMilliseconds + Configuration.Settings.General.MinimumMillisecondsBetweenLines;
            }

            if (next != null)
            {
                _wholeParagraphMaxMilliseconds = next.StartTime.TotalMilliseconds - Configuration.Settings.General.MinimumMillisecondsBetweenLines;
            }
        }
    }

    private void SetMinAndMax()
    {
        try
        {
            _wholeParagraphMinMilliseconds = 0;
            _wholeParagraphMaxMilliseconds = double.MaxValue;
            if (_mouseDownParagraph != null)
            {
                List<Paragraph> paragraphs;
                lock (_lock)
                {
                    paragraphs = _subtitle.Paragraphs.ToList();
                }
                var p = paragraphs.FirstOrDefault(p => p.Id == _mouseDownParagraph.Id);

                if (p == null)
                {
                    OnStatus?.Invoke(this, new ParagraphEventArgs(new Paragraph() { Text = "SetMinAndMax p not found!!!" }));
                }

                if (p != null)
                {
                    var curIdx = paragraphs.IndexOf(p);

                    if (curIdx > 0)
                    {
                        _wholeParagraphMinMilliseconds = paragraphs[curIdx - 1].EndTime.TotalMilliseconds + Configuration.Settings.General.MinimumMillisecondsBetweenLines;
                    }

                    if (curIdx < paragraphs.Count - 1)
                    {
                        _wholeParagraphMaxMilliseconds = paragraphs[curIdx + 1].StartTime.TotalMilliseconds - Configuration.Settings.General.MinimumMillisecondsBetweenLines;
                    }
                }
            }
        }
        catch (Exception e)
        {
            //TODO: Fix multiple accessors to _subtitle.Paragraphs ???
            Console.WriteLine(e);
            //throw;
        }
    }

    private bool SetParagraphBorderHit(int milliseconds, List<Paragraph> paragraphs)
    {
        foreach (var p in paragraphs)
        {
            var hit = SetParagraphBorderHit(milliseconds, p);
            if (hit)
            {
                return true;
            }
        }
        return false;
    }

    private bool SetParagraphBorderHit(int milliseconds, Paragraph? paragraph)
    {
        if (paragraph == null)
        {
            return false;
        }

        if (IsParagraphBorderStartHit(milliseconds, paragraph.StartTime.TotalMilliseconds))
        {
            var idx = _displayableParagraphs.IndexOf(paragraph);
            if (idx > 0)
            {
                var prev = _displayableParagraphs[idx - 1];
                if (IsParagraphBorderStartHit(milliseconds, prev.EndTime.TotalMilliseconds) && !ModifierKeys.Alt)
                {
                    _mouseDownParagraph = null;
                    _mouseDownParagraphs = new List<Paragraph> { prev, paragraph }.ToArray();
                    _mouseDownParagraphType = MouseDownParagraphType.StartOrEnd;
                    OnStatus?.Invoke(this, new ParagraphEventArgs(new Paragraph() { Text = "MouseDownParagraphType.StartOrEnd" }));
                    return true;
                }
            }

            _oldParagraph = new Paragraph(paragraph, false);
            _mouseDownParagraph = paragraph;
            _mouseDownParagraphs = null;
            _mouseDownParagraphType = MouseDownParagraphType.Start;
            OnStatus?.Invoke(this, new ParagraphEventArgs(new Paragraph() { Text = "MouseDownParagraphType.Start" }));
            return true;
        }

        if (IsParagraphBorderEndHit(milliseconds, paragraph.EndTime.TotalMilliseconds))
        {
            var idx = _displayableParagraphs.IndexOf(paragraph);
            if (idx < _displayableParagraphs.Count - 2 && !ModifierKeys.Alt)
            {
                var next = _displayableParagraphs[idx + 1];
                if (IsParagraphBorderStartHit(milliseconds, next.StartTime.TotalMilliseconds))
                {
                    _mouseDownParagraph = null;
                    _mouseDownParagraphs = new List<Paragraph> { paragraph, next }.ToArray();
                    _mouseDownParagraphType = MouseDownParagraphType.StartOrEnd;
                    OnStatus?.Invoke(this, new ParagraphEventArgs(new Paragraph() { Text = "MouseDownParagraphType.StartOrEnd" }));
                    return true;
                }
            }

            _oldParagraph = new Paragraph(paragraph, false);
            _mouseDownParagraph = paragraph;
            _mouseDownParagraphs = null;
            _mouseDownParagraphType = MouseDownParagraphType.End;
            OnStatus?.Invoke(this, new ParagraphEventArgs(new Paragraph() { Text = "MouseDownParagraphType.End" }));
            return true;
        }

        return false;
    }

    private void PointerReleased(object? sender, PointerEventArgs e)
    {
        if (WavePeaks == null)
        {
            return;
        }

        var point = e.GetPosition(this);
        if (!point.HasValue)
        {
            return;
        }

        var x = (int)Math.Round(point.Value.X, MidpointRounding.AwayFromZero);

        try
        {
            if (MouseStatus.MouseButton1 && OnSingleClick != null)
            {
                var diff = Math.Abs(_mouseMoveStartX - x);
                if (_mouseMoveStartX == -1 || _mouseMoveEndX == -1 || diff < 10 && TimeSpan.FromTicks(DateTime.UtcNow.Ticks - _buttonDownTimeTicks).TotalSeconds < 0.25)
                {
                    if (ModifierKeys.Shift && SelectedParagraph != null)
                    {
                        var seconds = RelativeXPositionToSeconds(x);
                        var milliseconds = (int)(seconds * TimeCode.BaseUnit);
                        if (_mouseDownParagraphType == MouseDownParagraphType.None || _mouseDownParagraphType == MouseDownParagraphType.Whole)
                        {
                            if (seconds < SelectedParagraph.EndTime.TotalSeconds)
                            {
                                _oldParagraph = new Paragraph(SelectedParagraph, false);
                                _mouseDownParagraph = SelectedParagraph;
                                _mouseDownParagraph.StartTime.TotalMilliseconds = milliseconds;
                                OnStartTimeChanged?.Invoke(this, new ParagraphEventArgs(seconds, _mouseDownParagraph, _oldParagraph));
                            }
                        }
                        return;
                    }
                    if (ModifierKeys.Control && SelectedParagraph != null)
                    {
                        var seconds = RelativeXPositionToSeconds(x);
                        var milliseconds = (int)(seconds * TimeCode.BaseUnit);
                        if (_mouseDownParagraphType == MouseDownParagraphType.None || _mouseDownParagraphType == MouseDownParagraphType.Whole)
                        {
                            if (seconds > SelectedParagraph.StartTime.TotalSeconds)
                            {
                                _oldParagraph = new Paragraph(SelectedParagraph, false);
                                _mouseDownParagraph = SelectedParagraph;
                                _mouseDownParagraph.EndTime.TotalMilliseconds = milliseconds;
                                OnTimeChanged?.Invoke(this, new ParagraphEventArgs(seconds, _mouseDownParagraph, _oldParagraph));
                            }
                        }
                        return;
                    }
                    if (ModifierKeys.Control && ModifierKeys.Shift && SelectedParagraph != null)
                    {
                        var seconds = RelativeXPositionToSeconds(x);
                        if (_mouseDownParagraphType == MouseDownParagraphType.None || _mouseDownParagraphType == MouseDownParagraphType.Whole)
                        {
                            _oldParagraph = new Paragraph(SelectedParagraph, false);
                            _mouseDownParagraph = SelectedParagraph;
                            OnTimeChangedAndOffsetRest?.Invoke(this, new ParagraphEventArgs(seconds, _mouseDownParagraph));
                        }
                        return;
                    }
                    if (ModifierKeys.Alt && SelectedParagraph != null)
                    {
                        var seconds = RelativeXPositionToSeconds(x);
                        var milliseconds = (int)(seconds * TimeCode.BaseUnit);
                        if (_mouseDownParagraphType == MouseDownParagraphType.None || _mouseDownParagraphType == MouseDownParagraphType.Whole)
                        {
                            _oldParagraph = new Paragraph(SelectedParagraph, false);
                            _mouseDownParagraph = SelectedParagraph;
                            var durationMilliseconds = _mouseDownParagraph.DurationTotalMilliseconds;
                            _mouseDownParagraph.StartTime.TotalMilliseconds = milliseconds;
                            _mouseDownParagraph.EndTime.TotalMilliseconds = _mouseDownParagraph.StartTime.TotalMilliseconds + durationMilliseconds;
                            OnTimeChanged?.Invoke(this, new ParagraphEventArgs(seconds, _mouseDownParagraph, _oldParagraph));
                        }

                        return;
                    }

                    if (_mouseDownParagraphType == MouseDownParagraphType.None || _mouseDownParagraphType == MouseDownParagraphType.Whole)
                    {
                        var seconds = RelativeXPositionToSeconds(x);
                        var milliseconds = (int)(seconds * TimeCode.BaseUnit);
                        var p = GetParagraphAtMilliseconds(milliseconds);
                        if (p != null)
                        {
                            OnSingleClick?.Invoke(this, new ParagraphEventArgs(RelativeXPositionToSeconds(x), p));
                        }
                    }
                }
            }
        }
        finally
        {
            _mouseDown = false;
            _mouseDownParagraph = null;
            MouseStatus.MouseButton1 = false;
            MouseStatus.MouseButton2 = false;
        }
    }

    private void PointerMoved(object? sender, PointerEventArgs e)
    {
        if (WavePeaks == null)
        {
            return;
        }

        var point = e.GetPosition(this);
        if (point == null)
        {
            return;
        }

        List<Paragraph> paragraphs;
        lock (_lock)
        {
            paragraphs = _subtitle.Paragraphs.ToList();
        }

        var x = (int)Math.Round(point.Value.X, MidpointRounding.AwayFromZero);

        var oldMouseMoveLastX = _mouseMoveLastX;
        if (x < 0 && _startPositionSeconds > 0.1 && _mouseDown)
        {
            if (x < _mouseMoveLastX)
            {
                StartPositionSeconds -= 0.1;
                if (_mouseDownParagraph == null && _mouseDownParagraphs == null)
                {
                    _mouseMoveEndX = 0;
                    _mouseMoveStartX += (int)(WavePeaks.SampleRate * 0.1);
                    OnPositionSelected?.Invoke(this, new ParagraphEventArgs(_startPositionSeconds, null));
                }
            }
            _mouseMoveLastX = x;
            return;
        }

        if (x > _info.Width && _startPositionSeconds + 0.1 < WavePeaks.LengthInSeconds && _mouseDown)
        {
            StartPositionSeconds += 0.1;
            if (_mouseDownParagraph == null && _mouseDownParagraphs == null)
            {
                _mouseMoveEndX = (int)_info.Width;
                _mouseMoveStartX -= (int)(WavePeaks.SampleRate * 0.1);
                OnPositionSelected?.Invoke(this, new ParagraphEventArgs(_startPositionSeconds, null));
            }
            _mouseMoveLastX = x;
            return;
        }

        _mouseMoveLastX = x;

        if (x < 0 || x > _info.Width)
        {
            return;
        }

        if (MouseStatus.MouseButtonNone)
        {
            var seconds = RelativeXPositionToSeconds(x);
            var milliseconds = (int)(seconds * TimeCode.BaseUnit);

            if (IsParagraphBorderHit(milliseconds, NewSelectionParagraph))
            {
                SetCursor(CursorIcon.ResizeLeftRight); // VSplit
            }
            else if (IsParagraphBorderHit(milliseconds, SelectedParagraph) ||
                     IsParagraphBorderHit(milliseconds, _displayableParagraphs))
            {
                SetCursor(CursorIcon.ResizeLeftRight); // VSplit;
            }
            else
            {
                SetCursor(CursorIcon.Arrow); // Default;
            }
        }
        else if (MouseStatus.MouseButton1)
        {
            if (oldMouseMoveLastX == x)
            {
                return; // no horizontal movement
            }

            if (_mouseDown)
            {
                if (_mouseDownParagraphType == MouseDownParagraphType.StartOrEnd && _firstMove && !ModifierKeys.Alt)
                {
                    var seconds = RelativeXPositionToSeconds(x);
                    var milliseconds = (int)(seconds * TimeCode.BaseUnit);

                    if (_firstMove && Math.Abs(oldMouseMoveLastX - x) < Configuration.Settings.General.MinimumMillisecondsBetweenLines && GetParagraphAtMilliseconds(milliseconds) == null)
                    {
                        if (_mouseDownParagraphType == MouseDownParagraphType.StartOrEnd && _mouseDownParagraphs?.Length == 2 && Math.Abs(_mouseDownParagraphs[0].StartTime.TotalMilliseconds - _mouseDownParagraphs[0].EndTime.TotalMilliseconds) <= ClosenessForBorderSelection + 15)
                        {
                            return; // do not decide which paragraph to move yet
                        }

                        if (_mouseDownParagraphType == MouseDownParagraphType.StartOrEnd && _mouseDownParagraphs?.Length == 2 && Math.Abs(_mouseDownParagraphs[1].EndTime.TotalMilliseconds - _mouseDownParagraphs[1].StartTime.TotalMilliseconds) <= ClosenessForBorderSelection + 15)
                        {
                            return; // do not decide which paragraph to move yet
                        }
                    }

                    if (_mouseDownParagraphs?.Length == 2)
                    {
                        // decide which paragraph to move
                        if (_firstMove && x > oldMouseMoveLastX)
                        {
                            if (milliseconds >= _mouseDownParagraphs[1].StartTime.TotalMilliseconds && milliseconds < _mouseDownParagraphs[1].EndTime.TotalMilliseconds)
                            {
                                _mouseDownParagraph = _mouseDownParagraphs[1];
                                _mouseDownParagraphType = MouseDownParagraphType.Start;
                                _mouseDownParagraphs = null;
                                _oldParagraph = new Paragraph(_mouseDownParagraph, false);
                                _firstMove = false;
                            }
                        }
                        else if (_firstMove && x < oldMouseMoveLastX)
                        {
                            if (milliseconds <= _mouseDownParagraphs[0].EndTime.TotalMilliseconds && milliseconds > _mouseDownParagraphs[0].StartTime.TotalMilliseconds)
                            {
                                _mouseDownParagraph = _mouseDownParagraphs[0];
                                _mouseDownParagraphType = MouseDownParagraphType.End;
                                _mouseDownParagraphs = null;
                                _oldParagraph = new Paragraph(_mouseDownParagraph, false);
                                _firstMove = false;
                            }
                        }

                        return;
                    }
                }

                if (_mouseDownParagraph != null)
                {
                    var seconds = RelativeXPositionToSeconds(x);
                    var milliseconds = (int)(seconds * TimeCode.BaseUnit);
                    var subtitleIndex = paragraphs.IndexOf(_mouseDownParagraph);
                    _prevParagraph = GetParagraphOrDefault(paragraphs, subtitleIndex - 1);
                    _nextParagraph = GetParagraphOrDefault(paragraphs, subtitleIndex + 1);

                    if (_firstMove && Math.Abs(oldMouseMoveLastX - x) < Configuration.Settings.General.MinimumMillisecondsBetweenLines && GetParagraphAtMilliseconds(milliseconds) == null)
                    {
                        if (_mouseDownParagraphType == MouseDownParagraphType.StartOrEnd && _prevParagraph != null && Math.Abs(_mouseDownParagraph.StartTime.TotalMilliseconds - _prevParagraph.EndTime.TotalMilliseconds) <= ClosenessForBorderSelection + 15)
                        {
                            return; // do not decide which paragraph to move yet
                        }

                        if (_mouseDownParagraphType == MouseDownParagraphType.StartOrEnd && _nextParagraph != null && Math.Abs(_mouseDownParagraph.EndTime.TotalMilliseconds - _nextParagraph.StartTime.TotalMilliseconds) <= ClosenessForBorderSelection + 15)
                        {
                            return; // do not decide which paragraph to move yet
                        }
                    }

                    if (_firstMove && !ModifierKeys.Alt && !ModifierKeys.Shift &&
                        !Configuration.Settings.VideoControls.WaveformAllowOverlap)
                    {
                        // decide which paragraph to move
                        if (_firstMove && x > oldMouseMoveLastX && _nextParagraph != null && _mouseDownParagraphType == MouseDownParagraphType.End)
                        {
                            if (milliseconds >= _nextParagraph.StartTime.TotalMilliseconds && milliseconds < _nextParagraph.EndTime.TotalMilliseconds &&
                                (_mouseDownParagraph == null || _nextParagraph.Id != _mouseDownParagraph.Id))
                            {
                                _mouseDownParagraph = _nextParagraph;
                                _mouseDownParagraphType = MouseDownParagraphType.Start;
                            }
                        }
                        else if (_firstMove && x < oldMouseMoveLastX && _prevParagraph != null && _mouseDownParagraphType == MouseDownParagraphType.Start)
                        {
                            if (milliseconds <= _prevParagraph.EndTime.TotalMilliseconds && milliseconds > _prevParagraph.StartTime.TotalMilliseconds)
                            {
                                _mouseDownParagraph = _prevParagraph;
                                _mouseDownParagraphType = MouseDownParagraphType.End;
                            }
                        }
                    }
                    _firstMove = false;

                    if (_mouseDownParagraphType == MouseDownParagraphType.Start)
                    {
                        if (_mouseDownParagraph.EndTime.TotalMilliseconds - milliseconds > MinimumSelectionMilliseconds)
                        {
                            if (AllowMovePrevOrNext)
                            {
                                SetMinAndMaxMoveStart();
                            }
                            else
                            {
                                SetMinAndMax();
                            }

                            _mouseDownParagraph.StartTime.TotalMilliseconds = milliseconds;

                            if (Configuration.Settings.VideoControls.WaveformSnapToShotChanges && !ModifierKeys.Shift &&
                                _shotChanges?.Count > 0)
                            {
                                var nearestShotChange = ShotChangeHelper.GetClosestShotChange(_shotChanges, new TimeCode(milliseconds));
                                if (nearestShotChange != null && Math.Abs(x - SecondsToXPosition(nearestShotChange.Value - _startPositionSeconds)) < ShotChangeSnapPixels)
                                {
                                    _mouseDownParagraph.StartTime.TotalMilliseconds = (nearestShotChange.Value * 1000) + TimeCodesBeautifierUtils.GetInCuesGapMs();
                                }
                            }

                            if (PreventOverlap && _mouseDownParagraph.StartTime.TotalMilliseconds <= _wholeParagraphMinMilliseconds)
                            {
                                _mouseDownParagraph.StartTime.TotalMilliseconds = _wholeParagraphMinMilliseconds + 1;
                            }

                            if (NewSelectionParagraph != null)
                            {
                                NewSelectionParagraph.StartTime.TotalMilliseconds = milliseconds;

                                if (Configuration.Settings.VideoControls.WaveformSnapToShotChanges && !ModifierKeys.Shift)
                                {
                                    var nearestShotChange = ShotChangeHelper.GetClosestShotChange(_shotChanges, new TimeCode(milliseconds));
                                    if (nearestShotChange != null && Math.Abs(x - SecondsToXPosition(nearestShotChange.Value - _startPositionSeconds)) < ShotChangeSnapPixels)
                                    {
                                        NewSelectionParagraph.StartTime.TotalMilliseconds = (nearestShotChange.Value * 1000) + TimeCodesBeautifierUtils.GetInCuesGapMs();
                                    }
                                }

                                if (PreventOverlap && NewSelectionParagraph.StartTime.TotalMilliseconds <= _wholeParagraphMinMilliseconds)
                                {
                                    NewSelectionParagraph.StartTime.TotalMilliseconds = _wholeParagraphMinMilliseconds + 1;
                                }

                                _mouseMoveStartX = x;
                            }
                            else
                            {
                                OnTimeChanged?.Invoke(this, new ParagraphEventArgs(seconds, _mouseDownParagraph, _oldParagraph, _mouseDownParagraphType, AllowMovePrevOrNext));
                                return;
                            }
                        }
                    }
                    else if (_mouseDownParagraphType == MouseDownParagraphType.End)
                    {
                        if (milliseconds - _mouseDownParagraph.StartTime.TotalMilliseconds > MinimumSelectionMilliseconds)
                        {
                            if (AllowMovePrevOrNext)
                            {
                                SetMinAndMaxMoveEnd();
                            }
                            else
                            {
                                SetMinAndMax();
                            }

                            _mouseDownParagraph.EndTime.TotalMilliseconds = milliseconds;

                            if (Configuration.Settings.VideoControls.WaveformSnapToShotChanges && !ModifierKeys.Shift)
                            {
                                var nearestShotChange = ShotChangeHelper.GetClosestShotChange(_shotChanges, new TimeCode(milliseconds));
                                if (nearestShotChange != null && Math.Abs(x - SecondsToXPosition(nearestShotChange.Value - _startPositionSeconds)) < ShotChangeSnapPixels)
                                {
                                    _mouseDownParagraph.EndTime.TotalMilliseconds = (nearestShotChange.Value * 1000) - TimeCodesBeautifierUtils.GetOutCuesGapMs();
                                }
                            }

                            if (PreventOverlap && _mouseDownParagraph.EndTime.TotalMilliseconds >= _wholeParagraphMaxMilliseconds)
                            {
                                _mouseDownParagraph.EndTime.TotalMilliseconds = _wholeParagraphMaxMilliseconds - 1;
                            }

                            if (NewSelectionParagraph != null)
                            {
                                NewSelectionParagraph.EndTime.TotalMilliseconds = milliseconds;

                                if (Configuration.Settings.VideoControls.WaveformSnapToShotChanges && !ModifierKeys.Shift)
                                {
                                    var nearestShotChange = ShotChangeHelper.GetClosestShotChange(_shotChanges, new TimeCode(milliseconds));
                                    if (nearestShotChange != null && Math.Abs(x - SecondsToXPosition(nearestShotChange.Value - _startPositionSeconds)) < ShotChangeSnapPixels)
                                    {
                                        NewSelectionParagraph.EndTime.TotalMilliseconds = (nearestShotChange.Value * 1000) - TimeCodesBeautifierUtils.GetOutCuesGapMs();
                                    }
                                }

                                if (PreventOverlap && NewSelectionParagraph.EndTime.TotalMilliseconds >= _wholeParagraphMaxMilliseconds)
                                {
                                    NewSelectionParagraph.EndTime.TotalMilliseconds = _wholeParagraphMaxMilliseconds - 1;
                                }

                                _mouseMoveEndX = x;
                            }
                            else
                            {
                                OnTimeChanged?.Invoke(this, new ParagraphEventArgs(seconds, _mouseDownParagraph, _oldParagraph, _mouseDownParagraphType, AllowMovePrevOrNext));
                                return;
                            }
                        }
                    }
                    else if (_mouseDownParagraphType == MouseDownParagraphType.Whole)
                    {
                        var durationMilliseconds = _mouseDownParagraph.DurationTotalMilliseconds;
                        var oldStart = _mouseDownParagraph.StartTime.TotalMilliseconds;
                        _mouseDownParagraph.StartTime.TotalMilliseconds = milliseconds - _moveWholeStartDifferenceMilliseconds;
                        _mouseDownParagraph.EndTime.TotalMilliseconds = _mouseDownParagraph.StartTime.TotalMilliseconds + durationMilliseconds;

                        if (Configuration.Settings.VideoControls.WaveformSnapToShotChanges && !ModifierKeys.Shift)
                        {
                            var nearestShotChangeInFront = ShotChangeHelper.GetClosestShotChange(_shotChanges, _mouseDownParagraph.StartTime);
                            var nearestShotChangeInBack = ShotChangeHelper.GetClosestShotChange(_shotChanges, _mouseDownParagraph.EndTime);

                            if (nearestShotChangeInFront != null && Math.Abs(SecondsToXPosition(_mouseDownParagraph.StartTime.TotalSeconds - _startPositionSeconds) - SecondsToXPosition(nearestShotChangeInFront.Value - _startPositionSeconds)) < ShotChangeSnapPixels)
                            {
                                var nearestShotChangeInFrontMs = (nearestShotChangeInFront.Value * 1000) + TimeCodesBeautifierUtils.GetInCuesGapMs();
                                _mouseDownParagraph.StartTime.TotalMilliseconds = nearestShotChangeInFrontMs;
                                _mouseDownParagraph.EndTime.TotalMilliseconds = nearestShotChangeInFrontMs + durationMilliseconds;
                            }
                            else if (nearestShotChangeInBack != null && Math.Abs(SecondsToXPosition(_mouseDownParagraph.EndTime.TotalSeconds - _startPositionSeconds) - SecondsToXPosition(nearestShotChangeInBack.Value - _startPositionSeconds)) < ShotChangeSnapPixels)
                            {
                                var nearestShotChangeInBackMs = (nearestShotChangeInBack.Value * 1000) - TimeCodesBeautifierUtils.GetOutCuesGapMs();
                                _mouseDownParagraph.EndTime.TotalMilliseconds = nearestShotChangeInBackMs;
                                _mouseDownParagraph.StartTime.TotalMilliseconds = nearestShotChangeInBackMs - durationMilliseconds;
                            }
                        }

                        if (PreventOverlap && _mouseDownParagraph.EndTime.TotalMilliseconds >= _wholeParagraphMaxMilliseconds)
                        {
                            _mouseDownParagraph.EndTime.TotalMilliseconds = _wholeParagraphMaxMilliseconds - 1;
                            _mouseDownParagraph.StartTime.TotalMilliseconds = _mouseDownParagraph.EndTime.TotalMilliseconds - durationMilliseconds;
                        }
                        else if (PreventOverlap && _mouseDownParagraph.StartTime.TotalMilliseconds <= _wholeParagraphMinMilliseconds)
                        {
                            _mouseDownParagraph.StartTime.TotalMilliseconds = _wholeParagraphMinMilliseconds + 1;
                            _mouseDownParagraph.EndTime.TotalMilliseconds = _mouseDownParagraph.StartTime.TotalMilliseconds + durationMilliseconds;
                        }

                        if (PreventOverlap &&
                            (_mouseDownParagraph.StartTime.TotalMilliseconds <= _wholeParagraphMinMilliseconds ||
                             _mouseDownParagraph.EndTime.TotalMilliseconds >= _wholeParagraphMaxMilliseconds))
                        {
                            _mouseDownParagraph.StartTime.TotalMilliseconds = oldStart;
                            _mouseDownParagraph.EndTime.TotalMilliseconds = oldStart + durationMilliseconds;
                            return;
                        }

                        OnTimeChanged?.Invoke(this, new ParagraphEventArgs(seconds, _mouseDownParagraph, _oldParagraph, _mouseDownParagraphType) { AdjustMs = _mouseDownParagraph.StartTime.TotalMilliseconds - oldStart });
                    }
                }
                else
                {
                    _mouseMoveEndX = x;
                    if (NewSelectionParagraph == null && Math.Abs(_mouseMoveEndX - _mouseMoveStartX) > 2)
                    {
                        if (AllowNewSelection)
                        {
                            NewSelectionParagraph = new Paragraph();
                        }
                    }

                    if (NewSelectionParagraph != null)
                    {
                        var start = Math.Min(_mouseMoveStartX, _mouseMoveEndX);
                        var end = Math.Max(_mouseMoveStartX, _mouseMoveEndX);

                        var startTotalSeconds = RelativeXPositionToSeconds(start);
                        var endTotalSeconds = RelativeXPositionToSeconds(end);

                        NewSelectionParagraph.StartTime.TotalSeconds = startTotalSeconds;
                        NewSelectionParagraph.EndTime.TotalSeconds = endTotalSeconds;

                        if (Configuration.Settings.VideoControls.WaveformSnapToShotChanges && !ModifierKeys.Shift)
                        {
                            var nearestShotChangeInFront = ShotChangeHelper.GetClosestShotChange(_shotChanges, TimeCode.FromSeconds(startTotalSeconds));
                            var nearestShotChangeInBack = ShotChangeHelper.GetClosestShotChange(_shotChanges, TimeCode.FromSeconds(endTotalSeconds));

                            if (nearestShotChangeInFront != null && Math.Abs(SecondsToXPosition(NewSelectionParagraph.StartTime.TotalSeconds - _startPositionSeconds) - SecondsToXPosition(nearestShotChangeInFront.Value - _startPositionSeconds)) < ShotChangeSnapPixels)
                            {
                                NewSelectionParagraph.StartTime.TotalMilliseconds = (nearestShotChangeInFront.Value * 1000) + TimeCodesBeautifierUtils.GetInCuesGapMs();
                            }
                            if (nearestShotChangeInBack != null && Math.Abs(SecondsToXPosition(NewSelectionParagraph.EndTime.TotalSeconds - _startPositionSeconds) - SecondsToXPosition(nearestShotChangeInBack.Value - _startPositionSeconds)) < ShotChangeSnapPixels)
                            {
                                NewSelectionParagraph.EndTime.TotalMilliseconds = (nearestShotChangeInBack.Value * 1000) - TimeCodesBeautifierUtils.GetOutCuesGapMs();
                            }
                        }

                        if (PreventOverlap && endTotalSeconds * TimeCode.BaseUnit >= _wholeParagraphMaxMilliseconds)
                        {
                            NewSelectionParagraph.EndTime.TotalMilliseconds = _wholeParagraphMaxMilliseconds - 1;
                        }
                        if (PreventOverlap && startTotalSeconds * TimeCode.BaseUnit <= _wholeParagraphMinMilliseconds)
                        {
                            NewSelectionParagraph.StartTime.TotalMilliseconds = _wholeParagraphMinMilliseconds + 1;
                        }
                    }
                }
            }
        }
    }

    public Paragraph? GetParagraphOrDefault(List<Paragraph> paragraphs, int index)
    {
        return paragraphs.Count <= index || index < 0
            ? null
            : paragraphs[index];
    }

    CursorIcon _lastCursor = CursorIcon.Arrow;
    private void SetCursor(CursorIcon cursor)
    {
        if (cursor == _lastCursor || !AllowMove)
        {
            return;
        }

        if (Handler?.MauiContext is MauiContext context)
        {
            this.SetCustomCursor(cursor, context);
            _lastCursor = cursor;
        }
    }

    private Paragraph? GetParagraphAtMilliseconds(int milliseconds)
    {
        Paragraph? p = null;
        if (IsParagraphHit(milliseconds, SelectedParagraph))
        {
            p = SelectedParagraph;
        }

        if (p == null)
        {
            foreach (var pNext in _displayableParagraphs)
            {
                if (IsParagraphHit(milliseconds, pNext))
                {
                    p = pNext;
                    break;
                }
            }
        }

        return p;
    }

    private static bool IsParagraphHit(int milliseconds, Paragraph? paragraph)
    {
        if (paragraph == null)
        {
            return false;
        }

        return milliseconds >= paragraph.StartTime.TotalMilliseconds && milliseconds <= paragraph.EndTime.TotalMilliseconds;
    }

    private void SetMinAndMaxMoveStart()
    {
        _wholeParagraphMinMilliseconds = 0;
        _wholeParagraphMaxMilliseconds = double.MaxValue;
        if (_mouseDownParagraph != null)
        {
            List<Paragraph> paragraphs;
            lock (_lock)
            {
                paragraphs = _subtitle.Paragraphs.ToList();
            }

            var curIdx = paragraphs.IndexOf(_mouseDownParagraph);
            if (curIdx >= 0)
            {
                var gap = Math.Abs(paragraphs[curIdx - 1].EndTime.TotalMilliseconds - paragraphs[curIdx].StartTime.TotalMilliseconds);
                _wholeParagraphMinMilliseconds = paragraphs[curIdx - 1].StartTime.TotalMilliseconds + gap + 200;
            }
        }
    }

    private void SetMinAndMaxMoveEnd()
    {
        _wholeParagraphMinMilliseconds = 0;
        _wholeParagraphMaxMilliseconds = double.MaxValue;
        if (_mouseDownParagraph != null)
        {
            List<Paragraph> paragraphs;
            lock (_lock)
            {
                paragraphs = _subtitle.Paragraphs.ToList();
            }

            var curIdx = paragraphs.IndexOf(_mouseDownParagraph);
            if (curIdx >= 0)
            {
                if (curIdx < _subtitle.Paragraphs.Count - 1)
                {
                    var gap = Math.Abs(paragraphs[curIdx].EndTime.TotalMilliseconds - paragraphs[curIdx + 1].StartTime.TotalMilliseconds);
                    _wholeParagraphMaxMilliseconds = paragraphs[curIdx + 1].EndTime.TotalMilliseconds - gap - 200;
                }
            }
        }
    }

    private bool IsParagraphBorderHit(int milliseconds, Paragraph? paragraph)
    {
        if (paragraph == null)
        {
            return false;
        }

        return IsParagraphBorderStartHit(milliseconds, paragraph.StartTime.TotalMilliseconds) ||
               IsParagraphBorderEndHit(milliseconds, paragraph.EndTime.TotalMilliseconds);
    }

    private bool IsParagraphBorderHit(int milliseconds, List<Paragraph> paragraphs)
    {
        var list = paragraphs.ToList();
        foreach (var p in list)
        {
            var hit = IsParagraphBorderHit(milliseconds, p);
            if (hit)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsParagraphBorderStartHit(double milliseconds, double startMs)
    {
        return Math.Abs(milliseconds - (startMs - 5)) - 10 <= ClosenessForBorderSelection / ZoomFactor;
    }

    private bool IsParagraphBorderEndHit(double milliseconds, double endMs)
    {
        return Math.Abs(milliseconds - (endMs - 22)) - 7 <= ClosenessForBorderSelection / ZoomFactor;
    }

    private bool PreventOverlap
    {
        get
        {
            if (ModifierKeys.Shift)
            {
                return AllowOverlap;
            }

            return !AllowOverlap;
        }
    }

    private bool AllowMovePrevOrNext => _gapAtStart is >= 0 and < 500 && ModifierKeys.Alt;

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
#if WINDOWS
        if (Handler != null)          
        {
            var view = Handler.PlatformView as SkiaSharp.Views.Windows.SKXamlCanvas;
            if (view != null) 
            {
                view.PointerWheelChanged += (s, e) =>
                {
                    var point = e.GetCurrentPoint(s as Microsoft.Maui.Platform.ContentPanel);
                    var delta = point.Properties.MouseWheelDelta;
                    var positionSeconds = _currentVideoPositionSeconds + delta / 80.0;
                    OnVideoPositionChanged?.Invoke(this, new PositionEventArgs { PositionInSeconds = positionSeconds });
                };
            }  
        }  
#endif
    }

    private void TappedSingle(object? sender, TappedEventArgs? e)
    {
        if (WavePeaks == null || e == null)
        {
            return;
        }

        var point = e.GetPosition(this);
        if (!point.HasValue)
        {
            return;
        }

        if (e.Buttons == ButtonsMask.Primary)
        {
            MouseStatus.MouseButton1 = true;
            MouseStatus.MouseButton2 = false;
        }

        var positionInSeconds = RelativeXPositionToSeconds(point.Value.X);
        OnVideoPositionChanged?.Invoke(this, new PositionEventArgs { PositionInSeconds = positionInSeconds });
    }

    private void TappedDouble(object? sender, TappedEventArgs e)
    {
        if (WavePeaks == null)
        {
            return;
        }

        var point = e.GetPosition(this);
        if (!point.HasValue)
        {
            return;
        }

        var positionInSeconds = RelativeXPositionToSeconds(point.Value.X);
        OnDoubleTapped?.Invoke(this, new PositionEventArgs { PositionInSeconds = positionInSeconds });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private double RelativeXPositionToSeconds(double x)
    {
        if (WavePeaks == null)
        {
            return 0;
        }

        return _startPositionSeconds + x / WavePeaks.SampleRate / _zoomFactor;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private double RelativeXPositionToSeconds(int x)
    {
        if (WavePeaks == null)
        {
            return 0;
        }

        return _startPositionSeconds + (double)x / WavePeaks.SampleRate / _zoomFactor;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int SecondsToXPosition(double seconds)
    {
        if (WavePeaks == null)
        {
            return 0;
        }

        return (int)Math.Round(seconds * WavePeaks.SampleRate * _zoomFactor, MidpointRounding.AwayFromZero);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int SecondsToSampleIndex(double seconds)
    {
        if (WavePeaks == null)
        {
            return 0;
        }

        return (int)Math.Round(seconds * WavePeaks.SampleRate, MidpointRounding.AwayFromZero);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private double SampleIndexToSeconds(int index)
    {
        if (WavePeaks == null)
        {
            return 0;
        }

        return (double)index / WavePeaks.SampleRate;
    }

    public double EndPositionSeconds
    {
        get
        {
            if (WavePeaks == null)
            {
                return 0;
            }

            return RelativeXPositionToSeconds(_info.Width);
        }
    }

    public void SetPosition(double startPositionSeconds, Subtitle subtitle, double currentVideoPositionSeconds, int subtitleIndex, int[] selectedIndexes)
    {
        if (TimeSpan.FromTicks(DateTime.UtcNow.Ticks - _lastMouseWheelScroll).TotalSeconds > 0.25)
        { // don't set start position when scrolling with mouse wheel as it will make a bad (jumping back) forward scrolling
            StartPositionSeconds = startPositionSeconds;
        }
        _currentVideoPositionSeconds = currentVideoPositionSeconds;
        LoadParagraphs(subtitle, subtitleIndex, selectedIndexes);
    }

    private void LoadParagraphs(Subtitle subtitle, int primarySelectedIndex, int[] selectedIndexes)
    {
        lock (_lock)
        {
            _subtitle.Paragraphs.Clear();
            _displayableParagraphs.Clear();
            SelectedParagraph = null;
            _allSelectedParagraphs.Clear();

            if (WavePeaks == null)
            {
                return;
            }

            const double additionalSeconds = 15.0; // Helps when scrolling
            var startThresholdMilliseconds = (_startPositionSeconds - additionalSeconds) * TimeCode.BaseUnit;
            var endThresholdMilliseconds = (EndPositionSeconds + additionalSeconds) * TimeCode.BaseUnit;
            var displayableParagraphs = new List<Paragraph>();
            for (var i = 0; i < subtitle.Paragraphs.Count; i++)
            {
                var p = subtitle.Paragraphs[i];

                if (p.StartTime.IsMaxTime)
                {
                    continue;
                }

                _subtitle.Paragraphs.Add(p);
                if (p.EndTime.TotalMilliseconds >= startThresholdMilliseconds && p.StartTime.TotalMilliseconds <= endThresholdMilliseconds)
                {
                    displayableParagraphs.Add(p);
                    if (displayableParagraphs.Count > 99)
                    {
                        break;
                    }
                }
            }

            displayableParagraphs = displayableParagraphs.OrderBy(p => p.StartTime.TotalMilliseconds).ToList();
            var lastStartTime = -1d;
            foreach (var p in displayableParagraphs)
            {
                if (displayableParagraphs.Count > 30 &&
                    (p.Duration.TotalMilliseconds < 0.01 || p.StartTime.TotalMilliseconds - lastStartTime < 90))
                {
                    continue;
                }

                _displayableParagraphs.Add(p);
                lastStartTime = p.StartTime.TotalMilliseconds;
            }

            var primaryParagraph = subtitle.GetParagraphOrDefault(primarySelectedIndex);
            if (primaryParagraph != null && !primaryParagraph.StartTime.IsMaxTime)
            {
                SelectedParagraph = primaryParagraph;
                _allSelectedParagraphs.Add(primaryParagraph);
            }

            foreach (var index in selectedIndexes)
            {
                var p = subtitle.GetParagraphOrDefault(index);
                if (p != null && !p.StartTime.IsMaxTime)
                {
                    _allSelectedParagraphs.Add(p);
                }
            }
        }
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        // base.OnPaintSurface(e);

        _canvas = e.Surface.Canvas;
        _canvas.Clear(SKColors.Black); // clears the canvas for every frame
        _info = e.Info;

        DrawGridLines();
        if (WavePeaks == null)
        {
            return;
        }

        DrawWaveForm();
        DrawParagraphs(e);
        DrawCurrentVideoPosition();
        DrawNewParagraph();

        if (_mouseOver)
        {
            _canvas.DrawRect(0, 0, _info.Width, _info.Height, _paintPenSelected);
        }

        e.Surface.Canvas.Flush();
    }

    private void DrawNewParagraph()
    {
        // current selection
        if (NewSelectionParagraph == null || _canvas == null)
        {
            return;
        }

        var currentRegionLeft = SecondsToXPosition(NewSelectionParagraph.StartTime.TotalSeconds - _startPositionSeconds);
        var currentRegionRight = SecondsToXPosition(NewSelectionParagraph.EndTime.TotalSeconds - _startPositionSeconds);
        var currentRegionWidth = currentRegionRight - currentRegionLeft;
        if (currentRegionRight >= 0 && currentRegionLeft <= _info.Width)
        {
            _canvas.DrawRect(currentRegionLeft, 0, currentRegionWidth, _info.Height, _paintBackground);

            if (currentRegionWidth > 40)
            {
                //TODO:
                //using (var brush = new SolidBrush(CursorColor))
                //{
                //    graphics.DrawString($"{(double)currentRegionWidth / WavePeaks.SampleRate / _zoomFactor:0.###} {LanguageSettings.Current.Waveform.Seconds}", Font, brush, new PointF(currentRegionLeft + 3, Height - 32));
                //}
            }
        }
    }

    private void DrawParagraphs(SKPaintSurfaceEventArgs e)
    {
        var startPositionMilliseconds = _startPositionSeconds * 1000.0;
        var endPositionMilliseconds = RelativeXPositionToSeconds(_info.Width) * 1000.0;
        var paragraphStartList = new List<int>();
        var paragraphEndList = new List<int>();

        lock (_lock)
        {
            var paragraphs = _displayableParagraphs;
            foreach (var p in paragraphs)
            {
                if (p.EndTime.TotalMilliseconds >= startPositionMilliseconds && p.StartTime.TotalMilliseconds <= endPositionMilliseconds)
                {
                    paragraphStartList.Add(SecondsToXPosition(p.StartTime.TotalSeconds - _startPositionSeconds));
                    paragraphEndList.Add(SecondsToXPosition(p.EndTime.TotalSeconds - _startPositionSeconds));
                    DrawParagraph(p, e);
                }
            }
        }
    }

    private void DrawParagraph(Paragraph paragraph, SKPaintSurfaceEventArgs e)
    {
        var currentRegionLeft = SecondsToXPosition(paragraph.StartTime.TotalSeconds - _startPositionSeconds);
        var currentRegionRight = SecondsToXPosition(paragraph.EndTime.TotalSeconds - _startPositionSeconds);
        var currentRegionWidth = currentRegionRight - currentRegionLeft;

        if (currentRegionWidth <= 5 || _canvas == null)
        {
            return;
        }

        if (_info.Height < 1)
        {
            return;
        }

        _canvas.DrawRect(currentRegionLeft, 0, currentRegionWidth, _info.Height, _paintBackground);
        _canvas.DrawLine(currentRegionLeft, 0, currentRegionLeft, (float)_info.Height, _paintLeft);
        _canvas.DrawLine(currentRegionRight - 1, 0, currentRegionRight - 1, (float)_info.Height, _paintRight);


        // draw clipped text
        var text = HtmlUtil.RemoveHtmlTags(paragraph.Text, true);
        if (text.Length > 200)
        {
            text = text.Substring(0, 100).TrimEnd() + "...";
        }

        var textBounds = new SKRect(currentRegionLeft + 1, 0, currentRegionRight - 2, _info.Height);
        _canvas.Save();
        _canvas.ClipRect(textBounds);
        var arr = text.SplitToLines();
        if (Configuration.Settings.VideoControls.WaveformUnwrapText)
        {
            text = string.Join("  ", arr);
            _canvas.DrawText(text, currentRegionLeft + 3, 14, _paintText);
        }
        else
        {
            float addY = 0f;
            foreach (var line in arr)
            {
                _canvas.DrawText(line, currentRegionLeft + 3, 14 + addY, _paintText);
                var fontMetrics = _paintText.FontMetrics;
                float height = fontMetrics.Descent - fontMetrics.Ascent;
                addY += height;
            }
        }

        _canvas.Restore();
    }

    private void DrawCurrentVideoPosition()
    {
        if (_canvas == null)
        {
            return;
        }

        // current video position
        var currentPositionPos = SecondsToXPosition(_currentVideoPositionSeconds - _startPositionSeconds);
        if (_currentVideoPositionSeconds > 0 && currentPositionPos > 0 && currentPositionPos < _info.Width)
        {
            using var paintStyle = new SKPaint
            {
                Color = SKColors.Cyan,
                StrokeWidth = 1,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
            };
            _canvas.DrawLine(currentPositionPos, 0, currentPositionPos, (float)_info.Height, paintStyle);
        }
    }

    private readonly SKPaint _paintWaveform = new SKPaint
    {
        Color = SKColor.Parse("#ff4cc2ff"),
        StrokeWidth = 1,
        IsAntialias = true,
        Style = SKPaintStyle.Stroke,
    };

    private readonly SKPaint _paintPenSelected = new SKPaint
    {
        Color = SKColor.Parse("#ffff8888"),
        StrokeWidth = 1,
        IsAntialias = true,
        Style = SKPaintStyle.Stroke,
    };

    private void DrawWaveForm()
    {
        if (WavePeaks == null || _canvas == null)
        {
            return;
        }

        var _showWaveform = true;
        if (_showWaveform)
        {
            var waveformHeight = _info.Height;
            var isSelectedHelper = new IsSelectedHelper(_allSelectedParagraphs, WavePeaks.SampleRate);
            var baseHeight = (int)(WavePeaks.HighestPeak / _verticalZoomFactor);
            var halfWaveformHeight = waveformHeight / 2;

            var div = WavePeaks.SampleRate * _zoomFactor;
            for (var x = 0; x < _info.Width; x++)
            {
                var pos = (_startPositionSeconds + x / div) * WavePeaks.SampleRate;
                var pos0 = (int)pos;
                var pos1 = pos0;
                pos1++;
                if (pos1 >= WavePeaks.Peaks.Count)
                {
                    break;
                }

                var pos1Weight = pos - pos0;
                var pos0Weight = 1F - pos1Weight;
                var peak0 = WavePeaks.Peaks[pos0];
                var peak1 = WavePeaks.Peaks[pos1];
                var max = peak0.Max * pos0Weight + peak1.Max * pos1Weight;
                var min = peak0.Min * pos0Weight + peak1.Min * pos1Weight;
                var yMax = CalculateY(max, baseHeight, halfWaveformHeight);
                var yMin = Math.Max(CalculateY(min, baseHeight, halfWaveformHeight), yMax + 0.1F);
                var pen = isSelectedHelper.IsSelected(pos0) ? _paintPenSelected : _paintWaveform;
                _canvas.DrawLine(x, yMax, x, yMin, pen);
            }
        }
    }

    private static float CalculateY(double value, int baseHeight, int halfWaveformHeight)
    {
        var offset = value / baseHeight * halfWaveformHeight;
        if (offset > halfWaveformHeight)
        {
            offset = halfWaveformHeight;
        }

        if (offset < -halfWaveformHeight)
        {
            offset = -halfWaveformHeight;
        }

        return (float)(halfWaveformHeight - offset);
    }

    private void DrawGridLines()
    {
        if (!ShowGridLines || _canvas == null)
        {
            return;
        }

        if (WavePeaks == null)
        {
            for (var i = 0; i < _info.Width; i += 10)
            {
                _canvas.DrawLine(i, 0, i, (float)_info.Height, _paintGridColor);
                _canvas.DrawLine(0, i, (float)_info.Width, i, _paintGridColor);
            }
        }
        else
        {
            var seconds = Math.Ceiling(_startPositionSeconds) - _startPositionSeconds - 1;
            var xPosition = SecondsToXPosition(seconds);
            var yPosition = 0;
            var yCounter = 0d;
            var interval = _zoomFactor >= 0.4d ?
                0.1d : // a pixel is 0.1 second
                1.0d;  // a pixel is 1.0 second

            while (xPosition < _info.Width)
            {
                _canvas.DrawLine(xPosition, 0, xPosition, (float)_info.Height, _paintGridColor);

                seconds += interval;
                xPosition = SecondsToXPosition(seconds);
            }

            while (yPosition < _info.Height)
            {
                _canvas.DrawLine(0, yPosition, (float)_info.Width, yPosition, _paintGridColor);

                yCounter += interval;
                yPosition = Convert.ToInt32(yCounter * WavePeaks.SampleRate * _zoomFactor);
            }
        }
    }
}