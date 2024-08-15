namespace SubtitleAlchemist.Features.Translate;

public class TranslatePage : ContentPage
{
    public TranslatePage(TranslateModel vm)
    {
        BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"];

        BindingContext = vm;

        var grid = new Grid
        {
            Margin = new Thickness(10, 10, 10, 10),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
            },
        };

        var titleLabel = new Label
        {
            Margin = new Thickness(15, 15, 15, 15),
            Text = "Powered by",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
            TextColor = (Color)Application.Current.Resources["TextColor"],
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        };
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);


        var gridLeft = new Grid
        {
            Padding = new Thickness(10),
            HorizontalOptions = LayoutOptions.Fill,
            RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
            },
        };

        var enginePicker = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            SelectedIndex = 0,

        };
        enginePicker.SetBinding(Picker.ItemsSourceProperty, "AutoTranslators");
        enginePicker.SetBinding(Picker.SelectedItemProperty, "SelectedAutoTranslator");
        gridLeft.Add(enginePicker, 0, 0);

        var fromLabel = new Label
        {
            Text = "From:",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
            TextColor = (Color)Application.Current.Resources["TextColor"],
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 10, 0),
        };
        gridLeft.Add(fromLabel, 2, 0);

        var fromPicker = new Picker
        {
            TextColor = (Color)Application.Current.Resources["TextColor"],
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Items =
            {
                "English", "Danish",
            },
            SelectedIndex = 0,
        };
        gridLeft.Add(fromPicker, 3, 0);


        grid.Add(gridLeft, 0, 1);

        var right = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Children =
            {
                new Label
                {
                    Text = "To:",
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 18,
                    TextColor = (Color)Application.Current.Resources["TextColor"],
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0,0,10,0),
                },
                new Picker
                {
                    FontSize = 18,
                    TextColor = (Color)Application.Current.Resources["TextColor"],
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Items =
                    {
                        "English","Danish",
                    },
                    SelectedIndex = 0,
                },
            },
        };

        grid.Add(right, 1, 1);


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
                var numberLabel = new Label { VerticalTextAlignment = TextAlignment.Center };
                numberLabel.SetBinding(Label.TextProperty, "Number");

                var startTimeLabel = new Label { VerticalTextAlignment = TextAlignment.Center };
                startTimeLabel.SetBinding(Label.TextProperty, new Binding("StartTime", stringFormat: "{0:HH:mm:ss}"));

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
            BackgroundColor = Color.FromRgb(22,22,22), //TODO: Add to resources, header background color
            Padding = new Thickness(5),
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) }
            }
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
                new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
            }
        };

        gridLayout.Add(headerGrid, 0, 0);
        gridLayout.Add(collectionView, 0, 1);


        grid.Add(gridLayout, 0, 2);
        Grid.SetColumnSpan(gridLayout, 2);



        //// Add the header and CollectionView to the main layout
        //var stackLayout = new StackLayout
        //{
        //    Children =
        //    {
        //        headerGrid,
        //        new BoxView { HeightRequest = 1, Color = Colors.Black }, // Divider
        //        collectionView,
        //    }
        //};

        //grid.Add(stackLayout, 0, 2);
        //Grid.SetColumnSpan(stackLayout, 2);


        Content = grid;

        collectionView.SetBinding(ItemsView.ItemsSourceProperty, "Lines");
    }
}
