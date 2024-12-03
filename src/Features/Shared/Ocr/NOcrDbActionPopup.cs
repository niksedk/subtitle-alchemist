using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Features.Shared.Ocr;

public sealed class NOcrDbActionPopup : Popup
{
    public NOcrDbActionPopup(NOcrDbActionPopupModel vm)
    {
        BindingContext = vm;

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
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            WidthRequest = 600,
            HeightRequest = 400,
        }.BindDynamicTheme();

        var labelTitle = new Label
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            FontAttributes = FontAttributes.Bold,
            Margin = new Thickness(0, 0, 25, 25),
        }.AsTitle();
        labelTitle.SetBinding(Label.TextProperty, nameof(vm.Title));
        grid.Add(labelTitle, 0, 0);

        var buttonNew = new Button
        {
            Text = "New",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.NewCommand,
            Margin = new Thickness(10),
        }.BindDynamicTheme().BindIsVisible(nameof(vm.IsNewVisible));

        var labelNewName = new Label
        {
            Text = "Name",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 10, 0),
        }.BindDynamicTheme();
        var entryNewName = new Entry
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 10, 0),
            WidthRequest = 200,
        }.BindDynamicTheme().BindText(nameof(vm.NewName));
        vm.EntryNewName = entryNewName;
        var buttonOk = new Button
        {
            Text = "Ok",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.OkCommand,
            Margin = new Thickness(0, 0 , 10, 0),
        }.BindDynamicTheme();

        var labelNewNameError = new Label
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 10, 0),
            TextColor = (Color)Application.Current!.Resources[ThemeNames.ErrorTextColor],
        }.BindDynamicTheme().BindIsVisible(nameof(vm.IsErrorFileNameVisible)).BindText(nameof(vm.ErrorFileName));

        var stackNewName = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(10),
            Children =
            {
                labelNewName,
                entryNewName,
                buttonOk,
                labelNewNameError,
            },
        }.BindIsVisible(nameof(vm.IsNewNameVisible));

        var buttonDelete = new Button
        {
            Text = "Delete",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.DeleteCommand,
            Margin = new Thickness(10),
        }.BindDynamicTheme().BindIsVisible(nameof(vm.IsEditVisible));

        var buttonEdit = new Button
        {
            Text = "Edit",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.EditCommand,
            Margin = new Thickness(10),
        }.BindDynamicTheme().BindIsVisible(nameof(vm.IsEditVisible));

        var buttonCancel = new Button
        {
            Text = "Cancel",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.CancelCommand,
            Margin = new Thickness(10),

        }.BindDynamicTheme();

        var buttonBar = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 15, 0, 0),
            Children =
            {
                buttonEdit,
                buttonDelete,
                buttonNew,
                stackNewName,
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
    }
}
