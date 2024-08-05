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

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpDownView"/> class.
        /// </summary>
        public UpDownView()
        {
            WidthRequest = 25;
            HeightRequest = 25;
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
            set => SetValue(TextColorProperty, value);
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
            DrawNeedArrows(canvas, width, height);
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
                    Color = SKColors.Transparent,
                });
        }

        /// <summary>
        ///     Draws the up and down arrows.
        /// </summary>
        /// <param name="canvas">The canvas to draw on.</param>
        private static void DrawNeedArrows(SKCanvas canvas, float width, float height)
        {
            var left =  width - ButtonsWidth;
            var h = (int)height / 2 - 4;
            var top = 2;
            ArrowDrawer.DrawArrowUp(canvas, new SKPaint { Color = SKColors.Black }, left, top, h);

            top = h + 5;
            ArrowDrawer.DrawArrowDown(canvas, new SKPaint { Color = SKColors.Black }, left, top, h);
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
