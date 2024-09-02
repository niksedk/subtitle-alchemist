using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;
using MainViewModel = SubtitleAlchemist.Features.Main.MainViewModel;

namespace SubtitleAlchemist
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            SeSettings.LoadSettings();

            MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);
            MainViewModel.Window = window;
            return window;
        }
    }
}
