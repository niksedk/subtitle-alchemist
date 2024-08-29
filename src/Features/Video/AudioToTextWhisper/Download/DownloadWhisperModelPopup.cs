using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Video.AudioToTextWhisper.Download;

public class DownloadWhisperModelPopup : Popup
{

    public DownloadWhisperModelPopup(DownloadWhisperModelPopupModel vm)
    {
        BindingContext = vm;

        this.BindDynamicTheme();
        CanBeDismissedByTappingOutsideOfPopup = false;

        var grid = new Grid
        {
            RowDefinitions =
            {
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
            },
            Margin = new Thickness(2),
            Padding = new Thickness(30, 20, 30, 10),
            RowSpacing = 20,
            ColumnSpacing = 10,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();

        var titleLabel = new Label
        {
            Text = "Download Whisper model",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
            Padding = new Thickness(0, 0, 0, 10),
        }.BindDynamicTheme();
        grid.Add(titleLabel, 0, 0);



        vm.ModelPicker = new Picker
        {
            Title = "Select model",
            HorizontalOptions = LayoutOptions.Fill,
            ItemsSource = vm.Models,
        }.BindDynamicTheme();

        vm.ModelPicker.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedModel));

        grid.Add(vm.ModelPicker, 0, 1);

        var buttonDownload = new Button
        {
            Text = "Download",
            HorizontalOptions = LayoutOptions.Center,
            Command = vm.DownloadCommand,
        }.BindDynamicTheme();
        grid.Add(buttonDownload, 1, 1);


        var progressLabel = new Label
        {
            Text = "...",
            FontAttributes = FontAttributes.Bold,
            FontSize = 15,
        }.BindDynamicTheme();
        progressLabel.SetBinding(Label.TextProperty, nameof(vm.Progress));
        grid.Add(progressLabel, 0, 2);
        grid.SetColumnSpan(progressLabel, 2);

        vm.ProgressBar  = new ProgressBar
        {
            Progress = 0.5,
            ProgressColor = Colors.Orange,
            HorizontalOptions = LayoutOptions.Fill,
        };
        vm.ProgressBar.SetBinding(ProgressBar.ProgressProperty, nameof(vm.ProgressValue));
        grid.Add(vm.ProgressBar, 0, 3);
        grid.SetColumnSpan(vm.ProgressBar, 2);
        vm.ProgressBar.IsVisible = false;

        var cancelButton = new Button
        {
            Text = "Cancel",
            HorizontalOptions = LayoutOptions.Center,
            Command = vm.CancelCommand,
            Margin = new Thickness(5, 15, 5, 5),
        }.BindDynamicTheme();
        grid.Add(cancelButton, 0, 4);
        grid.SetColumnSpan(cancelButton, 2);

        var border = new Border
        {
            StrokeThickness = 1,
            Padding = new Thickness(4, 1, 1, 0),
            Margin = new Thickness(2),
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
            Content = grid,
            HeightRequest = 275,
        }.BindDynamicTheme();

        Content = border;


        vm.Popup = this;
    }
}
