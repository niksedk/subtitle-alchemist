using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Converters;

namespace SubtitleAlchemist.Features.Edit.RedoUndoHistory;

public class UndoRedoHistoryPage : ContentPage
{
    public UndoRedoHistoryPage(UndoRedoHistoryPageModel vm)
    {
        this.BindDynamicTheme();
        Padding = new Thickness(10);
        vm.Page = this;
        BindingContext = vm;
        Resources.Add(ThemeHelper.GetGridSelectionStyle());

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
            Text = "Undo/redo History",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
        }.BindDynamicTheme();
        grid.Add(titleLabel, 0);
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

        var buttonCompareHistoryItems = new Button
        {
            Text = "Compare history items",
            Command = vm.CompareHistoryItemsCommand,
            Margin = new Thickness(0, 0, 10, 0),
        }.BindDynamicTheme();

        var buttonCompareWithCurrent = new Button
        {
            Text = "Compare with current",
            Command = vm.CompareWithCurrentCommand,
            Margin = new Thickness(0, 0, 10, 0),
        }.BindDynamicTheme();

        var buttonRollback = new Button
        {
            Text = "Rollback to selected item",
            Command = vm.RollbackToCommand,
            Margin = new Thickness(0, 0, 10, 0),
        }.BindDynamicTheme();

        var buttonCancel = new Button
        {
            Text = "Cancel",
            Command = vm.CancelCommand,
            Margin = new Thickness(0, 0, 10, 0),
        }.BindDynamicTheme();

        var buttonBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.End,
            Children =
            {
                buttonCompareHistoryItems,
                buttonCompareWithCurrent,
                buttonRollback,
                buttonCancel,
            }
        }.BindDynamicTheme();

        grid.Add(buttonBar, 1, 2);

        Content = grid;
    }

    private CollectionView MakeCollectionView(UndoRedoHistoryPageModel vm)
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
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Number of lines
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Selected line number
                        new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) }, // Description
                    },
                };

                IValueConverter converterShort = new DataTimeToTimeConverter();

                var labelDateAndTime = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                }.BindDynamicThemeTextColorOnly();
                labelDateAndTime.SetBinding(Label.TextProperty, nameof(UndoRedoItemDisplay.Created), BindingMode.Default, converterShort);
                rulesItemsGrid.Add(labelDateAndTime, 0);

                var labelFileName = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                }.BindDynamicThemeTextColorOnly();
                labelFileName.SetBinding(Label.TextProperty, nameof(UndoRedoItemDisplay.NumberOfLines));
                rulesItemsGrid.Add(labelFileName, 1);

                var labelExtension = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                }.BindDynamicThemeTextColorOnly();
                labelExtension.SetBinding(Label.TextProperty, nameof(UndoRedoItemDisplay.SelectedLineNumber));
                rulesItemsGrid.Add(labelExtension, 2);

                var labelSize = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                }.BindDynamicThemeTextColorOnly();
                labelSize.SetBinding(Label.TextProperty, nameof(UndoRedoItemDisplay.Description));
                rulesItemsGrid.Add(labelSize, 3);

                return rulesItemsGrid;
            }),
        };

        collectionView.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.UndoRedoItems));
        collectionView.SetBinding(SelectableItemsView.SelectedItemProperty, nameof(vm.SelectedUndoRedoItem));

        collectionView.SelectionChanged += vm.SelectionChanged;

        return collectionView;
    }

    private Grid MakeHeader(UndoRedoHistoryPageModel vm)
    {
        // Create the header grid
        var gridHeader = new Grid
        {
            BackgroundColor = Color.FromRgb(22, 22, 22), //TODO: Add to resources, header background color
            Padding = new Thickness(5),
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }, // Date and time
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Number of lines
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Selected line number
                new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) }, // Description
            },
        };

        // Add headers
        gridHeader.Add(
            new Label
            {
                Text = "Time",
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center
            }, 0);
        gridHeader.Add(
            new Label
            {
                Text = "Number of lines",
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center
            }, 1);
        gridHeader.Add(
            new Label
            {
                Text = "Selected line#",
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center
            }, 2);
        gridHeader.Add(
            new Label
            {
                Text = "Description",
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center
            }, 3);

        return gridHeader;
    }
}