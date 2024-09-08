using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Features.Main;
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
            Margin = new Thickness(50, 0, 0, 0),
            Placeholder = "Search",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        vm.EntrySearch.SetBinding(Entry.TextProperty, nameof(vm.SearchText));
        vm.EntrySearch.TextChanged += vm.EntrySearch_TextChanged;

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
        pickerLanguage.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedLanguage), BindingMode.TwoWay);

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

                var exampleLabel = new Label { }.BindDynamicTheme();
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
            Children = { buttonNext, buttonCancel },
        }.BindDynamicTheme();

        grid.Add(stackLayout, 0, 2);

        collectionView.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.FixRules));

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
                new() { Height = new GridLength(1, GridUnitType.Star) }, // fixes
                new() { Height = new GridLength(1, GridUnitType.Star) }, // sutitle
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

        var viewFixes = MakeFixesView(vm);
        grid.Add(viewFixes, 0, 2);
        Grid.SetColumnSpan(viewFixes, 2);

        var viewSubtitle = MakeSubtitleView(vm);
        grid.Add(viewSubtitle, 0, 3);
        Grid.SetColumnSpan(viewSubtitle, 2);

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

        grid.Add(stackLayout, 0, 4);

        return grid;
    }

    private static View MakeFixesView(FixCommonErrorsModel vm)
    {
        // Create the header grid
        var gridHeader = new Grid
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
        gridHeader.Add(
            new Label
            {
                Text = "Number",
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center
            }, 0, 0);
        gridHeader.Add(
            new Label
            {
                Text = "Action",
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center
            }, 1, 0);
        gridHeader.Add(
            new Label
            {
                Text = "Before",
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center
            }, 2, 0);
        gridHeader.Add(
            new Label
            {
                Text = "After",
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center
            }, 3, 0);


        var gridFixes = new CollectionView
        {
            ItemTemplate = new DataTemplate(() =>
            {
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
                var labelNumber =
                    new Label { VerticalTextAlignment = TextAlignment.Center }.BindDynamicThemeTextColorOnly();
                labelNumber.SetBinding(Label.TextProperty, "Number");

                var labelAction =
                    new Label { VerticalTextAlignment = TextAlignment.Center }.BindDynamicThemeTextColorOnly();
                labelAction.SetBinding(Label.TextProperty, nameof(FixDisplayItem.Action));

                var labelBefore =
                    new Label { VerticalTextAlignment = TextAlignment.Center }.BindDynamicThemeTextColorOnly();
                labelBefore.SetBinding(Label.TextProperty, nameof(FixDisplayItem.Before));

                var labelAfter =
                    new Label { VerticalTextAlignment = TextAlignment.Center }.BindDynamicThemeTextColorOnly();
                labelAfter.SetBinding(Label.TextProperty, nameof(FixDisplayItem.After));

                // Add labels to grid
                gridTexts.Add(labelNumber, 0, 0);
                gridTexts.Add(labelAction, 1, 0);
                gridTexts.Add(labelBefore, 2, 0);
                gridTexts.Add(labelAfter, 3, 0);

                return gridTexts;
            })
        }.BindDynamicTheme();
        gridFixes.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.Fixes));

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

        gridLayout.Add(gridHeader, 0, 0);
        gridLayout.Add(gridFixes, 0, 1);

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

    private static View MakeSubtitleView(FixCommonErrorsModel vm)
    {
        var gridHeader = new Grid
        {
            BackgroundColor = Color.FromRgb(22, 22, 22), //TODO: Add to resources, header background color
            Padding = new Thickness(5),
            HorizontalOptions = LayoutOptions.Fill,
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) }
            },
        };

        gridHeader.Add(
            new Label
            {
                Text = "Number",
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center
            }, 0, 0);
        gridHeader.Add(
            new Label
            {
                Text = "Show",
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center
            }, 1, 0);
        gridHeader.Add(
            new Label
            {
                Text = "Hide",
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center
            }, 2, 0);
        gridHeader.Add(
            new Label
            {
                Text = "Duration",
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center
            }, 3, 0);
        gridHeader.Add(
            new Label
            {
                Text = "Text",
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center
            }, 4, 0);


        var gridSubtitle = new CollectionView
        {
            VerticalOptions = LayoutOptions.Fill,
            ItemTemplate = new DataTemplate(() =>
            {
                var gridTexts = new Grid
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    Padding = new Thickness(5),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) }
                    },
                    RowDefinitions =
                    {
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                    }
                };

                // Bind each cell to the appropriate property
                var labelNumber =
                    new Label { VerticalTextAlignment = TextAlignment.Center }.BindDynamicThemeTextColorOnly();
                labelNumber.SetBinding(Label.TextProperty, nameof(DisplayParagraph.Number));

                var labelShow =
                    new Label { VerticalTextAlignment = TextAlignment.Center }.BindDynamicThemeTextColorOnly();
                labelShow.SetBinding(Label.TextProperty, nameof(DisplayParagraph.Start));

                var labelHide =
                    new Label { VerticalTextAlignment = TextAlignment.Center }.BindDynamicThemeTextColorOnly();
                labelHide.SetBinding(Label.TextProperty, nameof(DisplayParagraph.End));

                var labelDuration =
                    new Label { VerticalTextAlignment = TextAlignment.Center }.BindDynamicThemeTextColorOnly();
                labelDuration.SetBinding(Label.TextProperty, nameof(DisplayParagraph.Duration));

                var labelText =
                    new Label { VerticalTextAlignment = TextAlignment.Center }.BindDynamicThemeTextColorOnly();
                labelText.SetBinding(Label.TextProperty, nameof(DisplayParagraph.Text));

                // Add labels to grid
                gridTexts.Add(labelNumber, 0, 0);
                gridTexts.Add(labelShow, 1, 0);
                gridTexts.Add(labelHide, 2, 0);
                gridTexts.Add(labelDuration, 3, 0);
                gridTexts.Add(labelText, 4, 0);

                return gridTexts;
            })
        }.BindDynamicTheme();
        gridSubtitle.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.Paragraphs));

        var gridEdit = new Grid
        {
            RowDefinitions = new RowDefinitionCollection
            {
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Star) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new() { Width = new GridLength(1, GridUnitType.Auto) },
            }
        };

        var editorText = new Editor
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();
        gridEdit.Add(editorText, 1, 0);

        var gridLayout = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill,
            RowDefinitions = new RowDefinitionCollection
            {
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Star) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new() { Width = new GridLength(1, GridUnitType.Star) },
            }
        }.BindDynamicTheme();

        gridLayout.Add(gridHeader, 0, 0);
        gridLayout.Add(gridSubtitle, 0, 1);
        gridLayout.Add(gridEdit, 0, 2);

        var border = new Border
        {
            VerticalOptions = LayoutOptions.Fill,
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