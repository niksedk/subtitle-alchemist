using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Shared.PickMatroskaTrack;

public sealed class PickMatroskaTrackPopup : Popup
{
    public PickMatroskaTrackPopup(PickMatroskaTrackPopupModel vm)
    {
        BindingContext = vm;

        CanBeDismissedByTappingOutsideOfPopup = false;

        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star },
                new RowDefinition { Height = GridLength.Auto },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
            },
            Margin = new Thickness(2),
            Padding = new Thickness(30, 20, 30, 10),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            WidthRequest = 700,
            HeightRequest = 500,
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
        grid.Add(labelTitle, 0);

        var videoGrid =  MakeSubtitleListGrid(vm);
        grid.Add(videoGrid, 0, 1);


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
            Margin = new Thickness(0, 15, 0, 0),
            Children =
            {
                buttonOk,
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

    private static Border MakeSubtitleListGrid(PickMatroskaTrackPopupModel vm)
    {

        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star }, // Track list
                new ColumnDefinition { Width = GridLength.Star }, // Selected track details
            },
            Margin = new Thickness(0, 0, 0, 10),
            Padding = new Thickness(0, 0, 0, 10),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();

        var collectionView = new CollectionView
        {
            SelectionMode = SelectionMode.Single,
            ItemTemplate = new DataTemplate(() =>
            {
                var gridItem = new Grid
                {
                    RowDefinitions =
                    {
                        new RowDefinition { Height = GridLength.Auto },
                    },
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Star },
                    },
                    Margin = new Thickness(0, 0, 0, 10),
                    Padding = new Thickness(0, 0, 0, 10),
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                }.BindDynamicTheme();
                var label = new Label
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 16,
                    FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(0, 0, 0, 0),
                }.BindDynamicTheme();
                label.SetBinding(Label.TextProperty, nameof(MatroskaTrackItem.DisplayName));
                gridItem.Add(label, 0);
                return gridItem;
            }),
        }.BindDynamicTheme();
        collectionView.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.TrackItems));
        collectionView.SetBinding(SelectableItemsView.SelectedItemProperty, nameof(vm.SelectedTrackItem));
        collectionView.SelectionChanged += vm.OnTrackSelected;
        grid.Add(collectionView, 0);

        var editor = new Editor
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            IsReadOnly = true,
            Margin = new Thickness(10, 0, 0, 0),
        }.BindDynamicTheme();
        editor.SetBinding(Editor.TextProperty, new Binding(nameof(vm.TrackInfo)));

        grid.Add(editor, 1);


        var border = new Border
        {
            Content = grid,
            Padding = new Thickness(5),
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
        }.BindDynamicTheme();

        return border;
    }
}
