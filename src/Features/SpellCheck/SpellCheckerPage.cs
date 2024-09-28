using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.SpellCheck;

public class SpellCheckerPage : ContentPage
{
    public SpellCheckerPage(SpellCheckerPageModel vm)
    {
        this.BindDynamicTheme();
        vm.Page = this;
        BindingContext = vm;

        var grid = new Grid
        {
            RowDefinitions = new RowDefinitionCollection
            {
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new() { Width = new GridLength(1, GridUnitType.Star) },
                new() { Width = new GridLength(1, GridUnitType.Star) },
                new() { Width = new GridLength(1, GridUnitType.Star) },
            },
            Margin = new Thickness(25),
        };

        var labelTitle = new Label
        {
            Text = "Spell Checker",
            FontSize = 24,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 15),
        };
        grid.Add(labelTitle, 0);
        grid.SetColumnSpan(labelTitle, 2);


        var labelCurrentText = new Label
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
        };
        labelCurrentText.SetBinding(Label.TextProperty, nameof(vm.CurrentText));
        var borderCurrentText = new Border
        {
            Content = labelCurrentText,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            HeightRequest = 150,
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
        };
        grid.Add(borderCurrentText, 0, 1);

        var buttonEditWholeText = new Button
        {
            Text = "Edit Whole Text",
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 5, 0, 25)
        }.BindDynamicTheme();
        grid.Add(buttonEditWholeText, 0, 2);

        var labelWordNotFound = new Label
        {
            Text = "Word Not Found",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 5)
        };
        grid.Add(labelWordNotFound, 0, 3);

        var entryWordNotFound = new Entry
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 5),
        }.BindDynamicTheme();
        entryWordNotFound.SetBinding(Entry.TextProperty, nameof(vm.CurrentWord));
        grid.Add(entryWordNotFound, 0, 4);

        var gridWordButtons = MakeButtonGrid(vm);
        grid.Add(gridWordButtons, 0, 5);


        var column2Grid = MakeSuggestionsGrid(vm);
        grid.Add(column2Grid, 1, 1);
        grid.SetRowSpan(column2Grid, 9);

        // add video player / subtitle list (from model in query method)

        Content = grid;
    }

    private static Grid MakeButtonGrid(SpellCheckerPageModel vm)
    {
        var gridWordButtons = new Grid
        {
            RowDefinitions = new RowDefinitionCollection
            {
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new() { Width = new GridLength(1, GridUnitType.Star) },
                new() { Width = new GridLength(1, GridUnitType.Star) },
            },
            ColumnSpacing = 10,
            RowSpacing = 10,
        };


        var buttonChange = new Button
        {
            Text = "Change",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Command = vm.ChangeWordCommand,
        }.BindDynamicTheme();
        gridWordButtons.Add(buttonChange, 0);

        var buttonChangeAll = new Button
        {
            Text = "Change All",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Command = vm.ChangeAllWordsCommand,
        }.BindDynamicTheme();
        gridWordButtons.Add(buttonChangeAll, 1);

        var buttonSkip = new Button
        {
            Text = "Skip",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Command = vm.SkipWordCommand,
        }.BindDynamicTheme();
        gridWordButtons.Add(buttonSkip, 0, 1);

        var buttonSkipAll = new Button
        {
            Text = "Skip All",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Command = vm.SkipAllWordCommand,
        }.BindDynamicTheme();
        gridWordButtons.Add(buttonSkipAll, 1, 1);

        var buttonAddToNamesList = new Button
        {
            Text = "Add to Names List",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Command = vm.AddToNamesCommand,
        }.BindDynamicTheme();
        gridWordButtons.Add(buttonAddToNamesList, 0, 2);
        gridWordButtons.SetColumnSpan(buttonAddToNamesList, 2);

        var buttonAddToUserDictionary = new Button
        {
            Text = "Add to User Dictionary",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Command = vm.AddToUserDictionaryCommand,
        }.BindDynamicTheme();
        gridWordButtons.Add(buttonAddToUserDictionary, 0, 3);
        gridWordButtons.SetColumnSpan(buttonAddToUserDictionary, 2);

        var buttonGoogleIt = new Button
        {
            Text = "Google It",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Command = vm.GoogleItCommand,
        }.BindDynamicTheme();
        gridWordButtons.Add(buttonGoogleIt, 0, 4);
        gridWordButtons.SetColumnSpan(buttonGoogleIt, 2);

        return gridWordButtons;
    }

    private static Grid MakeSuggestionsGrid(SpellCheckerPageModel vm)
    {
        var grid = new Grid
        {
            RowDefinitions = new RowDefinitionCollection
            {
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Star) }, // suggestions
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new() { Width = new GridLength(1, GridUnitType.Star) },
                new() { Width = new GridLength(1, GridUnitType.Star) },
            },
            ColumnSpacing = 10,
            RowSpacing = 10,
            Margin = new Thickness(25, 0, 25, 0),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        };

        var labelLanguage = new Label
        {
            Text = "Language",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        };

        var pickerLanguage = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 150,
            Margin = new Thickness(10, 0, 10, 0),
        }.BindDynamicTheme();
        pickerLanguage.SetBinding(Picker.ItemsSourceProperty, nameof(vm.Languages));
        pickerLanguage.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedLanguage));
        pickerLanguage.SelectedIndexChanged += vm.LanguageChanged;


        var buttonDownloadDictionary = new Button
        {
            Text = "...",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.DownloadDictionaryCommand,
        }.BindDynamicTheme();

        var languageBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Margin = new Thickness(0, 0, 0, 15),
            Children =
            {
                labelLanguage,
                pickerLanguage,
                buttonDownloadDictionary,
            }
        };
        grid.Add(languageBar, 0);
        grid.SetColumnSpan(languageBar, 2);

        var labelSuggestions = new Label
        {
            Text = "Suggestions",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        };
        grid.Add(labelSuggestions, 0, 1);

        var collectionViewSuggestions = new CollectionView
        {
            ItemTemplate = new DataTemplate(() =>
            {
                var labelSuggestion = new Label
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(5),
                };
                labelSuggestion.SetBinding(Label.TextProperty, ".");
                return labelSuggestion;
            }),
            SelectionMode = SelectionMode.Single,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 10),
        };
        collectionViewSuggestions.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.Suggestions));

        var borderSuggestions = new Border
        {
            Content = collectionViewSuggestions,
            StrokeThickness = 1,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Padding = new Thickness(5),
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
        };

        grid.Add(borderSuggestions, 0, 2);

        var buttonSuggestionUse = new Button
        {
            Text = "Use once",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 10, 0),
            Command = vm.SuggestionUseAlwaysCommand,
        }.BindDynamicTheme();

        var buttonSuggestionUseAlways = new Button
        {
            Text = "Use always",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Command = vm.SuggestionUseAlwaysCommand,
        }.BindDynamicTheme();

        var buttonBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Margin = new Thickness(0, 10, 0, 0),
            Children =
            {
                buttonSuggestionUse,
                buttonSuggestionUseAlways,
            }
        };
        grid.Add(buttonBar, 0, 3);


        return grid;
    }
}