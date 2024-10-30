using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Options.Settings;

public sealed class EditShortcutPopup : Popup
{
    public EditShortcutPopup(EditShortcutPopupModel vm)
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
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            WidthRequest = 500,
            HeightRequest = 480,
        }.BindDynamicTheme();

        var labelTitle = new Label
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            FontSize = 20,
            FontAttributes = FontAttributes.Bold,
            Margin = new Thickness(0, 0, 0, 25),
        }.BindDynamicTheme();
        labelTitle.SetBinding(Label.TextProperty, nameof(vm.Title));
        grid.Add(labelTitle, 0, 0);
        grid.SetColumnSpan(labelTitle, 2);

        var labelModifiers = new Label
        {
            Text = "Modifiers",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 10),
        }.BindDynamicTheme();
        grid.Add(labelModifiers, 0, 1);

        var switchModifierCtrl = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 10, 0),
        }.BindDynamicTheme();
        switchModifierCtrl.SetBinding(Switch.IsToggledProperty, nameof(vm.ModifierCtrl));

        var switchModifierAlt = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 10, 0),
        }.BindDynamicTheme();
        switchModifierAlt.SetBinding(Switch.IsToggledProperty, nameof(vm.ModifierAlt));

        var switchModifierShift = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 10, 0),
        }.BindDynamicTheme();
        switchModifierShift.SetBinding(Switch.IsToggledProperty, nameof(vm.ModifierShift));

        var stackModifiersCtrl = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 10),
            Children =
            {
                new Label { Text = "Ctrl", WidthRequest = 70, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, Margin = new Thickness(0, 0, 10, 0), }.BindDynamicTheme(),
                switchModifierCtrl,
            },
        };

        var stackModifiersAlt = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 10),
            Children =
            {
                new Label { Text = "Alt", WidthRequest = 70, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, Margin = new Thickness(0, 0, 10, 0), }.BindDynamicTheme(),
                switchModifierAlt,
            },
        };

        var stackModifiersShift = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 10),
            Children =
            {
                new Label { Text = "Shift", WidthRequest = 70, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, Margin = new Thickness(0, 0, 10, 0), }.BindDynamicTheme(),
                switchModifierShift,
            },
        };

        grid.Add(stackModifiersCtrl, 1, 1);
        grid.Add(stackModifiersAlt, 1, 2);
        grid.Add(stackModifiersShift, 1, 3);

        var labelKey1 = new Label
        {
            Text = "Key1",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelKey1, 0, 4);

        var pickerKey = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 200,
            Margin = new Thickness(0, 0, 0, 5),
        }.BindDynamicTheme();
        pickerKey.SetBinding(Picker.ItemsSourceProperty, nameof(vm.Keys));
        pickerKey.SetBinding(Picker.SelectedItemProperty, nameof(vm.Key1));
        grid.Add(pickerKey, 1, 4);

        var labelKey2 = new Label
        {
            Text = "Key2",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelKey2, 0, 5);

        var pickerKey2 = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 200,
            Margin = new Thickness(0, 0, 0, 5),
        }.BindDynamicTheme();
        pickerKey2.SetBinding(Picker.ItemsSourceProperty, nameof(vm.Keys));
        pickerKey2.SetBinding(Picker.SelectedItemProperty, nameof(vm.Key2));
        grid.Add(pickerKey2, 1, 5);

        var labelKey3 = new Label
        {
            Text = "Key3",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelKey3, 0, 6);

        var pickerKey3 = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 200,
            Margin = new Thickness(0, 0, 0, 5),
        }.BindDynamicTheme();
        pickerKey3.SetBinding(Picker.ItemsSourceProperty, nameof(vm.Keys));
        pickerKey3.SetBinding(Picker.SelectedItemProperty, nameof(vm.Key3));
        grid.Add(pickerKey3, 1, 6);


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
            Margin = new Thickness(0, 35, 0, 0),
            Children =
            {
                buttonOk,
                buttonCancel,
            },
        };

        grid.Add(buttonBar, 0, 7);


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
