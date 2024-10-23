using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Features.Video.TextToSpeech;

public class ReviewSpeechPage : ContentPage
{
    public ReviewSpeechPage(ReviewSpeechPageModel vm)
    {
        this.BindDynamicTheme();

        BindingContext = vm;

        vm.Page = this;

        Resources.Add(ThemeHelper.GetGridSelectionStyle());

        var grid = new Grid
        {
            Margin = new Thickness(10, 10, 10, 10),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            RowDefinitions = new RowDefinitionCollection
            {
                new() { Height = new GridLength(1, GridUnitType.Auto) }, // title
                new() { Height = new GridLength(1, GridUnitType.Star) }, // collection view / audio segments
                new() { Height = new GridLength(100, GridUnitType.Absolute) }, // waveform
                new() { Height = new GridLength(1, GridUnitType.Auto) }, // buttons (done)  
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new() { Width = new GridLength(4, GridUnitType.Star) }, 
                new() { Width = new GridLength(1, GridUnitType.Star) }, // buttons
            },
        }.BindDynamicTheme();

        var labelTitle = new Label
        {
            Margin = new Thickness(5, 15, 15, 0),
            Text = "Review audio segments",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            FontSize = 20,
        }.BindDynamicTheme();
        grid.Add(labelTitle, 0);

        var audioSegmentsBorder = MakeAudioSegmentsView(vm);
        grid.Add(audioSegmentsBorder, 0, 1);

        var buttonEditText = new Button
        {
            Text = "Edit text",
            Margin = new Thickness(10, 10, 10 ,25),
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.EditTextCommand,
        }.BindDynamicTheme();

        var buttonRegenerate = new Button
        {
            Text = "Regenerate audio",
            Margin = new Thickness(10),
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.RegenerateCommand,
        }.BindDynamicTheme();
        buttonRegenerate.SetBinding(IsEnabledProperty, nameof(vm.IsRegenerateEnabled));

        var labelEngine = new Label
        {
            Text = "Engine",
            Margin = new Thickness(10, 5, 5, 0),
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        var pickerEngine = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        pickerEngine.SetBinding(Picker.ItemsSourceProperty, nameof(vm.Engines));
        pickerEngine.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedEngine));
        pickerEngine.SelectedIndexChanged += vm.PickerEngineSelectedIndexChanged;

        var labelEngineSettings = new Label
        {
            Text = "Settings",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(10, 0, 0, 0),
            TextDecorations = TextDecorations.Underline,
        }.BindDynamicTheme();
        labelEngineSettings.GestureRecognizers.Add(new TapGestureRecognizer { Command = vm.ShowEngineSettingsCommand });
        vm.LabelEngineSettings = labelEngineSettings;
        var engineSettingsPointerGesture = new PointerGestureRecognizer();
        engineSettingsPointerGesture.PointerEntered += vm.LabelEngineSettingsMouseEntered;
        engineSettingsPointerGesture.PointerExited += vm.LabelEngineSettingsMouseExited;
        labelEngineSettings.GestureRecognizers.Add(engineSettingsPointerGesture);
        labelEngineSettings.SetBinding(Label.IsVisibleProperty, nameof(vm.IsEngineSettingsVisible));


        var stackEngine = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(10, 5, 0, 0),
            Children =
            {
                labelEngine,
                pickerEngine,
                labelEngineSettings,
            }
        };


        var labelVoice = new Label
        {
            Text = "Voice",
            Margin = new Thickness(10, 5, 5, 0),
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        var pickerVoice = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        pickerVoice.SetBinding(Picker.ItemsSourceProperty, nameof(vm.Voices));
        pickerVoice.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedVoice));
        var stackVoice = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(10, 5, 0, 0),
            Children =
            {
                labelVoice,
                pickerVoice,
            }
        };

        var labelLanguage = new Label
        {
            Text = "Language",
            Margin = new Thickness(10, 5, 5, 0),
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        var pickerLanguage = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        pickerLanguage.SetBinding(Picker.ItemsSourceProperty, nameof(vm.Languages));
        pickerLanguage.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedLanguage));
        var stackLanguage = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(10, 5, 0, 0),
            Children =
            {
                labelLanguage,
                pickerLanguage,
            }
        };
        stackLanguage.SetBinding(IsVisibleProperty, nameof(vm.HasLanguageParameter));

        var buttonPlay = new Button
        {
            Text = "Play",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.PlayCommand,
        }.BindDynamicTheme();

        var buttonStop = new Button
        {
            Text = "Stop",
            Margin = new Thickness(10),
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.StopCommand,
        }.BindDynamicTheme();

        var stackPlayStop = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(10, 25, 0 , 0),
            Children =
            {
                buttonPlay,
                buttonStop,
            }
        };


        var labelAutoContinue = new Label
        {
            Text = "Auto continue",
            Margin = new Thickness(10,0,0,0),
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();

        var switchAutoContinue = new Switch
        {
            IsToggled = vm.AutoContinue,
            Margin = new Thickness(5, 0, 0, 0),
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        switchAutoContinue.SetBinding(Switch.IsToggledProperty, nameof(vm.AutoContinue));

        var stackAutoContinue = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(10, 0, 0, 0),
            Children =
            {
                labelAutoContinue,
                switchAutoContinue,
            }
        };


        var buttonRow = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(5),
            Children =
            {
                buttonEditText,
                buttonRegenerate,
                stackEngine,
                stackVoice,
                stackLanguage,
                stackPlayStop,
                stackAutoContinue,
            }
        };

        grid.Add(buttonRow, 1);
        grid.SetRowSpan(buttonRow, 2);   

        var waveformView = MakeWaveformView(vm);
        grid.Add(waveformView, 0, 2);
        Grid.SetColumnSpan(waveformView, 2);

        var buttonExport = new Button
        {
            Text = "Export...",
            Margin = new Thickness(10),
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.ExportCommand,
        }.BindDynamicTheme();

        var buttonDone = new Button
        {
            Text = "Done",
            Margin = new Thickness(10),
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.DoneCommand,
        }.BindDynamicTheme();

        var stackButtons = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Children =
            {
                buttonExport,
                buttonDone,
            }
        };

        grid.Add(stackButtons, 0, 3);

        Content = grid;
    }

    private View MakeWaveformView(ReviewSpeechPageModel vm)
    {
        vm.Player.WidthRequest = 1;
        vm.Player.HeightRequest = 1;
        vm.Player.ZIndex = -1000;

        vm.AudioVisualizer.HorizontalOptions = LayoutOptions.Fill;
        vm.AudioVisualizer.VerticalOptions = LayoutOptions.Fill;

        var grid = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Children =
            {
                vm.Player,
                vm.AudioVisualizer,
            }
        };

        return grid;
    }

    private Border MakeAudioSegmentsView(ReviewSpeechPageModel vm)
    {
        // Create the header grid
        var headerGrid = new Grid
        {
            BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.TableHeaderBackgroundColor],
            Padding = new Thickness(5),
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Include
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Number
                new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }, // Voice
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Chars/sec
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Speed
                new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) }, // Text
            },
        };

        // Add headers
        headerGrid.Add(new Label { Text = "Include", FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Center }, 0);
        headerGrid.Add(new Label { Text = "Number", FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Center }, 1);
        headerGrid.Add(new Label { Text = "Voice", FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Center }, 2);
        headerGrid.Add(new Label { Text = "Chars/sec", FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Center }, 3);
        headerGrid.Add(new Label { Text = "Speed", FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Center }, 4);
        headerGrid.Add(new Label { Text = "Text", FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Center }, 5);

        // Create collection view
        var collectionView = new CollectionView
        {
            SelectionMode = SelectionMode.Single,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            ItemTemplate = new DataTemplate(() =>
            {
                var rulesItemsGrid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Include
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Number
                        new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }, // Voice
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Chars/sec
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Speed
                        new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) }, // Text
                    },
                };

                var switchInclude = new Switch
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(5,0,0,0),
                }.BindDynamicThemeTextOnly();
                switchInclude.SetBinding(Switch.IsToggledProperty, nameof(ReviewRow.Include));
                rulesItemsGrid.Add(switchInclude, 0);

                var labelDateAndTime = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                }.BindDynamicThemeTextColorOnly();
                labelDateAndTime.SetBinding(Label.TextProperty, nameof(ReviewRow.Number));
                rulesItemsGrid.Add(labelDateAndTime, 1);

                var labelVoice = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                }.BindDynamicThemeTextColorOnly();
                labelVoice.SetBinding(Label.TextProperty, nameof(ReviewRow.Voice));
                rulesItemsGrid.Add(labelVoice, 2);

                var labelCps = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                }.BindDynamicThemeTextColorOnly();
                labelCps.SetBinding(Label.TextProperty, nameof(ReviewRow.Cps));
                rulesItemsGrid.Add(labelCps, 3);

                var labelSpeed = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                }.BindDynamicThemeTextColorOnly();
                labelSpeed.SetBinding(Label.TextProperty, nameof(ReviewRow.Speed));
                rulesItemsGrid.Add(labelSpeed, 4);

                var labelText = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Fill,
                }.BindDynamicThemeTextColorOnly();
                labelText.SetBinding(Label.TextProperty, nameof(ReviewRow.Text));
                rulesItemsGrid.Add(labelText, 5);

                return rulesItemsGrid;
            }),
        };
        vm.CollectionView = collectionView;


        // Add content
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

        gridLayout.Add(headerGrid, 0);
        gridLayout.Add(vm.CollectionView, 0, 1);

        vm.CollectionView.SelectionMode = SelectionMode.Single;
        vm.CollectionView.SelectionChanged += vm.CollectionViewSelectionChanged;
        vm.CollectionView.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.Lines));
        vm.CollectionView.SetBinding(SelectableItemsView.SelectedItemProperty, nameof(vm.SelectedLine));

        var border = new Border
        {
            Margin = new Thickness(0,10,0,10),
            Content = gridLayout,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
        }.BindDynamicTheme();

        return border;
    }

    protected override void OnDisappearing() {
        base.OnDisappearing();
        (BindingContext as ReviewSpeechPageModel)?.OnDisappearing();
    }
}
