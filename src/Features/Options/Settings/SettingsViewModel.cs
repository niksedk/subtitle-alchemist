using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Controls.ColorPickerControl;
using SubtitleAlchemist.Features.Options.DownloadFfmpeg;
using SubtitleAlchemist.Logic.Media;
using static SubtitleAlchemist.Features.Options.Settings.SettingsPage;

namespace SubtitleAlchemist.Features.Options.Settings;

public partial class SettingsViewModel : ObservableObject
{
    public Dictionary<PageNames, View> Pages { get; set; }
    public Border Page { get; set; }
    public VerticalStackLayout LeftMenu { get; set; }
    public SettingsPage SettingsPage { get; set; }
    public BoxView SyntaxErrorColorBox { get; set; }

    [ObservableProperty]
    private string _theme;

    [ObservableProperty]
    private string _ffmpegPath;

    private readonly IPopupService _popupService;

    public SettingsViewModel(IPopupService popupService)
    {
        _popupService = popupService;

        Pages = new Dictionary<PageNames, View>();
        Page = new Border();
        LeftMenu = new VerticalStackLayout();
        SyntaxErrorColorBox = new BoxView();

        Theme = "Dark";
        _ffmpegPath = string.Empty;

        LoadSettings();
    }

    public async Task LeftMenuTapped(object? sender, TappedEventArgs e, PageNames pageName)
    {
        if (Page.Content != null)
        {
            await Page.Content.FadeTo(0, 200);
        }

        Pages[pageName].Opacity = 0;
        Page.Content = Pages[pageName];

        foreach (var child in LeftMenu.Children)
        {
            if (child is Label label)
            {
                if (label.ClassId == pageName.ToString())
                {
                    label.TextColor = Colors.BlueViolet;
                }
                else
                {
                    label.TextColor = (Color)Application.Current.Resources["TextColor"];
                }
            }
        }

        await Page.Content.FadeTo(1, 200);
    }

    [RelayCommand]
    public async Task PickSyntaxErrorColor()
    {
        var result = await _popupService.ShowPopupAsync<ColorPickerPopupModel>(
                onPresenting: vm => vm.SetCurrentColor(SyntaxErrorColorBox.Color),
                CancellationToken.None);

        if (result is Color color)
        {
            SyntaxErrorColorBox.Color = color;
        }
    }

    public void LoadSettings()
    {
        Theme = Configuration.Settings.General.UseDarkTheme ? "Dark" : "Light";
        FfmpegPath = Configuration.Settings.General.FFmpegLocation;
    }

    public void SaveSettings()
    {
        Configuration.Settings.General.FFmpegLocation = FfmpegPath;
        Configuration.Settings.General.UseDarkTheme = Theme == "Dark";
    }

    public async Task BrowseForFfmpeg(object? sender, EventArgs eventArgs)
    {
        var fileHelper = new FileHelper();
        var subtitleFileName = await fileHelper.PickFfmpeg("Pick ffmpeg");
        if (string.IsNullOrEmpty(subtitleFileName))
        {
            return;
        }

        FfmpegPath = subtitleFileName;
    }

    public async Task DownloadFfmpeg(object? sender, EventArgs eventArgs)
    {
        var answer = await SettingsPage.DisplayAlert(
            "Download ffmpeg?",
            "Download and use ffmpeg?",
            "Yes",
            "No");

        if (!answer)
        {
            return;
        }

        var result = await _popupService.ShowPopupAsync<DownloadFfmpegModel>(CancellationToken.None);
        if (result is string ffmpegFileName)
        {
            FfmpegPath = ffmpegFileName;
        }
    }
}