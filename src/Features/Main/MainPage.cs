using CommunityToolkit.Maui.Views;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Features.Main;

public class MainPage : ContentPage
{
    private readonly MainPageModel _pageModel;

    public MainPage(MainPageModel vm)
    {
        _pageModel = vm;

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
                { ThemeNames.ProgressColor, Colors.Orange },
                { ThemeNames.ErrorTextColor, Colors.Red },
            };
            mergedDictionaries.Add(theme);

        }
        ThemeHelper.UpdateTheme(themeName);
        Resources.Add(ThemeHelper.GetGridSelectionStyle());

        BindingContext = _pageModel;
        _pageModel.MainPage = this;
        this.BindDynamicTheme();

        InitMenuBar.CreateMenuBar(this, _pageModel);

        _pageModel.VideoPlayer = new MediaElement
        {
            ZIndex = -10000,
            Margin = new Thickness(10),
        }.BindDynamicTheme();

        _pageModel.SubtitleListBorder = InitSubtitleListView.MakeSubtitleListView(_pageModel);
        _pageModel.ListViewAndEditBox = new Grid();

        MakeLayout(Se.Settings.General.LayoutNumber); 

        Loaded += OnLoaded!;
    }

    protected override void OnDisappearing()
    {
        _pageModel.Stop();
        base.OnDisappearing();
    }

    public void MakeLayout(int layoutNumber)
    {
        InitLayout.MakeLayout(this, _pageModel, layoutNumber);
    }

    private async void OnLoaded(object s, EventArgs e)
    {
        _pageModel.Loaded(this);
        _pageModel.Start();
        await SharpHookHandler.RunAsync();
    }
}
