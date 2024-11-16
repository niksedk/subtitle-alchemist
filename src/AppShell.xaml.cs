using SubtitleAlchemist.Features.Edit.RedoUndoHistory;
using SubtitleAlchemist.Features.Files;
using SubtitleAlchemist.Features.Files.ExportBinary.EbuExport;
using SubtitleAlchemist.Features.Main;
using SubtitleAlchemist.Features.Options.Settings;
using SubtitleAlchemist.Features.SpellCheck;
using SubtitleAlchemist.Features.Sync.AdjustAllTimes;
using SubtitleAlchemist.Features.Tools.AdjustDuration;
using SubtitleAlchemist.Features.Tools.BatchConvert;
using SubtitleAlchemist.Features.Tools.ChangeCasing;
using SubtitleAlchemist.Features.Tools.FixCommonErrors;
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

            AddRoutes(
                typeof(MainPage),
                typeof(SettingsPage),
                typeof(AudioToTextWhisperPage),
                typeof(WhisperAdvancedPage),
                typeof(TranslatePage),
                typeof(AdjustDurationPage),
                typeof(FixCommonErrorsPage),
                typeof(SpellCheckerPage),
                typeof(RestoreAutoBackupPage),
                typeof(ExportEbuPage),
                typeof(UndoRedoHistoryPage),
                typeof(AdjustAllTimesPage),
                typeof(BurnInPage),
                typeof(TransparentSubPage),
                typeof(TextToSpeechPage),
                typeof(ReviewSpeechPage),
                typeof(BatchConvertPage),
                typeof(ChangeCasingPage),
                typeof(FixNamesPage)
            );
        }

        private static void AddRoutes(params Type[] types)
        {
            foreach (var type in types)
            {
                Routing.RegisterRoute(type.Name, type);
            }
        }
    }
}
