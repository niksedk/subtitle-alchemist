using Microsoft.Maui.Controls.Shapes;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Logic.Constants;
using SubtitleAlchemist.Logic.Converters;

namespace SubtitleAlchemist.Features.Translate;

public class TranslatePage : ContentPage
{
    private readonly TranslatePageModel _vm;

    public TranslatePage(TranslatePageModel vm)
    {
        this.BindDynamicTheme();

        BindingContext = vm;
        _vm = vm;

        vm.TranslatePage = this;

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

        var menuFlyoutMain = new MenuFlyout();
        var flyoutItem = new MenuFlyoutItem();
        flyoutItem.Text = "Advanced settings...";
        flyoutItem.Command = vm.ShowAdvancedSettingsCommand;
        menuFlyoutMain.Add(flyoutItem);
        FlyoutBase.SetContextFlyout(grid, menuFlyoutMain);

        var poweredByLabel = new Label
        {
            Margin = new Thickness(15, 15, 0, 0),
            Text = "Powered by ",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            FontSize = 12,
        }.BindDynamicTheme();

        vm.TitleLabel = new Label
        {
            Margin = new Thickness(5, 15, 15, 0),
            Text = "...",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            FontSize = 12,
            TextDecorations = TextDecorations.Underline,
        }.BindDynamicTheme();

        var titleTexts = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Children =
            {
                poweredByLabel,
                vm.TitleLabel,
            }
        };

        var pointerGesture = new PointerGestureRecognizer();
        pointerGesture.PointerEnteredCommand = new Command(vm.MouseEnteredPoweredBy);
        pointerGesture.PointerExitedCommand = new Command(vm.MouseExitedPoweredBy);
        vm.TitleLabel.GestureRecognizers.Add(pointerGesture);
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += vm.MouseClickedPoweredBy;
        vm.TitleLabel.GestureRecognizers.Add(tapGesture);

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

        vm.EnginePicker = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            SelectedIndex = 0,
        }.BindDynamicTheme();
        vm.EnginePicker.SetBinding(Picker.ItemsSourceProperty, "AutoTranslators");
        vm.EnginePicker.ItemDisplayBinding = new Binding("Name");
        vm.EnginePicker.SetBinding(Picker.SelectedItemProperty, "SelectedAutoTranslator");
        vm.EnginePicker.SelectedIndexChanged += vm.EngineSelectedIndexChanged;
        gridLeft.Add(vm.EnginePicker, 0, 0);

        var fromLabel = new Label
        {
            Text = "From:",
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 10, 0),
        }.BindDynamicTheme();
        gridLeft.Add(fromLabel, 2, 0);

        vm.SourceLanguagePicker = new Picker
        {
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        vm.SourceLanguagePicker.ItemsSource = vm.SourceLanguages;
        vm.SourceLanguagePicker.SetBinding(Picker.SelectedItemProperty, nameof(vm.SourceLanguage));
        gridLeft.Add(vm.SourceLanguagePicker, 3, 0);

        grid.Add(gridLeft, 0, 1);


        var rightGrid = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            RowDefinitions = new RowDefinitionCollection
            {
                new() { Height = new GridLength(1, GridUnitType.Auto) },
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new() { Width = new GridLength(1, GridUnitType.Auto) },
                new() { Width = new GridLength(1, GridUnitType.Auto) },
                new() { Width = new GridLength(1, GridUnitType.Auto) },
                new() { Width = new GridLength(1, GridUnitType.Star) },
            },
        };

        rightGrid.Add(new Label
        {
            Text = "To:",
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 10, 0),
        }.BindDynamicTheme(), 0, 0);


        vm.TargetLanguagePicker = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        rightGrid.Add(vm.TargetLanguagePicker, 1, 0);
        vm.TargetLanguagePicker.ItemsSource = vm.TargetLanguages;
        vm.TargetLanguagePicker.SetBinding(Picker.SelectedItemProperty, nameof(vm.TargetLanguage));
        vm.TargetLanguagePicker.SelectedIndexChanged += vm.TargetLanguagePickerSelectedIndexChanged;

        vm.ButtonTranslate = new Button
        {
            Text = "Translate",
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(10, 0, 0, 0),
            Command = vm.TranslateCommand,
        }.BindDynamicTheme();

        rightGrid.Add(vm.ButtonTranslate, 2, 0);

        vm.ProgressBar = new ProgressBar
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            IsVisible = false,
            IsEnabled = false,
            Progress = 0.0,
            Margin = new Thickness(5, 0, 5, 0),
            ProgressColor = (Color)Application.Current!.Resources[ThemeNames.ProgressColor],
        };

        rightGrid.Add(vm.ProgressBar, 3, 0);

        grid.Add(rightGrid, 1, 1);

        // Define CollectionView
        vm.CollectionView = new CollectionView
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
                startTimeLabel.SetBinding(Label.TextProperty, nameof(TranslateRow.StartTime), BindingMode.Default, new TimeSpanToStringConverter());
                startTimeLabel.SetBinding(BackgroundColorProperty, "BackgroundColor");

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


        var menuFlyoutLines = new MenuFlyout();
        var flyoutItem2 = new MenuFlyoutItem();
        flyoutItem2.Text = "Translate from this line";
        flyoutItem2.Command = vm.TranslateFromCurrentLineCommand;
        menuFlyoutLines.Add(flyoutItem2);
        var flyoutItemCurrentLineOnly = new MenuFlyoutItem();
        flyoutItemCurrentLineOnly.Text = "Translate this line only";
        flyoutItemCurrentLineOnly.Command = vm.TranslateCurrentLineOnlyCommand;
        flyoutItemCurrentLineOnly.KeyboardAccelerators.Add(new KeyboardAccelerator
        {
            Modifiers = KeyboardAcceleratorModifiers.Ctrl,
            Key = "R"
        });
        menuFlyoutLines.Add(flyoutItemCurrentLineOnly);
        menuFlyoutLines.Add(new MenuFlyoutSeparator());
        var flyoutItemAdvancedSettings = new MenuFlyoutItem();
        flyoutItemAdvancedSettings.Text = "Advanced settings...";
        flyoutItemAdvancedSettings.Command = vm.ShowAdvancedSettingsCommand;
        menuFlyoutLines.Add(flyoutItemAdvancedSettings);

        FlyoutBase.SetContextFlyout(vm.CollectionView, menuFlyoutLines);


        // Create the header grid
        var headerGrid = new Grid
        {
            BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.TableHeaderBackgroundColor],
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
        headerGrid.Add(new Label { Text = "Show", FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Center }, 1, 0);
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
        gridLayout.Add(vm.CollectionView, 0, 1);

        vm.CollectionView.SelectionMode = SelectionMode.Single;
        vm.CollectionView.SelectionChanged += vm.CollectionViewSelectionChanged;

        var frame = new Border
        {
            Content = gridLayout,
            Padding = new Thickness(5),
            Margin = new Thickness(10),
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
        }.BindDynamicTheme();
        frame.Content = gridLayout;

        grid.Add(frame, 0, 2);
        Grid.SetColumnSpan(frame, 2);

        vm.LabelApiKey = new Label
        {
            Text = "API key",
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(20, 0, 0, 0),
        }.BindDynamicTheme();

        vm.EntryApiKey = new Entry
        {
            Text = string.Empty,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 200,
            Placeholder = "Enter API key",
            Margin = new Thickness(3, 0, 10, 0),
        }.BindDynamicTheme();

        vm.LabelApiUrl = new Label
        {
            Text = "API url",
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(20, 0, 0, 0),
        }.BindDynamicTheme();

        vm.EntryApiUrl = new Entry
        {
            Text = string.Empty,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 250,
            Placeholder = "Enter API url",
            Margin = new Thickness(3, 0, 0, 0),
        }.BindDynamicTheme();

        vm.ButtonApiUrl = new Button
        {
            Text = "...",
            VerticalOptions = LayoutOptions.Center,
            Command = vm.PickApiUrlCommand,
        }.BindDynamicTheme();

        vm.LabelModel = new Label
        {
            Text = "Model",
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(20, 0, 0, 0),
        }.BindDynamicTheme();

        vm.EntryModel = new Entry
        {
            Text = string.Empty,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 250,
            Placeholder = "Enter model",
            Margin = new Thickness(3, 0, 0, 0),
        };

        vm.ButtonModel = new Button
        {
            Text = "...",
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(3, 0, 0, 0),
            Command = vm.PickModelCommand,
        }.BindDynamicTheme();

        vm.LabelFormality = new Label
        {
            Text = "Formality",
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(20, 0, 0, 0),
        }.BindDynamicTheme();

        vm.PickerFormality = new Picker
        {
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(3, 0, 0, 0),
        }.BindDynamicTheme();
        vm.PickerFormality.ItemsSource = vm.Formalities;
        vm.PickerFormality.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedFormality));

        var settingsRow = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(10),
            Children =
            {
                vm.LabelApiKey,
                vm.EntryApiKey,
                vm.LabelApiUrl,
                vm.EntryApiUrl,
                vm.ButtonApiUrl,
                vm.LabelModel,
                vm.EntryModel,
                vm.ButtonModel,
                vm.LabelFormality,
                vm.PickerFormality,
            }
        };

        grid.Add(settingsRow, 0, 3);
        Grid.SetColumnSpan(settingsRow, 2);

        vm.ButtonOk = new Button
        {
            Text = "OK",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 10, 0),
            Command = vm.OkCommand,
        }.BindDynamicTheme();

        vm.ButtonCancel = new Button
        {
            Text = "Cancel",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.CancelCommand,
        }.BindDynamicTheme();

        var buttonRow = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(10),
            Children =
            {
                vm.ButtonOk,
                vm.ButtonCancel,
            }
        };

        grid.Add(buttonRow, 0, 4);
        Grid.SetColumnSpan(buttonRow, 2);


        Content = grid;

        vm.CollectionView.SetBinding(ItemsView.ItemsSourceProperty, "Lines");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (_vm.Lines.Count > 0 && _vm.CollectionView.SelectedItem == null)
                {
                    _vm.Lines[0].BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.ActiveBackgroundColor];
                    _vm.CollectionView.SelectedItem = _vm.Lines[0];
                    _vm.Lines = _vm.Lines;
                }
                else if (_vm.CollectionView.SelectedItem is TranslateRow tr)
                {
                    tr.BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.ActiveBackgroundColor];
                    _vm.Lines = _vm.Lines;
                }
            });

            if (!string.IsNullOrWhiteSpace(Se.Settings.Tools.AutoTranslateLastName))
            {
                var item = _vm.AutoTranslators.FirstOrDefault(p => p.Name == Se.Settings.Tools.AutoTranslateLastName);
                if (item != null)
                {
                    _vm.SelectedAutoTranslator = item;
                }
            }

            return false;
        });
    }
}
