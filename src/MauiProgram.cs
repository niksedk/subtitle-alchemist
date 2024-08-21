using System.Text;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using SubtitleAlchemist.Controls.ColorPickerControl;
using SubtitleAlchemist.Controls.PickerControl;
using SubtitleAlchemist.Features.Help.About;
using SubtitleAlchemist.Features.LayoutPicker;
using SubtitleAlchemist.Features.Main;
using SubtitleAlchemist.Features.Options.DownloadFfmpeg;
using SubtitleAlchemist.Features.Options.Settings;
using SubtitleAlchemist.Features.Tools.AdjustDuration;
using SubtitleAlchemist.Features.Translate;
using SubtitleAlchemist.Features.Video.AudioToTextWhisper;
using SubtitleAlchemist.Services;

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
                .UseSkiaSharp()
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
            builder.Services.AddTransient<AdjustDurationPage>();
            builder.Services.AddTransient<AdjustDurationModel>();

            builder.Services.AddTransientPopup<AboutPopup, AboutModel>();
            builder.Services.AddTransientPopup<LayoutPickerPopup, LayoutPickerModel>();
            builder.Services.AddTransientPopup<DownloadFfmpegPopup, DownloadFfmpegModel>();
            builder.Services.AddTransientPopup<ColorPickerPopup, ColorPickerPopupModel>();
            builder.Services.AddTransientPopup<PickerPopup, PickerPopupModel>();

            builder.Services.AddHttpClient<IFfmpegDownloadService, FfmpegDownloadService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

//            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            return builder.Build();
        }
    }
}
