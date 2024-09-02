using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using SubtitleAlchemist.Controls.ColorPickerControl;
using SubtitleAlchemist.Controls.PickerControl;
using SubtitleAlchemist.Controls.RadialControl;
using SubtitleAlchemist.Features.Help.About;
using SubtitleAlchemist.Features.LayoutPicker;
using SubtitleAlchemist.Features.Main;
using SubtitleAlchemist.Features.Options.DownloadFfmpeg;
using SubtitleAlchemist.Features.Options.Settings;
using SubtitleAlchemist.Features.Tools.AdjustDuration;
using SubtitleAlchemist.Features.Tools.FixCommonErrors;
using SubtitleAlchemist.Features.Translate;
using SubtitleAlchemist.Features.Video.AudioToTextWhisper;
using SubtitleAlchemist.Features.Video.AudioToTextWhisper.Download;
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
            builder.Services.AddTransient<FixCommonErrorsPage>();
            builder.Services.AddTransient<FixCommonErrorsModel>();
            builder.Services.AddTransient<WhisperAdvancedPage>();
            builder.Services.AddTransient<WhisperAdvancedModel>();

            builder.Services.AddTransientPopup<AboutPopup, AboutModel>();
            builder.Services.AddTransientPopup<LayoutPickerPopup, LayoutPickerModel>();
            builder.Services.AddTransientPopup<DownloadFfmpegPopup, DownloadFfmpegModel>();
            builder.Services.AddTransientPopup<ColorPickerPopup, ColorPickerPopupModel>();
            builder.Services.AddTransientPopup<PickerPopup, PickerPopupModel>();
            builder.Services.AddTransientPopup<TranslateAdvancedSettingsPopup, TranslateAdvancedSettingsPopupModel>();
            builder.Services.AddTransientPopup<RadialPopup, RadialPopupModel>();
            builder.Services.AddTransientPopup<DownloadWhisperPopup, DownloadWhisperPopupModel>();
            builder.Services.AddTransientPopup<DownloadWhisperModelPopup, DownloadWhisperModelPopupModel>();
            builder.Services.AddTransientPopup<WhisperPostProcessingPopup, WhisperPostProcessingPopupModel>();

            builder.Services.AddHttpClient<IFfmpegDownloadService, FfmpegDownloadService>();
            builder.Services.AddHttpClient<IWhisperDownloadService, WhisperDownloadService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            //            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            return builder.Build();
        }
    }
}
