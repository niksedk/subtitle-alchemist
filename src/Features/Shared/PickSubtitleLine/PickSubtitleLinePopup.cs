using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Features.Main;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Constants;
using SubtitleAlchemist.Logic.Converters;

namespace SubtitleAlchemist.Features.Shared.PickSubtitleLine;

public sealed class PickSubtitleLinePopup : Popup
{
    public PickSubtitleLinePopup(PickSubtitleLinePopupModel vm)
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
            HeightRequest = 375,
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

        var subtitleGrid =  MakeSubtitleGrid(vm);
        grid.Add(subtitleGrid, 0, 1);


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

    private static Border MakeSubtitleGrid(PickSubtitleLinePopupModel vm)
    {
        // Create the header grid
        var headerGrid = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.TableHeaderBackgroundColor],
            Padding = new Thickness(5),
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) }
            },
        };

        // Add headers
        headerGrid.Add(new Label { Text = "#", FontAttributes = FontAttributes.Bold }, 0, 0);
        headerGrid.Add(new Label { Text = "Show", FontAttributes = FontAttributes.Bold }, 1, 0);
        headerGrid.Add(new Label { Text = "Text", FontAttributes = FontAttributes.Bold }, 2, 0);

        vm.SubtitleList = new CollectionView
        {
            ItemTemplate = new DataTemplate(() =>
            {
                // Each row will be a Grid
                var gridTexts = new Grid
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    Padding = new Thickness(5),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) }
                    },
                    RowDefinitions =
                    {
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) }
                    }
                };

                // Bind each cell to the appropriate property
                var numberLabel = new Label { VerticalTextAlignment = TextAlignment.Center }.BindDynamicThemeTextColorOnly();
                numberLabel.SetBinding(Label.TextProperty, nameof(DisplayParagraph.Number));

                var startTimeLabel = new Label { VerticalTextAlignment = TextAlignment.Center }.BindDynamicThemeTextColorOnly();
                startTimeLabel.SetBinding(Label.TextProperty, nameof(DisplayParagraph.Start), BindingMode.Default, new TimeSpanToStringConverter());

                var translatedTextLabel = new Label { VerticalTextAlignment = TextAlignment.Center }.BindDynamicThemeTextColorOnly();
                translatedTextLabel.SetBinding(Label.TextProperty, nameof(DisplayParagraph.Text));

                // Add labels to grid
                gridTexts.Add(numberLabel, 0, 0);
                gridTexts.Add(startTimeLabel, 1, 0);
                gridTexts.Add(translatedTextLabel, 2, 0);

                return gridTexts;
            })
        }.BindDynamicTheme();


        var gridLayout = new Grid
        {
            RowDefinitions = new RowDefinitionCollection
            {
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Star) },
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new() { Width = new GridLength(1, GridUnitType.Star) },
            }
        }.BindDynamicTheme();

        gridLayout.Add(headerGrid, 0, 0);
        gridLayout.Add(vm.SubtitleList, 0, 1);

        vm.SubtitleList.SelectionMode = SelectionMode.Single;
        vm.SubtitleList.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.Paragraphs), BindingMode.TwoWay);
        vm.SubtitleList.SetBinding(SelectableItemsView.SelectedItemProperty, nameof(vm.SelectedParagraph), BindingMode.TwoWay);

        var border = new Border
        {
            Content = gridLayout,
            Padding = new Thickness(5),
            Margin = new Thickness(10),
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
        }.BindDynamicTheme();
        border.Content = gridLayout;

        return border;
    }
}
