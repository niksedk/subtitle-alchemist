using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Features.Tools.RemoveTextForHearingImpaired;

public class RemoveTextForHiPage : ContentPage
{
    private const int LeftMenuWidth = 400;

    public RemoveTextForHiPage(RemoveTextForHiPageModel vm)
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
            VerticalOptions = LayoutOptions.Start,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, // Title
                new RowDefinition { Height = GridLength.Star }, // Options + fixes
                new RowDefinition { Height = GridLength.Auto }, // Buttons
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
            }
        }.BindDynamicTheme();

        var titleLabel = new Label
        {
            Text = "Remove text for hearing impaired",
        }.AsTitle();
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);


        var optionsView = MakeOptions(vm);
        var fixesView = RemoveTextForHiPage.MakeFixesView(vm);

        grid.Add(optionsView, 0, 1);
        grid.Add(fixesView, 1, 1);

        var buttonOk = new Button
        {
            Text = "OK",
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

        grid.Add(buttonBar, 0, 2);
        Grid.SetColumnSpan(buttonBar, 2);

        Content = grid;
    }

    private static View MakeFixesView(RemoveTextForHiPageModel vm)
    {
        var headerGrid = new Grid
        {
            BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.TableHeaderBackgroundColor],
            Padding = new Thickness(10),
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = 100 }, // Apply
                new ColumnDefinition { Width = 100 }, // Line#
                new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) }, // Before
                new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) }, // After
            },
        };

        var labelApply = new Label
        {
            Text = "Apply",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        headerGrid.Add(labelApply, 0, 0);

        var labelLine = new Label
        {
            Text = "Line#",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        headerGrid.Add(labelLine, 1, 0);

        var labelBefore = new Label
        {
            Text = "Before",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        headerGrid.Add(labelBefore, 2, 0);

        var labelAfter = new Label
        {
            Text = "After",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        headerGrid.Add(labelAfter, 3, 0);

        var collectionView = new CollectionView
        {
            SelectionMode = SelectionMode.Single,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            ItemTemplate = new DataTemplate(() =>
            {
                var rulesItemsGrid = new Grid
                {
                    Padding = new Thickness(10, 2, 2, 2),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = 100 }, // Apply
                        new ColumnDefinition { Width = 100 }, // Line#
                        new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) }, // Before
                        new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) }, // After
                    },
                };

                var switchApply = new Switch
                {
                    VerticalOptions = LayoutOptions.Center,
                }.BindDynamicThemeTextColorOnly();
                switchApply.SetBinding(Switch.IsToggledProperty, nameof(RemoveItem.Apply));
                rulesItemsGrid.Add(switchApply, 0, 0);

                var labelLineNumber = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                }.BindDynamicThemeTextColorOnly();
                labelLineNumber.SetBinding(Label.TextProperty, nameof(RemoveItem.IndexDisplay));
                rulesItemsGrid.Add(labelLineNumber, 1, 0);

                var labelBefore = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(10, 0, 0, 0),
                }.BindDynamicThemeTextColorOnly();
                labelBefore.SetBinding(Label.TextProperty, nameof(RemoveItem.Before));
                rulesItemsGrid.Add(labelBefore, 2, 0);

                var labelAfter = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(10, 0, 0, 0),
                }.BindDynamicThemeTextColorOnly();
                labelAfter.SetBinding(Label.TextProperty, nameof(RemoveItem.After));
                rulesItemsGrid.Add(labelAfter, 3, 0);

                return rulesItemsGrid;
            }),
        };

        collectionView.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.Fixes));
        collectionView.SetBinding(SelectableItemsView.SelectedItemProperty, nameof(vm.SelectedFix));
        collectionView.SelectionChanged += vm.FixSelectionChanged;

        var gridHeaderAndCollectionView = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
            }
        };

        gridHeaderAndCollectionView.Add(headerGrid, 0, 0);
        gridHeaderAndCollectionView.Add(collectionView, 0, 1);

        var border = new Border
        {
            Margin = new Thickness(10),
            StrokeThickness = 1,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Content = gridHeaderAndCollectionView,
            MinimumWidthRequest = 700,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
        }.BindDynamicTheme();

        var labelText = new Label
        {
            Text = "Text",
            VerticalOptions = LayoutOptions.Center,
            FontAttributes = FontAttributes.Bold,
            Margin = new Thickness(10, 0, 0, 0),
        }.BindDynamicThemeTextColorOnly();

        var entryText = new Entry
        {
            Placeholder = string.Empty,
            HorizontalOptions = LayoutOptions.Fill,
            Margin = new Thickness(10, 0, 0, 0),
            MaximumHeightRequest = 150,
        }.BindDynamicThemeTextOnly();
        entryText.SetBinding(Entry.TextProperty, nameof(vm.FixText));
        entryText.SetBinding(Entry.IsEnabledProperty, nameof(vm.FixTextEnabled));
        entryText.TextChanged += vm.FixTextChanged;

        var stackFixes = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            Children =
            {
                border,
                labelText,
                entryText,
            }
        };

        return stackFixes;
    }

    private static ScrollView MakeOptions(RemoveTextForHiPageModel vm)
    {
        var grid = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            WidthRequest = LeftMenuWidth,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
            }
        };

        var row = 0;
        var viewRemoveBetween = MakeGridRemoveBetween(vm);
        grid.Add(viewRemoveBetween, row++);

        var viewRemoveBeforeColon = MakeViewRemoveBeforeColon(vm);
        grid.Add(viewRemoveBeforeColon, 0, row++);

        var viewRemoveUppercase = MakeViewRemoveIfUppercase(vm);
        grid.Add(viewRemoveUppercase, 0, row++);

        var viewRemoveIfTextContains = MakeRemoveIfTextContains(vm);
        grid.Add(viewRemoveIfTextContains, 0, row++);

        var viewRemoveIf = MakeRemoveIfOnlyMusicSymbols(vm);
        grid.Add(viewRemoveIf, 0, row++);

        var viewInterjections = MakeInterjections(vm);
        grid.Add(viewInterjections, 0, row++);

        var scrollView = new ScrollView
        {
            Content = grid,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        };

        return scrollView;
    }

    private static Border MakeInterjections(RemoveTextForHiPageModel vm)
    {
        var grid = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            WidthRequest = LeftMenuWidth,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
            }
        };

        var row = 0;

        var labelOnlyTextContains = new Label
        {
            Text = "Interjections",
            VerticalOptions = LayoutOptions.Center,
            FontAttributes = FontAttributes.Bold,
        }.BindDynamicThemeTextColorOnly();
        grid.Add(labelOnlyTextContains, 0, row);
        grid.SetColumnSpan(labelOnlyTextContains, 2);

        var pickerLanguage = new Picker
        {
            Title = "Language",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        pickerLanguage.SetBinding(Picker.ItemsSourceProperty, nameof(vm.Languages));
        pickerLanguage.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedLanguage));

        var buttonEditLanguage = new Button
        {
            Text = "Edit",
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(10, 0, 0, 0),
            Command = vm.EditInterjectionsCommand,
            BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.BackgroundColor],
        }.BindDynamicTheme();

        var stackPickerAndButton = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Children =
            {
                pickerLanguage,
                buttonEditLanguage,
            }
        };

        grid.Add(stackPickerAndButton, 1, row++);

        var labelRemoveInterjections = new Label
        {
            Text = "Remove interjections",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        grid.Add(labelRemoveInterjections, 0, row);

        var switchRemoveInterjections = new Switch
        {
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(10, 0, 0, 0),
        }.BindDynamicThemeTextColorOnly();
        switchRemoveInterjections.SetBinding(Switch.IsToggledProperty, nameof(vm.IsRemoveInterjectionsOn));
        grid.Add(switchRemoveInterjections, 1, row++);

        var labelOnlyIfSeparateLine = new Label
        {
            Text = "Only if separate line",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        grid.Add(labelOnlyIfSeparateLine, 0, row);

        var switchOnlyIfSeparateLine = new Switch
        {
            Margin = new Thickness(10, 0, 0, 0),
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        switchOnlyIfSeparateLine.SetBinding(Switch.IsToggledProperty, nameof(vm.IsInterjectionsSeparateLineOn));
        grid.Add(switchOnlyIfSeparateLine, 1, row++);

        var border = new Border
        {
            Padding = new Thickness(10),
            StrokeThickness = 0,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Content = grid,
            BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.TableHeaderBackgroundColor],
            Margin = new Thickness(0, 0, 0, 15),
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
        }.BindDynamicTheme();
        return border;
    }

    private static Border MakeRemoveIfTextContains(RemoveTextForHiPageModel vm)
    {
        var grid = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            WidthRequest = LeftMenuWidth,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
            }
        };

        var row = 0;

        var labelOnlyTextContains = new Label
        {
            Text = "Remove if text contains",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        grid.Add(labelOnlyTextContains, 0, row);

        var switchRemoveIfTextContains = new Switch
        {
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(10, 0, 0, 0),
        }.BindDynamicThemeTextColorOnly();
        switchRemoveIfTextContains.SetBinding(Switch.IsToggledProperty, nameof(vm.IsRemoveTextContainsOn));

        var entryContains = new Entry
        {
            Placeholder = "Contains",
            HorizontalOptions = LayoutOptions.Fill,
            Margin = new Thickness(10, 0, 0, 10),
        }.BindDynamicThemeTextOnly();
        entryContains.SetBinding(Entry.TextProperty, nameof(vm.TextContains));

        var stackSwitchAndEntry = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Children =
            {
                entryContains,
                switchRemoveIfTextContains,
            }
        };

        grid.Add(stackSwitchAndEntry, 1, row++);

        var border = new Border
        {
            Padding = new Thickness(10),
            StrokeThickness = 0,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Content = grid,
            BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.TableHeaderBackgroundColor],
            Margin = new Thickness(0, 0, 0, 15),
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
        }.BindDynamicTheme();
        return border;
    }

    private static Border MakeRemoveIfOnlyMusicSymbols(RemoveTextForHiPageModel vm)
    {
        var grid = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            WidthRequest = LeftMenuWidth,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
            }
        };

        var row = 0;

        var labelRemoveIfOnlyMusicSymbols = new Label
        {
            Text = "Remove if only music symbols",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        grid.Add(labelRemoveIfOnlyMusicSymbols, 0, row);

        var switchRemoveIfOnlyMusicSymbols = new Switch
        {
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(10, 0, 0, 0),
        }.BindDynamicThemeTextColorOnly();
        switchRemoveIfOnlyMusicSymbols.SetBinding(Switch.IsToggledProperty, nameof(vm.IsRemoveOnlyMusicSymbolsOn));
        grid.Add(switchRemoveIfOnlyMusicSymbols, 1, row++);

        var border = new Border
        {
            Padding = new Thickness(10),
            StrokeThickness = 0,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Content = grid,
            BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.TableHeaderBackgroundColor],
            Margin = new Thickness(0, 0, 0, 15),
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
        }.BindDynamicTheme();
        return border;
    }

    private static Border MakeViewRemoveBeforeColon(RemoveTextForHiPageModel vm)
    {
        var grid = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            WidthRequest = LeftMenuWidth,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
            }
        };

        var row = 0;
        var labelRemoveTextBeforeColon = new Label
        {
            Text = "Remove text before colon",
            VerticalOptions = LayoutOptions.Center,
            FontAttributes = FontAttributes.Bold,
        }.BindDynamicThemeTextColorOnly();
        grid.Add(labelRemoveTextBeforeColon, 0, row);

        var switchRemoveTextBeforeColon = new Switch
        {
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(10, 0, 0, 0),
        }.BindDynamicThemeTextColorOnly();
        switchRemoveTextBeforeColon.SetBinding(Switch.IsToggledProperty, nameof(vm.IsRemoveTextBeforeColonOn));
        grid.Add(switchRemoveTextBeforeColon, 1, row++);

        var labelOnlyTextUppercase = new Label
        {
            Text = "Only if text is uppercase",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        grid.Add(labelOnlyTextUppercase, 0, row);

        var switchOnlyTextUppercase = new Switch
        {
            Margin = new Thickness(10, 0, 0, 0),
        }.BindDynamicThemeTextColorOnly();
        switchOnlyTextUppercase.SetBinding(Switch.IsToggledProperty, nameof(vm.IsRemoveTextBeforeColonUppercaseOn));
        grid.Add(switchOnlyTextUppercase, 1, row++);

        var labelOnlyIfSeparateLine = new Label
        {
            Text = "Only if separate line",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        grid.Add(labelOnlyIfSeparateLine, 0, row);

        var switchOnlyIfSeparateLine = new Switch
        {
            Margin = new Thickness(10, 0, 0, 0),
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        switchOnlyIfSeparateLine.SetBinding(Switch.IsToggledProperty, nameof(vm.IsRemoveTextBeforeColonSeparateLineOn));
        grid.Add(switchOnlyIfSeparateLine, 1, row++);

        var borderRemoveBeforeColon = new Border
        {
            Padding = new Thickness(10),
            StrokeThickness = 0,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Content = grid,
            BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.TableHeaderBackgroundColor],
            Margin = new Thickness(0, 0, 0, 15),
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
        }.BindDynamicTheme();
        return borderRemoveBeforeColon;
    }

    private static Border MakeViewRemoveIfUppercase(RemoveTextForHiPageModel vm)
    {
        var grid = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            WidthRequest = LeftMenuWidth,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
            }
        };

        var row = 0;
        var labelOnlyTextUppercase = new Label
        {
            Text = "Only if text is uppercase",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        grid.Add(labelOnlyTextUppercase, 0, row);

        var switchOnlyTextUppercase = new Switch
        {
            Margin = new Thickness(10, 0, 0, 0),
        }.BindDynamicThemeTextColorOnly();
        switchOnlyTextUppercase.SetBinding(Switch.IsToggledProperty, nameof(vm.IsRemoveTextUppercaseLineOn));
        grid.Add(switchOnlyTextUppercase, 1, row++);

        var borderRemoveBeforeColon = new Border
        {
            Padding = new Thickness(10),
            StrokeThickness = 0,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Content = grid,
            BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.TableHeaderBackgroundColor],
            Margin = new Thickness(0, 0, 0, 15),
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
        }.BindDynamicTheme();
        return borderRemoveBeforeColon;
    }

    private static Border MakeGridRemoveBetween(RemoveTextForHiPageModel vm)
    {
        var gridRemoveBetween = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            WidthRequest = LeftMenuWidth,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
            }
        };

        var row = 0;

        var labelRemoveTextBetween = new Label
        {
            Text = "Remove text between",
            VerticalOptions = LayoutOptions.Center,
            FontAttributes = FontAttributes.Bold,
        }.BindDynamicThemeTextColorOnly();
        gridRemoveBetween.Add(labelRemoveTextBetween, 0, row++);


        var labelRemoveBrackets = new Label
        {
            Text = "Remove brackets",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        gridRemoveBetween.Add(labelRemoveBrackets, 0, row);

        var switchBrackets = new Switch
        {
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(10, 0, 0, 0),
        }.BindDynamicThemeTextColorOnly();
        switchBrackets.SetBinding(Switch.IsToggledProperty, nameof(vm.IsRemoveBracketsOn));
        gridRemoveBetween.Add(switchBrackets, 1, row++);

        var labelRemoveCurly = new Label
        {
            Text = "Remove curly brackets",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        gridRemoveBetween.Add(labelRemoveCurly, 0, row);

        var switchCurlyBrackets = new Switch
        {
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(10, 0, 0, 0),
        }.BindDynamicThemeTextColorOnly();
        switchCurlyBrackets.SetBinding(Switch.IsToggledProperty, nameof(vm.IsRemoveCurlyBracketsOn));
        gridRemoveBetween.Add(switchCurlyBrackets, 1, row++);


        var labelParentheses = new Label
        {
            Text = "Remove parentheses",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        gridRemoveBetween.Add(labelParentheses, 0, row);

        var switchParentheses = new Switch
        {
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(10, 0, 0, 0),
        }.BindDynamicThemeTextColorOnly();
        switchParentheses.SetBinding(Switch.IsToggledProperty, nameof(vm.IsRemoveParenthesesOn));
        gridRemoveBetween.Add(switchParentheses, 1, row++);


        var entryCustomStart = new Entry
        {
            Placeholder = "Start",
            HorizontalOptions = LayoutOptions.Fill,
        }.BindDynamicThemeTextOnly();
        entryCustomStart.SetBinding(Entry.TextProperty, nameof(RemoveTextForHiPageModel.CustomStart));

        var labelAnd = new Label
        {
            Text = "and",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();

        var entryCustomEnd = new Entry
        {
            Placeholder = "End",
            HorizontalOptions = LayoutOptions.Fill,
        }.BindDynamicThemeTextOnly();
        entryCustomEnd.SetBinding(Entry.TextProperty, nameof(RemoveTextForHiPageModel.CustomEnd));

        var stackCustom = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Children =
            {
                entryCustomStart,
                labelAnd,
                entryCustomEnd,
            }
        };
        gridRemoveBetween.Add(stackCustom, 0, row);

        var switchCustom = new Switch
        {
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(10, 0, 0, 0),
        }.BindDynamicThemeTextColorOnly();
        switchCustom.SetBinding(Switch.IsToggledProperty, nameof(vm.IsRemoveCustomOn));
        gridRemoveBetween.Add(switchCustom, 1, row++);


        var labelOnlySeparateLines = new Label
        {
            Text = "Only separate lines",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        gridRemoveBetween.Add(labelOnlySeparateLines, 0, row);

        var switchOnlySeparateLines = new Switch
        {
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(10, 0, 0, 0),
        }.BindDynamicThemeTextColorOnly();
        switchOnlySeparateLines.SetBinding(Switch.IsToggledProperty, nameof(vm.IsOnlySeparateLine));
        gridRemoveBetween.Add(switchOnlySeparateLines, 1, row++);

        var borderRemoveBetween = new Border
        {
            Padding = new Thickness(10),
            StrokeThickness = 0,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Content = gridRemoveBetween,
            BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.TableHeaderBackgroundColor],
            Margin = new Thickness(0, 0, 0, 15),
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
        }.BindDynamicTheme();

        return borderRemoveBetween;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
}
