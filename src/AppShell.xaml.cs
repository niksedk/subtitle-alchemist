using SubtitleAlchemist.Features.Edit.RedoUndoHistory;
using SubtitleAlchemist.Features.Files;
using SubtitleAlchemist.Features.Files.ExportBinary.EbuExport;
using SubtitleAlchemist.Features.Files.ExportImage;
using SubtitleAlchemist.Features.Main;
using SubtitleAlchemist.Features.Options.Settings;
using SubtitleAlchemist.Features.Shared.Ocr;
using SubtitleAlchemist.Features.SpellCheck;
using SubtitleAlchemist.Features.Sync.AdjustAllTimes;
using SubtitleAlchemist.Features.Tools.AdjustDuration;
using SubtitleAlchemist.Features.Tools.BatchConvert;
using SubtitleAlchemist.Features.Tools.ChangeCasing;
using SubtitleAlchemist.Features.Tools.FixCommonErrors;
using SubtitleAlchemist.Features.Tools.RemoveTextForHearingImpaired;
using SubtitleAlchemist.Features.Translate;
using SubtitleAlchemist.Features.Video.AudioToTextWhisper;
using SubtitleAlchemist.Features.Video.BurnIn;
using SubtitleAlchemist.Features.Video.TextToSpeech;
using SubtitleAlchemist.Features.Video.TransparentSubtitles;

namespace SubtitleAlchemist
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            var types = new[]
            {
                typeof(AdjustAllTimesPage),
                typeof(AdjustDurationPage),
                typeof(AudioToTextWhisperPage),
                typeof(BatchConvertPage),
                typeof(BurnInPage),
                typeof(ChangeCasingPage),
                typeof(EditInterjectionsPage),
                typeof(ExportEbuPage),
                typeof(ExportImagePage),
                typeof(FixCommonErrorsPage),
                typeof(FixNamesPage),
                typeof(MainPage),
                typeof(NOcrCharacterAddPage),
                typeof(NOcrCharacterInspectPage),
                typeof(NOcrDbEditPage),
                typeof(OcrPage),
                typeof(RestoreAutoBackupPage),
                typeof(RemoveTextForHiPage),
                typeof(ReviewSpeechPage),
                typeof(SettingsPage),
                typeof(SpellCheckerPage),
                typeof(TextToSpeechPage),
                typeof(TranslatePage),
                typeof(TransparentSubPage),
                typeof(UndoRedoHistoryPage),
                typeof(WhisperAdvancedPage),
            };

            foreach (var type in types)
            {
                Routing.RegisterRoute(type.Name, type);
            }
        }
    }
}
