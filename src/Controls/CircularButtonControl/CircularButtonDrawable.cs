using System.Reflection;

namespace SubtitleAlchemist.Controls.CircularButtonControl;

public class CircularButtonDrawable : IDrawable
{
    public Color StrokeColor { get; set; } = Colors.DarkGrey;

    public bool AreShadowsEnabled { get; set; } = true;

    /// <summary>
    /// A string containing the Image name.
    /// </summary>
    public string Image { get; set; } = string.Empty;

    public int Width { get; set; } = 0;
    public int Height { get; set; } = 0;

    public Color ButtonColor { get; set; } = Colors.White;

    public bool SetInvisible { get; set; } = false;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (SetInvisible)
        {
            return;
        }

        canvas.StrokeColor = StrokeColor;

        var width = Width != 0 ? Width : dirtyRect.Width;
        var height = Height != 0 ? Height : dirtyRect.Height;

        var limitingDim = width > height ? height : width;
        var centerOfCircle = new PointF(width / 2, height / 2);
        canvas.FillColor = this.ButtonColor;
        canvas.FillCircle(centerOfCircle, limitingDim / 2);

#if WINDOWS
        canvas.FillColor = this.ButtonColor;
        canvas.FillCircle(centerOfCircle, limitingDim / 2);
#elif ANDROID || IOS
        var assembly = GetType().GetTypeInfo().Assembly;
        using var stream = assembly.GetManifestResourceStream("ShoppingList.Resources.Images." + Image);
        var image = Microsoft.Maui.Graphics.Platform.PlatformImage.FromStream(stream);
        if (image is null)
        {
            throw new FileNotFoundException("ShoppingList.Resources.Images." + Image);
        }
        canvas.DrawImage(image, dirtyRect.X + dirtyRect.Width / 4, dirtyRect.Y + dirtyRect.Height / 4, dirtyRect.Width / 2, dirtyRect.Height / 2);

#endif
    }
}