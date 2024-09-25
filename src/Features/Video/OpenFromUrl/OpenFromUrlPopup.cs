using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Video.OpenFromUrl;

public sealed class OpenFromUrlPopup : Popup
{
    public OpenFromUrlPopup(OpenFromUrlPopupModel vm)
    {
        BindingContext = vm;

        CanBeDismissedByTappingOutsideOfPopup = true;

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
            Padding = new Thickness(30, 20, 30, 10),
            RowSpacing = 20,
            ColumnSpacing = 10,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();

        var labelGoToLineNumber = new Label
        {
            Text = "Open video from url",
            FontSize = 20,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();
        grid.Add(labelGoToLineNumber, 0);


        vm.EntryVideoUrl = new Entry
        {
            Placeholder = "Enter url",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            WidthRequest = 400,
        };
        vm.EntryVideoUrl.SetBinding(Entry.TextProperty, nameof(vm.VideoUrl));
        grid.Add(vm.EntryVideoUrl, 0, 1);


        var buttonOk = new Button
        {
            Text = "OK",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Command = vm.OkCommand,
            Margin = new Thickness(0, 0, 10, 0),
        }.BindDynamicTheme();

        var buttonCancel = new Button
        {
            Text = "Cancel",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Command = vm.CancelCommand,
        }.BindDynamicTheme();

        var buttonBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Fill,
            Children =
            {
                buttonOk,
                buttonCancel,
            },
        };
        grid.Add(buttonBar, 0, 3);


        var windowBorder = new Border
        {
            StrokeThickness = 1,
            Padding = new Thickness(1),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5),
            },
            BackgroundColor = Colors.Transparent,
            Content = grid,
        }.BindDynamicTheme();

        Content = windowBorder;

        this.BindDynamicTheme();
  
        vm.Popup = this;
    }
}
