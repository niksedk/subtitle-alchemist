using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Features.Options.DownloadFfmpeg;

public class DownloadFfmpegPopup : Popup
{
    public DownloadFfmpegPopup(DownloadFfmpegModel vm)
    {
        BindingContext = vm;

        Color = (Color)Application.Current!.Resources[ThemeNames.BackgroundColor];
        CanBeDismissedByTappingOutsideOfPopup = false;

        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
            },
            Margin = new Thickness(2),
            Padding = new Thickness(30, 20, 30 ,10),
            RowSpacing = 20,
            ColumnSpacing = 10,
            BackgroundColor = (Color)Application.Current.Resources[ThemeNames.BackgroundColor],
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        };

        var titleLabel = new Label
        {
            Text = "Downloading ffmpeg",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
            TextColor = (Color)Application.Current.Resources[ThemeNames.TextColor],
            BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.BackgroundColor],
            Padding = new Thickness(0, 0, 0, 10),
        };
        grid.Add(titleLabel, 0, 0);

        var progressLabel = new Label
        {
            Text = "...",
            FontAttributes = FontAttributes.Bold,
            FontSize = 15,
            TextColor = (Color)Application.Current.Resources[ThemeNames.TextColor],
            BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.BackgroundColor],
        }; //.Bind(Label.TextProperty, vm.Progress);
        progressLabel.SetBinding(Label.TextProperty, nameof(vm.Progress));
        grid.Add(progressLabel, 0, 1);

        var progressBar = new ProgressBar
        {
            Progress = 0.5,
            ProgressColor = Colors.Orange,
            HorizontalOptions = LayoutOptions.Fill,
        };
        progressBar.SetBinding(ProgressBar.ProgressProperty, nameof(vm.ProgressValue));
        grid.Add(progressBar, 0, 2);

        var cancelButton = new Button
        {
            Text = "Cancel",
            BackgroundColor = Colors.DarkGray, // (Color)Application.Current.Resources["BackgroundColor"],
            TextColor = (Color)Application.Current.Resources[ThemeNames.TextColor],
            HorizontalOptions = LayoutOptions.Center,
            Command = vm.CancelCommand,
        };
        grid.Add(cancelButton, 0, 3);

        var border = new Border
        {
            Stroke = (Color)Application.Current.Resources[ThemeNames.TextColor], // change to blue when focused
            Background = (Color)Application.Current!.Resources[ThemeNames.BackgroundColor],
            StrokeThickness = 1,
            Padding = new Thickness(4, 1, 1, 0),
            Margin = new Thickness(2),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
            Content = grid,
        };

        Content = border;

        vm.Popup = this;

        vm.StartDownload();
    }
}