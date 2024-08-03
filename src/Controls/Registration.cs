using SkiaSharp.Views.Maui.Handlers;

namespace SubtitleAlchemist.Controls;

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