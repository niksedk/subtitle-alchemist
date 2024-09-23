using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.SpellCheck;

public sealed class GetDictionaryPopup : Popup
{
    public GetDictionaryPopup(GetDictionaryPopupModel vm)
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
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            WidthRequest = 400,
            HeightRequest = 340,
        }.BindDynamicTheme();

        var titleLabel = new Label
        {
            Text = "Get dictionaries",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
            Padding = new Thickness(0, 0, 0, 10),
        }.BindDynamicTheme();
        grid.Add(titleLabel, 0);

        var pickerDictionaries = new Picker
        {
            Title = "Select dictionary",
            SelectedItem = vm.Dictionaries.FirstOrDefault(),
            HorizontalOptions = LayoutOptions.Fill,
            WidthRequest = 250,
        }.BindDynamicTheme();
        pickerDictionaries.SetBinding(Picker.ItemsSourceProperty, nameof(vm.Dictionaries), BindingMode.TwoWay);
        pickerDictionaries.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedDictionary));
        pickerDictionaries.SelectedIndexChanged += vm.SelectedIndexChanged;

        var buttonStartDownload = new ImageButton
        {
            Source = "download.png",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.End,
            WidthRequest = 30,
            HeightRequest = 30,
            Command = vm.DownloadCommand,
            Margin = new Thickness(15, 0, 0, 0),
        }.BindDynamicTheme();
        ToolTipProperties.SetText(buttonStartDownload, "Download");

        var folderBrowseButton = new ImageButton
        {
            Source = "open.png",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.End,
            WidthRequest = 30,
            HeightRequest = 30,
            Command = vm.OpenFolderCommand,
            Padding = new Thickness(10, 0, 5, 0),
        }.BindDynamicTheme();
        ToolTipProperties.SetText(folderBrowseButton, "Open dictionaries folder");

        var buttonBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.End,
            Children = { pickerDictionaries, buttonStartDownload, folderBrowseButton },
        };

        grid.Add(buttonBar, 0, 1);

        var labelDescription = new Label
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            LineBreakMode = LineBreakMode.WordWrap,
            Margin = new Thickness(0, 10, 0, 25),
            WidthRequest = 350,
        }.BindDynamicTheme();
        labelDescription.SetBinding(Label.TextProperty, nameof(vm.Description), BindingMode.TwoWay);
        grid.Add(labelDescription, 0, 2);

        var progressLabel = new Label
        {
            Text = "...",
            FontAttributes = FontAttributes.Bold,
        }.BindDynamicTheme(); 
        progressLabel.SetBinding(Label.TextProperty, nameof(vm.Progress));
        progressLabel.SetBinding(VisualElement.IsVisibleProperty, nameof(vm.IsDownloading));
        grid.Add(progressLabel, 0, 3);

        var progressBar = new ProgressBar
        {
            Progress = 0.5,
            ProgressColor = Colors.Orange,
            HorizontalOptions = LayoutOptions.Fill,
        };
        progressBar.SetBinding(ProgressBar.ProgressProperty, nameof(vm.ProgressValue));
        progressBar.SetBinding(VisualElement.IsVisibleProperty, nameof(vm.IsDownloading));
        grid.Add(progressBar, 0, 4);

        var cancelButton = new Button
        {
            Text = "Cancel",
            HorizontalOptions = LayoutOptions.Center,
            Command = vm.CancelCommand,
        }.BindDynamicTheme();
        grid.Add(cancelButton, 0, 5);

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