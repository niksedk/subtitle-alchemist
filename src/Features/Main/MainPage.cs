using CommunityToolkit.Maui.Views;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Features.Main;

public class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;

    public MainPage(MainViewModel vm)
    {
        _viewModel = vm;

        var themeName = Nikse.SubtitleEdit.Core.Common.Configuration.Settings.General.UseDarkTheme
            ? "Dark"
            : "Light";
        ICollection<ResourceDictionary>? mergedDictionaries = Application.Current?.Resources?.MergedDictionaries;
        if (mergedDictionaries != null)
        {
            var theme = new ResourceDictionary
            {
                { ThemeNames.BackgroundColor, Colors.Pink },
                { ThemeNames.TextColor, Colors.Pink },
                { ThemeNames.SecondaryBackgroundColor, Colors.Pink },
                { ThemeNames.BorderColor, Colors.Pink },
                { ThemeNames.ActiveBackgroundColor, Colors.Pink },
                { ThemeNames.LinkColor, Colors.Pink },
                { ThemeNames.TableHeaderBackgroundColor, Colors.Pink },
            };
            mergedDictionaries.Add(theme);

        }
        ThemeHelper.UpdateTheme(themeName);
        Resources.Add(ThemeHelper.GetGridSelectionStyle());

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

        MakeLayout(Se.Settings.General.LayoutNumber); 

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
