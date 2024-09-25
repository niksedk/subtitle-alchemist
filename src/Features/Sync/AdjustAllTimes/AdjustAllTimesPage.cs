using SubtitleAlchemist.Controls.SubTimeControl;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Sync.AdjustAllTimes;

public class AdjustAllTimesPage : ContentPage
{
    public AdjustAllTimesPage(AdjustAllTimesPageModel vm)
    {
        this.BindDynamicTheme();
        vm.Page = this;
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
                new ColumnDefinition { Width = GridLength.Auto },
            },
            Margin = new Thickness(25),
            RowSpacing = 0,
            ColumnSpacing = 0,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();

        var labelGoToLineNumber = new Label
        {
            Text = "Adjust all times (show earlier/later)",
            FontSize = 20,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(5, 0, 0, 15),
        }.BindDynamicTheme();
        grid.Add(labelGoToLineNumber, 0);


        var labelTime = new Label
        {
            Text = "Hour:minutes:seconds:milliseconds",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();
        grid.Add(labelTime, 0, 1);



        var timeUpDown = new SubTimeUpDown
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();
        timeUpDown.SetBinding(SubTimeUpDown.TimeProperty, nameof(vm.AdjustTime));
        grid.Add(timeUpDown, 0, 2);


        var radioNormal = new RadioButton
        {
            Content = "All lines",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0,15,0,0),
            Padding = new Thickness(0),
        };
        radioNormal.SetBinding(RadioButton.IsCheckedProperty, nameof(vm.AllLines));
        grid.Add(radioNormal, 0, 3);

        var radioCaseInsensitive = new RadioButton
        {
            Content = "Selected lines only",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0),
            Padding = new Thickness(0),
        };
        radioCaseInsensitive.SetBinding(RadioButton.IsCheckedProperty, nameof(vm.SelectedLinesOnly));
        grid.Add(radioCaseInsensitive, 0, 4);

        var radioRegularExpression = new RadioButton
        {
            Content = "Selected and subsequent lines",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 25),
            Padding = new Thickness(0),
        };
        radioRegularExpression.SetBinding(RadioButton.IsCheckedProperty, nameof(vm.SelectedAndSubsequentLines));
        grid.Add(radioRegularExpression, 0, 5);


        var buttonOk = new Button
        {
            Text = "OK",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Command = vm.OkCommand,
            Margin = new Thickness(0, 0, 10, 10),
            WidthRequest = 175,
        }.BindDynamicTheme();

        var buttonCancel = new Button
        {
            Text = "Cancel",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Command = vm.CancelCommand,
            Margin = new Thickness(0, 0, 10, 10),
            WidthRequest = 175,
        }.BindDynamicTheme();

        var buttonBar = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(15, 0, 0, 0),
            Children =
            {
                buttonOk,
                buttonCancel,
            },
        };
        grid.Add(buttonBar, 1, 3);
        grid.SetRowSpan(buttonBar, 5);

        Content = grid;

        this.BindDynamicTheme();
    }
}