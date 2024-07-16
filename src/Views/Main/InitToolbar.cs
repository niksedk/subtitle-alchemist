using ImageButton = Microsoft.Maui.Controls.ImageButton;

namespace SubtitleAlchemist.Views.Main
{
    internal static class InitToolbar
    {
        internal static StackLayout CreateToolbarBar(MainPage page, MainViewModel viewModel)
        {
            var imagePath = Path.Combine("Resources", "Images", "DarkTheme");

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
                        Command = new Command(viewModel.SubtitleNew)
                    },
                    new ImageButton
                    {
                        Source = ImageSource.FromFile(Path.Combine(imagePath,"Open.png")),
                        Padding = 5,
                        WidthRequest = 16,
                        HeightRequest = 16,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        Command = new Command(async () => await viewModel.SubtitleOpen())
                    },
                    new ImageButton
                    {
                        Source = ImageSource.FromFile(Path.Combine(imagePath,"Save.png")),
                        Padding = 5,
                        WidthRequest = 16,
                        HeightRequest = 16,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                    },
                    new ImageButton
                    {
                        Source = ImageSource.FromFile(Path.Combine(imagePath,"SaveAs.png")),
                        Padding = 5,
                        WidthRequest = 16,
                        HeightRequest = 16,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        Command = viewModel.SubtitleSaveAsCommand,
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
                        Command = new Command(async () => await viewModel.ShowLayoutPicker())
                    },
                    new Label
                    {
                        Text = "Format",
                        VerticalTextAlignment = TextAlignment.Center,
                        Padding = new Thickness(30, 0, 0, 7),
                        HeightRequest = 16,
                    },
                    new Picker
                    {
                        Items = { "SubRip", "Advanced Sub Station Alpha", "EBU stl", "PAC", "More..." },
                        SelectedIndex = 0,
                        WidthRequest = 100,
                        HeightRequest = 16,
                    },
                    new Label
                    {
                        Text = "Encoding",
                        VerticalTextAlignment = TextAlignment.Center,
                        Padding = new Thickness(30, 0, 0, 7),
                        HeightRequest = 16,
                    },
                    new Picker
                    {
                        Items = { "UTF-8", "UTF-8 with BOM", "More..." },
                        SelectedIndex = 0,
                        WidthRequest = 100,
                        HeightRequest = 16,
                    },
                }
            };

            return stackLayout;
        }
    }
}


