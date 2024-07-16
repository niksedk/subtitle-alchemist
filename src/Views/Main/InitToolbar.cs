using ImageButton = Microsoft.Maui.Controls.ImageButton;

namespace SubtitleAlchemist.Views.Main
{
    internal static class InitToolbar
    {
        internal static StackLayout CreateToolbarBar(MainPage page, MainViewModel vm)
        {
            var imagePath = Path.Combine("Resources", "Images", "DarkTheme");

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
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    new ImageButton
                    {
                        Source = ImageSource.FromFile(Path.Combine(imagePath,"New.png")),
                        Padding = 5,
                        WidthRequest = 16,
                        HeightRequest = 16,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        Command = vm.SubtitleNewCommand,
                    },
                    new ImageButton
                    {
                        Source = ImageSource.FromFile(Path.Combine(imagePath,"Open.png")),
                        Padding = 5,
                        WidthRequest = 16,
                        HeightRequest = 16,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        Command = vm.SubtitleOpenCommand,
                    },
                    new ImageButton
                    {
                        Source = ImageSource.FromFile(Path.Combine(imagePath,"Save.png")),
                        Padding = 5,
                        WidthRequest = 16,
                        HeightRequest = 16,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        Command = vm.SubtitleSaveCommand,
                    },
                    new ImageButton
                    {
                        Source = ImageSource.FromFile(Path.Combine(imagePath,"SaveAs.png")),
                        Padding = 5,
                        WidthRequest = 16,
                        HeightRequest = 16,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        Command = vm.SubtitleSaveAsCommand,
                    },
                    new ImageButton
                    {
                        Source = ImageSource.FromFile(Path.Combine(imagePath,"Find.png")),
                        Padding = 5,
                        WidthRequest = 16,
                        HeightRequest = 16,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                    },
                    new ImageButton
                    {
                        Source = ImageSource.FromFile(Path.Combine(imagePath,"Replace.png")),
                        Padding = 5,
                        WidthRequest = 16,
                        HeightRequest = 16,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                    },
                    new ImageButton
                    {
                        Source = ImageSource.FromFile(Path.Combine(imagePath,"Help.png")),
                        Padding = 5,
                        WidthRequest = 16,
                        HeightRequest = 16,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                    },
                    new ImageButton
                    {
                        Source = ImageSource.FromFile(Path.Combine(imagePath,"Layout.png")),
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
}


