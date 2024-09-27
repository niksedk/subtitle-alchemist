using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Sync.ChangeFrameRate;

public sealed class ChangeFrameRatePopup : Popup
{
    public ChangeFrameRatePopup(ChangeFrameRatePopupModel vm)
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
            WidthRequest = 500,
            HeightRequest = 275,
        }.BindDynamicTheme();

        var labelTitle = new Label
        {
            Text = "Change frame rate",
            FontSize = 20,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();
        grid.Add(labelTitle, 0);


        var pickerFromFrameRate = new Picker
        {
            Title = "From frame rate",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(5, 0),
        }.BindDynamicTheme();
        pickerFromFrameRate.SetBinding(Picker.ItemsSourceProperty, nameof(vm.FrameRates));
        pickerFromFrameRate.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedFromFrameRate));

        var buttonFromFrameRateBrowse = new Button
        {
            Text = "...",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(5, 0),
            Command = vm.BrowseFromFrameRateCommand,
        }.BindDynamicTheme();


        var buttonSwap = new ImageButton
        {
            Source = "swap.png",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(15, 0),
            Command = vm.SwapCommand,
            WidthRequest = 16,
            HeightRequest = 16,
        }.BindDynamicTheme();
        ToolTipProperties.SetText(buttonSwap, "Swap");

        var pickerToFrameRate = new Picker
        {
            Title = "To frame rate",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(5, 0),
        }.BindDynamicTheme();
        pickerToFrameRate.SetBinding(Picker.ItemsSourceProperty, nameof(vm.FrameRates));
        pickerToFrameRate.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedToFrameRate), BindingMode.TwoWay);

        var buttonToFrameRateBrowse = new Button
        {
            Text = "...",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(5, 0),
            Command = vm.BrowseToFrameRateCommand,
        }.BindDynamicTheme();

        var controlBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Children =
            {
                pickerFromFrameRate,
                buttonFromFrameRateBrowse,
                buttonSwap,
                pickerToFrameRate,
                buttonToFrameRateBrowse,
            },
        };
        grid.Add(controlBar, 0, 1);


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

        var okCancelBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 35, 0, 20),
            Children =
            {
                buttonOk,
                buttonCancel,
            },
        };
        grid.Add(okCancelBar, 0, 2);


        var windowBorder = new Border
        {
            StrokeThickness = 1,
            Padding = new Thickness(1),
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
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
