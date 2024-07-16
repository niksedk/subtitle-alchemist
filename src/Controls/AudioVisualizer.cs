using System.Runtime.CompilerServices;
using Nikse.SubtitleEdit.Core.Common;
using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;
using SkiaSharp.Views.Maui.Handlers;
using SKPaintSurfaceEventArgs = SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs;

namespace SubtitleAlchemist.Controls
{
    public static class Registration
    {
        public static MauiAppBuilder UseAudioVisualizer(this MauiAppBuilder builder)
        {
            builder.ConfigureMauiHandlers(h =>
            {
                h.AddHandler<AudioVisualizer, SKCanvasViewHandler>();
            });

            return builder;
        }
    }

    public class AudioVisualizer : SKCanvasView
    {
        private readonly object _lock = new();

        // actual canvas instance to draw on
        private SKCanvas _canvas;

        // holds information about the dimensions, etc.
        private SKImageInfo _info;

        public WavePeakData WavePeaks { get; set; }

        public const double ZoomMinimum = 0.1;
        public const double ZoomMaximum = 2.5;
        private double _zoomFactor = 1.0; // 1.0=no zoom
        private long _lastMouseWheelScroll = -1;
        private Subtitle _subtitle = new Subtitle();
        private readonly List<Paragraph> _displayableParagraphs = new List<Paragraph>();
        private readonly List<Paragraph> _allSelectedParagraphs = new List<Paragraph>();
        public Paragraph SelectedParagraph { get; private set; }

        public bool ShowGridLines { get; set; } = true;

        private double _currentVideoPositionSeconds = -1;

        public event PositionEventHandler OnVideoPositionChanged;

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
                    InvalidateSurface();
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
                    InvalidateSurface();
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
                    var endPositionSeconds = value + (double)Width / WavePeaks.SampleRate / _zoomFactor;
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
                    InvalidateSurface();
                }
            }
        }

        private readonly SKPaint _paintBackground = new SKPaint
        {
            Color = SKColor.Parse("#2effffff"),
            StrokeWidth = 1,
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
        };

        // left edge
        private readonly SKPaint _paintLeft = new SKPaint
        {
            Color = SKColor.Parse("#ff99ff99"),
            StrokeWidth = 1,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
        };

        private readonly SKPaint _paintRight = new SKPaint
        {
            Color = SKColor.Parse("#ffff0000"),
            StrokeWidth = 1,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
        };

        private readonly SKPaint _paintText = new SKPaint
        {
            Color = SKColors.WhiteSmoke,
            StrokeWidth = 1,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            TextSize = 14,
        };

        readonly PointerGestureRecognizer _pointerGestureRecognizer = new PointerGestureRecognizer();

        public AudioVisualizer()
        {
            GestureRecognizers.Add(new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
            });

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
            GestureRecognizers.Add(tapGestureRecognizer);

            _pointerGestureRecognizer = new PointerGestureRecognizer();

            _pointerGestureRecognizer.PointerMoved += (s, e) =>
            {
                // Handle the pointer moved event
            };

            //TODO: test mpv player with _canvas.Handle
        }
        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();
#if WINDOWS
            var view = Handler.PlatformView as SkiaSharp.Views.Windows.SKXamlCanvas;
            view.PointerWheelChanged += (s, e) =>
            {
                var point = e.GetCurrentPoint(s as Microsoft.Maui.Platform.ContentPanel);
                var delta = point.Properties.MouseWheelDelta;
                var positionSeconds = _currentVideoPositionSeconds + delta / 80.0;
                OnVideoPositionChanged?.Invoke(this, new PositionEventArgs { PositionInSeconds = positionSeconds });
            };
#endif
        }

        private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
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
            OnVideoPositionChanged?.Invoke(this, new PositionEventArgs { PositionInSeconds = positionInSeconds });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double RelativeXPositionToSeconds(double x)
        {
            return _startPositionSeconds + x / WavePeaks.SampleRate / _zoomFactor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double RelativeXPositionToSeconds(int x)
        {
            return _startPositionSeconds + (double)x / WavePeaks.SampleRate / _zoomFactor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int SecondsToXPosition(double seconds)
        {
            return (int)Math.Round(seconds * WavePeaks.SampleRate * _zoomFactor, MidpointRounding.AwayFromZero);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int SecondsToSampleIndex(double seconds)
        {
            return (int)Math.Round(seconds * WavePeaks.SampleRate, MidpointRounding.AwayFromZero);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double SampleIndexToSeconds(int index)
        {
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

                return RelativeXPositionToSeconds(Width);
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
            if (IsVisible)
            {
                try
                {
                    InvalidateSurface();
                }
                catch
                {
                    // ignore
                }
            }
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

                foreach (int index in selectedIndexes)
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

            e.Surface.Canvas.Flush();
        }

        private void DrawParagraphs(SKPaintSurfaceEventArgs e)
        {
            var startPositionMilliseconds = _startPositionSeconds * 1000.0;
            var endPositionMilliseconds = RelativeXPositionToSeconds(Width) * 1000.0;
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

            if (currentRegionWidth <= 1)
            {
                return;
            }

            if (_info.Height < 1)
            {
                return;
            }

            _canvas.DrawRect(currentRegionLeft, 0, currentRegionWidth, _info.Height, _paintBackground);
            _canvas.DrawLine(currentRegionLeft, 0, currentRegionLeft, (float)Height, _paintLeft);
            _canvas.DrawLine(currentRegionRight - 1, 0, currentRegionRight - 1, (float)Height, _paintRight);

            var n = _zoomFactor * WavePeaks.SampleRate;

            var text = HtmlUtil.RemoveHtmlTags(paragraph.Text, true);
            if (Configuration.Settings.VideoControls.WaveformUnwrapText)
            {
                text = text.Replace(Environment.NewLine, "  ");
            }

            var bounds = new SKRect();
            var x = _paintText.MeasureText(text, ref bounds);
            _canvas.DrawText(text, currentRegionLeft + 3, 14, _paintText);
        }

        private void DrawCurrentVideoPosition()
        {
            // current video position
            var currentPositionPos = SecondsToXPosition(_currentVideoPositionSeconds - _startPositionSeconds);
            if (_currentVideoPositionSeconds > 0 && currentPositionPos > 0 && currentPositionPos < Width)
            {
                using var paintStyle = new SKPaint
                {
                    Color = SKColors.Cyan,
                    StrokeWidth = 1,
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                };
                _canvas.DrawLine(currentPositionPos, 0, currentPositionPos, (float)Height, paintStyle);
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
            Color = SKColor.Parse("#ff88ffff"),
            StrokeWidth = 1,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
        };

        private void DrawWaveForm()
        {
            if (WavePeaks == null)
            {
                return;
            }

            var _showWaveform = true;
            if (_showWaveform)
            {

                var waveformHeight = _info.Height;
                //  var isSelectedHelper = new IsSelectedHelper(_allSelectedParagraphs, _wavePeaks.SampleRate);
                var baseHeight = (int)(WavePeaks.HighestPeak / _verticalZoomFactor);
                var halfWaveformHeight = waveformHeight / 2;

                var div = WavePeaks.SampleRate * _zoomFactor;
                for (var x = 0; x < Width; x++)
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
                    //var pen = isSelectedHelper.IsSelected(pos0) ? penSelected : penNormal;
                    _canvas.DrawLine(x, yMax, x, yMin, _paintWaveform);
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

        private readonly SKPaint _paintGridColor = new SKPaint
        {
            Color = SKColor.Parse("#ff222222"),
            StrokeWidth = 1,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
        };

        private void DrawGridLines()
        {
            if (!ShowGridLines)
            {
                return;
            }

            if (WavePeaks == null)
            {
                for (var i = 0; i < Width; i += 10)
                {
                    _canvas.DrawLine(i, 0, i, (float)Height, _paintGridColor);
                    _canvas.DrawLine(0, i, (float)Width, i, _paintGridColor);
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

                while (xPosition < Width)
                {
                    _canvas.DrawLine(xPosition, 0, xPosition, (float)Height, _paintGridColor);

                    seconds += interval;
                    xPosition = SecondsToXPosition(seconds);
                }

                while (yPosition < Height)
                {
                    _canvas.DrawLine(0, yPosition, (float)Width, yPosition, _paintGridColor);

                    yCounter += interval;
                    yPosition = Convert.ToInt32(yCounter * WavePeaks.SampleRate * _zoomFactor);
                }
            }
        }
    }
}

