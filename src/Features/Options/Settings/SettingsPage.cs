using Microsoft.Maui.Controls.Shapes;
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
        vm.Page = this;
        this.BindDynamicTheme();
        vm.Theme = Preferences.Get("Theme", "Dark");

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
        grid.Add(searchBar, 1);

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
        BindingContext = vm;
        this.BindingContext = vm;
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

    private static View MakeSettingItems(SettingsViewModel vm)
    {
        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
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
        
        var collectionView = new CollectionView
        {
            Margin = new Thickness(0, 0, 0, 0),
            ItemsLayout = new GridItemsLayout(1, ItemsLayoutOrientation.Vertical),
            SelectionMode = SelectionMode.None,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Default,
            VerticalScrollBarVisibility = ScrollBarVisibility.Always,
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
        vm.SettingList = grid;

        var row = 0;
        foreach (var setting in vm.AllSettings)
        {
            if (!string.IsNullOrEmpty(setting.Text))
            {
                var label = new Label { Text = setting.Text, WidthRequest = setting.TextWidth };
                if (setting.TextWidth > 0)
                {
                    label.WidthRequest = setting.TextWidth;
                }
                grid.Add(label, 0, row);
            }

            grid.Add(setting.WholeView, 1, row);

            row++;
        }
        
        var scrollView = new ScrollView()
        {
            Content = grid,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        };

        return scrollView;
    }

    private static void MakeGeneralSettings(SettingsViewModel vm)
    {
        vm.AllSettings.Add(new SettingItem("General", SectionName.General));
        vm.AllSettings.Add(new SettingItem("Rules"));

        var ruleTextWidth = 200;
        var controlWidth = 200;

        // Rules
        var entrySingleLineMaxWidth = new Entry
        {
            Placeholder = "Enter single line max width",
            HorizontalOptions = LayoutOptions.Start,
            Keyboard = Keyboard.Numeric,
            BindingContext = vm,
            WidthRequest = controlWidth,
        }.BindDynamicTheme();
        entrySingleLineMaxWidth.SetBinding(Entry.TextProperty, nameof(vm.SubtitleLineMaximumLength), BindingMode.TwoWay);

        vm.AllSettings.Add(new SettingItem("Single line max length", ruleTextWidth, string.Empty, entrySingleLineMaxWidth));

        var entryOptimalCharsSec = new Entry
        {
            Placeholder = "Enter optimal chars/sec",
            HorizontalOptions = LayoutOptions.Start,
            Keyboard = Keyboard.Numeric,
            BindingContext = vm,
            WidthRequest = controlWidth,
        }.BindDynamicTheme();
        entryOptimalCharsSec.SetBinding(Entry.TextProperty, nameof(vm.SubtitleOptimalCharactersPerSeconds), BindingMode.TwoWay);
        vm.AllSettings.Add(new SettingItem("Optimal chars/sec", ruleTextWidth, string.Empty, entryOptimalCharsSec));

        var entryMaxCharsSec = new Entry
        {
            Placeholder = "Enter max chars/sec",
            HorizontalOptions = LayoutOptions.Start,
            Keyboard = Keyboard.Numeric,
            BindingContext = vm,
            WidthRequest = controlWidth,
        }.BindDynamicTheme();
        entryMaxCharsSec.SetBinding(Entry.TextProperty, nameof(vm.SubtitleMaximumCharactersPerSeconds), BindingMode.TwoWay);
        vm.AllSettings.Add(new SettingItem("Max chars/sec", ruleTextWidth, string.Empty, entryMaxCharsSec));

        var entryMaxWordsMin = new Entry
        {
            Placeholder = "Enter max words/min",
            HorizontalOptions = LayoutOptions.Start,
            Keyboard = Keyboard.Numeric,
            BindingContext = vm,
            WidthRequest = controlWidth,
        }.BindDynamicTheme();
        entryMaxWordsMin.SetBinding(Entry.TextProperty, nameof(vm.SubtitleMaximumWordsPerMinute), BindingMode.TwoWay);
        vm.AllSettings.Add(new SettingItem("Max words/min", ruleTextWidth, string.Empty, entryMaxWordsMin));

        var entryMinDuration = new Entry
        {
            Placeholder = "Enter min duration in milliseconds",
            HorizontalOptions = LayoutOptions.Start,
            Keyboard = Keyboard.Numeric,
            BindingContext = vm,
            WidthRequest = controlWidth,
        }.BindDynamicTheme();
        entryMinDuration.SetBinding(Entry.TextProperty, nameof(vm.SubtitleMinimumDisplayMilliseconds), BindingMode.TwoWay);
        vm.AllSettings.Add(new SettingItem("Min duration in milliseconds", ruleTextWidth, string.Empty, entryMinDuration));

        var entryMaxDuration = new Entry
        {
            Placeholder = "Enter max duration in milliseconds",
            HorizontalOptions = LayoutOptions.Start,
            Keyboard = Keyboard.Numeric,
            BindingContext = vm,
            WidthRequest = controlWidth,
        }.BindDynamicTheme();
        entryMaxDuration.SetBinding(Entry.TextProperty, nameof(vm.SubtitleMaximumDisplayMilliseconds), BindingMode.TwoWay);
        vm.AllSettings.Add(new SettingItem("Max duration in milliseconds", ruleTextWidth, string.Empty, entryMaxDuration));

        var entryMinGap = new Entry
        {
            Placeholder = "Enter min gap between subtitles in milliseconds",
            HorizontalOptions = LayoutOptions.Start,
            Keyboard = Keyboard.Numeric,
            BindingContext = vm,
            WidthRequest = controlWidth,
        }.BindDynamicTheme();
        entryMinGap.SetBinding(Entry.TextProperty, nameof(vm.MinimumMillisecondsBetweenLines), BindingMode.TwoWay);
        vm.AllSettings.Add(new SettingItem("Min gap between subtitles in milliseconds", ruleTextWidth, string.Empty,
            entryMinGap));

        var pickerMaxLines = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            BindingContext = vm,
            WidthRequest = controlWidth,
        }.BindDynamicTheme();
        pickerMaxLines.SetBinding(Picker.ItemsSourceProperty, nameof(vm.MaxNumberOfLines));
        pickerMaxLines.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedMaxNumberOfLines));
        vm.AllSettings.Add(new SettingItem("Max number of lines", ruleTextWidth, string.Empty, pickerMaxLines));

        var entryUnbreakShorterThan = new Entry
        {
            Placeholder = "Enter unbreak subtitles shorter than",
            HorizontalOptions = LayoutOptions.Start,
            Keyboard = Keyboard.Numeric,
            BindingContext = vm,
            WidthRequest = controlWidth,
        }.BindDynamicTheme();
        entryUnbreakShorterThan.SetBinding(Entry.TextProperty, nameof(vm.MergeLinesShorterThan), BindingMode.TwoWay);
        vm.AllSettings.Add(new SettingItem("Unbreak subtitles shorter than", ruleTextWidth, string.Empty,
            entryUnbreakShorterThan));

        var pickerDialogStyle = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            BindingContext = vm,
            MinimumWidthRequest = controlWidth,
        }.BindDynamicTheme();
        pickerDialogStyle.SetBinding(Picker.ItemsSourceProperty, nameof(vm.DialogStyles));
        pickerDialogStyle.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedDialogStyle));
        vm.AllSettings.Add(new SettingItem("Dialog style", ruleTextWidth, string.Empty, pickerDialogStyle));

        var pickerContinuationStyle = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            BindingContext = vm,
            MinimumWidthRequest = controlWidth,
        }.BindDynamicTheme();
        pickerContinuationStyle.SetBinding(Picker.ItemsSourceProperty, nameof(vm.ContinuationStyles));
        pickerContinuationStyle.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedContinuationStyle));
        vm.AllSettings.Add(new SettingItem("Continuation style", ruleTextWidth, string.Empty, pickerContinuationStyle));

        var pickerCpsLineLength = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            BindingContext = vm,
            MinimumWidthRequest = controlWidth,
        }.BindDynamicTheme();
        pickerCpsLineLength.SetBinding(Picker.ItemsSourceProperty, nameof(vm.CpsLineLengthStrategies));
        pickerCpsLineLength.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedCpsLineLengthStrategy));
        vm.AllSettings.Add(new SettingItem("CPS/line-length", ruleTextWidth, string.Empty, pickerCpsLineLength));

        // Misc.
        vm.AllSettings.Add(new SettingItem("Misc."));

        var pickerDefaultFrameRate = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            BindingContext = vm,
            MinimumWidthRequest = controlWidth,
        }.BindDynamicTheme();
        pickerDefaultFrameRate.SetBinding(Picker.ItemsSourceProperty, nameof(vm.DefaultFrameRates));
        pickerDefaultFrameRate.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedDefaultFrameRate));
        vm.AllSettings.Add(new SettingItem("Default frame rate", ruleTextWidth, string.Empty, pickerDefaultFrameRate));


        var pickerDefaultFileEncoding = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            BindingContext = vm,
            MinimumWidthRequest = controlWidth,
        }.BindDynamicTheme();
        pickerDefaultFileEncoding.SetBinding(Picker.ItemsSourceProperty, nameof(vm.DefaultFileEncodings));
        pickerDefaultFileEncoding.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedDefaultFileEncoding));
        vm.AllSettings.Add(new SettingItem("Default file encoding", ruleTextWidth, string.Empty, pickerDefaultFileEncoding));


        var switchAutoDetectAnsiEncoding = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
            BindingContext = vm,
        }.BindDynamicTheme();
        switchAutoDetectAnsiEncoding.SetBinding(Switch.IsToggledProperty, nameof(vm.AutodetectAnsiEncoding));
        vm.AllSettings.Add(new SettingItem("Auto detect ANSI encoding", ruleTextWidth, string.Empty, switchAutoDetectAnsiEncoding));

        var labelLanguageFilter = new Label
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 10, 0),
            BindingContext = vm,
        }.BindDynamicTheme();
        labelLanguageFilter.SetBinding(Label.TextProperty, nameof(vm.LanguageFiltersDisplay));
        var buttonLanguageFilter = new Button
        {
            Text = "Edit",
            HorizontalOptions = LayoutOptions.Start,
            BindingContext = vm,
        }.BindDynamicTheme();
        var stackLanguageFilter = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Children =
            {
                labelLanguageFilter,
                buttonLanguageFilter,
            }
        };
        vm.AllSettings.Add(new SettingItem("Language filter", ruleTextWidth, string.Empty, stackLanguageFilter));

        var switchPromptForDeleteLines = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
            BindingContext = vm,
        }.BindDynamicTheme();
        switchPromptForDeleteLines.SetBinding(Switch.IsToggledProperty, nameof(vm.PromptForDeleteLines));
        vm.AllSettings.Add(new SettingItem("Prompt for delete lines", ruleTextWidth, string.Empty, switchPromptForDeleteLines));

        var pickerTimeCodeMode = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            BindingContext = vm,
            MinimumWidthRequest = controlWidth,
        }.BindDynamicTheme();
        pickerTimeCodeMode.SetBinding(Picker.ItemsSourceProperty, nameof(vm.TimeCodeModes));
        pickerTimeCodeMode.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedTimeCodeMode));
        vm.AllSettings.Add(new SettingItem("Time code mode", ruleTextWidth, string.Empty, pickerTimeCodeMode));


        var pickerSplitBehavior = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            BindingContext = vm,
            MinimumWidthRequest = controlWidth,
        }.BindDynamicTheme();
        pickerSplitBehavior.SetBinding(Picker.ItemsSourceProperty, nameof(vm.SplitBehaviors));
        pickerSplitBehavior.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedSplitBehavior));
        vm.AllSettings.Add(new SettingItem("Split behavior", ruleTextWidth, string.Empty, pickerSplitBehavior));

        var pickerSubtitleListDoubleClickAction = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            BindingContext = vm,
            MinimumWidthRequest = controlWidth,
        }.BindDynamicTheme();
        pickerSubtitleListDoubleClickAction.SetBinding(Picker.ItemsSourceProperty, nameof(vm.SubtitleListDoubleClickActions));
        pickerSubtitleListDoubleClickAction.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedSubtitleListDoubleClickAction));
        vm.AllSettings.Add(new SettingItem("Subtitle list double click action", ruleTextWidth, string.Empty,
            pickerSubtitleListDoubleClickAction));

        var pickerAutoBackupInterval = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            BindingContext = vm,
            MinimumWidthRequest = controlWidth,
        }.BindDynamicTheme();
        pickerAutoBackupInterval.SetBinding(Picker.ItemsSourceProperty, nameof(vm.AutoBackupIntervals));
        pickerAutoBackupInterval.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedAutoBackupInterval));
        vm.AllSettings.Add(new SettingItem("Auto-backup", ruleTextWidth, string.Empty, pickerAutoBackupInterval));

        var pickerAutoBackupDeletAfter = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            BindingContext = vm,
            MinimumWidthRequest = controlWidth,
        }.BindDynamicTheme();
        pickerAutoBackupDeletAfter.SetBinding(Picker.ItemsSourceProperty, nameof(vm.AutoBackupDeleteOptions));
        pickerAutoBackupDeletAfter.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedAutoBackupDeleteOption));
        vm.AllSettings.Add(new SettingItem("Auto-backup delete", ruleTextWidth, string.Empty, pickerAutoBackupDeletAfter));
    }

    private static void MakeSubtitleFormatSettings(SettingsViewModel vm)
    {
        vm.AllSettings.Add(new SettingItem("Subtitle formats", SectionName.SubtitleFormats));

        var textWidth = 200;
        var pickerDefaultSubtitleFormat = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            BindingContext = vm,
            MinimumWidthRequest = textWidth,
        }.BindDynamicTheme();
        pickerDefaultSubtitleFormat.SetBinding(Picker.ItemsSourceProperty, nameof(vm.SubtitleFormats));
        pickerDefaultSubtitleFormat.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedDefaultSubtitleFormat));
        vm.AllSettings.Add(new SettingItem("Single line max length", textWidth, string.Empty, pickerDefaultSubtitleFormat));

        var pickerDefaultSaveAsFormat = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            BindingContext = vm,
            MinimumWidthRequest = textWidth,
        }.BindDynamicTheme();
        pickerDefaultSaveAsFormat.SetBinding(Picker.ItemsSourceProperty, nameof(vm.SubtitleSaveAsSubtitleFormats));
        pickerDefaultSaveAsFormat.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedDefaultSaveAsSubtitleFormat));
        vm.AllSettings.Add(new SettingItem("Default save as format", textWidth, string.Empty, pickerDefaultSaveAsFormat));

        vm.AllSettings.Add(new SettingItem("Favorites"));
        MakeFavorites(vm);
        //var stackFavorites = new StackLayout
        //{
        //    Orientation = StackOrientation.Vertical,
        //    HorizontalOptions = LayoutOptions.Start,
        //    VerticalOptions = LayoutOptions.Start,
        //}.BindDynamicTheme();
        //var favorites = MakeFavorites(vm);
        //foreach (var favorite in favorites)
        //{
        //    stackFavorites.Children.Add(favorite);
        //}
        //var favoriteBorder = new Border
        //{
        //    StrokeThickness = 1,
        //    Padding = new Thickness(10),
        //    Margin = new Thickness(2),
        //    StrokeShape = new RoundRectangle
        //    {
        //        CornerRadius = new CornerRadius(5)
        //    },
        //    Content = stackFavorites,
        //}.BindDynamicTheme();
        //vm.AllSettings.Add(new SettingItem(string.Empty, textWidth, string.Empty, stackFavorites));

        var buttonAdd = new Button
        {
            Text = "Add",
            HorizontalOptions = LayoutOptions.Start,
            BindingContext = vm,
        }.BindDynamicTheme();
        //buttonAdd.Clicked += vm.AddFavoriteSubtitleFormat;
        vm.AllSettings.Add(new SettingItem(string.Empty, textWidth, string.Empty, buttonAdd));
    }

    private static void MakeShortcutsSettings(SettingsViewModel vm)
    {
        vm.AllSettings.Add(new SettingItem("Shortcuts", SectionName.Shortcuts));

        var textWidth = 200;

       var gridShortcuts = new Grid
       {
           RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
            },
           ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
            },
           Padding = new Thickness(0, 0, 0, 0),
           RowSpacing = 0,
           ColumnSpacing = 5,
           HorizontalOptions = LayoutOptions.Start,
           VerticalOptions = LayoutOptions.Start,
           Margin = new Thickness(textWidth, 0, 0, 0),
       }.BindDynamicTheme();

        var labelHeaderArea = new Label
        {
            Text = "Area",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            FontAttributes = FontAttributes.Bold,
            WidthRequest = 200,
        }.BindDynamicTheme();
        gridShortcuts.Add(labelHeaderArea, 0, 0);

        var labelHeaderDescription = new Label
        {
            Text = "Description",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            FontAttributes = FontAttributes.Bold,
            WidthRequest = 200,
        }.BindDynamicTheme();
        gridShortcuts.Add(labelHeaderDescription, 1, 0);

        var labelHeaderShortcut = new Label
        {
            Text = "Shortcut",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            FontAttributes = FontAttributes.Bold,
            WidthRequest = 200,
        }.BindDynamicTheme();
        gridShortcuts.Add(labelHeaderShortcut, 2, 0);


        var row = 0;
        foreach (var shortcut in vm.Shortcuts)
        {
            row++;

            var labelArea = new Label
            {
                Text = shortcut.Area,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 200,
            }.BindDynamicTheme();
            gridShortcuts.Add(labelArea, 0, row);

            var labelDescription = new Label
            {
                Text = shortcut.Name,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 200,
            }.BindDynamicTheme();
            gridShortcuts.Add(labelDescription, 1, row);

            var labelShortcut = new Label
            {
                Text = shortcut.Keys.ToString(),
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 200,
            }.BindDynamicTheme();
            gridShortcuts.Add(labelShortcut, 2, row);
        }

        vm.AllSettings.Add(new SettingItem(string.Empty, gridShortcuts));
    }

    public static void MakeFavorites(SettingsViewModel vm)
    {
        var favorites = new List<StackLayout>();

        // header
        var labelHeader = new Label
        {
            Text = "Subtitle format",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            FontAttributes = FontAttributes.Bold,
            WidthRequest = 200,
        }.BindDynamicTheme();

        var labelRemove = new Label
        {
            Text = "Action",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            FontAttributes = FontAttributes.Bold,
            WidthRequest = 200,
        }.BindDynamicTheme();

        var stackHeader = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            HeightRequest = 30,
            Margin = new Thickness(5),
            Children =
            {
                labelHeader,
                labelRemove,
            }
        };

        vm.AllSettings.Add(new SettingItem(string.Empty, 200, string.Empty, stackHeader));

        // items
        foreach (var favorite in vm.FavoriteSubtitleFormats)
        {
            var labelName = new Label
            {
                Text = favorite,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 200,
            }.BindDynamicTheme();
            var button = new Button
            {
                Text = "Remove",
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start,
                BindingContext = vm,
            }.BindDynamicTheme();
            //button.Clicked += vm.RemoveFavoriteSubtitleFormat;
            var stack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start,
                HeightRequest = 50,
                Margin = new Thickness(5),
                Children =
                {
                    labelName,
                    button,
                }
            };
            //favorites.Add(stack);
            vm.AllSettings.Add(new SettingItem(string.Empty, 200, string.Empty, stack));
        }
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
            BindingContext = vm,
        }.BindDynamicTheme();
        pickerTheme.SetBinding(Picker.SelectedItemProperty, nameof(vm.Theme));
        vm.AllSettings.Add(new SettingItem("Theme", textWidth, string.Empty, pickerTheme));

        vm.AllSettings.Add(new SettingItem("Font", SectionName.Appearance));

        var pickerFontName = new Picker
        {
            ItemsSource = new List<string> { "Arial", "Courier New", "Times New Roman" },
            HorizontalOptions = LayoutOptions.Start,
            BindingContext = vm,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Font name", textWidth, string.Empty, pickerFontName));

        var entrySubtitleListViewFontSize = new Entry
        {
            Placeholder = "Enter subtitle list view font size",
            HorizontalOptions = LayoutOptions.Start,
            BindingContext = vm,
        }.BindDynamicTheme();
        vm.AllSettings.Add(new SettingItem("Subtitle list view font size", textWidth, string.Empty, entrySubtitleListViewFontSize));
    }

    private static void MakeWaveformSpectrogramSettings(SettingsViewModel vm)
    {
        vm.AllSettings.Add(new SettingItem("Waveform/spectrogram", SectionName.WaveformSpectrogram));

        vm.AllSettings.Add(new SettingItem("Waveform"));
        vm.AllSettings.Add(new SettingItem("Spectrogram"));
        vm.AllSettings.Add(new SettingItem("FFmpeg"));
        vm.AllSettings.Add(new SettingItem("Cleanup"));
    }

    private static void MakeVideoPlayerSettings(SettingsViewModel vm)
    {
        vm.AllSettings.Add(new SettingItem("Video player", SectionName.VideoPlayer));
        vm.AllSettings.Add(new SettingItem("Video engine"));
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
}