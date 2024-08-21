using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Controls.PickerControl;

public class PickerPopup : Popup
{
    public PickerPopup(PickerPopupModel vm)
    {
        BindingContext = vm;

        this.BindDynamicTheme();
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
            HeightRequest = 300,
        }.BindDynamicTheme();

        var titleLabel = new Label
        {
            Text = "Choose item",
            FontAttributes = FontAttributes.Bold,
            Padding = new Thickness(0, 0, 0, 10),
        }.BindDynamicTheme();
        grid.Add(titleLabel, 0, 0);

        var picker = new Picker
        {
            ItemsSource = vm.Items,
            SelectedItem = vm.SelectedItem,
        }.BindDynamicTheme();
        grid.Add(picker, 0, 1);


        var okButton = new Button
        {
            Text = "OK",
            HorizontalOptions = LayoutOptions.Center,
            Command = vm.OkCommand,
        }.BindDynamicTheme();

        var cancelButton = new Button
        {
            Text = "Cancel",
            HorizontalOptions = LayoutOptions.Center,
            Command = vm.CancelCommand,
        }.BindDynamicTheme();

        var buttonRow = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Center,
            Padding = new Thickness(15),
            Children =
            {
                okButton,
                cancelButton,
            },
        };

        grid.Add(buttonRow, 0, 2);

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