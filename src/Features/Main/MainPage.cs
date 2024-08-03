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
            if (true)
            {
                var darkTheme = new ResourceDictionary
                {
                    { ThemeNames.BackgroundColor, Color.FromRgb(0x1F, 0x1F, 0x1F) },
                    { ThemeNames.TextColor, Colors.WhiteSmoke },
                };
                mergedDictionaries.Add(darkTheme);
            }
            else
            {
                var lightTheme = new ResourceDictionary
                {
                    { ThemeNames.BackgroundColor, Colors.White },
                    { ThemeNames.TextColor, Colors.Black }
                };
                mergedDictionaries.Add(lightTheme);
            }
        }

        BindingContext = _viewModel;
        _viewModel.MainPage = this;

        InitMenuBar.CreateMenuBar(this, _viewModel);

        _viewModel.VideoPlayer = new MediaElement
        {
            ZIndex = -10000,
            BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"],
        };

        _viewModel.SubtitleList = InitSubtitleListView.MakeSubtitleListView(_viewModel);
        _viewModel.ListViewAndEditBox = new Grid();

        MakeLayout(0); // TODO: use settings to determine layout

        Unloaded += OnUnloaded!;
        Loaded += OnLoaded!;

        SharpHookHandler.AddKeyPressed(vm.KeyPressed);
    }

    public void MakeLayout(int layoutNumber)
    {
        InitLayout.MakeLayout(this, _viewModel, layoutNumber);
    }

    private async void OnLoaded(object s, EventArgs e)
    {
        _viewModel.Loaded(this);
        await SharpHookHandler.RunAsync();
    }

    private void OnUnloaded(object s, EventArgs e)
    {
        _viewModel.CleanUp();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        // TODO: start timer + keyboard listener?

        base.OnNavigatedTo(args);
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        // TODO: stop timer + keyboard listener?
        base.OnNavigatedFrom(args);
    }
}
