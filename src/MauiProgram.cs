using System.Text;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Plugin.Maui.Audio;
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
using SubtitleAlchemist.Features.Files.ExportImage;
using SubtitleAlchemist.Features.Help.About;
using SubtitleAlchemist.Features.Main;
using SubtitleAlchemist.Features.Main.LayoutPicker;
using SubtitleAlchemist.Features.Options.DownloadFfmpeg;
using SubtitleAlchemist.Features.Options.Settings;
using SubtitleAlchemist.Features.Shared.Ocr;
using SubtitleAlchemist.Features.Shared.PickSubtitleLine;
using SubtitleAlchemist.Features.Shared.PickVideoPosition;
using SubtitleAlchemist.Features.SpellCheck;
using SubtitleAlchemist.Features.Sync.AdjustAllTimes;
using SubtitleAlchemist.Features.Sync.ChangeFrameRate;
using SubtitleAlchemist.Features.Sync.ChangeSpeed;
using SubtitleAlchemist.Features.Tools.AdjustDuration;
using SubtitleAlchemist.Features.Tools.BatchConvert;
using SubtitleAlchemist.Features.Tools.FixCommonErrors;
using SubtitleAlchemist.Features.Translate;
using SubtitleAlchemist.Features.Video.AudioToTextWhisper;
using SubtitleAlchemist.Features.Video.AudioToTextWhisper.Download;
using SubtitleAlchemist.Features.Video.BurnIn;
using SubtitleAlchemist.Features.Video.OpenFromUrl;
using SubtitleAlchemist.Features.Video.TextToSpeech;
using SubtitleAlchemist.Features.Video.TransparentSubtitles;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Dictionaries;
using SubtitleAlchemist.Logic.Media;
using SubtitleAlchemist.Services;
using SubtitleAlchemist.Features.Video.TextToSpeech.DownloadTts;
using SubtitleAlchemist.Features.Tools.ChangeCasing;
using SubtitleAlchemist.Features.Shared.PickMatroskaTrack;
using SubtitleAlchemist.Features.Tools.RemoveTextForHearingImpaired;

namespace SubtitleAlchemist;

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
            .AddAudio()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("RobotoMono-Regular.ttf", "RobotoMono");
            });

        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<MainPageModel>();
        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddTransient<SettingsPageModel>();
        builder.Services.AddTransient<AudioToTextWhisperPage>();
        builder.Services.AddTransient<AudioToTextWhisperModel>();
        builder.Services.AddTransient<TranslatePage>();
        builder.Services.AddTransient<TranslatePageModel>();
        builder.Services.AddTransient<AdjustDurationPage>();
        builder.Services.AddTransient<AdjustDurationPageModel>();
        builder.Services.AddTransient<FixCommonErrorsPage>();
        builder.Services.AddTransient<FixCommonErrorsPageModel>();
        builder.Services.AddTransient<WhisperAdvancedPage>();
        builder.Services.AddTransient<WhisperAdvancedPageModel>();
        builder.Services.AddTransient<SpellCheckerPage>();
        builder.Services.AddTransient<SpellCheckerPageModel>();
        builder.Services.AddTransient<RestoreAutoBackupPage>();
        builder.Services.AddTransient<RestoreAutoBackupPageModel>();
        builder.Services.AddTransient<ExportEbuPage>();
        builder.Services.AddTransient<ExportEbuPageModel>();
        builder.Services.AddTransient<UndoRedoHistoryPage>();
        builder.Services.AddTransient<UndoRedoHistoryPageModel>();
        builder.Services.AddTransient<AdjustAllTimesPage>();
        builder.Services.AddTransient<AdjustAllTimesPageModel>();
        builder.Services.AddTransient<BurnInPage>();
        builder.Services.AddTransient<BurnInPageModel>();
        builder.Services.AddTransient<BurnInPage>();
        builder.Services.AddTransient<BurnInPageModel>();
        builder.Services.AddTransient<TransparentSubPage>();
        builder.Services.AddTransient<TransparentSubPageModel>();
        builder.Services.AddTransient<TextToSpeechPage>();
        builder.Services.AddTransient<TextToSpeechPageModel>();
        builder.Services.AddTransient<ReviewSpeechPage>();
        builder.Services.AddTransient<ReviewSpeechPageModel>();
        builder.Services.AddTransient<BatchConvertPage>();
        builder.Services.AddTransient<BatchConvertPageModel>();
        builder.Services.AddTransient<ChangeCasingPage>();
        builder.Services.AddTransient<ChangeCasingPageModel>();
        builder.Services.AddTransient<FixNamesPage>();
        builder.Services.AddTransient<FixNamesPageModel>();
        builder.Services.AddTransient<NOcrCharacterAddPage>();
        builder.Services.AddTransient<NOcrCharacterAddPageModel>();
        builder.Services.AddTransient<NOcrCharacterInspectPage>();
        builder.Services.AddTransient<NOcrCharacterInspectPageModel>();
        builder.Services.AddTransient<OcrPage>();
        builder.Services.AddTransient<OcrPageModel>();
        builder.Services.AddTransient<NOcrDbEditPage>();
        builder.Services.AddTransient<NOcrDbEditPageModel>();
        builder.Services.AddTransient<ExportImagePage>();
        builder.Services.AddTransient<ExportImagePageModel>();
        builder.Services.AddTransient<RemoveTextForHiPage>();
        builder.Services.AddTransient<RemoveTextForHiPageModel>();

        builder.Services.AddTransient<TaskbarList>();
        builder.Services.AddTransient<ISpellCheckManager, SpellCheckManager>();
        builder.Services.AddTransient<INamesList, SeNamesList>();
        builder.Services.AddTransient<IAutoBackup, AutoBackup>();
        builder.Services.AddTransient<IFileHelper, FileHelper>();
        builder.Services.AddTransient<IZipUnpacker, ZipUnpacker>();
        builder.Services.AddTransient<IUndoRedoManager, UndoRedoManager>();
        builder.Services.AddTransient<IFindService, FindService>();
        builder.Services.AddTransient<IInsertManager, InsertManager>();
        builder.Services.AddTransient<IMergeManager, MergeManager>();
        builder.Services.AddTransient<IShortcutManager, ShortcutManager>();
        builder.Services.AddTransient<IMainShortcutActions, MainShortcutActions>();
        builder.Services.AddTransient<IBatchConverter, BatchConverter>();
        builder.Services.AddTransient<INOcrCaseFixer, NOcrCaseFixer>();

        builder.Services.AddTransientPopup<AboutPopup, AboutPopupModel>();
        builder.Services.AddTransientPopup<LayoutPickerPopup, LayoutPickerPopupModel>();
        builder.Services.AddTransientPopup<DownloadFfmpegPopup, DownloadFfmpegPopupModel>();
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
        builder.Services.AddTransientPopup<OutputPropertiesPopup, OutputPropertiesPopupModel>();
        builder.Services.AddTransientPopup<ResolutionPopup, ResolutionPopupModel>();
        builder.Services.AddTransientPopup<PickSubtitleLinePopup, PickSubtitleLinePopupModel>();
        builder.Services.AddTransientPopup<PickVideoPositionPopup, PickVideoPositionPopupModel>();
        builder.Services.AddTransientPopup<DownloadTtsPopup, DownloadTtsPopupModel>();
        builder.Services.AddTransientPopup<AudioSettingsPopup, AudioSettingsPopupModel>();
        builder.Services.AddTransientPopup<EditTextPopup, EditTextPopupModel>();
        builder.Services.AddTransientPopup<ElevenLabSettingsPopup, ElevenLabSettingsPopupModel>();
        builder.Services.AddTransientPopup<EditShortcutPopup, EditShortcutPopupModel>();
        builder.Services.AddTransientPopup<BatchConvertOutputPropertiesPopup, BatchConvertOutputPropertiesPopupModel>();
        builder.Services.AddTransientPopup<NOcrDbActionPopup, NOcrDbActionPopupModel>();
        builder.Services.AddTransientPopup<TesseractDownloadPopup, TesseractDownloadPopupModel>();
        builder.Services.AddTransientPopup<TesseractDictionaryDownloadPopup, TesseractDictionaryDownloadPopupModel>();
        builder.Services.AddTransientPopup<PickMatroskaTrackPopup, PickMatroskaTrackPopupModel>();

        builder.Services.AddHttpClient<IFfmpegDownloadService, FfmpegDownloadService>();
        builder.Services.AddHttpClient<IWhisperDownloadService, WhisperDownloadService>();
        builder.Services.AddHttpClient<ISpellCheckDictionaryDownloadService, SpellCheckDictionaryDownloadService>();
        builder.Services.AddHttpClient<ITtsDownloadService, TtsDownloadService>();
        builder.Services.AddHttpClient<ITesseractDownloadService, TesseractDownloadService>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        return builder.Build();
    }
}