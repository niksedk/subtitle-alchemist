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
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());
            MainPageModel.Window = window;
            return window;
        }
    }
}
