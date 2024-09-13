using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Files;

public partial class RestoreAutoBackupPage : ContentPage
{
    public RestoreAutoBackupPage(Files.RestoreAutoBackupModel vm)
    {
        this.BindDynamicTheme();
        Padding = new Thickness(10);
        vm.Page = this;
        BindingContext = vm;

        var grid = new Grid
        {
            Padding = new Thickness(20),
            RowSpacing = 20,
            ColumnSpacing = 10,
            HorizontalOptions = LayoutOptions.Fill,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, // title
                new RowDefinition { Height = GridLength.Star }, // files header + files collection view
                new RowDefinition { Height = GridLength.Auto }, // link label and buttons
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star },
            }
        }.BindDynamicTheme();

        var titleLabel = new Label
        {
            Text = "Restore auto-back",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
        }.BindDynamicTheme();
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);


        var header = MakeHeader(vm);

        var collectionView = MakeCollectionView(vm);

        var border = new Border
        {
            Padding = new Thickness(10),
            Content = new StackLayout
            {
                HorizontalOptions = LayoutOptions.Fill,
                Children = { header, collectionView }
            },
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
        }.BindDynamicTheme();

        grid.Add(border, 0, 1);
        Grid.SetColumnSpan(border, 2);

        vm.LabelOpenFolder = new Label
        {
            TextDecorations = TextDecorations.Underline,
            Text = "Open containing folder",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(vm.LabelOpenFolder, 0, 2);
        var tapGestureRecognizer = new TapGestureRecognizer();
        tapGestureRecognizer.Tapped += (s, e) => vm.OpenContainingFolderTapped();
        vm.LabelOpenFolder.GestureRecognizers.Add(tapGestureRecognizer);
        var pointerGestureRecognizers = new PointerGestureRecognizer();
        pointerGestureRecognizers.PointerEntered += vm.OpenContainingFolderPointerEntered;
        pointerGestureRecognizers.PointerExited += vm.OpenContainingFolderPointerExited;
        vm.LabelOpenFolder.GestureRecognizers.Add(pointerGestureRecognizers);


        var buttonOk = new Button
        {
            Text = "Restore selected file",
            Command = vm.OkCommand,
            Margin = new Thickness(0, 0, 10, 0),
        }.BindDynamicTheme();

        var buttonCancel = new Button
        {
            Text = "Cancel",
            Command = vm.CancelCommand,
        }.BindDynamicTheme();

        var buttonBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.End,
            Children =
            {
                buttonOk,
                buttonCancel,
            }
        }.BindDynamicTheme();

        grid.Add(buttonBar, 1, 2);

        Content = grid;
    }

    private static CollectionView MakeCollectionView(RestoreAutoBackupModel vm)
    {
        var collectionView = new CollectionView
        {
            SelectionMode = SelectionMode.Single,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            ItemTemplate = new DataTemplate(() =>
            {
                var rulesItemsGrid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }, // Date and time
                        new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) }, // File name
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Extension
                        new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }, // Size
                    },
                };

                var labelDateAndTime = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                }.BindDynamicTheme();
                labelDateAndTime.SetBinding(Label.TextProperty, nameof(DisplayFile.DateAndTime));
                rulesItemsGrid.Add(labelDateAndTime, 0, 0);

                var labelFileName = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                }.BindDynamicTheme();
                labelFileName.SetBinding(Label.TextProperty, nameof(DisplayFile.FileName));
                rulesItemsGrid.Add(labelFileName, 1, 0);

                var labelExtension = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                }.BindDynamicTheme();
                labelExtension.SetBinding(Label.TextProperty, nameof(DisplayFile.Extension));
                rulesItemsGrid.Add(labelExtension, 2, 0);

                var labelSize = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                }.BindDynamicTheme();
                labelSize.SetBinding(Label.TextProperty, nameof(DisplayFile.Size));
                rulesItemsGrid.Add(labelSize, 3, 0);

                return rulesItemsGrid;
            }),
        }.BindDynamicTheme();
        
        collectionView.SetBinding(ItemsView.ItemsSourceProperty, nameof(RestoreAutoBackupModel.Files));
        collectionView.SetBinding(SelectableItemsView.SelectedItemProperty, nameof(RestoreAutoBackupModel.SelectedFile), BindingMode.TwoWay);
        return collectionView;
    }

    private Grid MakeHeader(RestoreAutoBackupModel vm)
    {
        // Create the header grid
        var gridHeader = new Grid
        {
            BackgroundColor = Color.FromRgb(22, 22, 22), //TODO: Add to resources, header background color
            Padding = new Thickness(5),
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }, // Date and time
                new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) }, // File name
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Extension
                new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }, // Size
            },
        };

        // Add headers
        gridHeader.Add(
            new Label
            {
                Text = "Date and time",
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center
            }, 0, 0);
        gridHeader.Add(
            new Label
            {
                Text = "File name",
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center
            }, 1, 0);
        gridHeader.Add(
            new Label
            {
                Text = "Extension",
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center
            }, 2, 0);
        gridHeader.Add(
            new Label
            {
                Text = "Size",
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center
            }, 3, 0);

        return gridHeader;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as RestoreAutoBackupModel)?.Initialize();
    }
}
