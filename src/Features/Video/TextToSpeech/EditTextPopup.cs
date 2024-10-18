using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Video.TextToSpeech;

public sealed class EditTextPopup : Popup
{
    public EditTextPopup(EditTextPopupModel vm)
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
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
            },
            Margin = new Thickness(2),
            Padding = new Thickness(30, 20, 30, 10),
            RowSpacing = 10,
            ColumnSpacing = 10,
            HorizontalOptions = LayoutOptions.Start, 
            VerticalOptions = LayoutOptions.Start,
            WidthRequest = 425,
            HeightRequest = 225,
        }.BindDynamicTheme();

        var titleLabel = new Label
        {
            Text = "Edit text",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
            Margin = new Thickness(0, 0, 0, 5),
        }.BindDynamicTheme();
        grid.Add(titleLabel, 0);
        grid.SetColumnSpan(titleLabel, 2);

        var editorText = new Editor
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            WidthRequest = 375,
            HeightRequest = 75,
        };
        editorText.SetBinding(Editor.TextProperty, nameof(vm.Text));
        grid.Add(editorText, 0, 1);

        var buttonOk = new Button
        {
            Text = "OK",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Command = vm.OkCommand,
            Margin = new Thickness(0, 0, 10, 0),
        }.BindDynamicTheme();

        var cancelButton = new Button
        {
            Text = "Cancel",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Command = vm.CancelCommand,
        }.BindDynamicTheme();

        var buttonBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            Margin = new Thickness(0,5,0,0),
            Children =
            {
                buttonOk,
                cancelButton,
            },
        }.BindDynamicTheme();
        grid.Add(buttonBar, 0, 3);
        grid.SetColumnSpan(buttonBar, 2);

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