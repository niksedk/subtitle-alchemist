using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;

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
        AutoTranslate,
        Toolbar,
        Appearance,
        FileTypeAssociations,
    }

    public SettingsPage(SettingsViewModel vm)
    {
        vm.Pages.Add(PageNames.General, MakeGeneralSettingsPage(vm));
        vm.Pages.Add(PageNames.SubtitleFormats, MakeSubtitleFormatsPage(vm));
        vm.Pages.Add(PageNames.Shortcuts, MakeShortcutsPage(vm));
        vm.Pages.Add(PageNames.SyntaxColoring, MakeSyntaxColoringPage(vm));
        vm.Pages.Add(PageNames.VideoPlayer, MakeVideoPlayerPage(vm));
        vm.Pages.Add(PageNames.WaveformSpectrogram, MakeWaveformSpectrogramPage(vm));
        vm.Pages.Add(PageNames.Tools, MakeToolsPage(vm));
        //vm.Pages.Add(PageNames.AutoTranslate, MakeAutoTranslatePage(vm));
        //vm.Pages.Add(PageNames.Toolbar, MakeToolbarPage(vm));
        vm.Pages.Add(PageNames.Appearance, MakeAppearancePage(vm));
        //vm.Pages.Add(PageNames.File type associations, MakeFileTypeAssociationsPage(vm));

        BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"];

        BindingContext = vm;

        vm.Page = new Border
        {
            Stroke = Color.FromArgb("#C49B33"),
            Background = (Color)Application.Current.Resources["BackgroundColor"],
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
            BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"],
            HorizontalOptions = LayoutOptions.Fill,
        };

        var stackLayout = new VerticalStackLayout
        {
            Children =
            {
                new Label
                {
                    Margin = 5, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center,
                    FontSize = 17, Text = "General"
                }.TapGesture(vm.Tapped(PageNames.General)),
                new Label
                {
                    Margin = 5, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center,
                    FontSize = 17, Text = "Subtitle formats"
                }.TapGesture(vm.Tapped(PageNames.SubtitleFormats)),
                new Label
                {
                    Margin = 5, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center,
                    FontSize = 17, Text = "Shortcuts"
                }.TapGesture(vm.Tapped(PageNames.Shortcuts)),
                new Label
                {
                    Margin = 5, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center,
                    FontSize = 17, Text = "Syntax coloring"
                }.TapGesture(vm.Tapped(PageNames.SyntaxColoring)),
                new Label
                {
                    Margin = 5, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center,
                    FontSize = 17, Text = "Video player"
                }.TapGesture(vm.Tapped(PageNames.VideoPlayer)),
                new Label
                {
                    Margin = 5, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center,
                    FontSize = 17, Text = "Waveform/spectrogram"
                }.TapGesture(vm.Tapped(PageNames.WaveformSpectrogram)),
                new Label
                {
                    Margin = 5, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center,
                    FontSize = 17, Text = "Tools"
                }.TapGesture(vm.Tapped(PageNames.Tools)),
                new Label
                {
                    Margin = 5, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center,
                    FontSize = 17, Text = "Auto-translate"
                },
                new Label
                {
                    Margin = 5, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center,
                    FontSize = 17, Text = "Toolbar"
                },
                new Label
                {
                    Margin = 5, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center,
                    FontSize = 17, Text = "Appearance"
                }.TapGesture(vm.Tapped(PageNames.Appearance)),
                new Label
                {
                    Margin = 5, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center,
                    FontSize = 17, Text = "File type associations"
                },
            }
        };

        grid.Add(stackLayout, 0, 0);
        grid.Add(vm.Page, 1, 0);

        Content = grid;
    }

    private View MakeToolsPage(SettingsViewModel vm)
    {
        var grid = new Grid
        {
            Padding = new Thickness(20),
            RowSpacing = 20,
            ColumnSpacing = 10,
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

        var titleLabel = new Label
            { Text = "Tools settings", FontAttributes = FontAttributes.Bold, FontSize = 18 };
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        var autoBreakLabel = new Label
            { Text = "Auto break settings", FontAttributes = FontAttributes.Bold, FontSize = 16 };
        grid.Add(autoBreakLabel, 0, 1);
        Grid.SetColumnSpan(autoBreakLabel, 2);

        grid.Add(new Label { Text = "Break early for dialogs:", VerticalOptions = LayoutOptions.Center }, 0, 2);
        grid.Add(new Switch { HorizontalOptions = LayoutOptions.Start }, 1, 2);

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
            BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"],
            HorizontalOptions = LayoutOptions.Fill,
        };

        var titleLabel = new Label
            { Text = "General", FontAttributes = FontAttributes.Bold, FontSize = 18 };
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);


        // Remember Recent Files
        grid.Add(new Label { Text = "Remember Recent Files:", VerticalOptions = LayoutOptions.Center }, 0, 1);
        grid.Add(new Switch { HorizontalOptions = LayoutOptions.Start }, 1, 1);

        // Single Line Max Length
        grid.Add(new Label { Text = "Single Line Max Length:", VerticalOptions = LayoutOptions.Center }, 0, 2);
        grid.Add(new Entry
        {
            Keyboard = Keyboard.Numeric,
            Placeholder = "Enter max length",
            HorizontalOptions = LayoutOptions.Start,
        }, 1, 2);

        return grid;
    }

    private View MakeSubtitleFormatsPage(SettingsViewModel vm)
    {
        var stackLayout = new StackLayout
        {
            Padding = new Thickness(20),
            Children =
                {
                    new Label
                    {
                        Text = "Select your favorite subtitle formats:",
                        FontAttributes = FontAttributes.Bold,
                        Margin = new Thickness(0, 0, 0, 10)
                    },

                }
        };

        return stackLayout;
    }

    private View MakeShortcutsPage(SettingsViewModel vm)
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
            BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"],
            HorizontalOptions = LayoutOptions.Fill,
        };

        grid.Add(new Label { Text = "Search:", VerticalOptions = LayoutOptions.Center }, 0, 0);
        grid.Add(new Entry { Placeholder = "Enter search text", HorizontalOptions = LayoutOptions.Start }, 1, 0);

        grid.Add(new Label { Text = "Shortcuts:", VerticalOptions = LayoutOptions.Center }, 0, 1);

        return grid;
    }

    private Switch shortDurationSwitch;
    private Switch longDurationSwitch;
    private Button textTooLongColorButton;
    private BoxView textTooLongColorPreview;
    private Color textTooLongColor = Colors.LightBlue;
    private View MakeSyntaxColoringPage(SettingsViewModel vm)
    {
        var grid = new Grid
        {
            Padding = new Thickness(20),
            RowSpacing = 20,
            ColumnSpacing = 10,
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

        var titleLabel = new Label
        { Text = "Syntax coloring settings", FontAttributes = FontAttributes.Bold, FontSize = 18 };
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        grid.Add(new Label { Text = "Color if duration too short" }, 0, 1);
        shortDurationSwitch = new Switch { HorizontalOptions = LayoutOptions.Start };
        grid.Add(shortDurationSwitch, 1, 1);

        grid.Add(new Label { Text = "Color if duration too long" }, 0, 2);
        longDurationSwitch = new Switch { HorizontalOptions = LayoutOptions.Start };
        grid.Add(longDurationSwitch, 1, 2);

        grid.Add(new Label { Text = "Color if text too long" }, 0, 3);
        var colorStack = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Start };
        textTooLongColorButton = new Button { Text = "Choose Color", FontSize = 12, Padding = new Thickness(5) };
        // textTooLongColorButton.Clicked += OnChooseColorClicked;
        colorStack.Children.Add(textTooLongColorButton);
        textTooLongColorPreview = new BoxView { WidthRequest = 30, HeightRequest = 30, Color = textTooLongColor };
        colorStack.Children.Add(textTooLongColorPreview);
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

        var titleLabel = new Label
            { Text = "Video player Settings", FontAttributes = FontAttributes.Bold, FontSize = 18 };
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        grid.Add(new Label { Text = "Video player" }, 0, 1);
        grid.Add(new Picker
        {
            ItemsSource = new List<string> { "mpv", "vlc", "System Default" },
            HorizontalOptions = LayoutOptions.Start,
        }, 1, 1);

        return grid;
    }


    private View MakeWaveformSpectrogramPage(SettingsViewModel vm)
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
            BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"],
            HorizontalOptions = LayoutOptions.Fill,
        };

        // FFmpeg Location
        grid.Add(new Label { Text = "FFmpeg Location:", VerticalOptions = LayoutOptions.Center }, 0, 0);
        grid.Add(new Entry { Placeholder = "Enter FFmpeg path", HorizontalOptions = LayoutOptions.Start }, 1, 0);

        return grid;
    }

    private View MakeAppearancePage(SettingsViewModel vm)
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
            BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"],
            HorizontalOptions = LayoutOptions.Fill,
        };

        // Theme
        grid.Add(new Label { Text = "Theme:", VerticalOptions = LayoutOptions.Center }, 0, 1);
        grid.Add(new Picker
        {
            ItemsSource = new List<string> { "Light", "Dark", "System Default" },
            HorizontalOptions = LayoutOptions.Start,
        }, 1, 1);

        return grid;
    }
}