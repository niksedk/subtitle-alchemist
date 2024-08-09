using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Views;

namespace SubtitleAlchemist.Features.Options.DownloadFfmpeg;

public class DownloadFfmpegPopup : Popup
{
    public DownloadFfmpegPopup(DownloadFfmpegModel vm)
    {
        BindingContext = vm;

        Color = (Color)Application.Current.Resources["BackgroundColor"];

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
            WidthRequest = 500,
            HeightRequest = 300
        };

        var titleLabel = new Label
        {
            Text = "Downloading ffmpeg",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
            TextColor = (Color)Application.Current.Resources["TextColor"],
            BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"],
        };
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        var progressLabel = new Label
        {
            Text = "...",
            FontAttributes = FontAttributes.Bold,
            FontSize = 15,
            TextColor = (Color)Application.Current.Resources["TextColor"],
            BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"],
        }; //.Bind(Label.TextProperty, vm.Progress);
        progressLabel.SetBinding(Label.TextProperty, nameof(vm.Progress));
        grid.Add(progressLabel, 0, 1);
        Grid.SetColumnSpan(progressLabel, 2);

        Content = grid;

        vm.Popup = this;

        vm.StartDownload(CancellationToken.None);
    }
}