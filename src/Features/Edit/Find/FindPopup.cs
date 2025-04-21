using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Edit.Find;

public sealed class FindPopup : Popup
{
    public FindPopup(FindPopupModel vm)
    {
        BindingContext = vm;
        vm.Popup = this;
        this.BindDynamicTheme();

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
                new ColumnDefinition { Width = 250 },
                new ColumnDefinition { Width = GridLength.Auto },
            },
            Padding = new Thickness(30, 20, 30, 10),
            RowSpacing = 0,
            ColumnSpacing = 0,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            WidthRequest = 500,
        }.BindDynamicTheme();

        var labelGoToLineNumber = new Label
        {
            Text = "Find",
            FontSize = 20,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(5, 0, 0, 10),
        }.BindDynamicTheme();
        grid.Add(labelGoToLineNumber, 0);


        vm.SearchBar = new SearchBar
        {
            Placeholder = "Search text",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            WidthRequest = 250,
            MaximumWidthRequest = 250,
        };
        vm.SearchBar.SetBinding(Entry.TextProperty, nameof(vm.SearchText));
        vm.SearchBar.SearchButtonPressed += vm.SearchButtonPressed;
        grid.Add(vm.SearchBar, 0, 1);
        
        var labelWholeWord = new Label
        {
            Text = "Whole word",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 10, 15),
        }.BindDynamicTheme();

        var switchWholeWord = new Switch
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 15),
        };
        switchWholeWord.SetBinding(Switch.IsToggledProperty, nameof(vm.WholeWord));

        var wholeWordBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Children =
            {
                labelWholeWord,
                switchWholeWord,
            },
        };
        grid.Add(wholeWordBar, 0, 2);


        var radioNormal = new RadioButton
        {
            Content = "Normal",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0),
            Padding = new Thickness(0),
        };
        radioNormal.SetBinding(RadioButton.IsCheckedProperty, nameof(vm.Normal));
        grid.Add(radioNormal, 0, 3);

        var radioCaseInsensitive = new RadioButton
        {
            Content = "Case insensitive",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0),
            Padding = new Thickness(0),
        };
        radioCaseInsensitive.SetBinding(RadioButton.IsCheckedProperty, nameof(vm.CaseInsensitive));
        grid.Add(radioCaseInsensitive, 0, 4);

        var radioRegularExpression = new RadioButton
        {
            Content = "Regular expression",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0,0,0,25),
            Padding = new Thickness(0),
        };
        radioRegularExpression.SetBinding(RadioButton.IsCheckedProperty, nameof(vm.RegularExpression));
        grid.Add(radioRegularExpression, 0, 5);


        var buttonFind = new Button
        {
            Text = "Find next",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Command = vm.FindCommand,
            Margin = new Thickness(0, 0, 10, 10),
            WidthRequest = 175,
        }.BindDynamicTheme();

        var buttonFindPrevious = new Button
        {
            Text = "Find previous",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Command = vm.CancelCommand,
            Margin = new Thickness(0, 0, 10, 10),
            WidthRequest = 175,
        }.BindDynamicTheme();

        var buttonCount = new Button
        {
            Text = "Count",
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
                buttonFind,
                buttonFindPrevious,
                buttonCount,
            },
        };
        grid.Add(buttonBar, 1, 1);
        grid.SetRowSpan(buttonBar, 5);


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
    }
}
