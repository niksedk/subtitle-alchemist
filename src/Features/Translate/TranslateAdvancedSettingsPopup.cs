using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Translate;

public class TranslateAdvancedSettingsPopup : Popup
{
    public TranslateAdvancedSettingsPopup(TranslateAdvancedSettingsPopupModel vm)
    {
        BindingContext = vm;

        this.BindDynamicTheme();
        CanBeDismissedByTappingOutsideOfPopup = true;
        vm.Popup = this;

        var grid = new Grid
        {
            RowDefinitions =
            {
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
            Margin = new Thickness(2),
            Padding = new Thickness(30, 20, 30, 10),
            RowSpacing = 20,
            ColumnSpacing = 10,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            WidthRequest = 500,
        }.BindDynamicTheme();


        var titleLabel = new Label
        {
            Text = "Advanced translate options",
            Padding = new Thickness(0, 0, 0, 0),
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        grid.Add(titleLabel, 0, 0);
        grid.SetColumnSpan(titleLabel, 2);


        var lineMergeLabel = new Label
        {
            Text = "Line merge",
            Padding = new Thickness(0, 0, 0, 0),
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        grid.Add(lineMergeLabel, 0, 1);

        vm.PickerLineMerge = new Picker
        {
            ItemsSource = vm.LineMergeItems,
            SelectedItem = vm.LineMergeSelectedItem,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start,
            WidthRequest = 200,
        }.BindDynamicTheme();
        vm.PickerLineMerge.SelectedIndexChanged += vm.LineMergeSelectedIndexChanged;
        vm.PickerLineMerge.SetBinding(Picker.SelectedItemProperty, vm.LineMergeSelectedItem, BindingMode.TwoWay);
        grid.Add(vm.PickerLineMerge, 1, 1);


        var delayLabel = new Label
        {
            Text = "Delay in seconds between server calls",
            Padding = new Thickness(0, 0, 0, 0),
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        grid.Add(delayLabel, 0, 2);

        vm.DelayEntry = new Entry
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.DelayEntry.SetBinding(Entry.TextProperty, vm.DelayInSeconds, BindingMode.TwoWay); 
        grid.Add(vm.DelayEntry, 1, 2);


        var maxBytesLabel = new Label
        {
            Text = "Max. bytes pr. server call",
            Padding = new Thickness(0, 0, 0, 0),
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        grid.Add(maxBytesLabel, 0, 3);

        vm.MaximumBytesEntry = new Entry
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        vm.MaximumBytesEntry.SetBinding(Entry.TextProperty, vm.LineMergeSelectedItem); ;
        grid.Add(vm.MaximumBytesEntry, 1, 3);


        vm.PromptLabel = new Label
        {
            Text = "Prompt for X",
            Padding = new Thickness(0, 0, 0, 0),
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        grid.Add(vm.PromptLabel, 0, 4);
        grid.SetColumnSpan(vm.PromptLabel, 2);

        vm.PromptEntry = new Entry
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        grid.Add(vm.PromptEntry, 0, 5);
        grid.SetColumnSpan(vm.PromptEntry, 2);


        var okButton = new Button
        {
            Text = "OK",
            HorizontalOptions = LayoutOptions.Center,
            Command = vm.OkCommand,
            Margin = new Thickness(0, 0, 10, 0),
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
            Margin = new Thickness(15),
            Children =
            {
                okButton,
                cancelButton,
            },
        };

        grid.Add(buttonRow, 0, 6);
        grid.SetColumnSpan(buttonRow, 2);

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

        vm.SetValues();
    }
}
