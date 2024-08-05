using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace SubtitleAlchemist.Controls.UpDownControl
{
    public class UpDownView : SKCanvasView
    {
        private const float MinValue = 0f;
        private const float MaxValue = 100f;
        private const int ButtonsWidth = 13;

        private bool _focused;
        private bool _focusedUpArrow;
        private bool _focusedDownArrow;

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
        }

        private void PointerMoved(object? sender, PointerEventArgs e)
        {
            var point = e.GetPosition(this);
            if (point == null)
            {
                return;
            }

            var x = (int)Math.Round(point.Value.X, MidpointRounding.AwayFromZero);
            var y = (int)Math.Round(point.Value.Y, MidpointRounding.AwayFromZero);

            if (_focused)
            {
                //if (x > Width - ButtonsWidth)
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
                //else
                //{
                //    _focusedUpArrow = false;
                //    _focusedDownArrow = false;
                //}
            }
            else
            {
                _focusedUpArrow = false;
                _focusedDownArrow = false;
            }
        }

        private void PointerPressed(object? sender, PointerEventArgs e)
        {

        }

        private void PointerReleased(object? sender, PointerEventArgs e)
        {

        }

        private void PointerEntered(object? sender, PointerEventArgs e)
        {
            _focused = true;
        }

        private void PointerExited(object? sender, PointerEventArgs e)
        {
            _focused = false;
            _focusedUpArrow = false;
            _focusedDownArrow = false;
            InvalidateSurface();
        }

        /// <summary>
        ///     Identifies the Value bindable property.
        /// </summary>
        public static readonly BindableProperty ValueProperty
            = BindableProperty.Create(
                nameof(Value),
                typeof(float),
                typeof(UpDownView),
                0.0f,
                propertyChanged: OnValueChanged);

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

        /// <summary>
        ///     Called when the surface needs to be painted.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);

            var canvas = e.Surface.Canvas;
            canvas.Clear();

            float width = e.Info.Width;
            float height = e.Info.Height;
            float size = Math.Min(width, height);

            //float centerX = width / 2;
            //float centerY = height / 2;

            //float scale = size / 210f;

            //canvas.Translate(centerX, centerY);
            //canvas.Scale(scale);

            DrawBackground(canvas, size);
            DrawUpDownArrows(canvas, width, height);
        }

        /// <summary>
        ///     Draws the background of the UpDownView.
        /// </summary>
        /// <param name="canvas">The canvas to draw on.</param>
        /// <param name="size">The size of the canvas.</param>
        private static void DrawBackground(SKCanvas canvas, float size)
        {
            canvas.DrawRect(new SKRect(-size / 2, -size / 2, size / 2, size / 2),
                new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = SKColors.Transparent, //TODO:  use background color?
                });
        }

        /// <summary>
        ///     Draws the up and down arrows.
        /// </summary>
        /// <param name="canvas">The canvas to draw on.</param>
        private void DrawUpDownArrows(SKCanvas canvas, float width, float height)
        {
            var left = width - ButtonsWidth;
            var h = (int)height / 2 - 4;
            var top = 2;
            var paintUp = _focusedUpArrow ? _arrowUpPaintOver : _arrowUpPaint;
            ArrowDrawer.DrawArrowUp(canvas, paintUp, left, top, h);

            var downTop = h + 5;
            var paintDown = _focusedDownArrow ? _arrowDownPaintOver : _arrowDownPaint;
            ArrowDrawer.DrawArrowDown(canvas, paintDown, left, downTop, h);
        }

        /// <summary>
        ///     Called when the value property changes.
        /// </summary>
        /// <param name="bindable">The bindable object.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static async void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            //if (bindable is UpDownView view)
            //{
            //   await view.AnimateNeedleAsync((float)newValue);
            //}
        }
    }
}
