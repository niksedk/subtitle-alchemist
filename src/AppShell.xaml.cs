using SubtitleAlchemist.Views.Options.Settings;
using SubtitleAlchemist.Views.Translate;
using SubtitleAlchemist.Views.Video.AudioToTextWhisper;

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
        }
    }
}
