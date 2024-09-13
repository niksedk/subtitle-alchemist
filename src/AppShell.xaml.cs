using SubtitleAlchemist.Features.Main;
using SubtitleAlchemist.Features.Options.Settings;
using SubtitleAlchemist.Features.SpellCheck;
using SubtitleAlchemist.Features.Tools.AdjustDuration;
using SubtitleAlchemist.Features.Tools.FixCommonErrors;
using SubtitleAlchemist.Features.Translate;
using SubtitleAlchemist.Features.Video.AudioToTextWhisper;

namespace SubtitleAlchemist
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            //Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(AudioToTextWhisperPage), typeof(AudioToTextWhisperPage));
            Routing.RegisterRoute(nameof(WhisperAdvancedPage), typeof(WhisperAdvancedPage));
            Routing.RegisterRoute(nameof(TranslatePage), typeof(TranslatePage));
            Routing.RegisterRoute(nameof(AdjustDurationPage), typeof(AdjustDurationPage));
            Routing.RegisterRoute(nameof(FixCommonErrorsPage), typeof(FixCommonErrorsPage));
            Routing.RegisterRoute(nameof(SpellCheckerPage), typeof(SpellCheckerPage));
        }
    }
}
