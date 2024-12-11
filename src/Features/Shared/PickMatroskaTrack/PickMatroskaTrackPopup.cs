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
                new RowDefinition { Height = GridLength.Auto }, // Title
                new RowDefinition { Height = GridLength.Auto }, // File name
                new RowDefinition { Height = GridLength.Star }, // Track list + track info
                new RowDefinition { Height = GridLength.Auto }, // buttons
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
            },
            Margin = new Thickness(0),
            Padding = new Thickness(30, 20, 30, 10),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            WidthRequest = 900,
            HeightRequest = 500,
        }.BindDynamicTheme();

        var row = 0;
        var labelTitle = new Label
        {
            Text = "Pick Matroska Track",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            FontAttributes = FontAttributes.Bold,
            Margin = new Thickness(0, 0, 0, 25),
        }.AsTitle();
        grid.Add(labelTitle, 0, row++);

        var labelFileName = new Label
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            FontSize = 16,
            Margin = new Thickness(0, 0, 0, 10),
        }.BindDynamicTheme();
        labelFileName.SetBinding(Label.TextProperty, nameof(vm.FileNameInfo));

        grid.Add(labelFileName, 0, row++);

        var videoGrid =  MakeTrackListGrid(vm);
        grid.Add(videoGrid, 0, row++);


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
            Margin = new Thickness(0, 0, 10, 0),
        }.BindDynamicTheme();

        var buttonExport = new Button
        {
            Text = "Export",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.ExportCommand,
            Margin = new Thickness(0, 0, 10, 0),
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
                buttonExport,
            },
        };

        grid.Add(buttonBar, 0, row);


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

    private static Border MakeTrackListGrid(PickMatroskaTrackPopupModel vm)
    {
        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto }, // Track list
                new ColumnDefinition { Width = GridLength.Star }, // Selected track details
            },
            Margin = new Thickness(0, 0, 0, 10),
            Padding = new Thickness(0, 0, 0, 10),
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            HeightRequest = 240,
        }.BindDynamicTheme();

        var collectionView = new CollectionView
        {
            SelectionMode = SelectionMode.Single,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            HeightRequest = 240,
            ItemTemplate = new DataTemplate(() =>
            {
                var label = new Label
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Start,
                    Margin = new Thickness(0, 0, 10, 10),
                }.BindDynamicTheme();
                label.SetBinding(Label.TextProperty, nameof(MatroskaTrackItem.DisplayName));
                return label;
            }),
        }.BindDynamicTheme();
        collectionView.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.TrackItems));
        collectionView.SetBinding(SelectableItemsView.SelectedItemProperty, nameof(vm.SelectedTrackItem));
        collectionView.SelectionChanged += vm.OnTrackSelected;

        grid.Add(new Label { Text = "Tracks", FontAttributes = FontAttributes.Bold, Margin = new Thickness(10)}, 0);
        grid.Add(collectionView, 0, 1);

        
        var editor = new Editor
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            IsReadOnly = true,
            Margin = new Thickness(10, 0, 0, 0),
            AutoSize = EditorAutoSizeOption.TextChanges,
            MinimumWidthRequest = 400
        }.BindDynamicTheme();
        editor.SetBinding(Editor.TextProperty, new Binding(nameof(vm.TrackInfo)));

        var listViewImages = new ListView
        {
            ItemTemplate = new DataTemplate(() =>
            {
                var image = new Image();
                image.SetBinding(Image.SourceProperty, ".");
                image.Margin = new Thickness(0, 5, 0, 5);
                return new ViewCell { View = image };
            }),
            HeightRequest = 150,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(10, 0, 0, 0),
            
        };
        listViewImages.SetBinding(ListView.ItemsSourceProperty, new Binding(nameof(vm.SelectedTrackImages)));

        var stackEditorAndImages = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Children =
            {
                editor,
                listViewImages,
            },
        };

        var scrollViewEditorAndImages = new ScrollView
        {
            Content = stackEditorAndImages,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(10, 0, 0, 0),
            HeightRequest = 240,
        };

        grid.Add(new Label { Text = "Selected track info", FontAttributes = FontAttributes.Bold, Margin = new Thickness(20, 10, 10, 10) }, 1);
        grid.Add(scrollViewEditorAndImages, 1, 1);


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
