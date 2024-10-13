using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Features.Main;
using SubtitleAlchemist.Features.Shared.PickSubtitleLine;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Constants;
using SubtitleAlchemist.Logic.Converters;

namespace SubtitleAlchemist.Features.Shared.PickVideoPosition;

public sealed class PickVideoPositionPopup : Popup
{
    public PickVideoPositionPopup(PickVideoPositionPopupModel vm)
    {
        BindingContext = vm;

        CanBeDismissedByTappingOutsideOfPopup = false;

        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star },
                new RowDefinition { Height = GridLength.Auto },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
            },
            Margin = new Thickness(2),
            Padding = new Thickness(30, 20, 30, 10),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            WidthRequest = 700,
            HeightRequest = 500,
        }.BindDynamicTheme();

        var labelTitle = new Label
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            FontSize = 20,
            FontAttributes = FontAttributes.Bold,
            Margin = new Thickness(0, 0, 0, 25),
        }.BindDynamicTheme();
        labelTitle.SetBinding(Label.TextProperty, nameof(vm.Title));
        grid.Add(labelTitle, 0, 0);

        var videoGrid =  MakeVideoGrid(vm);
        grid.Add(videoGrid, 0, 1);


        var buttonOk = new Button
        {
            Text = "OK",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.OkCommand,
            Margin = new Thickness(0, 0, 10, 0),
        }.BindDynamicTheme();

        var buttonCancel = new Button
        {
            Text = "Cancel",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.CancelCommand,
        }.BindDynamicTheme();

        var buttonBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 15, 0, 0),
            Children =
            {
                buttonOk,
                buttonCancel,
            },
        };

        grid.Add(buttonBar, 0, 2);


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

        Closed += (sender, args) =>
        {
            vm.VideoPlayer.Handler?.DisconnectHandler();
            vm.VideoPlayer.Dispose();
        };
    }

    private static Border MakeVideoGrid(PickVideoPositionPopupModel vm)
    {
        vm.VideoPlayer = new MediaElement
        {
            ZIndex = -10000,
            Margin = new Thickness(10),
        }.BindDynamicTheme();


        var border = new Border
        {
            Content = vm.VideoPlayer,
            Padding = new Thickness(5),
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
        }.BindDynamicTheme();

        return border;
    }
}
