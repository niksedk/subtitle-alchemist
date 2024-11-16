using SubtitleAlchemist.Features.Main;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Se.LoadSettings();

            MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);
            MainPageModel.Window = window;
            return window;
        }
    }
}
