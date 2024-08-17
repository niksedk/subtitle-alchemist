using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Features.Options.Settings;

public class SettingsPage : ContentPage
{
    public enum PageNames
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

        vm.Pages.Add(PageNames.General, MakeGeneralSettingsPage(vm));
        vm.Pages.Add(PageNames.SubtitleFormats, MakeSubtitleFormatsPage(vm));
        vm.Pages.Add(PageNames.Shortcuts, MakeShortcutsPage(vm));
        vm.Pages.Add(PageNames.SyntaxColoring, MakeSyntaxColoringPage(vm));
        vm.Pages.Add(PageNames.VideoPlayer, MakeVideoPlayerPage(vm));
        vm.Pages.Add(PageNames.WaveformSpectrogram, MakeWaveformSpectrogramPage(vm));
        vm.Pages.Add(PageNames.Tools, MakeToolsPage(vm));
        vm.Pages.Add(PageNames.Toolbar, MakeToolbarPage(vm));
        vm.Pages.Add(PageNames.Appearance, MakeAppearancePage(vm));
        vm.Pages.Add(PageNames.FileTypeAssociations, MakeFileTypeAssociationsPage(vm));
        ThemeHelper.SetDynamicTheme(this);

        BindingContext = vm;

        var border = 
        vm.Page = new Border
        {
            StrokeThickness = 1,
            Padding = new Thickness(1),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(2),
            },
            Content = vm.Pages[PageNames.General],
        };
        ThemeHelper.SetDynamicTheme(vm.Page);

        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Star },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Star }
            },
            Padding = new Thickness(20),
            RowSpacing = 20,
            ColumnSpacing = 10,
            HorizontalOptions = LayoutOptions.Fill,
        };
        ThemeHelper.SetDynamicTheme(grid);

        vm.LeftMenu = new VerticalStackLayout
        {
            Children =
            {
                MakeLeftMenuItem(vm, PageNames.General, "General"),
                MakeLeftMenuItem(vm, PageNames.SubtitleFormats, "Subtitle formats"),
                MakeLeftMenuItem(vm, PageNames.Shortcuts, "Shortcuts"),
                MakeLeftMenuItem(vm, PageNames.SyntaxColoring, "Syntax coloring"),
                MakeLeftMenuItem(vm, PageNames.VideoPlayer, "Video player"),
                MakeLeftMenuItem(vm, PageNames.WaveformSpectrogram, "Waveform/spectrogram"),
                MakeLeftMenuItem(vm, PageNames.Tools, "Tools"),
                MakeLeftMenuItem(vm, PageNames.Toolbar, "Toolbar"),
                MakeLeftMenuItem(vm, PageNames.Appearance, "Appearance"),
                MakeLeftMenuItem(vm, PageNames.FileTypeAssociations, "File type associations"),
            }
        };

        grid.Add(vm.LeftMenu, 0, 0);
        grid.Add(vm.Page, 1, 0);

        Content = grid;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _vm.SaveSettings();
    }

    private static IView MakeLeftMenuItem(SettingsViewModel vm, PageNames pageName, string text)
    {
        var label = new Label
        {
            Margin = 5,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            FontSize = 17,
            Text = text,
            ClassId = pageName.ToString(),
        };
        ThemeHelper.SetDynamicTheme(label);

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += async (sender, e) => await vm.LeftMenuTapped(sender, e, pageName);
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
        };
        ThemeHelper.SetDynamicTheme(grid);

        var titleLabel = new Label
        {
            Text = "Tools settings",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
        };
        ThemeHelper.SetDynamicTheme(titleLabel);
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        var autoBreakLabel = new Label
        {
            Text = "Auto break settings",
            FontAttributes = FontAttributes.Bold,
            FontSize = 16,
        };
        ThemeHelper.SetDynamicTheme(autoBreakLabel);
        grid.Add(autoBreakLabel, 0, 1);
        Grid.SetColumnSpan(autoBreakLabel, 2);

        var labelBreakEarly = new Label
        {
            Text = "Break early for dialogs:",
            VerticalOptions = LayoutOptions.Center,
        };
        ThemeHelper.SetDynamicTheme(labelBreakEarly);
        grid.Add(labelBreakEarly, 0, 2);

        var switchBreakEarly = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        };
        ThemeHelper.SetDynamicTheme(switchBreakEarly);

        grid.Add(switchBreakEarly, 1, 2);

        return grid;
    }

    private static View MakeGeneralSettingsPage(SettingsViewModel vm)
    {
        var grid = new Grid
        {
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
            },
            Padding = new Thickness(20),
            RowSpacing = 20,
            ColumnSpacing = 10,
            HorizontalOptions = LayoutOptions.Fill,
        };
        ThemeHelper.SetDynamicTheme(grid);

        var titleLabel = new Label
        {
            Text = "General",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
        };
        ThemeHelper.SetDynamicTheme(titleLabel);
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        // Remember Recent Files
        var labelRecentFiles = new Label
        {
            Text = "Remember Recent Files:",
            VerticalOptions = LayoutOptions.Center,
        };
        grid.Add(labelRecentFiles, 0, 1);
        ThemeHelper.SetDynamicTheme(labelRecentFiles);

        var switchRecentFiles = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        };
        ThemeHelper.SetDynamicTheme(switchRecentFiles);
        grid.Add(switchRecentFiles, 1, 1);

        // Single Line Max Length
        var labelMaxLength = new Label
        {
            Text = "Single Line Max Length:",
            VerticalOptions = LayoutOptions.Center,
        };
        grid.Add(labelMaxLength, 0, 2);
        ThemeHelper.SetDynamicTheme(labelMaxLength);

        var entryMaxLength = new Entry
        {
            Keyboard = Keyboard.Numeric,
            Placeholder = "Enter max length",
            HorizontalOptions = LayoutOptions.Start,
        };
        grid.Add(entryMaxLength, 1, 2);
        ThemeHelper.SetDynamicTheme(entryMaxLength);

        return grid;
    }

    private static View MakeSubtitleFormatsPage(SettingsViewModel vm)
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
        };
        grid.SetDynamicResource(BackgroundColorProperty, ThemeNames.BackgroundColor);

        var titleLabel = new Label
        {
            Text = "Subtitle formats",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
        };
        ThemeHelper.SetDynamicTheme(titleLabel);
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        var labelFavoriteSubtitleFormats = new Label
        {
            Text = "Select your favorite subtitle formats",
            VerticalOptions = LayoutOptions.Center,
        };
        ThemeHelper.SetDynamicTheme(labelFavoriteSubtitleFormats);
        grid.Add(labelFavoriteSubtitleFormats, 0, 1);

        return grid;
    }

    private static View MakeShortcutsPage(SettingsViewModel vm)
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
        };
        ThemeHelper.SetDynamicTheme(grid);

        var titleLabel = new Label
        {
            Text = "Shortcuts",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
        };
        ThemeHelper.SetDynamicTheme(titleLabel);
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        var labelSearch = new Label
        {
            Text = "Search:",
            VerticalOptions = LayoutOptions.Center,
        };
        grid.Add(labelSearch, 0, 1);
        ThemeHelper.SetDynamicTheme(labelSearch);

        var entrySearch = new Entry
        {
            Placeholder = "Enter search text",
            HorizontalOptions = LayoutOptions.Start,
        };
        ThemeHelper.SetDynamicTheme(entrySearch);
        grid.Add(entrySearch, 1, 1);

        var labelShortcuts = new Label
        {
            Text = "Shortcuts:",
            VerticalOptions = LayoutOptions.Center,
        };
        ThemeHelper.SetDynamicTheme(labelShortcuts);
        grid.Add(labelShortcuts, 0, 2);

        return grid;
    }

    private Switch? _shortDurationSwitch;
    private Switch? _longDurationSwitch;
    private Button? _textTooLongColorButton;
    private readonly Color _textTooLongColor = Colors.LightBlue;
    private View MakeSyntaxColoringPage(SettingsViewModel vm)
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
        };
        ThemeHelper.SetDynamicTheme(grid);

        var titleLabel = new Label
        {
            Text = "Syntax coloring ",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
        };
        ThemeHelper.SetDynamicTheme(titleLabel);
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        grid.Add(new Label
        {
            Text = "Color if duration too short",
            TextColor = (Color)Application.Current!.Resources[ThemeNames.TextColor],
        }, 0, 1);
        _shortDurationSwitch = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
            OnColor = (Color)Application.Current.Resources[ThemeNames.TextColor],
        };
        grid.Add(_shortDurationSwitch, 1, 1);

        grid.Add(new Label
        {
            Text = "Color if duration too long",
            TextColor = (Color)Application.Current!.Resources[ThemeNames.TextColor],
        }, 0, 2);
        _longDurationSwitch = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
            OnColor = (Color)Application.Current.Resources[ThemeNames.TextColor],
        };
        grid.Add(_longDurationSwitch, 1, 2);

        grid.Add(new Label
        {
            Text = "Color if text too long",
            TextColor = (Color)Application.Current!.Resources[ThemeNames.TextColor],
        }, 0, 3);
        var colorStack = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
        };
        _textTooLongColorButton = new Button
        {
            Text = "Choose Color",
            FontSize = 12,
            Padding = new Thickness(5),
            Command = vm.PickSyntaxErrorColorCommand,
        };
        _textTooLongColorButton.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.SecondaryBackgroundColor);
        _textTooLongColorButton.SetDynamicResource(Button.TextColorProperty, ThemeNames.TextColor);

        colorStack.Children.Add(_textTooLongColorButton);
        vm.SyntaxErrorColorBox = new BoxView
        {
            WidthRequest = 30,
            HeightRequest = 30,
            Color = _textTooLongColor,
            Margin = new Thickness(10, 0, 0, 0),
        };
        colorStack.Children.Add(vm.SyntaxErrorColorBox);
        grid.Add(colorStack, 1, 3);

        return grid;
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
        };
        grid.SetDynamicResource(BackgroundColorProperty, ThemeNames.BackgroundColor);

        var titleLabel = new Label
        {
            Text = "Video player",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
            TextColor = (Color)Application.Current!.Resources[ThemeNames.TextColor],
        };
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        grid.Add(new Label
        {
            Text = "Video player",
            TextColor = (Color)Application.Current!.Resources[ThemeNames.TextColor],
        }, 0, 1);
        grid.Add(new Picker
        {
            ItemsSource = new List<string> { "mpv", "vlc", "System Default" },
            HorizontalOptions = LayoutOptions.Start,
            TextColor = (Color)Application.Current.Resources["TextColor"],
        }, 1, 1);

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
        };
        grid.SetDynamicResource(BackgroundColorProperty, ThemeNames.BackgroundColor);

        var titleLabel = new Label
        {
            Text = "Waveform/spectrogram",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
            TextColor = (Color)Application.Current!.Resources[ThemeNames.TextColor],
        };
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        // FFmpeg Location
        grid.Add(new Label
        {
            Text = "FFmpeg Location:",
            VerticalOptions = LayoutOptions.Center,
            TextColor = (Color)Application.Current!.Resources[ThemeNames.TextColor],
        }, 0, 1);
        grid.Add(new Entry
        {
            Placeholder = "Enter FFmpeg path",
            HorizontalOptions = LayoutOptions.Start,
            TextColor = (Color)Application.Current.Resources["TextColor"],
            WidthRequest = 500,
            BindingContext = vm,
        }.Bind(nameof(vm.FfmpegPath)), 1, 1);
        var ffmpegBrowse = new ImageButton
        {
            Source = "open.png",
            HorizontalOptions = LayoutOptions.Start,
            WidthRequest = 30,
            HeightRequest = 30,
            Padding = new Thickness(10, 5, 5, 5),
        };
        ffmpegBrowse.SetDynamicResource(ImageButton.BackgroundColorProperty, ThemeNames.BackgroundColor);

        ffmpegBrowse.Clicked += async (sender, e) => await vm.BrowseForFfmpeg(sender, e);
        ToolTipProperties.SetText(ffmpegBrowse, "Browse for ffmpeg executable");
        grid.Add(ffmpegBrowse, 2, 1);
        var ffmpegDownloadButton = new ImageButton
        {
            Source = "download.png",
            BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.BackgroundColor],
            HorizontalOptions = LayoutOptions.Start,
            WidthRequest = 30,
            HeightRequest = 30,
            Padding = new Thickness(5, 5, 5, 5),
        };
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
        };
        grid.SetDynamicResource(BackgroundColorProperty, ThemeNames.BackgroundColor);

        var titleLabel = new Label
        {
            Text = "Toolbar",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
        };
        ThemeHelper.SetDynamicTheme(titleLabel);
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        grid.Add(new Label
        {
            Text = "Show \"File new\" icon",
            TextColor = (Color)Application.Current!.Resources[ThemeNames.TextColor],
        }, 0, 1);
        _shortDurationSwitch = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
            OnColor = (Color)Application.Current.Resources["TextColor"],
        };
        grid.Add(_shortDurationSwitch, 1, 1);

        grid.Add(new Label
        {
            Text = "Show \"File Save\" icon",
            TextColor = (Color)Application.Current!.Resources[ThemeNames.TextColor],
        }, 0, 2);
        _longDurationSwitch = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
            OnColor = (Color)Application.Current.Resources["TextColor"],
        };
        grid.Add(_longDurationSwitch, 1, 2);

        grid.Add(new Label
        {
            Text = "Show \"File Save as...\" icon",
            TextColor = (Color)Application.Current!.Resources[ThemeNames.TextColor],
        }, 0, 3);
        _longDurationSwitch = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
            OnColor = (Color)Application.Current.Resources["TextColor"],
        };
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
        };
        grid.SetDynamicResource(BackgroundColorProperty, ThemeNames.BackgroundColor);

        var titleLabel = new Label
        {
            Text = "Appearance",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
        };
        ThemeHelper.SetDynamicTheme(titleLabel);
        Grid.SetColumnSpan(titleLabel, 2);

        // Theme
        var themeLabel = new Label
        {
            Text = "Theme:",
            VerticalOptions = LayoutOptions.Center,
        };
        ThemeHelper.SetDynamicTheme(themeLabel);

        grid.Add(themeLabel, 0, 1);
        var picker = new Picker
        {
            ItemsSource = new List<string> { "Light", "Dark", "System Default" },
            HorizontalOptions = LayoutOptions.Start,
        };
        ThemeHelper.SetDynamicTheme(picker);
        picker.SelectedIndexChanged += vm.ThemeChanged;
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
        };
        grid.SetDynamicResource(BackgroundColorProperty, ThemeNames.BackgroundColor);

        var titleLabel = new Label
        {
            Text = "File type associations",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
            TextColor = (Color)Application.Current!.Resources[ThemeNames.TextColor],
        };
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        grid.Add(new Label
        {
            Text = "SubRip (.srt)",
            TextColor = (Color)Application.Current!.Resources[ThemeNames.TextColor],
        }, 0, 1);
        _shortDurationSwitch = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
            OnColor = (Color)Application.Current.Resources["TextColor"],
        };
        grid.Add(_shortDurationSwitch, 1, 1);

        grid.Add(new Label
        {
            Text = "Advanced Sub Station Alpha (.ass)",
            TextColor = (Color)Application.Current!.Resources[ThemeNames.TextColor],
        }, 0, 2);
        _longDurationSwitch = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
            OnColor = (Color)Application.Current.Resources["TextColor"],

        };
        grid.Add(_longDurationSwitch, 1, 2);

        grid.Add(new Label
        {
            Text = "EBU STL (.stl)",
            TextColor = (Color)Application.Current!.Resources[ThemeNames.TextColor],
        }, 0, 3);
        _longDurationSwitch = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
            OnColor = (Color)Application.Current.Resources["TextColor"],
        };
        grid.Add(_longDurationSwitch, 1, 3);

        return grid;
    }
}