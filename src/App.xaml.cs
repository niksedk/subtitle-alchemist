using SubtitleAlchemist.Views.Main;

namespace SubtitleAlchemist
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);
            MainViewModel.Window = window;
            return window;
        }
    }
}
