using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Controls.RadialControl;
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
        this.BindDynamicTheme();

        vm.Theme = Preferences.Get("Theme", "Dark");

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

        BindingContext = vm;

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
        }.BindDynamicTheme();

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
        }.BindDynamicTheme();

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
        }.BindDynamicTheme();

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
        }.BindDynamicTheme();

        var titleLabel = new Label
        {
            Text = "General",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
        }.BindDynamicTheme();
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        // Remember Recent Files
        var labelRecentFiles = new Label
        {
            Text = "Remember Recent Files:",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelRecentFiles, 0, 1);

        var switchRecentFiles = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        grid.Add(switchRecentFiles, 1, 1);

        // Single Line Max Length
        var labelMaxLength = new Label
        {
            Text = "Single Line Max Length:",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelMaxLength, 0, 2);

        var entryMaxLength = new Entry
        {
            Keyboard = Keyboard.Numeric,
            Placeholder = "Enter max length",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        grid.Add(entryMaxLength, 1, 2);

        var radial = new RadialView()
        {
            WidthRequest = 400,
            HeightRequest = 400,
        };
        radial.AddElement("C:\\git\\subtitle-alchemist\\src\\Resources\\Images\\theme_dark_error.png");
        radial.AddElement("C:\\git\\subtitle-alchemist\\src\\Resources\\Images\\theme_dark_information.png");
        radial.AddElement("C:\\git\\subtitle-alchemist\\src\\Resources\\Images\\theme_dark_question.png");
        radial.AddElement("C:\\git\\subtitle-alchemist\\src\\Resources\\Images\\theme_dark_error.png");
        radial.AddElement("C:\\git\\subtitle-alchemist\\src\\Resources\\Images\\theme_dark_information.png");
        radial.AddElement("C:\\git\\subtitle-alchemist\\src\\Resources\\Images\\theme_dark_question.png");
        radial.AddElement("C:\\git\\subtitle-alchemist\\src\\Resources\\Images\\theme_dark_error.png");
        radial.AddElement("C:\\git\\subtitle-alchemist\\src\\Resources\\Images\\theme_dark_information.png");
        radial.AddElement("C:\\git\\subtitle-alchemist\\src\\Resources\\Images\\theme_dark_question.png");
        radial.AddElement("C:\\git\\subtitle-alchemist\\src\\Resources\\Images\\theme_dark_error.png");
        radial.AddElement("C:\\git\\subtitle-alchemist\\src\\Resources\\Images\\theme_dark_information.png");
        radial.AddElement("C:\\git\\subtitle-alchemist\\src\\Resources\\Images\\theme_dark_question.png");
        radial.CenterImage = new RadialElement()
        {
            ImageUrl = "C:\\git\\subtitle-alchemist\\src\\Resources\\Images\\wheel.png",
        };
        grid.Add(radial, 0, 3);
        Grid.SetColumnSpan(radial, 2);


        var leftButton = new Button { Text = "Rotate Left" }.BindDynamicTheme();
        var rightButton = new Button { Text = "Rotate Right" }.BindDynamicTheme();

        leftButton.Clicked += (s, e) => radial.RotateLeft();
        rightButton.Clicked += (s, e) => radial.RotateRight();

        var buttonLayout = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Children = { leftButton, rightButton }
        };

        grid.Add(buttonLayout, 0, 4);
        Grid.SetColumnSpan(buttonLayout, 2);


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
        }.BindDynamicTheme();

        var titleLabel = new Label
        {
            Text = "Subtitle formats",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
        }.BindDynamicTheme();
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        var labelFavoriteSubtitleFormats = new Label
        {
            Text = "Select your favorite subtitle formats",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
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
        }.BindDynamicTheme();

        var titleLabel = new Label
        {
            Text = "Shortcuts",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
        }.BindDynamicTheme();
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        var labelSearch = new Label
        {
            Text = "Search:",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelSearch, 0, 1);

        var entrySearch = new Entry
        {
            Placeholder = "Enter search text",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        grid.Add(entrySearch, 1, 1);

        var labelShortcuts = new Label
        {
            Text = "Shortcuts:",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
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
        }.BindDynamicTheme();

        var titleLabel = new Label
        {
            Text = "Syntax coloring ",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
        }.BindDynamicTheme();
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        grid.Add(new Label
        {
            Text = "Color if duration too short",
        }.BindDynamicTheme(), 0, 1);
        _shortDurationSwitch = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        grid.Add(_shortDurationSwitch, 1, 1);

        grid.Add(new Label
        {
            Text = "Color if duration too long",
        }.BindDynamicTheme(), 0, 2);
        _longDurationSwitch = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        grid.Add(_longDurationSwitch, 1, 2);

        grid.Add(new Label
        {
            Text = "Color if text too long",
        }.BindDynamicTheme(), 0, 3);
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
        }.BindDynamicTheme();

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

        picker.SelectedIndexChanged += async(o, args) => await vm.ThemeChanged(o, args);
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