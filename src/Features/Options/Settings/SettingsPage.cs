namespace SubtitleAlchemist.Features.Options.Settings;

public class SettingsPage : ContentPage
{
    public SettingsPage(SettingsViewModel vm)
    {
        BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"];
        BindingContext = vm;
        Content = new HorizontalStackLayout
        {
            Children =
            {

                new VerticalStackLayout
                {
                    WidthRequest = 300,
                    Children =
                    {

                        new Label { Margin = 5, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, FontSize = 17, Text = "General" },
                        new Label { Margin = 5,HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, FontSize = 17, Text = "Subtitle formats" },
                        new Label { Margin = 5,HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, FontSize = 17, Text = "Shortcuts" },
                        new Label { Margin = 5,HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, FontSize = 17, Text = "Syntax coloring" },
                        new Label { Margin = 5,HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, FontSize = 17, Text = "Video player" },
                        new Label { Margin = 5,HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, FontSize = 17, Text = "Waveform/spectrogram" },
                        new Label { Margin = 5,HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, FontSize = 17, Text = "Tools" },
                        new Label { Margin = 5,HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, FontSize = 17, Text = "Auto-translate" },
                        new Label { Margin = 5,HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, FontSize = 17, Text = "Toolbar" },
                        new Label { Margin = 5,HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, FontSize = 17, Text = "Appearance" },
                        new Label { Margin = 5,HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, FontSize = 17, Text = "File type associations" },
                    }
                },
                new VerticalStackLayout
                {
                    Children =
                    {
                        new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, Text = "Welcome to .NET MAUI!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" },
                        new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, Text = "Welcome to .NET MAUI1!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" },
                    }
                },
            }
        };
    }
}