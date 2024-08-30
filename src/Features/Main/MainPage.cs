using CommunityToolkit.Maui.Views;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Features.Main;

public class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;

    public MainPage(MainViewModel vm)
    {
        _viewModel = vm;

        ICollection<ResourceDictionary>? mergedDictionaries = Application.Current?.Resources?.MergedDictionaries;
        if (mergedDictionaries != null)
        {
            //  mergedDictionaries.Clear(); //TODO: remove default styles!?
            if (Nikse.SubtitleEdit.Core.Common.Configuration.Settings.General.UseDarkTheme)
            {
                var darkTheme = new ResourceDictionary
                {
                    { ThemeNames.BackgroundColor, Color.FromRgb(0x1F, 0x1F, 0x1F) },
                    { ThemeNames.TextColor, Colors.WhiteSmoke },
                    { ThemeNames.SecondaryBackgroundColor, Color.FromRgb(20,20,20) },
                    { ThemeNames.BorderColor, Colors.DarkGray },
                    { ThemeNames.ActiveBackgroundColor, Colors.DarkGreen },
                    { ThemeNames.LinkColor, Colors.LightSkyBlue },
                };
                mergedDictionaries.Add(darkTheme);
            }
            else
            {
                var lightTheme = new ResourceDictionary
                {
                    { ThemeNames.BackgroundColor, Color.FromRgb(240,240,240) },
                    { ThemeNames.TextColor, Colors.Black },
                    { ThemeNames.SecondaryBackgroundColor, Color.FromRgb(253,253,253) },
                    { ThemeNames.BorderColor, Colors.DarkGray },
                    { ThemeNames.ActiveBackgroundColor, Colors.LightGreen },
                    { ThemeNames.LinkColor, Colors.DarkBlue },
                };
                mergedDictionaries.Add(lightTheme);
            }
        }

        BindingContext = _viewModel;
        _viewModel.MainPage = this;
        this.BindDynamicTheme();

        InitMenuBar.CreateMenuBar(this, _viewModel);

        _viewModel.VideoPlayer = new MediaElement
        {
            ZIndex = -10000,
            Margin = new Thickness(10),
        }.BindDynamicTheme();

        _viewModel.SubtitleList = InitSubtitleListView.MakeSubtitleListView(_viewModel);
        _viewModel.ListViewAndEditBox = new Grid();

        MakeLayout(0); // TODO: use settings to determine layout

        Loaded += OnLoaded!;
    }

    protected override void OnDisappearing()
    {
        _viewModel.Stop();
        base.OnDisappearing();
    }

    public void MakeLayout(int layoutNumber)
    {
        InitLayout.MakeLayout(this, _viewModel, layoutNumber);
    }

    private async void OnLoaded(object s, EventArgs e)
    {
        _viewModel.Loaded(this);
        _viewModel.Start();
        await SharpHookHandler.RunAsync();
    }
}
