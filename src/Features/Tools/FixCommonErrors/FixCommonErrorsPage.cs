using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Controls.SubTimeControl;
using SubtitleAlchemist.Features.Main;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Converters;

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
            Text = "Fix common errors - step 1 - select rules to fix",
            FontSize = 18,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();

        var searchBar = new SearchBar
        {
            Margin = new Thickness(0, 0, 25, 0),
            Placeholder = "Search",
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 300,
        }.BindDynamicTheme();
        searchBar.SetBinding(Entry.TextProperty, nameof(vm.SearchText));
        searchBar.TextChanged += vm.EntrySearch_TextChanged;

        var labelLanguage = new Label
        {
            Margin = new Thickness(5, 0, 5, 0),
            Text = "Language",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();

        var pickerLanguage = new Picker
        {
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        pickerLanguage.SetBinding(Picker.ItemsSourceProperty, nameof(vm.Languages));
        pickerLanguage.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedLanguage), BindingMode.TwoWay);
        pickerLanguage.SelectedIndexChanged += vm.PickerLanguageSelectedIndexChanged;

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
        topBarGrid.Add(searchBar, 1, 0);
        topBarGrid.Add(labelLanguage, 2, 0);
        topBarGrid.Add(pickerLanguage, 3, 0);

        grid.Add(topBarGrid, 0, 0);


        var collectionView = new CollectionView
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            SelectionMode = SelectionMode.Single,
            ItemTemplate = new DataTemplate(() =>
            {
                var rulesItemsGrid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    },
                };

                var isSelectedSwitch = new Switch
                {
                    VerticalOptions = LayoutOptions.Center,
                }.BindDynamicTheme();
                isSelectedSwitch.SetBinding(Switch.IsToggledProperty, nameof(FixRuleDisplayItem.IsSelected));
                rulesItemsGrid.Add(isSelectedSwitch, 0, 0);

                var nameLabel = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                }.BindDynamicTheme();
                nameLabel.SetBinding(Label.TextProperty, nameof(FixRuleDisplayItem.Name));
                rulesItemsGrid.Add(nameLabel, 1, 0);

                var exampleLabel = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                }.BindDynamicTheme();
                exampleLabel.SetBinding(Label.TextProperty, nameof(FixRuleDisplayItem.Example));
                rulesItemsGrid.Add(exampleLabel, 2, 0);

                return rulesItemsGrid;
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


        var buttonAll = new Button
        {
            Text = "Select all",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(5, 0, 5, 0),
            Command = vm.RulesSelectAllCommand,
        }.BindDynamicTheme();

        var buttonInverse = new Button
        {
            Text = "Inverse selection",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(5, 0, 5, 0),
            Command = vm.RulesInverseSelectedCommand,
        }.BindDynamicTheme();

        var labelProfile = new Label
        {
            Margin = new Thickness(25, 0, 5, 0),
            Text = "Profile",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        var pickerProfile = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        pickerProfile.SetBinding(Picker.ItemsSourceProperty, nameof(vm.Profiles));
        pickerProfile.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedProfile), BindingMode.TwoWay);
        pickerProfile.SelectedIndexChanged += vm.PickerProfileSelectedIndexChanged;

        var buttonProfile = new Button
        {
            Text = "...",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(5, 0, 5, 0),
            Command = vm.EditProfilesCommand,
        }.BindDynamicTheme();


        var stackLayoutLeft = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Children = { buttonAll, buttonInverse, labelProfile, pickerProfile, buttonProfile },
        }.BindDynamicTheme();

        var buttonNext = new Button
        {
            Text = "Apply fixes >",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(5, 0, 5, 0),
            Command = vm.GoToStep2Command,
        }.BindDynamicTheme();

        var buttonCancel = new Button
        {
            Text = "Cancel",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(5, 0, 5, 0),
            Command = vm.CancelCommand,
        }.BindDynamicTheme();

        var stackLayoutRight = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Children = { buttonNext, buttonCancel },
        }.BindDynamicTheme();

        var buttonBarGrid = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            RowDefinitions = new RowDefinitionCollection
            {
                new() { Height = new GridLength(1, GridUnitType.Auto) },
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new() { Width = new GridLength(1, GridUnitType.Star) },
                new() { Width = new GridLength(1, GridUnitType.Auto) },
            },
        };

        buttonBarGrid.Add(stackLayoutLeft, 0, 0);
        buttonBarGrid.Add(stackLayoutRight, 1, 0);
        grid.Add(buttonBarGrid, 0, 2);

        collectionView.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.FixRules));

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
                new() { Height = new GridLength(1, GridUnitType.Star) }, // subtitle
                new() { Height = new GridLength(1, GridUnitType.Auto) }, // back and ok buttons
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
            Text = "Fix common errors - step 2 - apply fixes",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            FontSize = 18,
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
            Margin = new Thickness(5, 0, 5, 0),
            Command = vm.OkCommand,
        }.BindDynamicTheme();

        var buttonCancel = new Button
        {
            Text = "< Back to fix list",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(5, 0, 5, 0),
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
        Grid.SetColumnSpan(stackLayout, 2);

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
                new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
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
                Text = "Apply",
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center
            }, 0, 0);
        gridHeader.Add(
            new Label
            {
                Text = "#",
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center
            }, 1, 0);
        gridHeader.Add(
            new Label
            {
                Text = "Action",
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center
            }, 2, 0);
        gridHeader.Add(
            new Label
            {
                Text = "Before",
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center
            }, 3, 0);
        gridHeader.Add(
            new Label
            {
                Text = "After",
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center
            }, 4, 0);


        var collectionViewFixes = new CollectionView
        {
            SelectionMode = SelectionMode.Single,
            ItemTemplate = new DataTemplate(() =>
            {
                var gridTexts = new Grid
                {
                    Padding = new Thickness(5),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) },
                    },
                    RowDefinitions =
                    {
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                    }
                };

                // Bind each cell to the appropriate property
                var isSelectedSwitch = new Switch().BindDynamicTheme();
                isSelectedSwitch.SetBinding(Switch.IsToggledProperty, nameof(FixDisplayItem.IsSelected));

                var labelNumber =
                    new Label { VerticalTextAlignment = TextAlignment.Center }.BindDynamicThemeTextColorOnly();
                labelNumber.SetBinding(Label.TextProperty, nameof(FixDisplayItem.Number));

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
                gridTexts.Add(isSelectedSwitch, 0, 0);
                gridTexts.Add(labelNumber, 1, 0);
                gridTexts.Add(labelAction, 2, 0);
                gridTexts.Add(labelBefore, 3, 0);
                gridTexts.Add(labelAfter, 4, 0);

                return gridTexts;
            })
        }.BindDynamicTheme();
        collectionViewFixes.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.Fixes));
        collectionViewFixes.SelectionChanged += vm.OnCollectionViewFixesSelectionChanged;

        var buttonFixBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Children =
            {
                new Button
                {
                    Text = "Select all",
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(5, 0, 5, 0),
                    Command = vm.FixesSelectAllCommand,
                }.BindDynamicTheme(),
                new Button
                {
                    Text = "Inverse selection",
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(5, 0, 5, 0),

                    Command = vm.FixesInverseSelectedCommand,
                }.BindDynamicTheme(),
                new Button
                {
                    Text = "Refresh fixes",
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(5, 0, 5, 0),
                    Command = vm.RefreshFixesCommand,
                }.BindDynamicTheme(),
                new Button
                {
                    Text = "Apply selected fixes",
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(5, 0, 5, 0),
                    Command = vm.ApplySelectedFixesCommand,
                }.BindDynamicTheme(),
            }
        }.BindDynamicTheme();

        var gridLayout = new Grid
        {
            RowDefinitions = new RowDefinitionCollection
            {
                new() { Height = new GridLength(1, GridUnitType.Auto) }, // header
                new() { Height = new GridLength(1, GridUnitType.Star) }, // fixes list
                new() { Height = new GridLength(1, GridUnitType.Auto) }, // buttons: select all, inverse selection, refresh fixes, apply selected fixes
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new() { Width = new GridLength(1, GridUnitType.Star) },
            }
        }.BindDynamicTheme();

        gridLayout.Add(gridHeader, 0, 0);
        gridLayout.Add(collectionViewFixes, 0, 1);
        gridLayout.Add(buttonFixBar, 0, 2);

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
                Text = "#",
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


        var collectionViewSubtitle = new CollectionView
        {
            SelectionMode = SelectionMode.Single,
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
                labelShow.SetBinding(Label.TextProperty, nameof(DisplayParagraph.Start), BindingMode.Default, new TimeSpanToStringConverter());

                var labelHide =
                    new Label { VerticalTextAlignment = TextAlignment.Center }.BindDynamicThemeTextColorOnly();
                labelHide.SetBinding(Label.TextProperty, nameof(DisplayParagraph.End), BindingMode.Default, new TimeSpanToStringConverter());

                var labelDuration =
                    new Label { VerticalTextAlignment = TextAlignment.Center }.BindDynamicThemeTextColorOnly();
                labelDuration.SetBinding(Label.TextProperty, nameof(DisplayParagraph.Duration), BindingMode.Default, new TimeSpanToShortStringConverter());

                var labelText = new Label { VerticalTextAlignment = TextAlignment.Center }.BindDynamicThemeTextColorOnly();
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
        collectionViewSubtitle.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.Paragraphs));
        collectionViewSubtitle.SetBinding(SelectableItemsView.SelectedItemProperty, nameof(vm.SelectedParagraph));
        collectionViewSubtitle.SelectionChanged += vm.OnCollectionViewSubtitleSelectionChanged;

        var gridEdit = new Grid
        {
            Margin = new Thickness(5, 15, 5, 5),
            RowDefinitions = new RowDefinitionCollection
            {
                new() { Height = new GridLength(1, GridUnitType.Auto) },
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new() { Width = new GridLength(1, GridUnitType.Auto) },
                new() { Width = new GridLength(1, GridUnitType.Auto) },
                new() { Width = new GridLength(1, GridUnitType.Auto) },
            }
        };

        var leftGrid = new Grid
        {
            Margin = new Thickness(10),
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new(GridLength.Auto),
                new(GridLength.Auto),
            },
            RowDefinitions = new RowDefinitionCollection
            {
                new() { Height = GridLength.Auto },
                new() { Height = GridLength.Auto },
            },
        };

        var startTimeUpDown = new SubTimeUpDown
            {
                DisplayText = "00:00:00,000",
                HorizontalOptions = LayoutOptions.Start,
            }
            .BindDynamicTheme()
            .Column(0)
            .Row(1);
        startTimeUpDown.BindingContext = vm;
        startTimeUpDown.ValueChanged += vm.EditShowChanged;
        startTimeUpDown.Bind(SubTimeUpDown.TimeProperty, nameof(vm.EditShow), BindingMode.TwoWay);

        var durationUpDown = new SubTimeUpDown
            {
                DisplayText = "00,000",
                UseShortFormat = true,
                HorizontalOptions = LayoutOptions.Start,
            }
            .BindDynamicTheme()
            .Column(0)
            .Row(1);
        durationUpDown.BindingContext = vm;
        durationUpDown.ValueChanged += vm.EditDurationChanged;
        durationUpDown.Bind(SubTimeUpDown.TimeProperty, nameof(vm.EditDuration), BindingMode.TwoWay);

        leftGrid.Add(new Label
        {
            Text = "Show",
            VerticalOptions = LayoutOptions.End,
            Padding = new Thickness(0, 0, 5, 5),
        }.BindDynamicTheme(), 0, 0);
        leftGrid.Add(startTimeUpDown, 1, 0);
        leftGrid.Add(new Label
        {
            Text = "Duration",
            VerticalOptions = LayoutOptions.End,
            Padding = new Thickness(0, 0, 5, 5),
        }.BindDynamicTheme(), 0, 1);
        leftGrid.Add(durationUpDown, 1, 1);
        gridEdit.Add(leftGrid, 0, 0);

        var editorText = new Editor
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            MinimumWidthRequest = 500,
        }.BindDynamicTheme();
        editorText.SetBinding(Editor.TextProperty, nameof(vm.EditText));
        editorText.TextChanged += vm.EditorTextTextChanged;
        gridEdit.Add(editorText, 1, 0);

        var buttonWrap = new Image
        {
            Source = "wrap.png",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 16,
            HeightRequest = 16,
        }; 
        ToolTipProperties.SetText(buttonWrap, "Auto-break");
        var buttonWrapTapGestureRecognizer = new TapGestureRecognizer();
        buttonWrapTapGestureRecognizer.Tapped += vm.AutoBreak;
        buttonWrap.GestureRecognizers.Add(buttonWrapTapGestureRecognizer);

        var buttonUnwrap = new Image
        {
            Source = "unwrap.png",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 16,
            HeightRequest = 16,
            Margin = new Thickness(0,10,0,0),
        }; 
        ToolTipProperties.SetText(buttonUnwrap, "Un-break");
        var buttonUnwrapTapGestureRecognizer = new TapGestureRecognizer();
        buttonUnwrapTapGestureRecognizer.Tapped += vm.Unbreak;
        buttonUnwrap.GestureRecognizers.Add(buttonUnwrapTapGestureRecognizer);

        var wrapBar = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(5),
            Padding = new Thickness(5,5,5,5),
            Children = { buttonWrap, buttonUnwrap },
        }.BindDynamicTheme();

        gridEdit.Add(wrapBar, 2, 0);

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
        gridLayout.Add(collectionViewSubtitle, 0, 1);
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