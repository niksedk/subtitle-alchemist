using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubtitleAlchemist.Controls.ColorPickerControl;
using SubtitleAlchemist.Features.Options.DownloadFfmpeg;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Logic.Constants;
using SubtitleAlchemist.Logic.Media;
using System.Collections.ObjectModel;
using static SubtitleAlchemist.Features.Options.Settings.SettingsPage;

namespace SubtitleAlchemist.Features.Options.Settings;

public partial class SettingsViewModel : ObservableObject
{
    public List<SettingItem> AllSettings { get; init; } = new();

    [ObservableProperty]
    private string _searchText;

    public VerticalStackLayout LeftMenu { get; set; }
    public CollectionView SettingList { get; set; }
    public SettingsPage? SettingsPage { get; set; }
    public BoxView SyntaxErrorColorBox { get; set; }

    [ObservableProperty] private string _ffmpegPath = string.Empty;

    [ObservableProperty] private string _theme = "Dark";
    [ObservableProperty] private bool _showRecentFiles;

    [ObservableProperty]
    private ObservableCollection<string> _themes = new() { "Light", "Dark", "Custom" };

    private readonly IPopupService _popupService;
    private SectionName _sectionName = SectionName.General;

    public SettingsViewModel(IPopupService popupService)
    {
        _popupService = popupService;

        LeftMenu = new VerticalStackLayout();
        SyntaxErrorColorBox = new BoxView();

        AllSettings = new List<SettingItem>();

        LoadSettings();
    }

    public void LeftMenuTapped(object? sender, TappedEventArgs e, SectionName sectionName)
    {
        _sectionName = sectionName;

        foreach (var child in LeftMenu.Children)
        {
            if (child is Label label)
            {
                if (label.ClassId == sectionName.ToString())
                {
                    label.BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.ActiveBackgroundColor];
                }
                else
                {
                    label.BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.BackgroundColor];
                }
            }
        }

        var settingItem = AllSettings.FirstOrDefault(x => x.SectionName == sectionName);
        SettingList.ScrollTo(settingItem, null, ScrollToPosition.Start);

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
        Theme = Se.Settings.General.Theme;
        FfmpegPath = Se.Settings.General.FfmpegPath;
        ShowRecentFiles = Se.Settings.File.ShowRecentFiles;
    }

    public void SaveSettings()
    {
        Se.Settings.General.FfmpegPath = FfmpegPath;
        Se.Settings.General.Theme = Theme;
        Se.Settings.File.ShowRecentFiles = ShowRecentFiles;

        Se.SaveSettings();
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
        var answer = await SettingsPage!.DisplayAlert(
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

    public async Task ThemeChanged(object? sender, EventArgs eventArgs)
    {
        if (sender is not Picker picker)
        {
            return;
        }

        var value = picker.SelectedItem?.ToString();
        if (value == null)
        {
            return;
        }

        Theme = value;

        Preferences.Set("Theme", Theme);

        ThemeHelper.UpdateTheme(Theme);

        LeftMenuTapped(null, new TappedEventArgs(null), _sectionName);
    }

    public void OnAppearing()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            LeftMenuTapped(null, new TappedEventArgs(null), SectionName.General);
        });
    }

    public void SearchButtonPressed(object? sender, EventArgs e)
    {
        if (SettingsPage == null)
        {
            return;
        }

        SettingsPage.BatchBegin();

        SettingItem? lastCategory = null;
        SettingItem? lastSubCategory = null;
        foreach (var setting in AllSettings)
        {
            if (setting.Type == SettingItemType.Category)
            {
                lastCategory = setting;
                lastSubCategory = null;
            }
            else if (setting.Type == SettingItemType.SubCategory)
            {
                lastSubCategory = setting;
            }

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                setting.WholeView.IsVisible = true;
            }
            else if (setting.Text.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
            {
                setting.WholeView.IsVisible = true;

                if (lastCategory != null)
                {
                    lastCategory.WholeView.IsVisible = true;
                }
                if (lastSubCategory != null)
                {
                    lastSubCategory.WholeView.IsVisible = true;
                }
            }
            else
            {
                setting.WholeView.IsVisible = false;
            }
        }

        SettingsPage.BatchCommit();
    }

    public void SearchBarTextChanged(object? sender, TextChangedEventArgs e)
    {
        SearchButtonPressed(sender, e);
    }
}