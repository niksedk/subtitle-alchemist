﻿using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Features.Main;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Converters;
using System.Collections.ObjectModel;
using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Features.SpellCheck;

public class SpellCheckerPage : ContentPage
{
    private readonly Grid _mainGrid;
    private readonly SpellCheckerPageModel _vm;

    public SpellCheckerPage(SpellCheckerPageModel vm)
    {
        this.BindDynamicTheme();
        vm.Page = this;
        BindingContext = vm;
        _vm = vm;

        var grid = new Grid
        {
            RowDefinitions = new RowDefinitionCollection
            {
                new() { Height = new GridLength(1, GridUnitType.Auto) }, // title
                new() { Height = new GridLength(1, GridUnitType.Star) }, // content
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new() { Width = new GridLength(2, GridUnitType.Star) }, // spell check
                new() { Width = new GridLength(1, GridUnitType.Star) }, // language and suggestions
                new() { Width = new GridLength(2, GridUnitType.Star) }, // subtitle list / video player
            },
            Margin = new Thickness(25),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
        };

        _mainGrid = grid;

        var labelTitle = new Label
        {
            FontSize = 20,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 15),
        };
        labelTitle.SetBinding(Label.TextProperty, nameof(vm.Title));
        grid.Add(labelTitle, 0);
        grid.SetColumnSpan(labelTitle, 2);

        grid.Add(MakeSpellCheckColumn(vm), 0, 1);    

        grid.Add(MakeLanguageAndSuggestionsColumn(vm), 1, 1);

        Content = grid;
    }

    private static StackLayout MakeSpellCheckColumn(SpellCheckerPageModel vm)
    {
        var labelCurrentText = new Label
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
        };
        labelCurrentText.SetBinding(Label.FormattedTextProperty, nameof(vm.CurrentFormattedText));

       var scrollViewCurrentText = new ScrollView
        {
            Content = labelCurrentText,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            HeightRequest = 120,
        }.BindDynamicTheme();

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

        var buttonEditWholeText = new Button
        {
            Text = "Edit Whole Text",
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 5, 0, 15),
            Command = vm.EditCurrentTextCommand,
        }.BindDynamicTheme();

        var labelWordNotFound = new Label
        {
            Text = "Word Not Found",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 5)
        };

        var entryWordNotFound = new Entry
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 5),
        }.BindDynamicTheme();
        entryWordNotFound.SetBinding(Entry.TextProperty, nameof(vm.CurrentWord));
        vm.EntryWordNotFound = entryWordNotFound;

        var gridWordButtons = MakeButtonGrid(vm);

        var column1 = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Children =
            {
                borderCurrentText,
                buttonEditWholeText,
                labelWordNotFound,
                entryWordNotFound,
                gridWordButtons,
            }
        };
        return column1;
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

        var labelStatusText = new Label
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
        };
        labelStatusText.SetBinding(Label.TextProperty, nameof(vm.StatusText));
        gridWordButtons.Add(labelStatusText, 0, 5);
        vm.LabelStatusText = labelStatusText;

        var okCancelBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Margin = new Thickness(0, 10, 0, 0),
            Children =
            {
                new Button
                {
                    Text = "OK",
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    Command = vm.OkCommand,
                    Margin = new Thickness(0, 0, 10, 0),
                }.BindDynamicTheme(),
                new Button
                {
                    Text = "Cancel",
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    Command = vm.CancelCommand,
                }.BindDynamicTheme(),
            }
        };

        gridWordButtons.Add(okCancelBar, 0, 6);
        gridWordButtons.SetColumnSpan(okCancelBar, 2);

        return gridWordButtons;
    }

    private static StackLayout MakeLanguageAndSuggestionsColumn(SpellCheckerPageModel vm)
    {
       var labelLanguage = new Label
        {
            Text = "Language",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        };

        var pickerLanguage = new Picker
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(10, 0, 10, 0),
        }.BindDynamicTheme();
        pickerLanguage.SetBinding(Picker.ItemsSourceProperty, nameof(vm.Languages));
        pickerLanguage.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedLanguage));
        pickerLanguage.SelectedIndexChanged += vm.LanguageChanged;

        var buttonDownloadDictionary = new Button
        {
            Text = "...",
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.DownloadDictionaryCommand,
            Margin = new Thickness(0,5,10,5),
        }.BindDynamicTheme();


        var languageGrid = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            RowDefinitions = new RowDefinitionCollection
            {
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new() { Width = new GridLength(1, GridUnitType.Auto) },
                new() { Width = new GridLength(1, GridUnitType.Star) },
            },
            ColumnSpacing = 0,
            RowSpacing = 0,
        };
        languageGrid.Add(labelLanguage, 0);
        languageGrid.Add(pickerLanguage, 1);
        languageGrid.Add(buttonDownloadDictionary, 1, 1);

       
        var labelSuggestions = new Label
        {
            Text = "Suggestions",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0,0,0,2),
        };

        var collectionViewSuggestions = new CollectionView
        {
            ItemTemplate = new DataTemplate(() =>
            {
                var labelSuggestion = new Label
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    Margin = new Thickness(5),
                };
                labelSuggestion.SetBinding(Label.TextProperty, ".");
                return labelSuggestion;
            }),
            SelectionMode = SelectionMode.Single,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 10),
            HeightRequest = 325,
        }.BindDynamicTheme();
        collectionViewSuggestions.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.Suggestions));
        collectionViewSuggestions.SetBinding(SelectableItemsView.SelectedItemProperty, nameof(vm.SelectedSuggestion), BindingMode.TwoWay);

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

        var buttonSuggestionUse = new Button
        {
            Text = "Use once",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 10, 0),
            Command = vm.SuggestionUseOnceCommand,
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

        var column = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(20, 0, 20, 0),
            Children =
            {
                languageGrid,
                labelSuggestions,
                borderSuggestions,
                buttonBar,
            }
        };

        return column;
    }

    public void Initialize(Subtitle subtitle, string videoFileName, SpellCheckerPageModel vm)
    {
        vm.SubtitleList.BatchBegin();
        vm.Paragraphs = new ObservableCollection<DisplayParagraph>(subtitle.Paragraphs.Select(p => new DisplayParagraph(p)));
        vm.SubtitleList.BatchCommit();

        var subtitleGrid = MakeSubtitleGrid(vm);
        if (string.IsNullOrEmpty(videoFileName))
        {
            // only subtitle list
            _mainGrid.Add(subtitleGrid, 2,1);
            return;
        }

        var grid = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.TableHeaderBackgroundColor],
            Padding = new Thickness(5),
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
            },
            RowDefinitions =
            {
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }, // video player
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }, // subtitle list
            },
            HeightRequest = 500,
        };

        // video player and subtitle list
        var mediaElementGrid = MakeMediaElementGrid(vm);
        mediaElementGrid.Margin = new Thickness(0, 0, 0, 10);
        vm.VideoPlayer.Source = MediaSource.FromFile(videoFileName);
        
        grid.Add(mediaElementGrid, 0);
        grid.Add(subtitleGrid, 0, 1);

        _mainGrid.Add(grid, 2, 1);
    }

    private static MediaElement MakeMediaElementGrid(SpellCheckerPageModel vm)
    {
        vm.VideoPlayer = new MediaElement { ZIndex = -10000 };
        return vm.VideoPlayer;
    }

    private static Border MakeSubtitleGrid(SpellCheckerPageModel vm)
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

    protected override void OnDisappearing()
    {
        _vm.OnDisappearing();
        base.OnDisappearing();
    }
}