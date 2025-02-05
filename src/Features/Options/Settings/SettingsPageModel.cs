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
    private ObservableCollection<SettingItem> _allSettings;

    [ObservableProperty]
    private string _searchText;

    public VerticalStackLayout LeftMenu { get; set; }
    public Grid SettingList { get; set; }
    public SettingsPage? Page { get; set; }
    public BoxView SyntaxErrorColorBox { get; set; }
    public ScrollView SettingListScrollView { get; set; }
    public Grid GridShortcutsGeneral { get; set; }
    public SettingItem SettingsItemShortcutsGeneral { get; set; }

    [ObservableProperty]
    private string _ffmpegPath = string.Empty;

    [ObservableProperty]
    private bool _showRecentFiles;

    [ObservableProperty]
    private ObservableCollection<string> _themes = new() { "Light", "Dark", "Custom" };

    [ObservableProperty]
    private string _theme = "Dark";

    [ObservableProperty]
    private int _subtitleLineMaximumLength;

    [ObservableProperty]
    private ObservableCollection<int> _maxNumberOfLines;

    [ObservableProperty]
    private int _selectedMaxNumberOfLines;

    [ObservableProperty]
    private int _maxNumberOfLinesPlusAbort;

    [ObservableProperty]
    private int _mergeLinesShorterThan;

    [ObservableProperty]
    private int _subtitleMinimumDisplayMilliseconds;

    [ObservableProperty]
    private int _subtitleMaximumDisplayMilliseconds;

    [ObservableProperty]
    private int _minimumMillisecondsBetweenLines;

    [ObservableProperty]
    private double _subtitleMaximumCharactersPerSeconds;

    [ObservableProperty]
    private double _subtitleOptimalCharactersPerSeconds;

    [ObservableProperty]
    private double _subtitleMaximumWordsPerMinute;

    [ObservableProperty]
    private ObservableCollection<DialogStyleDisplay> _dialogStyles;

    [ObservableProperty]
    private DialogStyleDisplay _selectedDialogStyle;

    [ObservableProperty]
    private ObservableCollection<ContinuationStyleDisplay> _continuationStyles;

    [ObservableProperty]
    private ContinuationStyleDisplay _selectedContinuationStyle;

    [ObservableProperty]
    private ObservableCollection<CpsLineLengthDisplay> _cpsLineLengthStrategies;

    [ObservableProperty]
    private CpsLineLengthDisplay _selectedCpsLineLengthStrategy;

    [ObservableProperty]
    private ObservableCollection<string> _defaultFrameRates;

    [ObservableProperty]
    private string _selectedDefaultFrameRate;

    [ObservableProperty]
    private ObservableCollection<string> _defaultFileEncodings;

    [ObservableProperty]
    private string _selectedDefaultFileEncoding;

    [ObservableProperty]
    private bool _autodetectAnsiEncoding;

    [ObservableProperty]
    private string _languageFiltersDisplay;

    [ObservableProperty]
    private bool _promptForDeleteLines;

    [ObservableProperty]
    private ObservableCollection<TimeCodeModeDisplay> _timeCodeModes;

    [ObservableProperty]
    private TimeCodeModeDisplay _selectedTimeCodeMode;

    [ObservableProperty]
    private ObservableCollection<SplitBehaviorDisplay> _splitBehaviors;

    [ObservableProperty]
    private SplitBehaviorDisplay _selectedSplitBehavior;

    [ObservableProperty]
    private ObservableCollection<SubtitleListDoubleClickActionDisplay> _subtitleListDoubleClickActions;

    [ObservableProperty]
    private SubtitleListDoubleClickActionDisplay _selectedSubtitleListDoubleClickAction;

    [ObservableProperty]
    private ObservableCollection<AutoBackupIntervalDisplay> _autoBackupIntervals;

    [ObservableProperty]
    private AutoBackupIntervalDisplay _selectedAutoBackupInterval;

    [ObservableProperty]
    private ObservableCollection<AutoBackupDeleteDisplay> _autoBackupDeleteOptions;

    [ObservableProperty]
    private AutoBackupDeleteDisplay _selectedAutoBackupDeleteOption;

    [ObservableProperty]
    private ObservableCollection<string> _subtitleFormats;

    [ObservableProperty]
    private string _selectedDefaultSubtitleFormat;


    [ObservableProperty]
    private ObservableCollection<string> _subtitleSaveAsSubtitleFormats;

    [ObservableProperty]
    private string _selectedDefaultSaveAsSubtitleFormat;

    [ObservableProperty]
    private ObservableCollection<string> _favoriteSubtitleFormats;


    // Shortcuts
    public List<ShortcutDisplay> Shortcuts;


    // Syntax coloring
    [ObservableProperty]
    private bool _colorDurationTooShort;

    [ObservableProperty]
    private bool _colorDurationTooLong;

    [ObservableProperty]
    private bool _colorTextTooLong;

    [ObservableProperty]
    private bool _colorTextTooWide;

    [ObservableProperty]
    private bool _colorTextTooManyLines;

    [ObservableProperty]
    private bool _colorTimeCodeOverlap;

    [ObservableProperty]
    private bool _colorGapTooShort;

    [ObservableProperty]
    private Color _colorErrorColor;


    // Toolbar
    [ObservableProperty]
    private bool _toolbarShowFileNew;

    [ObservableProperty]
    private bool _toolbarShowFileOpen;

    [ObservableProperty]
    private bool _toolbarShowVideoFileOpen;

    [ObservableProperty]
    private bool _toolbarShowSave;

    [ObservableProperty]
    private bool _toolbarShowSaveAs;

    [ObservableProperty]
    private bool _toolbarShowFind;

    [ObservableProperty]
    private bool _toolbarShowReplace;

    [ObservableProperty]
    private bool _toolbarShowFixCommonErrors;

    [ObservableProperty]
    private bool _toolbarShowSpellCheck;

    [ObservableProperty]
    private bool _toolbarShowHelp;


    private readonly IPopupService _popupService;
    private SettingsPage.SectionName _sectionName = SettingsPage.SectionName.General;

    private bool _loaded;

    public SettingsPageModel(IPopupService popupService)
    {
        _popupService = popupService;

        _searchText = string.Empty;
        LeftMenu = new VerticalStackLayout();
        SyntaxErrorColorBox = new BoxView();
        SettingList = new Grid();
        SettingListScrollView = new ScrollView();

        _allSettings = new ObservableCollection<SettingItem>();

        _maxNumberOfLines = new ObservableCollection<int>(Enumerable.Range(1, 5));
        _selectedMaxNumberOfLines = 2;

        _dialogStyles = new ObservableCollection<DialogStyleDisplay>(DialogStyleDisplay.GetDialogStyles());
        _selectedDialogStyle = _dialogStyles.First();

        _continuationStyles = new ObservableCollection<ContinuationStyleDisplay>(ContinuationStyleDisplay.GetDialogStyles());
        _selectedContinuationStyle = _continuationStyles.First();

        _cpsLineLengthStrategies = new ObservableCollection<CpsLineLengthDisplay>(CpsLineLengthDisplay.GetCpsLineLengthStrategies());
        _selectedCpsLineLengthStrategy = _cpsLineLengthStrategies.First();

        _defaultFrameRates = new ObservableCollection<string>(new List<string> { "23.976", "24", "25", "29.97", "30", "50", "59.94", "60" });
        _selectedDefaultFrameRate = DefaultFrameRates.First();

        _languageFiltersDisplay = "All";

        _defaultFileEncodings = new ObservableCollection<string>(EncodingHelper.GetEncodings().Select(p => p.DisplayName).ToList());
        _selectedDefaultFileEncoding = _defaultFileEncodings.First();

        _timeCodeModes = new ObservableCollection<TimeCodeModeDisplay>(TimeCodeModeDisplay.GetTimeCodeModes());
        _selectedTimeCodeMode = _timeCodeModes.First();

        _splitBehaviors = new ObservableCollection<SplitBehaviorDisplay>(SplitBehaviorDisplay.GetSplitBehaviors());
        _selectedSplitBehavior = _splitBehaviors.First();

        _subtitleListDoubleClickActions = new ObservableCollection<SubtitleListDoubleClickActionDisplay>(SubtitleListDoubleClickActionDisplay.GetSubtitleListDoubleClickActions());
        _selectedSubtitleListDoubleClickAction = _subtitleListDoubleClickActions.First();

        _autoBackupIntervals = new ObservableCollection<AutoBackupIntervalDisplay>(AutoBackupIntervalDisplay.GetAutoBackupIntervals());
        _selectedAutoBackupInterval = _autoBackupIntervals.First();

        _autoBackupDeleteOptions = new ObservableCollection<AutoBackupDeleteDisplay>(AutoBackupDeleteDisplay.GetAutoBackupDeleteOptions());
        _selectedAutoBackupDeleteOption = _autoBackupDeleteOptions.First();

        var allSubtitleFormats = SubtitleFormat.AllSubtitleFormats.Select(p => p.Name).ToList();
        _subtitleFormats = new ObservableCollection<string>(allSubtitleFormats);
        _selectedDefaultSubtitleFormat = _subtitleFormats.First();

        var allSaveAsSubtitleFormats = new List<string>(allSubtitleFormats);
        allSaveAsSubtitleFormats.Insert(0, "- Auto - ");
        _subtitleSaveAsSubtitleFormats = new ObservableCollection<string>(allSaveAsSubtitleFormats);
        _selectedDefaultSaveAsSubtitleFormat = _subtitleSaveAsSubtitleFormats.First();

        _favoriteSubtitleFormats = new ObservableCollection<string>(allSubtitleFormats.Take(5));

        Shortcuts = ShortcutDisplay.GetShortcuts();

        GridShortcutsGeneral = new Grid();
        SettingsItemShortcutsGeneral = new SettingItem("Shortcuts", SettingsPage.SectionName.Shortcuts);
        _colorErrorColor = Colors.Red;
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