using SubtitleAlchemist.Logic.Constants;
using ImageButton = Microsoft.Maui.Controls.ImageButton;

namespace SubtitleAlchemist.Features.Main;

internal static class InitToolbar
{
    internal static StackLayout CreateToolbarBar(MainPage page, MainViewModel vm)
    {
        var imagePrefix = "theme_dark_";

        vm.SubtitleFormatPicker = new Picker
        {
            ItemsSource = MainViewModel.SubtitleFormatNames,
            SelectedIndex = 0,
            WidthRequest = 225,
            HeightRequest = 16,
            Margin = new Thickness(5, 0),
        };

        vm.EncodingPicker = new Picker
        {
            ItemsSource = MainViewModel.EncodingNames,
            SelectedIndex = 0,
            WidthRequest = 225,
            HeightRequest = 16,
            Margin = new Thickness(5, 0),
        };

        var stackLayout = new StackLayout
        {
            BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.BackgroundColor],
            Orientation = StackOrientation.Horizontal,
            Children =
            {
                new ImageButton
                {
                    BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.BackgroundColor],
                    Source = $"{imagePrefix}new.png",
                    Padding = 5,
                    WidthRequest = 16,
                    HeightRequest = 16,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Command = vm.SubtitleNewCommand,
                },
                new ImageButton
                {
                    Source = $"{imagePrefix}open.png",
                    Padding = 5,
                    WidthRequest = 16,
                    HeightRequest = 16,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Command = vm.SubtitleOpenCommand,
                },
                new ImageButton
                {
                    Source = $"{imagePrefix}save.png",
                    Padding = 5,
                    WidthRequest = 16,
                    HeightRequest = 16,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Command = vm.SubtitleSaveCommand,
                },
                new ImageButton
                {
                    Source = $"{imagePrefix}save_as.png",
                    Padding = 5,
                    WidthRequest = 16,
                    HeightRequest = 16,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Command = vm.SubtitleSaveAsCommand,
                },
                new ImageButton
                {
                    Source = $"{imagePrefix}find.png",
                    Padding = 5,
                    WidthRequest = 16,
                    HeightRequest = 16,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                },
                new ImageButton
                {
                    Source = $"{imagePrefix}replace.png",
                    Padding = 5,
                    WidthRequest = 16,
                    HeightRequest = 16,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                },
                new ImageButton
                {
                    Source = $"{imagePrefix}help.png",
                    Padding = 5,
                    WidthRequest = 16,
                    HeightRequest = 16,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                },
                new ImageButton
                {
                    Source = $"{imagePrefix}layout.png",
                    Padding = 5,
                    WidthRequest = 16,
                    HeightRequest = 16,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Command = vm.ShowLayoutPickerCommand,
                },
                new Label
                {
                    Text = "Format",
                    VerticalTextAlignment = TextAlignment.Center,
                    Padding = new Thickness(30, 0, 0, 7),
                    HeightRequest = 16,
                },
                vm.SubtitleFormatPicker,
                new Label
                {
                    Text = "Encoding",
                    VerticalTextAlignment = TextAlignment.Center,
                    Padding = new Thickness(30, 0, 0, 7),
                    HeightRequest = 16,
                },
                vm.EncodingPicker,
            }
        };

        return stackLayout;
    }
}