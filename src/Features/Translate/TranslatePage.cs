using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Features.Translate;

public class TranslatePage : ContentPage
{
    public TranslatePage(TranslateModel vm)
    {
        BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.BackgroundColor];

        BindingContext = vm;

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

        vm.TitleLabel = new Label
        {
            Margin = new Thickness(15, 15, 15, 15),
            Text = "Powered by X",
            FontAttributes = FontAttributes.Bold,
            TextColor = (Color)Application.Current.Resources[ThemeNames.TextColor],
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        };
        var pointerGesture = new PointerGestureRecognizer();
        pointerGesture.PointerEnteredCommand = new Command(() => vm.MouseEnteredPoweredBy());
        pointerGesture.PointerExitedCommand = new Command(() => vm.MouseExitedPoweredBy());
        vm.TitleLabel.GestureRecognizers.Add(pointerGesture);
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += vm.MouseClickedPoweredBy;
        vm.TitleLabel.GestureRecognizers.Add(tapGesture);

        grid.Add(vm.TitleLabel, 0, 0);
        Grid.SetColumnSpan(vm.TitleLabel, 2);


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

        };
        vm.EnginePicker.SetBinding(Picker.ItemsSourceProperty, "AutoTranslators");
        vm.EnginePicker.SetBinding(Picker.SelectedItemProperty, "SelectedAutoTranslator");
        vm.EnginePicker.SelectedIndexChanged += vm.EngineSelectedIndexChanged;
        gridLeft.Add(vm.EnginePicker, 0, 0);

        var fromLabel = new Label
        {
            Text = "From:",
            FontAttributes = FontAttributes.Bold,
            TextColor = (Color)Application.Current.Resources[ThemeNames.TextColor],
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 10, 0),
        };
        gridLeft.Add(fromLabel, 2, 0);

        vm.SourceLanguagePicker = new Picker
        {
            TextColor = (Color)Application.Current.Resources[ThemeNames.TextColor],
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Items =
            {
                "English", "Danish",
            },
            SelectedIndex = 0,
        };
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
            TextColor = (Color)Application.Current.Resources[ThemeNames.TextColor],
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 10, 0),
        }, 0, 0);


        vm.TargetLanguagePicker = new Picker
        {
            TextColor = (Color)Application.Current.Resources[ThemeNames.TextColor],
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Items =
            {
                "English", "Danish",
            },
            SelectedIndex = 0,
        };
        rightGrid.Add(vm.TargetLanguagePicker, 1, 0);

        rightGrid.Add(new Button
        {
            Text = "Translate",
            TextColor = (Color)Application.Current.Resources[ThemeNames.TextColor],
            BackgroundColor = (Color)Application.Current.Resources[ThemeNames.SecondaryBackgroundColor],
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(10, 0, 0, 0),
            Command = vm.TranslateCommand,
        }, 2, 0);

        vm.ProgressBar = new ProgressBar
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            IsVisible = false,
            IsEnabled = false,
            Progress = 0.0,
            Margin = new Thickness(5, 0, 5, 0),
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
                var numberLabel = new Label { VerticalTextAlignment = TextAlignment.Center };
                numberLabel.SetBinding(Label.TextProperty, "Number");

                var startTimeLabel = new Label { VerticalTextAlignment = TextAlignment.Center };
                startTimeLabel.SetBinding(Label.TextProperty, new Binding("StartTime", stringFormat: "{HH:mm:ss.fff}"));

                var originalTextLabel = new Label { VerticalTextAlignment = TextAlignment.Center };
                originalTextLabel.SetBinding(Label.TextProperty, "OriginalText");

                var translatedTextLabel = new Label { VerticalTextAlignment = TextAlignment.Center };
                translatedTextLabel.SetBinding(Label.TextProperty, "TranslatedText");

                // Add labels to grid
                gridTexts.Add(numberLabel, 0, 0);
                gridTexts.Add(startTimeLabel, 1, 0);
                gridTexts.Add(originalTextLabel, 2, 0);
                gridTexts.Add(translatedTextLabel, 3, 0);

                return gridTexts;
            })
        };

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
        };

        gridLayout.Add(headerGrid, 0, 0);
        gridLayout.Add(vm.CollectionView, 0, 1);

        vm.CollectionView.SelectionChanged += vm.CollectionViewSelectionChanged;

        var frame = new Frame
        {
            Content = gridLayout,
            HasShadow = true,
            CornerRadius = 5,
            Padding = new Thickness(5),
            BackgroundColor = (Color)Application.Current.Resources[ThemeNames.BackgroundColor],
            BorderColor = (Color)Application.Current.Resources[ThemeNames.BorderColor],
            Margin = new Thickness(10),
        };
        frame.Content = gridLayout;

        grid.Add(frame, 0, 2);
        Grid.SetColumnSpan(frame, 2);

        vm.LabelApiKey = new Label
        {
            Text = "API key",
            FontAttributes = FontAttributes.Bold,
            TextColor = (Color)Application.Current.Resources[ThemeNames.TextColor],
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        };

        vm.EntryApiKey = new Entry
        {
            Text = string.Empty,
            TextColor = (Color)Application.Current.Resources[ThemeNames.TextColor],
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 150,
            Placeholder = "Enter API key",
            Margin = new Thickness(0, 0, 10, 0),
        };

        vm.LabelApiUrl = new Label
        {
            Text = "API url",
            FontAttributes = FontAttributes.Bold,
            TextColor = (Color)Application.Current.Resources[ThemeNames.TextColor],
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        };

        vm.EntryApiUrl = new Entry
        {
            Text = string.Empty,
            TextColor = (Color)Application.Current.Resources[ThemeNames.TextColor],
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 150,
            Placeholder = "Enter API url",
        };


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
            }
        };

        grid.Add(settingsRow, 0, 3);
        Grid.SetColumnSpan(settingsRow, 2);

        var buttonRow = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(10),
            Children =
            {
                new Button
                {
                    Text = "OK",
                    TextColor = (Color)Application.Current.Resources[ThemeNames.TextColor],
                    BackgroundColor = (Color)Application.Current.Resources[ThemeNames.SecondaryBackgroundColor],
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 0, 10, 0),
                    Command = vm.OkCommand,
                },
                new Button
                {
                    Text = "Cancel",
                    TextColor = (Color)Application.Current.Resources[ThemeNames.TextColor],
                    BackgroundColor = (Color)Application.Current.Resources[ThemeNames.SecondaryBackgroundColor],
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Command = vm.CancelCommand,
                },
            }
        };

        grid.Add(buttonRow, 0, 4);
        Grid.SetColumnSpan(buttonRow, 2);


        Content = grid;

        vm.CollectionView.SetBinding(ItemsView.ItemsSourceProperty, "Lines");
    }
}
