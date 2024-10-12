using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Video.BurnIn;

public sealed class OutputPropertiesPopup : Popup
{
    public OutputPropertiesPopup(OutputPropertiesPopupModel vm)
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
                new RowDefinition { Height = GridLength.Auto },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
            },
            Margin = new Thickness(2),
            Padding = new Thickness(30, 20, 30, 10),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            WidthRequest = 700,
            HeightRequest = 375,
        }.BindDynamicTheme();

        var labelTitle = new Label
        {
            Text = "Output properties",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            FontSize = 20,
            FontAttributes = FontAttributes.Bold,
            Margin = new Thickness(0, 0, 0, 25),
        }.BindDynamicTheme();
        grid.Add(labelTitle, 0, 0);
        grid.SetColumnSpan(labelTitle, 3);


        var labelUseSourceFolder = new Label
        {
            Text = "Use source folder",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelUseSourceFolder, 0, 1);

        var radioUseSourceFolder = new RadioButton
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        };
        radioUseSourceFolder.SetBinding(RadioButton.IsCheckedProperty, nameof(vm.UseSourceFolder));
        grid.Add(radioUseSourceFolder, 1, 1);

        var labelUseOutputFolder = new Label
        {
            Text = "Use output folder",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelUseOutputFolder, 0, 2);

        var radioUseOutputFolder = new RadioButton
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        };
        radioUseOutputFolder.SetBinding(RadioButton.IsCheckedProperty, nameof(vm.UseOutputFolder));
        grid.Add(radioUseOutputFolder, 1, 2);

        var entryOutputFolder = new Entry
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 400,
        }.BindDynamicTheme();
        entryOutputFolder.SetBinding(Entry.TextProperty, nameof(vm.OutputFolder));
        grid.Add(entryOutputFolder, 2, 2);

        var buttonBrowseOutputFolder = new Button
        {
            Text = "...",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(5, 0, 0, 0),
            Command = vm.BrowseOutputFolderCommand,
        }.BindDynamicTheme();
        grid.Add(buttonBrowseOutputFolder, 3, 2);


        var labelVideoOutputSuffix = new Label
        {
            Text = "Video output suffix",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0,25,10,0),
        }.BindDynamicTheme();
        grid.Add(labelVideoOutputSuffix, 0, 3);

        var entryVideoOutputSuffix = new Entry
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 200,
            Margin = new Thickness(0, 25, 0, 0),
        }.BindDynamicTheme();
        entryVideoOutputSuffix.SetBinding(Entry.TextProperty, nameof(vm.OutputSuffix));
        grid.Add(entryVideoOutputSuffix, 1, 3);
        grid.SetColumnSpan(entryVideoOutputSuffix,2);

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
            Children =
            {
                buttonOk,
                buttonCancel,
            },
        };
        grid.Add(buttonBar, 0, 4);
        grid.SetColumnSpan(buttonBar, 3);


        var windowBorder = new Border
        {
            StrokeThickness = 1,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5),
            },
            Content = grid,
        }.BindDynamicTheme();

        Content = windowBorder;

        this.BindDynamicTheme();

        vm.Popup = this;
    }
}
