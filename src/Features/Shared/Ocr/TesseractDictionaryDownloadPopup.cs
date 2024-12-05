using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Features.Shared.Ocr;

public sealed class TesseractDictionaryDownloadPopup : Popup
{
    public TesseractDictionaryDownloadPopup(TesseractDictionaryDownloadPopupModel vm)
    {
        BindingContext = vm;

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
            },
            Margin = new Thickness(2),
            Padding = new Thickness(30, 20, 30, 10),
            RowSpacing = 20,
            ColumnSpacing = 10,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            HeightRequest = 300,
        }.BindDynamicTheme();

        var titleLabel = new Label
        {
            Text = "Download Tesseract model",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
            Padding = new Thickness(0, 0, 0, 10),
        }.BindDynamicTheme().AsTitle();

        grid.Add(titleLabel, 0, 0);

        var pickerDictionaries = new Picker
        {
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
        }
        .BindDynamicTheme()
        .Bind(nameof(vm.TesseractDictionaryItems), nameof(vm.SelectedTesseractDictionaryItem))
        .BindIsEnabled(nameof(vm.IsPickerAndDownloadButtonEnabled));

        var buttonDownload = new Button
        {
            Text = "Download",
            VerticalOptions = LayoutOptions.Center,
            Command = vm.DownloadCommand,
            Margin = new Thickness(0, 0, 0, 0),
        }.BindDynamicTheme().BindIsEnabled(nameof(vm.IsPickerAndDownloadButtonEnabled));

        var stackDictionary = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Spacing = 10,
            Children =
            {
                new Label { 
                    Text = "Select model", 
                    VerticalOptions = LayoutOptions.Center,             
                    Margin = new Thickness(0, 0, 0, 0),
                }.BindDynamicTheme(),
                pickerDictionaries,
                buttonDownload,
            },
        }.BindDynamicTheme();

        grid.Add(stackDictionary, 0, 1);

        var progressLabel = new Label
        {
            Text = "...",
            FontAttributes = FontAttributes.Bold,
        }.BindDynamicTheme().BindIsVisible(nameof(vm.IsProgressVisible));
        progressLabel.SetBinding(Label.TextProperty, nameof(vm.Progress));
        grid.Add(progressLabel, 0, 2);

        var progressBar = new ProgressBar
        {
            Progress = 0.5,
            ProgressColor = (Color)Application.Current!.Resources[ThemeNames.ProgressColor],
            HorizontalOptions = LayoutOptions.Fill,
        }.BindIsVisible(nameof(vm.IsProgressVisible));
        progressBar.SetBinding(ProgressBar.ProgressProperty, nameof(vm.ProgressValue));
        grid.Add(progressBar, 0, 3);

        var cancelButton = new Button
        {
            Text = "Cancel",
            HorizontalOptions = LayoutOptions.Center,
            Command = vm.CancelCommand,
        }.BindDynamicTheme();
        grid.Add(cancelButton, 0, 4);

        var border = new Border
        {
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
        }.BindDynamicTheme();

        Content = border;

        vm.Popup = this;
    }
}
