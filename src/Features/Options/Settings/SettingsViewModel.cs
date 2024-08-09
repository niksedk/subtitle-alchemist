using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Features.Options.DownloadFfmpeg;
using SubtitleAlchemist.Logic.Media;
using static SubtitleAlchemist.Features.Options.Settings.SettingsPage;

namespace SubtitleAlchemist.Features.Options.Settings;

public partial class SettingsViewModel : ObservableObject
{
    public string Theme { get; set; }
    public Dictionary<PageNames, View> Pages { get; set; }
    public Border Page { get; set; }
    public VerticalStackLayout LeftMenu { get; set; }
    public SettingsPage SettingsPage { get; set; }

    [ObservableProperty] 
    private string _ffmpegPath;

    private readonly IPopupService _popupService;

    public SettingsViewModel(IPopupService popupService)
    {
        _popupService = popupService;

        Pages = new Dictionary<PageNames, View>();
        Page = new Border();
        LeftMenu = new VerticalStackLayout();
        Theme = "Dark";

        _ffmpegPath = string.Empty;

        LoadSettings();
    }

    public async Task Tapped(object? sender, TappedEventArgs e, PageNames pageName)
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

    public void LoadSettings()
    {
        FfmpegPath = Nikse.SubtitleEdit.Core.Common.Configuration.Settings.General.FFmpegLocation;
    }

    public void SaveSettings()
    {
        Nikse.SubtitleEdit.Core.Common.Configuration.Settings.General.FFmpegLocation = _ffmpegPath;
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
        if (Configuration.IsRunningOnWindows)
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
        }
    }
}