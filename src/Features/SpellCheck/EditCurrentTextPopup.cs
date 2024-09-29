using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.SpellCheck;

public sealed class EditCurrentTextPopup : Popup
{
    public EditCurrentTextPopup(EditCurrentTextPopupModel vm)
    {
        BindingContext = vm;

        this.BindDynamicTheme();
        CanBeDismissedByTappingOutsideOfPopup = false;

        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, // title label
                new RowDefinition { Height = GridLength.Auto }, // editor
                new RowDefinition { Height = GridLength.Auto }, // button bar
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
        }.BindDynamicTheme();

        var titleLabel = new Label
        {
            Text = "Edit text (line x of y)",
            FontSize = 20,
            Padding = new Thickness(0, 0, 0, 10),
        }.BindDynamicTheme();
        grid.Add(titleLabel, 0);

        var editorText = new Editor()
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            HeightRequest = 100,
            WidthRequest = 400,
            Margin = new Thickness(0, 0, 0, 0),
        }.BindDynamicTheme();
        editorText.SetBinding(Editor.TextProperty, new Binding(nameof(vm.Text)));
        grid.Add(editorText, 0, 1);


        var okButton = new Button
        {
            Text = "OK",
            HorizontalOptions = LayoutOptions.Center,
            Command = vm.OkCommand,
            Margin = new Thickness(0,0,10,0),
        }.BindDynamicTheme();

        var cancelButton = new Button
        {
            Text = "Cancel",
            HorizontalOptions = LayoutOptions.Center,
            Command = vm.CancelCommand,
        }.BindDynamicTheme();

        var buttonBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            Children =
            {
                okButton,
                cancelButton,
            }
        }.BindDynamicTheme();
        grid.Add(buttonBar, 0, 2);

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