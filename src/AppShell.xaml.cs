using SubtitleAlchemist.Features.Edit.RedoUndoHistory;
using SubtitleAlchemist.Features.Files;
using SubtitleAlchemist.Features.Files.ExportBinary.EbuExport;
using SubtitleAlchemist.Features.Options.Settings;
using SubtitleAlchemist.Features.SpellCheck;
using SubtitleAlchemist.Features.Sync.AdjustAllTimes;
using SubtitleAlchemist.Features.Tools.AdjustDuration;
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

            //Routing.RegisterRoute(nameof(MainPage), typeof(MainPage)); -- defined in xml... not sure how to remove from xml...
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(AudioToTextWhisperPage), typeof(AudioToTextWhisperPage));
            Routing.RegisterRoute(nameof(WhisperAdvancedPage), typeof(WhisperAdvancedPage));
            Routing.RegisterRoute(nameof(TranslatePage), typeof(TranslatePage));
            Routing.RegisterRoute(nameof(AdjustDurationPage), typeof(AdjustDurationPage));
            Routing.RegisterRoute(nameof(FixCommonErrorsPage), typeof(FixCommonErrorsPage));
            Routing.RegisterRoute(nameof(SpellCheckerPage), typeof(SpellCheckerPage));
            Routing.RegisterRoute(nameof(RestoreAutoBackupPage), typeof(RestoreAutoBackupPage));
            Routing.RegisterRoute(nameof(ExportEbuPage), typeof(ExportEbuPage));
            Routing.RegisterRoute(nameof(UndoRedoHistoryPage), typeof(UndoRedoHistoryPage));
            Routing.RegisterRoute(nameof(AdjustAllTimesPage), typeof(AdjustAllTimesPage));
            Routing.RegisterRoute(nameof(BurnInPage), typeof(BurnInPage));
            Routing.RegisterRoute(nameof(TransparentSubPage), typeof(TransparentSubPage));
            Routing.RegisterRoute(nameof(TextToSpeechPage), typeof(TextToSpeechPage));
        }
    }
}
