using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using Microsoft.Extensions.Logging;
using SubtitleAlchemist.Controls;
using SubtitleAlchemist.Services;
using SubtitleAlchemist.Views.Help.About;
using SubtitleAlchemist.Views.LayoutPicker;
using SubtitleAlchemist.Views.Main;
using SubtitleAlchemist.Views.Options.Settings;
using SubtitleAlchemist.Views.Translate;
using SubtitleAlchemist.Views.Video.AudioToTextWhisper;

namespace SubtitleAlchemist
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitMediaElement()
                .UseMauiCommunityToolkitMarkup()
                .UseAudioVisualizer()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<AudioToTextWhisperPage>();
            builder.Services.AddTransient<AudioToTextWhisperModel>();
            builder.Services.AddTransient<TranslatePage>();
            builder.Services.AddTransient<TranslateModel>();

            builder.Services.AddTransientPopup<AboutPopup, AboutModel>();
            builder.Services.AddTransientPopup<LayoutPickerPopup, LayoutPickerModel>();

            builder.Services.AddHttpClient<IFfmpegDownloadService, FfmpegDownloadService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
