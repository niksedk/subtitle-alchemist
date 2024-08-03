using SubtitleAlchemist.Features.Options.Settings;
using SubtitleAlchemist.Features.Tools.AdjustDuration;
using SubtitleAlchemist.Features.Translate;
using SubtitleAlchemist.Features.Video.AudioToTextWhisper;

namespace SubtitleAlchemist
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(AudioToTextWhisperPage), typeof(AudioToTextWhisperPage));
            Routing.RegisterRoute(nameof(TranslatePage), typeof(TranslatePage));
            Routing.RegisterRoute(nameof(AdjustDurationPage), typeof(AdjustDurationPage));
        }
    }
}
