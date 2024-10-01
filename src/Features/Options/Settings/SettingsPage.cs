using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Markup;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Options.Settings;

public class SettingsPage : ContentPage
{
    public enum SectionName
    {
        General,
        SubtitleFormats,
        Shortcuts,
        SyntaxColoring,
        VideoPlayer,
        WaveformSpectrogram,
        Tools,
        Toolbar,
        Appearance,
        FileTypeAssociations,
    }

    private readonly SettingsViewModel _vm;

    public SettingsPage(SettingsViewModel vm)
    {
        _vm = vm;
        vm.SettingsPage = this;
        this.BindDynamicTheme();
        vm.Theme = Preferences.Get("Theme", "Dark");
        BindingContext = vm;

        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Star }
            },
            Padding = new Thickness(20),
            RowSpacing = 0,
            ColumnSpacing = 5,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();

        var searchBar = new SearchBar
        {
            Placeholder = "Search text",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Fill,
            WidthRequest = 400,
        };
        searchBar.SetBinding(Entry.TextProperty, nameof(vm.SearchText));
        searchBar.SearchButtonPressed += vm.SearchButtonPressed;
        searchBar.TextChanged += vm.SearchBarTextChanged;
        grid.Add(searchBar, 1, 0);

        vm.LeftMenu = new VerticalStackLayout
        {
            Margin = new Thickness(0,0,50,0),
            Children =
            {
                MakeLeftMenuItem(vm, SectionName.General, "General"),
                MakeLeftMenuItem(vm, SectionName.SubtitleFormats, "Subtitle formats"),
                MakeLeftMenuItem(vm, SectionName.Shortcuts, "Shortcuts"),
                MakeLeftMenuItem(vm, SectionName.SyntaxColoring, "Syntax coloring"),
                MakeLeftMenuItem(vm, SectionName.VideoPlayer, "Video player"),
                MakeLeftMenuItem(vm, SectionName.WaveformSpectrogram, "Waveform/spectrogram"),
                MakeLeftMenuItem(vm, SectionName.Tools, "Tools"),
                MakeLeftMenuItem(vm, SectionName.Toolbar, "Toolbar"),
                MakeLeftMenuItem(vm, SectionName.Appearance, "Appearance"),
                MakeLeftMenuItem(vm, SectionName.FileTypeAssociations, "File type associations"),
            }
        };

        grid.Add(vm.LeftMenu, 0, 1);

        var settings = MakeSettingItems(vm);
        grid.Add(settings, 1, 1);

        Content = grid;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _vm.SaveSettings();
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();

        _vm.OnAppearing();
    }

    private static IView MakeLeftMenuItem(SettingsViewModel vm, SectionName sectionName, string text)
    {
        var label = new Label
        {
            Padding = 10,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            FontSize = 17,
            Text = text,
            ClassId = sectionName.ToString(),
        }.BindDynamicTheme();

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += (sender, e) => vm.LeftMenuTapped(sender, e, sectionName);
        label.GestureRecognizers.Add(tapGesture);

        return label;
    }

    private static View MakeToolsPage(SettingsViewModel vm)
    {
        var grid = new Grid
        {
            Padding = new Thickness(20),
            RowSpacing = 20,
            ColumnSpacing = 10,
            HorizontalOptions = LayoutOptions.Fill,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto }
            }
        }.BindDynamicTheme();

        var titleLabel = new Label
        {
            Text = "Tools settings",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
        }.BindDynamicTheme();
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        var autoBreakLabel = new Label
        {
            Text = "Auto break settings",
            FontAttributes = FontAttributes.Bold,
            FontSize = 16,
        }.BindDynamicTheme();
        grid.Add(autoBreakLabel, 0, 1);
        Grid.SetColumnSpan(autoBreakLabel, 2);

        var labelBreakEarly = new Label
        {
            Text = "Break early for dialogs:",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelBreakEarly, 0, 2);

        var switchBreakEarly = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();

        grid.Add(switchBreakEarly, 1, 2);

        return grid;
    }

    private static View MakeSettingItems(SettingsViewModel vm)
    {
        var collectionView = new CollectionView
        {
            Margin = new Thickness(0, 0, 0, 0),
            ItemsLayout = new GridItemsLayout(1, ItemsLayoutOrientation.Vertical),
            SelectionMode = SelectionMode.None,
            VerticalOptions = LayoutOptions.Start,
            ItemTemplate = new DataTemplate(() =>
            {
                var contentView = new ContentView();
                contentView.Margin = new Thickness(0, 0, 0, 0);
                contentView.Padding = new Thickness(0, 0, 0, 0);
                contentView.SetBinding(ContentView.ContentProperty,  nameof(SettingItem.WholeView));
                return contentView;
            }),
        }.BindDynamicTheme();

        MakeGeneralSettings(vm);
        MakeSubtitleFormatSettings(vm);
        MakeShortcutsSettings(vm);
        MakeSyntaxColoringSettings(vm);
        MakeVideoPlayerSettings(vm);
        MakeWaveformSpectrogramSettings(vm);
        MakeToolsSettings(vm);
        MakeToolbarSettings(vm);
        MakeAppearanceSettings(vm);
        MakeFileTypeAssociationsSettings(vm);

        collectionView.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.AllSettings));
        vm.SettingList = collectionView;

        return collectionView;
    }

    private static void MakeFileTypeAssociationsSettings(SettingsViewModel vm)
    {
        var textWidth = 200;

        vm.AllSettings.Add(new SettingItem("File type associations", SectionName.FileTypeAssociations));

        vm.AllSettings.Add(new SettingItem("File types that Subtitle Alchemist will handle"));

        var switchAssa = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Advanced Sub Station Alpha (.ass)", textWidth, string.Empty, switchAssa));

        var switchSrt = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("SubRip (.srt)", textWidth, string.Empty, switchSrt));

        var switchStl = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("EBU STL (.stl)", textWidth, string.Empty, switchStl));

        var switchDfxp = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Distribution Format Exchange Profile (.dfxp)", textWidth, string.Empty, switchDfxp));

        var switchVtt = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("WebVTT (.vtt)", textWidth, string.Empty, switchVtt));

        var switchSami = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("SAMI (.smi)", textWidth, string.Empty, switchSami));

        var switchSsa = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Sub Station Alpha (.ssa)", textWidth, string.Empty, switchSsa));
    }

    private static void MakeToolsSettings(SettingsViewModel vm)
    {
        var textWidth = 200;

        vm.AllSettings.Add(new SettingItem("Tools", SectionName.Tools));

        vm.AllSettings.Add(new SettingItem("Fix common errors"));
        vm.AllSettings.Add(new SettingItem("Spell check"));
        vm.AllSettings.Add(new SettingItem("auto-break"));
    }

    private static void MakeToolbarSettings(SettingsViewModel vm)
    {
        var textWidth = 200;

        vm.AllSettings.Add(new SettingItem("Toolbar", SectionName.Toolbar));

        var switchFileNew = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("File new", textWidth, string.Empty, switchFileNew));

        var switchFileOpen = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("File open", textWidth, string.Empty, switchFileOpen));

        var switchVideoFileOpen = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Video file open", textWidth, string.Empty, switchVideoFileOpen));

        var switchSave = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Save", textWidth, string.Empty, switchSave));

        var switchSaveAs = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Save as", textWidth, string.Empty, switchSaveAs));

        var switchFind = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Find", textWidth, string.Empty, switchFind));

        var switchReplace = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Replace", textWidth, string.Empty, switchReplace));

        var switchFixCommonErrors = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Fix common errors", textWidth, string.Empty, switchFixCommonErrors));

        var switchSpellCheck = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Spell check", textWidth, string.Empty, switchSpellCheck));

        var switchHelp = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Help", textWidth, string.Empty, switchHelp));


        vm.AllSettings.Add(new SettingItem("Toolbar buttons"));

        vm.AllSettings.Add(new SettingItem("Frame rate"));
    }

    private static void MakeAppearanceSettings(SettingsViewModel vm)
    {
        var textWidth = 200;

        vm.AllSettings.Add(new SettingItem("Appearance (UI)", SectionName.Appearance));

        vm.AllSettings.Add(new SettingItem("Theme", SectionName.Appearance));

        var pickerTheme = new Picker
        {
            ItemsSource = new List<string> { "Light", "Dark", "Custom" },
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Theme", textWidth, string.Empty, pickerTheme));

        vm.AllSettings.Add(new SettingItem("Font", SectionName.Appearance));

        var pickerFontName = new Picker
        {
            ItemsSource = new List<string> { "Arial", "Courier New", "Times New Roman" },
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Font name", textWidth, string.Empty, pickerFontName));

        var entrySubtitleListViewFontSize = new Entry
        {
            Placeholder = "Enter subtitle list view font size",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Subtitle list view font size", textWidth, string.Empty, entrySubtitleListViewFontSize));
    }

    private static void MakeWaveformSpectrogramSettings(SettingsViewModel vm)
    {
        var textWidth = 200;

        vm.AllSettings.Add(new SettingItem("Waveform/spectrogram", SectionName.WaveformSpectrogram));

        vm.AllSettings.Add(new SettingItem("Waveform"));
        vm.AllSettings.Add(new SettingItem("Spectrogram"));
        vm.AllSettings.Add(new SettingItem("FFmpeg"));
        vm.AllSettings.Add(new SettingItem("Cleanup"));
    }

    private static void MakeVideoPlayerSettings(SettingsViewModel vm)
    {
        var textWidth = 200;

        vm.AllSettings.Add(new SettingItem("Video player", SectionName.VideoPlayer));

        vm.AllSettings.Add(new SettingItem("Video engine"));
    }

    private static void MakeGeneralSettings(SettingsViewModel vm)
    {
        vm.AllSettings.Add(new SettingItem("General", SectionName.General));
        vm.AllSettings.Add(new SettingItem("Rules"));

        var ruleTextWidth = 200;

        var entrySingleLineMaxWidth = new Entry
        {
            Placeholder = "Enter single line max width",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Single line max length", ruleTextWidth, string.Empty, entrySingleLineMaxWidth));

        var entryOptimalCharsSec = new Entry
        {
            Placeholder = "Enter optimal chars/sec",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Optimal chars/sec", ruleTextWidth, string.Empty, entryOptimalCharsSec));

        var entryMaxCharsSec = new Entry
        {
            Placeholder = "Enter max chars/sec",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Max chars/sec", ruleTextWidth, string.Empty, entryMaxCharsSec));

        var entryMaxWordsMin = new Entry
        {
            Placeholder = "Enter max words/min",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Max words/min", ruleTextWidth, string.Empty, entryMaxWordsMin));

        var entryMinDuration = new Entry
        {
            Placeholder = "Enter min duration in milliseconds",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Min duration in milliseconds", ruleTextWidth, string.Empty, entryMinDuration));

        var entryMaxDuration = new Entry
        {
            Placeholder = "Enter max duration in milliseconds",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Max duration in milliseconds", ruleTextWidth, string.Empty, entryMaxDuration));

        var entryMinGap = new Entry
        {
            Placeholder = "Enter min gap between subtitles in milliseconds",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Min gap between subtitles in milliseconds", ruleTextWidth, string.Empty,
            entryMinGap));

        var entryMaxLines = new Entry
        {
            Placeholder = "Enter max number of lines",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Max number of lines", ruleTextWidth, string.Empty, entryMaxLines));

        var entryUnbreakShorterThan = new Entry
        {
            Placeholder = "Enter unbreak subtitles shorter than",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Unbreak subtitles shorter than", ruleTextWidth, string.Empty,
            entryUnbreakShorterThan));

        var entryDialogStyle = new Entry
        {
            Placeholder = "Enter dialog style",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Dialog style", ruleTextWidth, string.Empty, entryDialogStyle));

        var entryContinuationStyle = new Entry
        {
            Placeholder = "Enter continuation style",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Continuation style", ruleTextWidth, string.Empty, entryContinuationStyle));

        var entryCpsLineLength = new Entry
        {
            Placeholder = "Enter cps/line-length",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("cps/line-length", ruleTextWidth, string.Empty, entryCpsLineLength));


        vm.AllSettings.Add(new SettingItem("Misc."));

        var entryDefaultFrameRate = new Entry
        {
            Placeholder = "Enter default frame rate",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Default frame rate", ruleTextWidth, string.Empty, entryDefaultFrameRate));

        var entryDefaultFileEncoding = new Entry
        {
            Placeholder = "Enter default file encoding",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Default file encoding", ruleTextWidth, string.Empty, entryDefaultFileEncoding));

        var entryLanguageFilter = new Entry
        {
            Placeholder = "Enter language filter",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Language filter", ruleTextWidth, string.Empty, entryLanguageFilter));

        var entryAutoDetectAnsiEncoding = new Entry
        {
            Placeholder = "Enter auto detect ANSI encoding",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Auto detect ANSI encoding", ruleTextWidth, string.Empty,
            entryAutoDetectAnsiEncoding));

        var entryPromptForDeleteLines = new Entry
        {
            Placeholder = "Enter prompt for delete lines",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Prompt for delete lines", ruleTextWidth, string.Empty,
            entryPromptForDeleteLines));

        var entryTimeCodeMode = new Entry
        {
            Placeholder = "Enter time code mode",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Time code mode", ruleTextWidth, string.Empty, entryTimeCodeMode));

        var entrySplitBehavior = new Entry
        {
            Placeholder = "Enter split behavior",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Split behavior", ruleTextWidth, string.Empty, entrySplitBehavior));

        var entrySubtitleListDoubleClickAction = new Entry
        {
            Placeholder = "Enter subtitle list double click action",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Subtitle list double click action", ruleTextWidth, string.Empty,
            entrySubtitleListDoubleClickAction));

        var entrySaveAsBehavior = new Entry
        {
            Placeholder = "Enter save as behavior",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Save as behavior", ruleTextWidth, string.Empty, entrySaveAsBehavior));

        var entryTranslationFileAutoSuffix = new Entry
        {
            Placeholder = "Enter translation file auto suffix",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Translation file auto suffix", ruleTextWidth, string.Empty,
            entryTranslationFileAutoSuffix));

        var entryAutoBackup = new Entry
        {
            Placeholder = "Enter auto-backup",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Auto-backup", ruleTextWidth, string.Empty, entryAutoBackup));

        var entryAutoBackupDelete = new Entry
        {
            Placeholder = "Enter auto-backup delete",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Auto-backup delete", ruleTextWidth, string.Empty, entryAutoBackupDelete));

        var entryAutoSave = new Entry
        {
            Placeholder = "Enter auto-save",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Auto-save", ruleTextWidth, string.Empty, entryAutoSave));

        var entryCheckForUpdates = new Entry
        {
            Placeholder = "Enter check for updates",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Check for updates", ruleTextWidth, string.Empty, entryCheckForUpdates));
    }

    private static IView MakeGeneralRight(SettingsViewModel vm)
    {
        var stackLayout = new StackLayout
        {
            Padding = new Thickness(20),
            Spacing = 20,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Orientation = StackOrientation.Vertical,
        }.BindDynamicTheme();

        // Remember Recent Files
        var labelRecentFiles = new Label
        {
            Text = "Remember Recent Files:",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();

        var switchRecentFiles = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme().Bind(Switch.IsToggledProperty, nameof(vm.ShowRecentFiles));
        // switchRecentFiles.SetBinding(Switch.IsToggledProperty, nameof(vm.ShowRecentFiles));

        var stackRecentFiles = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Children =
            {
                labelRecentFiles,
                switchRecentFiles,
            }
        };

        stackLayout.Children.Add(stackRecentFiles);

        return stackLayout;
    }

    private static void MakeSubtitleFormatSettings(SettingsViewModel vm)
    {
        vm.AllSettings.Add(new SettingItem("Subtitle formats", SectionName.SubtitleFormats));

        var textWidth = 200;
        var pickerDefaultSubtitleFormat = new Picker
        {
            ItemsSource = new List<string> { "SubRip", "Advanced Sub Station Alpha", "EBU STL" },
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Single line max length", textWidth, string.Empty, pickerDefaultSubtitleFormat));

        var pickerDefaultSaveAsFormat = new Picker
        {
            ItemsSource = new List<string> { "SubRip", "Advanced Sub Station Alpha", "EBU STL" },
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Optimal chars/sec", textWidth, string.Empty, pickerDefaultSaveAsFormat));

        vm.AllSettings.Add(new SettingItem("Favorites"));
    }

    private static void MakeShortcutsSettings(SettingsViewModel vm)
    {
        vm.AllSettings.Add(new SettingItem("Shortcuts", SectionName.Shortcuts));

        var textWidth = 200;

        var pickerDefaultSubtitleFormat = new Picker
        {
            ItemsSource = new List<string> { "SubRip", "Advanced Sub Station Alpha", "EBU STL" },
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("TODO", textWidth, string.Empty, pickerDefaultSubtitleFormat));
    }

    private Switch? _shortDurationSwitch;
    private Switch? _longDurationSwitch;
    private Button? _textTooLongColorButton;
    private readonly Color _textTooLongColor = Colors.LightBlue;

    private static void MakeSyntaxColoringSettings(SettingsViewModel vm)
    {
        var textWidth = 200;

        vm.AllSettings.Add(new SettingItem("Syntax coloring", SectionName.SyntaxColoring));

        vm.AllSettings.Add(new SettingItem("Duration"));

        var switchColorDurationTooShort = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Color if duration is too short", textWidth, string.Empty, switchColorDurationTooShort));

        var switchColorDurationTooLong = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Color if duration is too long", textWidth, string.Empty, switchColorDurationTooLong));

        vm.AllSettings.Add(new SettingItem("Text"));

        var switchColorTextTooLong = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Color text if too long", textWidth, string.Empty, switchColorTextTooLong));

        var entryTextTooLong = new Entry
        {
            Placeholder = "Enter text too long threshold",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Text too long threshold", textWidth, string.Empty, entryTextTooLong));

        var switchColorTextTooWide = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Color if text is too wide", textWidth, string.Empty, switchColorTextTooWide));

        var entryTextTooWide = new Entry
        {
            Placeholder = "Enter text too wide threshold",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Text too wide threshold", textWidth, string.Empty, entryTextTooWide));

        var switchColorTextTooManyLines = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Color text if more than x lines", textWidth, string.Empty, switchColorTextTooManyLines));

        var entryTextTooManyLines = new Entry
        {
            Placeholder = "Enter text too many lines threshold",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Text too many lines threshold", textWidth, string.Empty, entryTextTooManyLines));


        vm.AllSettings.Add(new SettingItem("Misc."));

        var switchColorTimeCodeOverlap = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Color if time code overlap", textWidth, string.Empty, switchColorTimeCodeOverlap));

        var switchColorGapTooShort = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Color if gap too short", textWidth, string.Empty, switchColorGapTooShort));
    }

    private View MakeVideoPlayerPage(SettingsViewModel vm)
    {
        var grid = new Grid
        {
            Padding = new Thickness(20),
            RowSpacing = 20,
            ColumnSpacing = 10,
            HorizontalOptions = LayoutOptions.Fill,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Auto }
            }
        }.BindDynamicTheme();

        var titleLabel = new Label
        {
            Text = "Video player",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
        }.BindDynamicTheme();
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        grid.Add(new Label
        {
            Text = "Video player",
        }.BindDynamicTheme(), 0, 1);
        grid.Add(new Picker
        {
            ItemsSource = new List<string> { "mpv", "vlc", "System Default" },
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme(), 1, 1);

        return grid;
    }

    private View MakeWaveformSpectrogramPage(SettingsViewModel vm)
    {
        var grid = new Grid
        {
            Padding = new Thickness(20),
            RowSpacing = 20,
            ColumnSpacing = 10,
            HorizontalOptions = LayoutOptions.Fill,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Auto },
            }
        }.BindDynamicTheme();

        var titleLabel = new Label
        {
            Text = "Waveform/spectrogram",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
        }.BindDynamicTheme();
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        // FFmpeg Location
        grid.Add(new Label
        {
            Text = "FFmpeg Location:",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme(), 0, 1);
        grid.Add(new Entry
        {
            Placeholder = "Enter FFmpeg path",
            HorizontalOptions = LayoutOptions.Start,
            WidthRequest = 500,
            BindingContext = vm,
        }.Bind(nameof(vm.FfmpegPath)).BindDynamicTheme(), 1, 1);

        var ffmpegBrowse = new ImageButton
        {
            Source = "open.png",
            HorizontalOptions = LayoutOptions.Start,
            WidthRequest = 30,
            HeightRequest = 30,
            Padding = new Thickness(10, 5, 5, 5),
        }.BindDynamicTheme();
        ffmpegBrowse.Clicked += async (sender, e) => await vm.BrowseForFfmpeg(sender, e);
        ToolTipProperties.SetText(ffmpegBrowse, "Browse for ffmpeg executable");
        grid.Add(ffmpegBrowse, 2, 1);

        var ffmpegDownloadButton = new ImageButton
        {
            Source = "download.png",
            HorizontalOptions = LayoutOptions.Start,
            WidthRequest = 30,
            HeightRequest = 30,
            Padding = new Thickness(5, 5, 5, 5),
        }.BindDynamicTheme();
        ffmpegDownloadButton.Clicked += async (sender, e) => await vm.DownloadFfmpeg(sender, e);
        ToolTipProperties.SetText(ffmpegDownloadButton, "Click to download ffmpeg");
        grid.Add(ffmpegDownloadButton, 3, 1);

        return grid;
    }

    private View MakeToolbarPage(SettingsViewModel vm)
    {
        var grid = new Grid
        {
            Padding = new Thickness(20),
            RowSpacing = 20,
            ColumnSpacing = 10,
            HorizontalOptions = LayoutOptions.Fill,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Star }
            }
        }.BindDynamicTheme();

        var titleLabel = new Label
        {
            Text = "Toolbar",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
        };
        titleLabel.BindDynamicTheme();
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        grid.Add(new Label
        {
            Text = "Show \"File new\" icon",
        }.BindDynamicTheme(), 0, 1);
        _shortDurationSwitch = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        grid.Add(_shortDurationSwitch, 1, 1);

        grid.Add(new Label
        {
            Text = "Show \"File Save\" icon",
        }.BindDynamicTheme(), 0, 2);
        _longDurationSwitch = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        grid.Add(_longDurationSwitch, 1, 2);

        grid.Add(new Label
        {
            Text = "Show \"File Save as...\" icon",
        }.BindDynamicTheme(), 0, 3);
        _longDurationSwitch = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        grid.Add(_longDurationSwitch, 1, 3);

        return grid;
    }

    private View MakeAppearancePage(SettingsViewModel vm)
    {
        var grid = new Grid
        {
            Padding = new Thickness(20),
            RowSpacing = 20,
            ColumnSpacing = 10,
            HorizontalOptions = LayoutOptions.Fill,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Star }
            }
        }.BindDynamicTheme();

        var titleLabel = new Label
        {
            Text = "Appearance",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
        }.BindDynamicTheme();
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        // Theme
        var themeLabel = new Label
        {
            Text = "Theme:",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();

        grid.Add(themeLabel, 0, 1);
        var picker = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        picker.SetBinding(Picker.ItemsSourceProperty, nameof(vm.Themes));
        picker.SetBinding(Picker.SelectedItemProperty, nameof(vm.Theme));
        picker.SelectedItem = vm.Theme;

        picker.SelectedIndexChanged += async (o, args) => await vm.ThemeChanged(o, args);
        grid.Add(picker, 1, 1);

        return grid;
    }

    private View MakeFileTypeAssociationsPage(SettingsViewModel vm)
    {
        var grid = new Grid
        {
            Padding = new Thickness(20),
            RowSpacing = 20,
            ColumnSpacing = 10,
            HorizontalOptions = LayoutOptions.Fill,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Star }
            }
        }.BindDynamicTheme();

        var titleLabel = new Label
        {
            Text = "File type associations",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
        }.BindDynamicTheme();
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        grid.Add(new Label
        {
            Text = "SubRip (.srt)",
        }.BindDynamicTheme(), 0, 1);
        _shortDurationSwitch = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        grid.Add(_shortDurationSwitch, 1, 1);

        grid.Add(new Label
        {
            Text = "Advanced Sub Station Alpha (.ass)",
        }.BindDynamicTheme(), 0, 2);
        _longDurationSwitch = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        grid.Add(_longDurationSwitch, 1, 2);

        grid.Add(new Label
        {
            Text = "EBU STL (.stl)",
        }.BindDynamicTheme(), 0, 3);
        _longDurationSwitch = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        grid.Add(_longDurationSwitch, 1, 3);

        return grid;
    }
}