using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.SubtitleFormats;
using SubtitleAlchemist.Controls.ColorPickerControl;
using SubtitleAlchemist.Features.Options.DownloadFfmpeg;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Logic.Constants;
using SubtitleAlchemist.Logic.Media;
using System.Collections.ObjectModel;

namespace SubtitleAlchemist.Features.Options.Settings;

public partial class SettingsPageModel : ObservableObject
{
    [ObservableProperty]
    public partial ObservableCollection<SettingItem> AllSettings { get; set; }

    [ObservableProperty]
    public partial string SearchText { get; set; }
    public VerticalStackLayout LeftMenu { get; set; }
    public Grid SettingList { get; set; }
    public SettingsPage? Page { get; set; }
    public BoxView SyntaxErrorColorBox { get; set; }
    public ScrollView SettingListScrollView { get; set; }
    public Grid GridShortcutsGeneral { get; set; }
    public SettingItem SettingsItemShortcutsGeneral { get; set; }

    [ObservableProperty]
    public partial string FfmpegPath { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool ShowRecentFiles { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<string> Themes { get; set; } = new() { "Light", "Dark", "Custom" };

    [ObservableProperty]
    public partial string Theme { get; set; } = "Dark";

    [ObservableProperty]
    public partial int SubtitleLineMaximumLength { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<int> MaxNumberOfLines { get; set; }

    [ObservableProperty]
    public partial int SelectedMaxNumberOfLines { get; set; }

    [ObservableProperty]
    public partial int MaxNumberOfLinesPlusAbort { get; set; }

    [ObservableProperty]
    public partial int MergeLinesShorterThan { get; set; }

    [ObservableProperty]
    public partial int SubtitleMinimumDisplayMilliseconds { get; set; }

    [ObservableProperty]
    public partial int SubtitleMaximumDisplayMilliseconds { get; set; }

    [ObservableProperty]
    public partial int MinimumMillisecondsBetweenLines { get; set; }

    [ObservableProperty]
    public partial double SubtitleMaximumCharactersPerSeconds { get; set; }

    [ObservableProperty]
    public partial double SubtitleOptimalCharactersPerSeconds { get; set; }

    [ObservableProperty]
    public partial double SubtitleMaximumWordsPerMinute { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<DialogStyleDisplay> DialogStyles { get; set; }

    [ObservableProperty]
    public partial DialogStyleDisplay SelectedDialogStyle { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<ContinuationStyleDisplay> ContinuationStyles { get; set; }

    [ObservableProperty]
    public partial ContinuationStyleDisplay SelectedContinuationStyle { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<CpsLineLengthDisplay> CpsLineLengthStrategies { get; set; }

    [ObservableProperty]
    public partial CpsLineLengthDisplay SelectedCpsLineLengthStrategy { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<string> DefaultFrameRates { get; set; }

    [ObservableProperty]
    public partial string SelectedDefaultFrameRate { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<string> DefaultFileEncodings { get; set; }

    [ObservableProperty]
    public partial string SelectedDefaultFileEncoding { get; set; }

    [ObservableProperty]
    public partial bool AutodetectAnsiEncoding { get; set; }

    [ObservableProperty]
    public partial string LanguageFiltersDisplay { get; set; }

    [ObservableProperty]
    public partial bool PromptForDeleteLines { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<TimeCodeModeDisplay> TimeCodeModes { get; set; }

    [ObservableProperty]
    public partial TimeCodeModeDisplay SelectedTimeCodeMode { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<SplitBehaviorDisplay> SplitBehaviors { get; set; }

    [ObservableProperty]
    public partial SplitBehaviorDisplay SelectedSplitBehavior { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<SubtitleListDoubleClickActionDisplay> SubtitleListDoubleClickActions { get; set; }

    [ObservableProperty]
    public partial SubtitleListDoubleClickActionDisplay SelectedSubtitleListDoubleClickAction { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<AutoBackupIntervalDisplay> AutoBackupIntervals { get; set; }

    [ObservableProperty]
    public partial AutoBackupIntervalDisplay SelectedAutoBackupInterval { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<AutoBackupDeleteDisplay> AutoBackupDeleteOptions { get; set; }

    [ObservableProperty]
    public partial AutoBackupDeleteDisplay SelectedAutoBackupDeleteOption { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<string> SubtitleFormats { get; set; }

    [ObservableProperty]
    public partial string SelectedDefaultSubtitleFormat { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<string> SubtitleSaveAsSubtitleFormats { get; set; }

    [ObservableProperty]
    public partial string SelectedDefaultSaveAsSubtitleFormat { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<string> FavoriteSubtitleFormats { get; set; }


    // Shortcuts
    public List<ShortcutDisplay> Shortcuts;


    // Syntax coloring
    [ObservableProperty]
    public partial bool ColorDurationTooShort { get; set; }

    [ObservableProperty]
    public partial bool ColorDurationTooLong { get; set; }

    [ObservableProperty]
    public partial bool ColorTextTooLong { get; set; }

    [ObservableProperty]
    public partial bool ColorTextTooWide { get; set; }

    [ObservableProperty]
    public partial bool ColorTextTooManyLines { get; set; }

    [ObservableProperty]
    public partial bool ColorTimeCodeOverlap { get; set; }

    [ObservableProperty]
    public partial bool ColorGapTooShort { get; set; }

    [ObservableProperty]
    public partial Color ColorErrorColor { get; set; }


    // Toolbar
    [ObservableProperty]
    public partial bool ToolbarShowFileNew { get; set; }

    [ObservableProperty]
    public partial bool ToolbarShowFileOpen { get; set; }

    [ObservableProperty]
    public partial bool ToolbarShowVideoFileOpen { get; set; }

    [ObservableProperty]
    public partial bool ToolbarShowSave { get; set; }

    [ObservableProperty]
    public partial bool ToolbarShowSaveAs { get; set; }

    [ObservableProperty]
    public partial bool ToolbarShowFind { get; set; }

    [ObservableProperty]
    public partial bool ToolbarShowReplace { get; set; }

    [ObservableProperty]
    public partial bool ToolbarShowFixCommonErrors { get; set; }

    [ObservableProperty]
    public partial bool ToolbarShowSpellCheck { get; set; }

    [ObservableProperty]
    public partial bool ToolbarShowHelp { get; set; }

    private readonly IPopupService _popupService;
    private SettingsPage.SectionName _sectionName = SettingsPage.SectionName.General;

    private bool _loaded;

    public SettingsPageModel(IPopupService popupService)
    {
        _popupService = popupService;
        SearchText = string.Empty;
        LeftMenu = new VerticalStackLayout();
        SyntaxErrorColorBox = new BoxView();
        SettingList = new Grid();
        SettingListScrollView = new ScrollView();
        AllSettings = new ObservableCollection<SettingItem>();
        MaxNumberOfLines = new ObservableCollection<int>(Enumerable.Range(1, 5));
        SelectedMaxNumberOfLines = 2;
        DialogStyles = new ObservableCollection<DialogStyleDisplay>(DialogStyleDisplay.GetDialogStyles());
        SelectedDialogStyle = DialogStyles.First();
        ContinuationStyles = new ObservableCollection<ContinuationStyleDisplay>(ContinuationStyleDisplay.GetDialogStyles());
        SelectedContinuationStyle = ContinuationStyles.First();
        CpsLineLengthStrategies = new ObservableCollection<CpsLineLengthDisplay>(CpsLineLengthDisplay.GetCpsLineLengthStrategies());
        SelectedCpsLineLengthStrategy = CpsLineLengthStrategies.First();
        DefaultFrameRates = new ObservableCollection<string>(new List<string> { "23.976", "24", "25", "29.97", "30", "50", "59.94", "60" });
        SelectedDefaultFrameRate = DefaultFrameRates.First();
        LanguageFiltersDisplay = "All";
        DefaultFileEncodings = new ObservableCollection<string>(EncodingHelper.GetEncodings().Select(p => p.DisplayName).ToList());
        SelectedDefaultFileEncoding = DefaultFileEncodings.First();
        TimeCodeModes = new ObservableCollection<TimeCodeModeDisplay>(TimeCodeModeDisplay.GetTimeCodeModes());
        SelectedTimeCodeMode = TimeCodeModes.First();
        SplitBehaviors = new ObservableCollection<SplitBehaviorDisplay>(SplitBehaviorDisplay.GetSplitBehaviors());
        SelectedSplitBehavior = SplitBehaviors.First();
        SubtitleListDoubleClickActions = new ObservableCollection<SubtitleListDoubleClickActionDisplay>(SubtitleListDoubleClickActionDisplay.GetSubtitleListDoubleClickActions());
        SelectedSubtitleListDoubleClickAction = SubtitleListDoubleClickActions.First();
        AutoBackupIntervals = new ObservableCollection<AutoBackupIntervalDisplay>(AutoBackupIntervalDisplay.GetAutoBackupIntervals());
        SelectedAutoBackupInterval = AutoBackupIntervals.First();
        AutoBackupDeleteOptions = new ObservableCollection<AutoBackupDeleteDisplay>(AutoBackupDeleteDisplay.GetAutoBackupDeleteOptions());
        SelectedAutoBackupDeleteOption = AutoBackupDeleteOptions.First();

        var allSubtitleFormats = SubtitleFormat.AllSubtitleFormats.Select(p => p.Name).ToList();
        SubtitleFormats = new ObservableCollection<string>(allSubtitleFormats);
        SelectedDefaultSubtitleFormat = SubtitleFormats.First();

        var allSaveAsSubtitleFormats = new List<string>(allSubtitleFormats);
        allSaveAsSubtitleFormats.Insert(0, "- Auto - ");
        SubtitleSaveAsSubtitleFormats = new ObservableCollection<string>(allSaveAsSubtitleFormats);
        SelectedDefaultSaveAsSubtitleFormat = SubtitleSaveAsSubtitleFormats.First();
        FavoriteSubtitleFormats = new ObservableCollection<string>(allSubtitleFormats.Take(5));

        Shortcuts = ShortcutDisplay.GetShortcuts();

        GridShortcutsGeneral = new Grid();
        SettingsItemShortcutsGeneral = new SettingItem("Shortcuts", SettingsPage.SectionName.Shortcuts);
        ColorErrorColor = Colors.Red;
    }

    public void LeftMenuTapped(object? sender, TappedEventArgs e, SettingsPage.SectionName sectionName)
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

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var settingItem = AllSettings.FirstOrDefault(x => x.SectionName == sectionName);
            if (settingItem != null)
            {
                await SettingListScrollView.ScrollToAsync(settingItem.WholeView, ScrollToPosition.Start, true);
            }
        });
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
        FfmpegPath = Se.Settings.General.FfmpegPath;
        SubtitleLineMaximumLength = Se.Settings.General.SubtitleLineMaximumLength;
        SelectedMaxNumberOfLines = Se.Settings.General.MaxNumberOfLines;
        MaxNumberOfLinesPlusAbort = Se.Settings.General.MaxNumberOfLinesPlusAbort;
        MergeLinesShorterThan = Se.Settings.General.MergeLinesShorterThan;
        SubtitleMinimumDisplayMilliseconds = Se.Settings.General.SubtitleMinimumDisplayMilliseconds;
        SubtitleMaximumDisplayMilliseconds = Se.Settings.General.SubtitleMaximumDisplayMilliseconds;
        MinimumMillisecondsBetweenLines = Se.Settings.General.MinimumMillisecondsBetweenLines;
        SubtitleMaximumCharactersPerSeconds = Se.Settings.General.SubtitleMaximumCharactersPerSeconds;
        SubtitleOptimalCharactersPerSeconds = Se.Settings.General.SubtitleOptimalCharactersPerSeconds;
        SubtitleMaximumWordsPerMinute = Se.Settings.General.SubtitleMaximumWordsPerMinute;

        Theme = Se.Settings.Appearance.Theme;
        ToolbarShowFileNew = Se.Settings.Appearance.ToolbarShowFileNew;
        ToolbarShowFileOpen = Se.Settings.Appearance.ToolbarShowFileOpen;
        ToolbarShowVideoFileOpen = Se.Settings.Appearance.ToolbarShowVideoFileOpen;
        ToolbarShowSave = Se.Settings.Appearance.ToolbarShowSave;
        ToolbarShowSaveAs = Se.Settings.Appearance.ToolbarShowSaveAs;
        ToolbarShowFind = Se.Settings.Appearance.ToolbarShowFind;
        ToolbarShowReplace = Se.Settings.Appearance.ToolbarShowReplace;
        ToolbarShowFixCommonErrors = Se.Settings.Appearance.ToolbarShowFixCommonErrors;
        ToolbarShowSpellCheck = Se.Settings.Appearance.ToolbarShowSpellCheck;
        ToolbarShowHelp = Se.Settings.Appearance.ToolbarShowHelp;

        ShowRecentFiles = Se.Settings.File.ShowRecentFiles;
    }

    public void SaveSettings()
    {
        Se.Settings.General.FfmpegPath = FfmpegPath;
        Se.Settings.File.ShowRecentFiles = ShowRecentFiles;
        Se.Settings.General.SubtitleLineMaximumLength = SubtitleLineMaximumLength;
        Se.Settings.General.MaxNumberOfLines = SelectedMaxNumberOfLines;
        Se.Settings.General.MaxNumberOfLinesPlusAbort = MaxNumberOfLinesPlusAbort;
        Se.Settings.General.MergeLinesShorterThan = MergeLinesShorterThan;
        Se.Settings.General.SubtitleMinimumDisplayMilliseconds = SubtitleMinimumDisplayMilliseconds;
        Se.Settings.General.SubtitleMaximumDisplayMilliseconds = SubtitleMaximumDisplayMilliseconds;
        Se.Settings.General.MinimumMillisecondsBetweenLines = MinimumMillisecondsBetweenLines;
        Se.Settings.General.SubtitleMaximumCharactersPerSeconds = SubtitleMaximumCharactersPerSeconds;
        Se.Settings.General.SubtitleOptimalCharactersPerSeconds = SubtitleOptimalCharactersPerSeconds;
        Se.Settings.General.SubtitleMaximumWordsPerMinute = SubtitleMaximumWordsPerMinute;
        Se.Settings.General.ColorDurationTooShort = ColorDurationTooShort;
        Se.Settings.General.ColorDurationTooLong = ColorDurationTooLong;
        Se.Settings.General.ColorTextTooLong = ColorTextTooLong;
        Se.Settings.General.ColorTextTooWide = ColorTextTooWide;
        Se.Settings.General.ColorTextTooManyLines = ColorTextTooManyLines;
        Se.Settings.General.ColorTimeCodeOverlap = ColorTimeCodeOverlap;
        Se.Settings.General.ColorGapTooShort = ColorGapTooShort;

        Se.Settings.Appearance.Theme = Theme;
        Se.Settings.Appearance.ToolbarShowFileNew = ToolbarShowFileNew;
        Se.Settings.Appearance.ToolbarShowFileOpen = ToolbarShowFileOpen;
        Se.Settings.Appearance.ToolbarShowVideoFileOpen = ToolbarShowVideoFileOpen;
        Se.Settings.Appearance.ToolbarShowSave = ToolbarShowSave;
        Se.Settings.Appearance.ToolbarShowSaveAs = ToolbarShowSaveAs;
        Se.Settings.Appearance.ToolbarShowFind = ToolbarShowFind;
        Se.Settings.Appearance.ToolbarShowReplace = ToolbarShowReplace;
        Se.Settings.Appearance.ToolbarShowFixCommonErrors = ToolbarShowFixCommonErrors;
        Se.Settings.Appearance.ToolbarShowSpellCheck = ToolbarShowSpellCheck;
        Se.Settings.Appearance.ToolbarShowHelp = ToolbarShowHelp;

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
        var answer = await Page!.DisplayAlert(
            "Download ffmpeg?",
            "Download and use ffmpeg?",
            "Yes",
            "No");

        if (!answer)
        {
            return;
        }

        var result = await _popupService.ShowPopupAsync<DownloadFfmpegPopupModel>(CancellationToken.None);
        if (result is string ffmpegFileName)
        {
            FfmpegPath = ffmpegFileName;
        }
    }

    public void ThemeChanged(object? sender, EventArgs eventArgs)
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
        Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            if (_loaded)
            {
                return false;
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                LeftMenuTapped(null, new TappedEventArgs(null), SettingsPage.SectionName.General);
                LoadSettings();
            });

            _loaded = true;
            return false;
        });
    }

    public void SearchButtonPressed(object? sender, EventArgs e)
    {
        if (Page == null)
        {
            return;
        }

        Page.BatchBegin();

        SettingItem? lastCategory = null;
        SettingItem? lastSubCategory = null;
        var showNextFooter = false;
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
            else if (setting.Type == SettingItemType.Footer && showNextFooter)
            {
                setting.Show();
                showNextFooter = false;
                continue;
            }

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                setting.Show();
            }
            else if (setting.Text.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
            {
                setting.Show();

                if (lastCategory != null)
                {
                    lastCategory.WholeView.IsVisible = true;
                    showNextFooter = true;
                }
                if (lastSubCategory != null)
                {
                    lastSubCategory.WholeView.IsVisible = true;
                }
            }
            else
            {
                setting.Hide();
            }
        }

        Page.BatchCommit();
    }

    public void SearchBarTextChanged(object? sender, TextChangedEventArgs e)
    {
        SearchButtonPressed(sender, e);
    }

    public void RemoveFavoriteSubtitleFormat(string favorite)
    {

    }

    public void MoveFavoriteSubtitleFormatUp(string favorite)
    {

    }

    public void MoveFavoriteSubtitleFormatDown(string favorite)
    {

    }

    public async Task EditShortcut(ShortcutDisplay shortcut)
    {
        var result = await _popupService.ShowPopupAsync<EditShortcutPopupModel>(onPresenting: viewModel => 
            viewModel.Initialize("Edit shortcut: " + shortcut.Name, shortcut), CancellationToken.None);

        if (result is ShortcutDisplay shortcutDisplay)
        {
            var existing = Se.Settings.Shortcuts.FirstOrDefault(p => p.ActionName == shortcut.Type.ActionName);
            if (existing != null)
            {
                if (existing.Keys.Count == 0)
                {
                    Se.Settings.Shortcuts.RemoveAll(p => p.ActionName == shortcut.Type.ActionName);
                }
                else
                {
                    existing.Keys = shortcutDisplay.Type.Keys;
                }
            }
            else
            {
                Se.Settings.Shortcuts.Add(new SeShortCut(shortcut.Type.ActionName, shortcutDisplay.Type.Keys));
            }

            Shortcuts = ShortcutDisplay.GetShortcuts();

            var idx = AllSettings.IndexOf(SettingsItemShortcutsGeneral);
            AllSettings.RemoveAt(idx);
            var item = SettingsPage.UpdateShortcutsSection(this, ShortcutArea.General);
            AllSettings.Insert(idx, item);
            SettingsItemShortcutsGeneral = item;
            SettingsPage.BuildGrid(this, SettingList);
            Se.SaveSettings();

            //MainThread.BeginInvokeOnMainThread(async () =>
            //{
            //    await SettingListScrollView.ScrollToAsync(item.WholeView, ScrollToPosition.MakeVisible, false);
            //    await SettingListScrollView.ScrollToAsync(item.WholeView, ScrollToPosition.Start, true);
            //    await SettingListScrollView.ScrollToAsync(item.WholeView, ScrollToPosition.MakeVisible, false);
            //    await SettingListScrollView.ScrollToAsync(item.WholeView, ScrollToPosition.MakeVisible, false);
            //});

            //Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            //{
            //    MainThread.BeginInvokeOnMainThread(async () =>
            //    {
            //        LeftMenuTapped(null, new TappedEventArgs(null), SettingsPage.SectionName.Shortcuts);
            //     //   await SettingListScrollView.ScrollToAsync(item.WholeView, ScrollToPosition.MakeVisible, false);
            //    });
            //    return false;
            //});

        }
    }

    public void OnSizeAllocated(double width, double height)
    {
        var newWidth = Math.Max(100, width - 330);
        SettingListScrollView.WidthRequest = newWidth;

        var newHeight = Math.Max(100, height - 100);
        SettingListScrollView.HeightRequest = newHeight;
    }
}