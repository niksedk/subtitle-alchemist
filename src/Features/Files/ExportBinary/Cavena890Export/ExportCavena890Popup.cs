using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Controls.SubTimeControl;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Files.ExportBinary.Cavena890Export;

public sealed class ExportCavena890Popup : Popup
{
    public ExportCavena890Popup(ExportCavena890PopupModel vm)
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
                new ColumnDefinition { Width = GridLength.Star },
            },
            Margin = new Thickness(2),
            Padding = new Thickness(30, 20, 30, 10),
            RowSpacing = 20,
            ColumnSpacing = 10,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            WidthRequest = 550,
            HeightRequest = 560,
        }.BindDynamicTheme();


        var labelPopupTitle = new Label
        {
            Text = "Cavena 890 save options",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelPopupTitle, 0, 0);
        grid.SetColumnSpan(labelPopupTitle, 2);


        var buttonImport = new Button
        {
            Text = "Import...",
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.ImportCommand,
        }.BindDynamicTheme();
        grid.Add(buttonImport, 1, 0);


        var labelTitle = new Label
        {
            Text = "Translated title",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelTitle, 0, 1);

        var entryTitle = new Entry
        {
            Placeholder = "Enter translated title",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        entryTitle.SetBinding(Entry.TextProperty, nameof(vm.TranslatedTitle));
        grid.Add(entryTitle, 1, 1);

        var labelOriginalTitle = new Label
        {
            Text = "Original title",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelOriginalTitle, 0, 2);

        var entryOriginalTitle = new Entry
        {
            Placeholder = "Enter original title",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        entryOriginalTitle.SetBinding(Entry.TextProperty, nameof(vm.OriginalTitle));
        grid.Add(entryOriginalTitle, 1, 2);

        var labelTranslator = new Label
        {
            Text = "Translator",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelTranslator, 0, 3);

        var entryTranslator = new Entry
        {
            Placeholder = "Enter translator",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        entryTranslator.SetBinding(Entry.TextProperty, nameof(vm.Translator));
        grid.Add(entryTranslator, 1, 3);

        var labelComment = new Label
        {
            Text = "Comment",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelComment, 0, 4);

        var entryComment = new Entry
        {
            Placeholder = "Enter comment",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        entryComment.SetBinding(Entry.TextProperty, nameof(vm.Comment));
        grid.Add(entryComment, 1, 4);

        var labelLanguage = new Label
        {
            Text = "Language",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelLanguage, 0, 5);

        var pickerLanguage = new Picker
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        pickerLanguage.SetBinding(Picker.ItemsSourceProperty, nameof(vm.Languages));
        pickerLanguage.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedLanguage));
        grid.Add(pickerLanguage, 1, 5);

        var labelStartTime = new Label
        {
            Text = "Start of programme",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelStartTime, 0, 6);

        var timePickerStartTime = new SubTimeUpDown
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        timePickerStartTime.SetBinding(SubTimeUpDown.TimeProperty, nameof(vm.StartTime));
        grid.Add(timePickerStartTime, 1, 6);


        var buttonOk = new Button
        {
            Text = "OK",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.OkCommand,
            Margin = new Thickness(0, 0, 10, 0),
        }.BindDynamicTheme();

        var buttonCancel = new Button
        {
            Text = "Cancel",
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.CancelCommand,
        }.BindDynamicTheme();

        var buttonBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 35, 0, 0),
            Children =
            {
                buttonOk,
                buttonCancel,
            },
        }.BindDynamicTheme();

        grid.Add(buttonBar, 0, 7);
        Grid.SetColumnSpan(buttonBar, 2);


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


