using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Features.Main;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Converters;
using System.Collections.ObjectModel;

namespace SubtitleAlchemist.Features.SpellCheck;

public class SpellCheckerPage : ContentPage
{
    private readonly Grid _mainGrid;


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

        _mainGrid = grid;

        var labelTitle = new Label
        {
            FontSize = 24,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 15),
        };
        labelTitle.SetBinding(Label.TextProperty, nameof(vm.Title));
        grid.Add(labelTitle, 0);
        grid.SetColumnSpan(labelTitle, 2);


        var labelCurrentText = new Label
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
        };

        var scrollViewCurrentText = new ScrollView
        {
            Content = labelCurrentText,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            HeightRequest = 130,
        }.BindDynamicTheme();

        labelCurrentText.SetBinding(Label.FormattedTextProperty, nameof(vm.CurrentFormattedText));
        var borderCurrentText = new Border
        {
            Content = scrollViewCurrentText,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
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
                new() { Height = new GridLength(1, GridUnitType.Auto) }, // suggestions
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
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 10),
            HeightRequest = 395,
        };
        collectionViewSuggestions.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.Suggestions));

        var borderSuggestions = new Border
        {
            Content = collectionViewSuggestions,
            StrokeThickness = 1,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
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

    public void Initialize(Subtitle subtitle, string videoFileName, SpellCheckerPageModel vm)
    {
        vm.SubtitleList.BatchBegin();
        vm.Paragraphs = new ObservableCollection<DisplayParagraph>(subtitle.Paragraphs.Select(p => new DisplayParagraph(p)));
        vm.SubtitleList.BatchCommit();

        if (string.IsNullOrEmpty(videoFileName))
        {
            // no video player or waveform
            var subtitleGrid = MakeSubtitleGrid(vm);
            _mainGrid.Add(subtitleGrid, 2);
            _mainGrid.SetRowSpan(subtitleGrid, 8);

            return;
        }

        // video player and waveform
        var mediaElementGrid = MakeMediaElementGrid(vm);
        mediaElementGrid.Margin = new Thickness(0, 0, 0, 10);
        _mainGrid.Add(mediaElementGrid, 2);
        _mainGrid.SetRowSpan(mediaElementGrid, 9);

        vm.VideoPlayer.Source = MediaSource.FromFile(videoFileName);
    }

    private MediaElement MakeMediaElementGrid(SpellCheckerPageModel vm)
    {
        vm.VideoPlayer = new MediaElement { ZIndex = -10000 };
        return vm.VideoPlayer;
    }

    private Border MakeSubtitleGrid(SpellCheckerPageModel vm)
    {
        // Create the header grid
        var headerGrid = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            BackgroundColor = Color.FromRgb(22, 22, 22), //TODO: Add to resources, header background color
            Padding = new Thickness(5),
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) }
            },
        };

        // Add headers
        headerGrid.Add(new Label { Text = "Number", FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Center }, 0, 0);
        headerGrid.Add(new Label { Text = "Show", FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Center }, 1, 0);
        headerGrid.Add(new Label { Text = "Hide", FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Center }, 2, 0);
        headerGrid.Add(new Label { Text = "Text", FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Center }, 3, 0);



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
                        new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) }
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

                var originalTextLabel = new Label { VerticalTextAlignment = TextAlignment.Center }.BindDynamicThemeTextColorOnly();
                originalTextLabel.SetBinding(Label.TextProperty, nameof(DisplayParagraph.End), BindingMode.Default, new TimeSpanToStringConverter());

                var translatedTextLabel = new Label { VerticalTextAlignment = TextAlignment.Center }.BindDynamicThemeTextColorOnly();
                translatedTextLabel.SetBinding(Label.TextProperty, nameof(DisplayParagraph.Text));

                // Add labels to grid
                gridTexts.Add(numberLabel, 0, 0);
                gridTexts.Add(startTimeLabel, 1, 0);
                gridTexts.Add(originalTextLabel, 2, 0);
                gridTexts.Add(translatedTextLabel, 3, 0);

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
        vm.SubtitleList.SelectionChanged += vm.SubtitlesViewSelectionChanged;

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