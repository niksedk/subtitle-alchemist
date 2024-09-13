using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Tools.FixCommonErrors;

public sealed class FixCommonErrorsProfilePopup : Popup
{
    public FixCommonErrorsProfilePopup(FixCommonErrorsProfilePopupModel vm)
    {
        BindingContext = vm;

        this.BindDynamicTheme();
        CanBeDismissedByTappingOutsideOfPopup = false;
        vm.Popup = this;

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
            },
            Margin = new Thickness(2),
            Padding = new Thickness(30, 20, 30, 10),
            RowSpacing = 20,
            ColumnSpacing = 10,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();


        var titleLabel = new Label
        {
            Text = "Fix common errors profiles",
            Padding = new Thickness(0, 0, 0, 15),
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start,
            FontSize = 17,
        }.BindDynamicTheme();
        grid.Add(titleLabel, 0);
        grid.SetColumnSpan(titleLabel, 3);


        var labelProfile = new Label
        {
            Text = "Profiles",
            Padding = new Thickness(0, 0, 0, 0),
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        grid.Add(labelProfile, 0, 1);

        var pickerProfiles = new Picker
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start,
            WidthRequest = 250,
        }.BindDynamicTheme();
        pickerProfiles.SetBinding(Picker.ItemsSourceProperty, nameof(vm.ProfileNames));
        pickerProfiles.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedProfileName), BindingMode.TwoWay);

        grid.Add(pickerProfiles, 1, 1);

        var buttonRemove = new Button
        {
            Text = "Remove",
            HorizontalOptions = LayoutOptions.Start,
            Command = vm.RemoveCommand,
            Margin = new Thickness(0, 0, 10, 0),
        }.BindDynamicTheme();
        grid.Add(buttonRemove, 2, 1);


        var labelNew = new Label
        {
            Text = "New profile name",
            Padding = new Thickness(0, 0, 0, 0),
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        grid.Add(labelNew, 0, 2);

        var entryNewProfileName = new Entry
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start,
            WidthRequest = 250,
        }.BindDynamicTheme();
        entryNewProfileName.SetBinding(Entry.TextProperty, nameof(vm.EditProfileName), BindingMode.TwoWay);
        grid.Add(entryNewProfileName, 1, 2);

        var buttonAdd = new Button
        {
            Text = "Add",
            HorizontalOptions = LayoutOptions.Start,
            Command = vm.AddCommand,
            Margin = new Thickness(0, 0, 10, 0),
        }.BindDynamicTheme();
        grid.Add(buttonAdd, 2, 2);

        var labelStatus = new Label
        {
            Padding = new Thickness(0, 0, 0, 0),
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        labelStatus.SetBinding(Label.TextProperty, nameof(vm.Status), BindingMode.TwoWay);
        grid.Add(labelStatus, 1, 3);
        grid.SetColumnSpan(labelStatus, 2);


        var okButton = new Button
        {
            Text = "OK",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.End,
            Command = vm.OkCommand,
        }.BindDynamicTheme();

        var cancelButton = new Button
        {
            Margin = new Thickness(10, 0, 0, 0),
            Text = "Cancel",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.End,
            Command = vm.CancelCommand,
        }.BindDynamicTheme();

        var buttonRow = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.End,
            Margin = new Thickness(5,65,5,5),
            Children =
            {
                okButton,
                cancelButton,
            },
        };

        grid.Add(buttonRow, 0, 4);
        grid.SetColumnSpan(buttonRow, 3);

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
    }
}
