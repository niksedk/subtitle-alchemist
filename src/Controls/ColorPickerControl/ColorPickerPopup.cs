using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Controls.ColorPickerControl;

public class ColorPickerPopup : Popup
{
    public ColorPickerPopup(ColorPickerPopupModel vm)
    {
        BindingContext = vm;

        Color = Colors.Transparent;
        CanBeDismissedByTappingOutsideOfPopup = false;

        vm.ColorPickerView = new ColorPickerView
        {
            WidthRequest = 415,
            HeightRequest = 500,
        }.BindDynamicTheme();


        vm.ColorPickerView.ValueChanged += (sender, args) =>
        {
            vm.CurrentColor = args.Color;
        };

        var stackLayoutOkCancel = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Children =
            {
                new Button
                {
                    Text = "OK",
                    BackgroundColor = Colors.DarkGray,
                    TextColor = Colors.White,
                    Command = vm.OkCommand,
                    Margin = new Thickness(5, 5, 5, 25),
                },
                new Button
                {
                    Text = "Cancel",
                    BackgroundColor = Colors.DarkGray,
                    TextColor = Colors.White,
                    Command = vm.CancelCommand,
                    Margin = new Thickness(5, 5, 5, 25),
                },
            },
        };

        var stackLayout = new StackLayout
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Children =
            {
                vm.ColorPickerView,
                stackLayoutOkCancel,
            },
        }.BindDynamicTheme();

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
            Content = stackLayout,
        }.BindDynamicTheme();

        Content = new ContentView
        {
            BackgroundColor = Colors.Transparent,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Content = border,
        };

        vm.Popup = this;
    }
}

