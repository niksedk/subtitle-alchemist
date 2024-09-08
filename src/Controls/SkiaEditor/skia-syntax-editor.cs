using System.Text.RegularExpressions;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace SubtitleAlchemist.Controls.SkiaEditor
{
    public class SkiaSyntaxEditor : SKCanvasView
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(SkiaSyntaxEditor), "", BindingMode.TwoWay);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private SKPaint _textPaint;
        private SKPaint _backgroundPaint;
        private float _fontSize = 14;
        private float _lineHeight;
        private List<(string text, SKColor color)> _coloredTokens;

        public SkiaSyntaxEditor()
        {
            _textPaint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                TextSize = _fontSize,
                Typeface = SKTypeface.FromFamilyName("Courier New")
            };

            _backgroundPaint = new SKPaint
            {
                Color = SKColors.White,
                Style = SKPaintStyle.Fill
            };

            _lineHeight = _textPaint.FontSpacing * 1.2f;

            EnableTouchEvents = true;
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear(SKColors.White);

            if (string.IsNullOrEmpty(Text))
                return;

            _coloredTokens = TokenizeAndColorize(Text);

            float y = _lineHeight;
            foreach (var (token, color) in _coloredTokens)
            {
                _textPaint.Color = color;
                canvas.DrawText(token, 10, y, _textPaint);
                y += _lineHeight;
            }
        }

        private List<(string, SKColor)> TokenizeAndColorize(string text)
        {
            var tokens = new List<(string, SKColor)>();
            var lines = text.Split('\n');

            foreach (var line in lines)
            {
                // Simple syntax highlighting for C#
                var keywordPattern = @"\b(public|private|class|static|void|int|string|var)\b";
                var stringPattern = @""".*?""";
                var commentPattern = @"//.*";

                var parts = Regex.Split(line, $"({keywordPattern}|{stringPattern}|{commentPattern})");

                foreach (var part in parts)
                {
                    if (Regex.IsMatch(part, keywordPattern))
                        tokens.Add((part, SKColors.Blue));
                    else if (Regex.IsMatch(part, stringPattern))
                        tokens.Add((part, SKColors.Brown));
                    else if (Regex.IsMatch(part, commentPattern))
                        tokens.Add((part, SKColors.Green));
                    else
                        tokens.Add((part, SKColors.Black));
                }

                tokens.Add(("\n", SKColors.Black));
            }

            return tokens;
        }

        protected override void OnTouch(SKTouchEventArgs e)
        {
            // Handle touch events for cursor movement and text selection
            // This is a placeholder for more complex text editing logic
            base.OnTouch(e);
        }
    }
}
