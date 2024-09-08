using Microsoft.Maui;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Tools.FixCommonErrors;

public class FixCommonErrorsPage : ContentPage
{
    public FixCommonErrorsPage(FixCommonErrorsModel vm)
    {
        this.BindDynamicTheme();
        Padding = new Thickness(10);
        vm.Page = this;
        vm.Step1Grid = MakeStep1Grid(vm);
        vm.Step2Grid = MakeStep2Grid(vm);
        BindingContext = vm;
        Content = vm.Step1Grid;
    }

    public static Grid MakeStep1Grid(FixCommonErrorsModel vm)
    {
        var grid = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            RowDefinitions = new RowDefinitionCollection
            {
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Star) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new() { Width = new GridLength(1, GridUnitType.Star) },
            },
        }.BindDynamicTheme();

        var labelTitle = new Label
        {
            Text = "Select common errors to fix",
            FontSize = 18,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();

        vm.EntrySearch = new Entry
        {
            Margin = new Thickness(50,0,0,0),
            Placeholder = "Search",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        vm.EntrySearch.SetBinding(Entry.TextProperty, nameof(vm.SearchText));
        vm.EntrySearch.TextChanged +=  vm.EntrySearch_TextChanged;

        var imageSearch = new Image
        {
            Margin = new Thickness(0, 0, 40, 0),
            Source = "theme_dark_find.png",
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
        };

        var pickerLanguage = new Picker
        {
            Title = "Select language",
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        pickerLanguage.SetBinding(Picker.ItemsSourceProperty, nameof(vm.Languages));
        
        var topBarGrid = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 20),
            RowDefinitions = new RowDefinitionCollection
            {
                new() { Height = new GridLength(1, GridUnitType.Auto) },
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new() { Width = new GridLength(1, GridUnitType.Auto) },
                new() { Width = new GridLength(1, GridUnitType.Star) },
                new() { Width = new GridLength(1, GridUnitType.Auto) },
                new() { Width = new GridLength(1, GridUnitType.Auto) },
            },
        };

        topBarGrid.Add(labelTitle, 0, 0);
        topBarGrid.Add(vm.EntrySearch, 1, 0);
        topBarGrid.Add(imageSearch, 2, 0);
        topBarGrid.Add(pickerLanguage, 3, 0);
        
        grid.Add(topBarGrid, 0, 0);


        var collectionView = new CollectionView
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            SelectionMode = SelectionMode.Single,
            ItemTemplate = new DataTemplate(() =>
            {
                var fixItemsGrid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                    },
                };

                var nameLabel = new Label { }.BindDynamicTheme();
                nameLabel.SetBinding(Label.TextProperty, "Name");
                fixItemsGrid.Add(nameLabel, 0, 0);

                var exampleLabel = new Label {  }.BindDynamicTheme();
                exampleLabel.SetBinding(Label.TextProperty, "Example");
                fixItemsGrid.Add(exampleLabel, 1, 0);

                var isSelectedSwitch = new Switch().BindDynamicTheme();
                isSelectedSwitch.SetBinding(Switch.IsToggledProperty, "IsSelected");
                fixItemsGrid.Add(isSelectedSwitch, 2, 0);

                return fixItemsGrid;
            })
        }.BindDynamicTheme();

        var border = new Border
        {
            Content = collectionView,
            Padding = new Thickness(5),
            Margin = new Thickness(10),
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
        }.BindDynamicTheme();


        grid.Add(border, 0, 1);

        var buttonNext = new Button
        {
            Text = "Apply fixes >",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.GoToStep2Command,
        }.BindDynamicTheme();

        var buttonCancel = new Button
        {
            Text = "Cancel",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.CancelCommand,
        }.BindDynamicTheme();

        var stackLayout = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Children = {  buttonNext, buttonCancel },
        }.BindDynamicTheme();

        grid.Add(stackLayout, 0, 2);

        collectionView.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.FixItems));

        vm.InitStep1();

        return grid;
    }

    private Grid MakeStep2Grid(FixCommonErrorsModel vm)
    {
        var grid = new Grid
        {
            Margin = new Thickness(10, 10, 10, 10),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            RowDefinitions = new RowDefinitionCollection
            {
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Star) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new() { Width = new GridLength(1, GridUnitType.Star) },
                new() { Width = new GridLength(1, GridUnitType.Star) },
            },
        };

        var titleLabel = new Label
        {
            Margin = new Thickness(5, 15, 15, 0),
            Text = "Fix common errors",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            FontSize = 17,
        }.BindDynamicTheme();

        var titleTexts = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Children =
            {
                titleLabel,
            }
        };

        grid.Add(titleTexts, 0, 0);
        Grid.SetColumnSpan(titleTexts, 2);

        var gridLeft = new Grid
        {
            Padding = new Thickness(10),
            HorizontalOptions = LayoutOptions.Fill,
            RowDefinitions = new RowDefinitionCollection
            {
                new() { Height = new GridLength(1, GridUnitType.Auto) },
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new() { Width = new GridLength(1, GridUnitType.Auto) },
                new() { Width = new GridLength(1, GridUnitType.Star) },
                new() { Width = new GridLength(1, GridUnitType.Auto) },
                new() { Width = new GridLength(1, GridUnitType.Auto) },
            },
        };


        // Define CollectionView
        var collectionView = new CollectionView
        {
            ItemTemplate = new DataTemplate(() =>
            {
                // Each row will be a Grid
                var gridTexts = new Grid
                {
                    Padding = new Thickness(5),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) }
                    },
                    RowDefinitions =
                    {
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) }
                    }
                };

                // Bind each cell to the appropriate property
                var numberLabel = new Label { VerticalTextAlignment = TextAlignment.Center }.BindDynamicThemeTextColorOnly();
                numberLabel.SetBinding(Label.TextProperty, "Number");
                numberLabel.SetBinding(BackgroundColorProperty, "BackgroundColor");

                var startTimeLabel = new Label { VerticalTextAlignment = TextAlignment.Center }.BindDynamicThemeTextColorOnly();
                //startTimeLabel.SetBinding(Label.TextProperty, nameof(TranslateRow.StartTime), BindingMode.Default, new TimeSpanToStringConverter());
                //startTimeLabel.SetBinding(BackgroundColorProperty, "BackgroundColor");

                var originalTextLabel = new Label { VerticalTextAlignment = TextAlignment.Center }.BindDynamicThemeTextColorOnly();
                originalTextLabel.SetBinding(Label.TextProperty, "OriginalText");
                originalTextLabel.SetBinding(BackgroundColorProperty, "BackgroundColor");

                var translatedTextLabel = new Label { VerticalTextAlignment = TextAlignment.Center }.BindDynamicThemeTextColorOnly();
                translatedTextLabel.SetBinding(Label.TextProperty, "TranslatedText");
                translatedTextLabel.SetBinding(BackgroundColorProperty, "BackgroundColor");

                // Add labels to grid
                gridTexts.Add(numberLabel, 0, 0);
                gridTexts.Add(startTimeLabel, 1, 0);
                gridTexts.Add(originalTextLabel, 2, 0);
                gridTexts.Add(translatedTextLabel, 3, 0);

                return gridTexts;
            })
        }.BindDynamicTheme();

        // Create the header grid
        var headerGrid = new Grid
        {
            BackgroundColor = Color.FromRgb(22, 22, 22), //TODO: Add to resources, header background color
            Padding = new Thickness(5),
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) }
            },
        };

        // Add headers
        headerGrid.Add(new Label { Text = "Number", FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Center }, 0, 0);
        headerGrid.Add(new Label { Text = "Start Time", FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Center }, 1, 0);
        headerGrid.Add(new Label { Text = "Original Text", FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Center }, 2, 0);
        headerGrid.Add(new Label { Text = "Translated Text", FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Center }, 3, 0);


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
        gridLayout.Add(collectionView, 0, 1);

        collectionView.SelectionMode = SelectionMode.Single;
        //vm.CollectionView.SelectionChanged += vm.CollectionViewSelectionChanged;

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

        grid.Add(border, 0, 2);
        Grid.SetColumnSpan(border, 2);


        var buttonNext = new Button
        {
            Text = "OK",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.GoToStep2Command,
        }.BindDynamicTheme();

        var buttonCancel = new Button
        {
            Text = "< Back to fix list",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.CancelCommand,
        }.BindDynamicTheme();

        var stackLayout = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Children = { buttonNext, buttonCancel },
        }.BindDynamicTheme();

        grid.Add(stackLayout, 0, 3);

        collectionView.SetBinding(ItemsView.ItemsSourceProperty, "Lines");
        return grid;
    }
}