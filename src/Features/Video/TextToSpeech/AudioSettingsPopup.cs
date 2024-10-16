using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Video.TextToSpeech;

public sealed class AudioSettingsPopup : Popup
{
    public AudioSettingsPopup(AudioSettingsPopupModel vm)
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
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            WidthRequest = 350,
            HeightRequest = 300,
        }.BindDynamicTheme();

        var titleLabel = new Label
        {
            Text = "Audio Settings",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
            Margin = new Thickness(0, 0, 0, 35),
        }.BindDynamicTheme();
        grid.Add(titleLabel, 0);
        grid.SetColumnSpan(titleLabel, 2);


        var labelEncoding = new Label
        {
            Text = "Encoding",
            FontAttributes = FontAttributes.Bold,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelEncoding, 0, 1);

        var pickerEncoding = new Picker
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        };
        pickerEncoding.SetBinding(Picker.ItemsSourceProperty, nameof(vm.Encodings));
        pickerEncoding.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedEncoding));
        grid.Add(pickerEncoding, 1, 1);


        var labelStereo = new Label
        {
            Text = "Stereo",
            FontAttributes = FontAttributes.Bold,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelStereo, 0, 2);

        var switchStereo = new Switch
        {
            IsToggled = true,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
        };
        switchStereo.SetBinding(Switch.IsToggledProperty, nameof(vm.IsStereo));
        grid.Add(switchStereo, 1, 2);


        var buttonOk = new Button
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

        var buttonBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            Margin = new Thickness(0,25,0,0),
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