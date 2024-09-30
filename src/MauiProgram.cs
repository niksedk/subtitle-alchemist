using System.Text;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using SubtitleAlchemist.Controls.ColorPickerControl;
using SubtitleAlchemist.Controls.PickerControl;
using SubtitleAlchemist.Controls.RadialControl;
using SubtitleAlchemist.Features.Edit.Find;
using SubtitleAlchemist.Features.Edit.GoToLineNumber;
using SubtitleAlchemist.Features.Edit.RedoUndoHistory;
using SubtitleAlchemist.Features.Edit.Replace;
using SubtitleAlchemist.Features.Files;
using SubtitleAlchemist.Features.Files.ExportBinary.Cavena890Export;
using SubtitleAlchemist.Features.Files.ExportBinary.EbuExport;
using SubtitleAlchemist.Features.Files.ExportBinary.PacExport;
using SubtitleAlchemist.Features.Help.About;
using SubtitleAlchemist.Features.LayoutPicker;
using SubtitleAlchemist.Features.Main;
using SubtitleAlchemist.Features.Options.DownloadFfmpeg;
using SubtitleAlchemist.Features.Options.Settings;
using SubtitleAlchemist.Features.SpellCheck;
using SubtitleAlchemist.Features.Sync.AdjustAllTimes;
using SubtitleAlchemist.Features.Sync.ChangeFrameRate;
using SubtitleAlchemist.Features.Sync.ChangeSpeed;
using SubtitleAlchemist.Features.Tools.AdjustDuration;
using SubtitleAlchemist.Features.Tools.FixCommonErrors;
using SubtitleAlchemist.Features.Translate;
using SubtitleAlchemist.Features.Video.AudioToTextWhisper;
using SubtitleAlchemist.Features.Video.AudioToTextWhisper.Download;
using SubtitleAlchemist.Features.Video.BurnIn;
using SubtitleAlchemist.Features.Video.OpenFromUrl;
using SubtitleAlchemist.Features.Video.TextToSpeech;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Dictionaries;
using SubtitleAlchemist.Logic.Media;
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
                    fonts.AddFont("RobotoMono-Regular.ttf", "RobotoMono");
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
            builder.Services.AddTransient<SpellCheckerPage>();
            builder.Services.AddTransient<SpellCheckerPageModel>();
            builder.Services.AddTransient<RestoreAutoBackupPage>();
            builder.Services.AddTransient<RestoreAutoBackupModel>();
            builder.Services.AddTransient<ExportEbuPage>();
            builder.Services.AddTransient<ExportEbuModel>();
            builder.Services.AddTransient<UndoRedoHistoryPage>();
            builder.Services.AddTransient<UndoRedoHistoryPageModel>();
            builder.Services.AddTransient<AdjustAllTimesPage>();
            builder.Services.AddTransient<AdjustAllTimesPageModel>();
            builder.Services.AddTransient<BurnInPage>();
            builder.Services.AddTransient<BurnInPageModel>();
            builder.Services.AddTransient<TextToSpeechPage>();
            builder.Services.AddTransient<TextToSpeechPageModel>();
            builder.Services.AddTransient<TaskbarList>();
            builder.Services.AddTransient<ISpellCheckManager, SpellCheckManager>();
            builder.Services.AddTransient<INamesList, SeNamesList>();
            builder.Services.AddTransient<IAutoBackup, AutoBackup>();
            builder.Services.AddTransient<IFileHelper, FileHelper>();
            builder.Services.AddTransient<IZipUnpacker, ZipUnpacker>();
            builder.Services.AddTransient<IUndoRedoManager, UndoRedoManager>();
            builder.Services.AddTransient<IFindService, FindService>();
            builder.Services.AddTransient<IInsertManager, InsertManager>();

            builder.Services.AddTransientPopup<AboutPopup, AboutPopupModel>();
            builder.Services.AddTransientPopup<LayoutPickerPopup, LayoutPickerModel>();
            builder.Services.AddTransientPopup<DownloadFfmpegPopup, DownloadFfmpegModel>();
            builder.Services.AddTransientPopup<ColorPickerPopup, ColorPickerPopupModel>();
            builder.Services.AddTransientPopup<PickerPopup, PickerPopupModel>();
            builder.Services.AddTransientPopup<TranslateAdvancedSettingsPopup, TranslateAdvancedSettingsPopupModel>();
            builder.Services.AddTransientPopup<RadialPopup, RadialPopupModel>();
            builder.Services.AddTransientPopup<DownloadWhisperPopup, DownloadWhisperPopupModel>();
            builder.Services.AddTransientPopup<DownloadWhisperModelPopup, DownloadWhisperModelPopupModel>();
            builder.Services.AddTransientPopup<WhisperPostProcessingPopup, WhisperPostProcessingPopupModel>();
            builder.Services.AddTransientPopup<WhisperAdvancedHistoryPopup, WhisperAdvancedHistoryPopupModel>();
            builder.Services.AddTransientPopup<FixCommonErrorsProfilePopup, FixCommonErrorsProfilePopupModel>();
            builder.Services.AddTransientPopup<GoToLineNumberPopup, GoToLineNumberPopupModel>();
            builder.Services.AddTransientPopup<ExportCavena890Popup, ExportCavena890PopupModel>();
            builder.Services.AddTransientPopup<ExportPacPopup, ExportPacPopupModel>();
            builder.Services.AddTransientPopup<GetDictionaryPopup, GetDictionaryPopupModel>();
            builder.Services.AddTransientPopup<FindPopup, FindPopupModel>();
            builder.Services.AddTransientPopup<ReplacePopup, ReplacePopupModel>();
            builder.Services.AddTransientPopup<OpenFromUrlPopup, OpenFromUrlPopupModel>();
            builder.Services.AddTransientPopup<ChangeFrameRatePopup, ChangeFrameRatePopupModel>();
            builder.Services.AddTransientPopup<ChangeSpeedPopup, ChangeSpeedPopupModel>();
            builder.Services.AddTransientPopup<EditCurrentTextPopup, EditCurrentTextPopupModel>();

            builder.Services.AddHttpClient<IFfmpegDownloadService, FfmpegDownloadService>();
            builder.Services.AddHttpClient<IWhisperDownloadService, WhisperDownloadService>();
            builder.Services.AddHttpClient<ISpellCheckDictionaryDownloadService, SpellCheckDictionaryDownloadService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            return builder.Build();
        }
    }
}
