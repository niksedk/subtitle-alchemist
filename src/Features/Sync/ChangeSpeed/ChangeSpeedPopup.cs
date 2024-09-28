using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Features.Sync.ChangeFrameRate;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Sync.ChangeSpeed;

public sealed class ChangeSpeedPopup : Popup
{
    public ChangeSpeedPopup(ChangeSpeedPopupModel vm)
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
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
            },
            Margin = new Thickness(2),
            Padding = new Thickness(30, 20, 30, 10),
            RowSpacing = 0,
            ColumnSpacing = 20,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            WidthRequest = 550,
            HeightRequest = 375,
        }.BindDynamicTheme();

        var labelTitle = new Label
        {
            Text = "Adjust speed in percent",
            FontSize = 20,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 15),
        }.BindDynamicTheme();
        grid.Add(labelTitle, 0);
        grid.SetColumnSpan(labelTitle, 2);


        vm.EntrySpeedInPercent = new Entry
        {
            Placeholder = "Enter speed in percent",
            Keyboard = Keyboard.Numeric,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
        };
        vm.EntrySpeedInPercent.SetBinding(Entry.TextProperty, nameof(vm.SpeedPercent));

        var buttonFromDropFrame = new Button
        {
            Text = "From drop frame",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(10, 0, 10, 0),
            Command = vm.FromDropFrameCommand,
        }.BindDynamicTheme();

        var buttonToDropFrame = new Button
        {
            Text = "To drop frame",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Command = vm.ToDropFrameCommand,
        }.BindDynamicTheme();

        var buttonFromToDropFrameBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Children =
            {
                vm.EntrySpeedInPercent,
                buttonFromDropFrame,
                buttonToDropFrame,
            },
        };
        grid.Add(buttonFromToDropFrameBar, 0, 1);

        var radioAllLines = new RadioButton
        {
            Content = "All lines",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 20, 0, 0),
        };
        radioAllLines.SetBinding(RadioButton.IsCheckedProperty, nameof(vm.AllLines));
        grid.Add(radioAllLines, 0, 2);

        var radioSelectedLinesOnly = new RadioButton
        {
            Content = "Selected lines only",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 15),
        };
        radioSelectedLinesOnly.SetBinding(RadioButton.IsCheckedProperty, nameof(vm.SelectedLinesOnly));
        grid.Add(radioSelectedLinesOnly, 0, 3);


        var labelAllowOverlap = new Label
        {
            Text = "Allow overlap",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 15, 0),
        };

        var switchAllowOverlap = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        };
        switchAllowOverlap.SetBinding(Switch.IsToggledProperty, nameof(vm.AllowOverlap));
        
        var allowOverlapBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Children =
            {
                labelAllowOverlap,
                switchAllowOverlap,
            },
        };
        grid.Add(allowOverlapBar, 0, 4);


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

        var okCancelButtonBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 30, 0, 0),
            Children =
            {
                buttonOk,
                buttonCancel,
            },
        };
        grid.Add(okCancelButtonBar, 0, 5);


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
